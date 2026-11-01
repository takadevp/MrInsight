using App.Common;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace MrInsight;

public static class GitLabImporter
{
	private const int PageSize = 100;

	private const int MaxLogBodyLength = 2000;

	private static readonly HttpClient Client = new HttpClient();

	public static async Task<List<Project>> FetchProjectListAsync(CancellationToken cancellationToken)
	{
		try
		{
			return await FetchProjectListCoreAsync(cancellationToken);
		}
		catch (OperationCanceledException)
		{
			AppLog.Write("プロジェクトの取り込みをキャンセルしました。");
			throw;
		}
		catch (Exception exception)
		{
			AppLog.Write(exception);
			throw;
		}
	}

	public static async Task<List<MergeRequest>> FetchMergeRequestListAsync(DateTime? since, int projectId, CancellationToken cancellationToken)
	{
		AppLog.Write($"マージリクエストを取り込みます: projectId={projectId} since={since?.ToString("o") ?? "なし"}");
		try
		{
			var mergeRequestList = await FetchMergeRequestListCoreAsync(since, projectId, cancellationToken);
			AppLog.Write($"マージリクエストを {mergeRequestList.Count} 件取得しました: projectId={projectId}");
			return mergeRequestList;
		}
		catch (OperationCanceledException)
		{
			AppLog.Write($"マージリクエストの取り込みをキャンセルしました: projectId={projectId}");
			throw;
		}
		catch (Exception exception)
		{
			AppLog.Write(exception);
			throw;
		}
	}

	private static async Task<List<Project>> FetchProjectListCoreAsync(CancellationToken cancellationToken)
	{
		var projectList = new List<Project>();
		foreach (var element in await FetchAllAsync($"projects?membership=true&simple=true&order_by=id&sort=asc&per_page={PageSize}", cancellationToken))
		{
			projectList.Add(new Project
			{
				ProjectId = GitLabJson.GetInt(element, "id"),
				ProjectName = GitLabJson.GetString(element, "name"),
				ProjectPath = GitLabJson.GetString(element, "path_with_namespace"),
				WebUrl = GitLabJson.GetString(element, "web_url"),
			});
		}

		return projectList;
	}

	private static async Task<List<MergeRequest>> FetchMergeRequestListCoreAsync(DateTime? since, int projectId, CancellationToken cancellationToken)
	{
		var path = $"projects/{projectId}/merge_requests?scope=all&state=all&order_by=updated_at&sort=asc&per_page={PageSize}";
		if (since.HasValue)
		{
			var universal = since.Value.ToUniversalTime();
			path += $"&updated_after={Uri.EscapeDataString(universal.ToString("o"))}";
		}

		var mergeRequestList = new List<MergeRequest>();
		var useDiffs = true;
		foreach (var element in await FetchAllAsync(path, cancellationToken))
		{
			var mergeRequest = ToMergeRequest(element);
			useDiffs = await FillDiffStatsAsync(mergeRequest, projectId, useDiffs, cancellationToken);
			mergeRequest.ThreadList = await FetchThreadListAsync(projectId, mergeRequest.Iid, cancellationToken);
			mergeRequestList.Add(mergeRequest);
		}

		return mergeRequestList;
	}

	private static async Task<List<JsonElement>> FetchAllAsync(string path, CancellationToken cancellationToken)
	{
		var separator = path.Contains('?') ? "&" : "?";
		var elementList = new List<JsonElement>();
		var page = "1";
		while (page.Length > 0)
		{
			var (root, nextPage) = await FetchAsync($"{path}{separator}page={page}", cancellationToken);
			foreach (var element in root.EnumerateArray())
			{
				elementList.Add(element);
			}

			page = nextPage;
		}

		return elementList;
	}

	private static async Task<(JsonElement Root, string NextPage)> FetchAsync(string path, CancellationToken cancellationToken)
	{
		if (AppEnvironment.IsSample)
		{
			AppLog.Write($"見本 GET {path}");
			return (SampleGitLab.Find(path), string.Empty);
		}

		var settings = AppSettings.Load();
		var url = $"{settings.GitLabUrl.TrimEnd('/')}/api/v4/{path}";
		using var request = new HttpRequestMessage(HttpMethod.Get, url);
		request.Headers.Add("PRIVATE-TOKEN", settings.GitLabAccessToken);

		HttpResponseMessage response;
		var stopwatch = Stopwatch.StartNew();
		try
		{
			response = await Client.SendAsync(request, cancellationToken);
		}
		catch (HttpRequestException exception)
		{
			throw new AppException($"GitLab につながりません: {exception.Message}", new { url }, 400);
		}
		catch (TaskCanceledException) when (!cancellationToken.IsCancellationRequested)
		{
			throw new AppException("GitLab の応答が時間切れになりました。", new { url }, 400);
		}

		using (response)
		{
			AppLog.Write($"GET {url} {(int)response.StatusCode} {stopwatch.ElapsedMilliseconds}ms");
			if (!response.IsSuccessStatusCode)
			{
				var body = await response.Content.ReadAsStringAsync(cancellationToken);
				if (body.Length > MaxLogBodyLength)
				{
					body = body.Substring(0, MaxLogBodyLength);
				}

				var correlationId = response.Headers.TryGetValues("X-Request-Id", out var valueList) ? string.Join(",", valueList) : "";
				throw new GitLabStatusException($"GitLab から取得できませんでした: {(int)response.StatusCode} correlation_id={correlationId}", new { url, correlationId, body });
			}

			using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
			using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
			var nextPage = response.Headers.TryGetValues("X-Next-Page", out var nextPageList) ? string.Join("", nextPageList) : "";
			return (document.RootElement.Clone(), nextPage);
		}
	}

	private static MergeRequest ToMergeRequest(JsonElement element)
	{
		var mergeRequest = new MergeRequest
		{
			GitlabId = GitLabJson.GetInt(element, "id"),
			ProjectId = GitLabJson.GetInt(element, "project_id"),
			Iid = GitLabJson.GetInt(element, "iid"),
			Title = GitLabJson.GetString(element, "title"),
			Description = GitLabJson.GetString(element, "description"),
			State = ToState(GitLabJson.GetString(element, "state")),
			Draft = GitLabJson.GetBool(element, "draft") ?? GitLabJson.GetBool(element, "work_in_progress"),
			AuthorId = GitLabJson.GetInt(GitLabJson.GetObject(element, "author"), "id"),
			AuthorUsername = GitLabJson.GetString(GitLabJson.GetObject(element, "author"), "username"),
			AuthorName = GitLabJson.GetString(GitLabJson.GetObject(element, "author"), "name"),
			SourceBranch = GitLabJson.GetString(element, "source_branch"),
			TargetBranch = GitLabJson.GetString(element, "target_branch"),
			Milestone = GitLabJson.GetString(GitLabJson.GetObject(element, "milestone"), "title"),
			CreatedAt = GitLabJson.GetDate(element, "created_at"),
			UpdatedAt = GitLabJson.GetDate(element, "updated_at"),
			MergedAt = GitLabJson.GetDate(element, "merged_at"),
			ClosedAt = GitLabJson.GetDate(element, "closed_at"),
			MergedByUsername = GitLabJson.GetString(GitLabJson.GetObject(element, "merge_user") ?? GitLabJson.GetObject(element, "merged_by"), "username"),
			CommentCount = GitLabJson.GetInt(element, "user_notes_count"),
			UpvoteCount = GitLabJson.GetInt(element, "upvotes"),
			DownvoteCount = GitLabJson.GetInt(element, "downvotes"),
			HasConflicts = GitLabJson.GetBool(element, "has_conflicts"),
			WebUrl = GitLabJson.GetString(element, "web_url"),
		};

		foreach (var label in GitLabJson.GetArray(element, "labels"))
		{
			mergeRequest.LabelList.Add(new MergeRequestLabel { LabelName = label.GetString() });
		}

		foreach (var assignee in GitLabJson.GetArray(element, "assignees"))
		{
			mergeRequest.AssigneeList.Add(new MergeRequestAssignee
			{
				AssigneeUserId = GitLabJson.GetInt(assignee, "id"),
				AssigneeUsername = GitLabJson.GetString(assignee, "username"),
			});
		}

		foreach (var reviewer in GitLabJson.GetArray(element, "reviewers"))
		{
			mergeRequest.ReviewerList.Add(new MergeRequestReviewer
			{
				ReviewerUserId = GitLabJson.GetInt(reviewer, "id"),
				ReviewerUsername = GitLabJson.GetString(reviewer, "username"),
			});
		}

		return mergeRequest;
	}

	private static MergeRequestState ToState(string? state)
	{
		switch (state)
		{
			case "opened":
				return MergeRequestState.Opened;

			case "closed":
				return MergeRequestState.Closed;

			case "locked":
				return MergeRequestState.Locked;

			case "merged":
				return MergeRequestState.Merged;

			default:
				throw new AppException($"GitLab のマージリクエストの状態が分かりません: {state}", new { state }, 400);
		}
	}

	private static async Task<List<MergeRequestThread>> FetchThreadListAsync(int projectId, int? iid, CancellationToken cancellationToken)
	{
		var threadList = new List<MergeRequestThread>();
		foreach (var discussion in await FetchAllAsync($"projects/{projectId}/merge_requests/{iid}/discussions?per_page={PageSize}", cancellationToken))
		{
			var commentList = new List<MergeRequestComment>();
			var firstNote = (JsonElement?)null;
			var resolvable = false;
			var resolved = true;
			foreach (var note in GitLabJson.GetArray(discussion, "notes"))
			{
				if (GitLabJson.GetBool(note, "system") == true)
				{
					continue;
				}

				firstNote ??= note;
				if (GitLabJson.GetBool(note, "resolvable") == true)
				{
					resolvable = true;
					resolved = resolved && GitLabJson.GetBool(note, "resolved") == true;
				}

				commentList.Add(new MergeRequestComment
				{
					GitlabCommentId = GitLabJson.GetInt(note, "id"),
					AuthorUsername = GitLabJson.GetString(GitLabJson.GetObject(note, "author"), "username"),
					AuthorName = GitLabJson.GetString(GitLabJson.GetObject(note, "author"), "name"),
					Body = GitLabJson.GetString(note, "body"),
					CreatedAt = GitLabJson.GetDate(note, "created_at"),
					UpdatedAt = GitLabJson.GetDate(note, "updated_at"),
				});
			}

			if (!firstNote.HasValue)
			{
				continue;
			}

			var position = GitLabJson.GetObject(firstNote.Value, "position");
			threadList.Add(new MergeRequestThread
			{
				GitlabThreadId = GitLabJson.GetString(discussion, "id"),
				CreatedAt = GitLabJson.GetDate(firstNote.Value, "created_at"),
				FirstAuthorUsername = GitLabJson.GetString(GitLabJson.GetObject(firstNote.Value, "author"), "username"),
				FilePath = GitLabJson.GetString(position, "new_path") ?? GitLabJson.GetString(position, "old_path"),
				LineNumber = GitLabJson.GetInt(position, "new_line") ?? GitLabJson.GetInt(position, "old_line"),
				Resolvable = resolvable,
				Resolved = resolvable && resolved,
				CommentCount = commentList.Count,
				CommentList = commentList,
			});
		}

		return threadList;
	}

	private static async Task<bool> FillDiffStatsAsync(MergeRequest mergeRequest, int projectId, bool useDiffs, CancellationToken cancellationToken)
	{
		var path = $"projects/{projectId}/merge_requests/{mergeRequest.Iid}";
		if (useDiffs)
		{
			try
			{
				CountDiff(mergeRequest, await FetchAllAsync($"{path}/diffs", cancellationToken));
				return true;
			}
			catch (GitLabStatusException exception)
			{
				AppLog.Write($"警告: !{mergeRequest.Iid} の差分を /diffs で取れなかったので、この取り込みでは以降 /changes で取ります: {exception.Message}");
			}
		}

		try
		{
			var (root, _) = await FetchAsync($"{path}/changes", cancellationToken);
			if (GitLabJson.GetBool(root, "overflow") == true)
			{
				AppLog.Write($"警告: !{mergeRequest.Iid} の差分は大きすぎて GitLab が途中で切ったので、行数とファイル数を空にします。");
				return false;
			}

			CountDiff(mergeRequest, GitLabJson.GetArray(root, "changes"));
		}
		catch (GitLabStatusException exception)
		{
			AppLog.Write($"警告: !{mergeRequest.Iid} の差分を /changes でも取れなかったので、行数とファイル数を空にします: {exception.Message}");
		}

		return false;
	}

	private static void CountDiff(MergeRequest mergeRequest, List<JsonElement> diffList)
	{
		var additionCount = 0;
		var deletionCount = 0;
		foreach (var diff in diffList)
		{
			var text = GitLabJson.GetString(diff, "diff") ?? string.Empty;
			foreach (var line in text.Split('\n'))
			{
				if (line.StartsWith("+++", StringComparison.Ordinal) || line.StartsWith("---", StringComparison.Ordinal))
				{
					continue;
				}

				if (line.StartsWith('+'))
				{
					additionCount++;
				}
				else if (line.StartsWith('-'))
				{
					deletionCount++;
				}
			}
		}

		mergeRequest.AdditionCount = additionCount;
		mergeRequest.DeletionCount = deletionCount;
		mergeRequest.ChangedFileCount = diffList.Count;
	}
}
