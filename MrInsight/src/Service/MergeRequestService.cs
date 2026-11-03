using App.Common;
using System.Text.RegularExpressions;

namespace MrInsight;

/// <summary>
/// GitLab から取り込んだマージリクエストを集計するサービス
/// </summary>
public static class MergeRequestService
{
	/// <summary>
	/// 指定したプロジェクトの、作成者ごとのマージリクエストの件数を、多い順に取得する
	/// </summary>
	public static ResponseMergeRequestGetAuthorCountList GetAuthorCountList(RequestMergeRequestGetAuthorCountList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetAuthorCountList();

		var targetMergeRequestAuthorCountList = GMergeRequest.SummarizeMergeRequestAuthorCountByProjectId(request.ProjectId ?? default);
		response.CountList = targetMergeRequestAuthorCountList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトの、指定した作成者のマージリクエストに付いたバッジごとの件数と、その作成者に付いたバッジ全体に対する割合を、多い順に取得する
	/// </summary>
	public static ResponseMergeRequestGetAuthorBadgeCountList GetAuthorBadgeCountList(RequestMergeRequestGetAuthorBadgeCountList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetAuthorBadgeCountList();

		var targetMergeRequestBadgeCountList = GMergeRequestComment.SummarizeMergeRequestBadgeCountByProjectIdAndAuthorUsername(request.ProjectId ?? default, request.AuthorUsername ?? "");
		response.BadgeCountList = targetMergeRequestBadgeCountList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトの、バッジごとの件数と割合と受けた人数を、受けた人数の多い順に取得する
	/// </summary>
	public static ResponseMergeRequestGetTeamBadgeCountList GetTeamBadgeCountList(RequestMergeRequestGetTeamBadgeCountList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetTeamBadgeCountList();

		var targetMergeRequestTeamBadgeCountList = GMergeRequestComment.SummarizeMergeRequestTeamBadgeCountByProjectId(request.ProjectId ?? default);
		response.TeamBadgeCountList = targetMergeRequestTeamBadgeCountList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトの、指定した作成者のマージリクエストに、指定したバッジが付いたコメントを、新しい順に取得する
	/// </summary>
	public static ResponseMergeRequestGetBadgeCommentList GetBadgeCommentList(RequestMergeRequestGetBadgeCommentList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetBadgeCommentList();

		var targetMergeRequestBadgeCommentList = GMergeRequestComment.ListMergeRequestBadgeCommentByProjectIdAndAuthorUsernameAndBadgeName(request.ProjectId ?? default, request.AuthorUsername ?? "", request.BadgeName ?? "");
		response.BadgeCommentList = targetMergeRequestBadgeCommentList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトの、すべてのマージリクエストのコメントを、マージリクエストの番号順・書かれた順に取得する
	/// </summary>
	public static ResponseMergeRequestGetCommentDetailList GetCommentDetailList(RequestMergeRequestGetCommentDetailList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetCommentDetailList();

		var targetMergeRequestCommentDetailList = GMergeRequestComment.ListMergeRequestCommentDetailByProjectId(request.ProjectId ?? default);
		response.CommentDetailList = targetMergeRequestCommentDetailList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトの、作成月と作成者ごとのマージリクエストの件数を、新しい月から取得する
	/// </summary>
	public static ResponseMergeRequestGetAuthorMonthlyCountList GetAuthorMonthlyCountList(RequestMergeRequestGetAuthorMonthlyCountList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetAuthorMonthlyCountList();

		var targetMergeRequestAuthorMonthlyCountList = GMergeRequest.SummarizeMergeRequestAuthorMonthlyCountByProjectId(request.ProjectId ?? default);
		response.MonthlyCountList = targetMergeRequestAuthorMonthlyCountList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトの、コメントが書かれた月とバッジごとの件数と割合を、新しい月から取得する
	/// </summary>
	public static ResponseMergeRequestGetMonthlyBadgeCountList GetMonthlyBadgeCountList(RequestMergeRequestGetMonthlyBadgeCountList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetMonthlyBadgeCountList();

		var targetMergeRequestMonthlyBadgeCountList = GMergeRequestComment.SummarizeMergeRequestMonthlyBadgeCountByProjectId(request.ProjectId ?? default);
		response.MonthlyBadgeCountList = targetMergeRequestMonthlyBadgeCountList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 登録したバッジを、名前の順に取得する
	/// </summary>
	public static ResponseMergeRequestGetBadgeList GetBadgeList()
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetBadgeList();

		var targetGBadgeList = GBadge.GetAll();
		foreach (var targetGBadge in targetGBadgeList)
		{
			var responseBadge = new Badge
			{
				Id = targetGBadge.Id,
				BadgeName = targetGBadge.BadgeName,
			};

			response.BadgeList.Add(responseBadge);
		}

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// バッジを1つ登録する
	/// </summary>
	public static void AddBadge(RequestMergeRequestAddBadge request)
	{
		if (string.IsNullOrEmpty(request.BadgeName))
		{
			throw new AppException("バッジ名を指定してください。", statusCode: 400);
		}

		if (request.BadgeName.Length > 64)
		{
			throw new AppException("バッジ名は64文字以内で指定してください。", statusCode: 400);
		}

		if (!Regex.IsMatch(request.BadgeName, @"^[a-zA-Z0-9_-]+\z"))
		{
			throw new AppException("バッジ名に使用できない文字が含まれています。", statusCode: 400);
		}

		using var transaction = DatabaseTransaction.Begin();
		var uniqueBadgeNameGBadge = GBadge.FindFirstByBadgeName(request.BadgeName ?? "");
		if (uniqueBadgeNameGBadge != null)
		{
			throw new AppException("登録済みのバッジです。", statusCode: 400);
		}

		var targetGBadge = GBadge.CreateDefault(request.BadgeName!);

		targetGBadge.Save();

		transaction.Commit();
	}

	/// <summary>
	/// 登録したバッジを1つ消す
	/// </summary>
	public static void DeleteBadge(RequestMergeRequestDeleteBadge request)
	{
		if (request.BadgeId == null)
		{
			throw new AppException("バッジIDを指定してください。", statusCode: 400);
		}

		using var transaction = DatabaseTransaction.Begin();
		var targetGBadge = GBadge.FindById(request.BadgeId!.Value);

		if (targetGBadge == null)
		{
			throw new AppException("バッジが見つかりません。", statusCode: 404);
		}

		targetGBadge.Delete();

		transaction.Commit();
	}

	/// <summary>
	/// すべてのプロジェクトのコメントから、登録していないバッジごとの件数を、多い順に取得する
	/// </summary>
	public static ResponseMergeRequestGetUnregisteredBadgeCountList GetUnregisteredBadgeCountList()
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetUnregisteredBadgeCountList();

		var targetMergeRequestUnregisteredBadgeCountList = GMergeRequestComment.SummarizeMergeRequestUnregisteredBadgeCount();
		response.UnregisteredBadgeCountList = targetMergeRequestUnregisteredBadgeCountList;

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// GitLab からプロジェクトを取り込み、既にあるものは書き換える
	/// </summary>
	public static async Task ImportProjectList(CancellationToken cancellationToken)
	{
		using var transaction = DatabaseTransaction.Begin();
		var fetchedProjectList = await GitLabImporter.FetchProjectListAsync(cancellationToken);

		var fetchedProjectKeyList = new List<int>();
		foreach (var fetchedProject in fetchedProjectList)
		{
			fetchedProjectKeyList.Add(fetchedProject.ProjectId!.Value);
		}

		var currentGProjectList = GProject.FindByProjectIdList(fetchedProjectKeyList);
		var currentGProjectDict = new Dictionary<int, GProject>();
		var currentGProjectIdList = new List<int>();
		foreach (var currentGProject in currentGProjectList)
		{
			currentGProjectDict[currentGProject.ProjectId] = currentGProject;
			currentGProjectIdList.Add(currentGProject.Id);
		}

		var saveGProjectList = new List<GProject>();
		foreach (var fetchedProject in fetchedProjectList)
		{
			if (currentGProjectDict.TryGetValue(fetchedProject.ProjectId!.Value, out var updateGProject))
			{
				updateGProject.ProjectId = fetchedProject.ProjectId!.Value;
				updateGProject.ProjectName = fetchedProject.ProjectName!;
				updateGProject.ProjectPath = fetchedProject.ProjectPath!;
				updateGProject.WebUrl = fetchedProject.WebUrl ?? "";
				saveGProjectList.Add(updateGProject);
			}
			else
			{
				saveGProjectList.Add(GProject.CreateDefault(fetchedProject.ProjectId!.Value, fetchedProject.ProjectName!, fetchedProject.ProjectPath!, fetchedProject.WebUrl ?? ""));
			}
		}

		AbstractEntity.SaveAll(DbContextScope.GlobalContext, saveGProjectList);

		transaction.Commit();
	}

	/// <summary>
	/// 取り込んだプロジェクトを取得する
	/// </summary>
	public static ResponseMergeRequestGetProjectList GetProjectList()
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetProjectList();

		var targetGProjectList = GProject.GetAll();
		foreach (var targetGProject in targetGProjectList)
		{
			var responseProject = new Project
			{
				Id = targetGProject.Id,
				ProjectId = targetGProject.ProjectId,
				ProjectName = targetGProject.ProjectName,
				ProjectPath = targetGProject.ProjectPath,
				WebUrl = targetGProject.WebUrl,
			};

			response.ProjectList.Add(responseProject);
		}

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトのマージリクエストのうち、タイトル・本文・作成者ユーザー名に指定した文字を含み、作成日が指定した期間に入るものを、新しい順に取得する
	/// </summary>
	public static ResponseMergeRequestGetProjectMergeRequestList GetProjectMergeRequestList(RequestMergeRequestGetProjectMergeRequestList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetProjectMergeRequestList();

		var targetGMergeRequestList = GMergeRequest.FindByProjectIdAndTitleAndDescriptionAndAuthorUsernameAndStartDateAndEndDate(request.ProjectId ?? default, request.Title ?? "", request.Description ?? "", request.AuthorUsername ?? "", request.StartDate, request.EndDate);

		// N+1 を避けるため、ぶら下がる概念を一括取得するIDを集める
		var targetGMergeRequestIdList = new List<int>();
		foreach (var targetGMergeRequest in targetGMergeRequestList)
		{
			targetGMergeRequestIdList.Add(targetGMergeRequest.Id);
		}

		// ラベルを一括取得する
		var allGMergeRequestLabelList = GMergeRequestLabel.FindByMergeRequestIdList(targetGMergeRequestIdList);

		// アサインを一括取得する
		var allGMergeRequestAssigneeList = GMergeRequestAssignee.FindByMergeRequestIdList(targetGMergeRequestIdList);

		// レビュアーを一括取得する
		var allGMergeRequestReviewerList = GMergeRequestReviewer.FindByMergeRequestIdList(targetGMergeRequestIdList);

		// スレッドを一括取得する
		var allGMergeRequestThreadList = GMergeRequestThread.FindByMergeRequestIdList(targetGMergeRequestIdList);
		var allGMergeRequestThreadIdList = new List<int>();
		foreach (var allGMergeRequestThread in allGMergeRequestThreadList)
		{
			allGMergeRequestThreadIdList.Add(allGMergeRequestThread.Id);
		}

		// コメントを一括取得する
		var allGMergeRequestCommentList = GMergeRequestComment.FindByThreadIdList(allGMergeRequestThreadIdList);
		// コメントを組み立てて、親のIDでまとめる
		var responseMergeRequestCommentDict = new Dictionary<int, List<MergeRequestComment>>();
		foreach (var allGMergeRequestComment in allGMergeRequestCommentList)
		{
			var responseMergeRequestComment = new MergeRequestComment
			{
				Id = allGMergeRequestComment.Id,
				GitlabCommentId = allGMergeRequestComment.GitlabCommentId,
				AuthorUsername = allGMergeRequestComment.AuthorUsername,
				AuthorName = allGMergeRequestComment.AuthorName,
				Body = allGMergeRequestComment.Body,
				CreatedAt = allGMergeRequestComment.CreatedAt,
				UpdatedAt = allGMergeRequestComment.UpdatedAt,
			};
			if (!responseMergeRequestCommentDict.TryGetValue(allGMergeRequestComment.ThreadId, out var childMergeRequestCommentList))
			{
				childMergeRequestCommentList = new List<MergeRequestComment>();
				responseMergeRequestCommentDict[allGMergeRequestComment.ThreadId] = childMergeRequestCommentList;
			}
			childMergeRequestCommentList.Add(responseMergeRequestComment);
		}

		// スレッドを組み立てて、親のIDでまとめる
		var responseMergeRequestThreadDict = new Dictionary<int, List<MergeRequestThread>>();
		foreach (var allGMergeRequestThread in allGMergeRequestThreadList)
		{
			responseMergeRequestCommentDict.TryGetValue(allGMergeRequestThread.Id, out var childMergeRequestCommentList);
			var responseMergeRequestThread = new MergeRequestThread
			{
				Id = allGMergeRequestThread.Id,
				GitlabThreadId = allGMergeRequestThread.GitlabThreadId,
				CreatedAt = allGMergeRequestThread.CreatedAt,
				FirstAuthorUsername = allGMergeRequestThread.FirstAuthorUsername,
				FilePath = allGMergeRequestThread.FilePath,
				LineNumber = allGMergeRequestThread.LineNumber,
				Resolvable = allGMergeRequestThread.Resolvable,
				Resolved = allGMergeRequestThread.Resolved,
				CommentCount = allGMergeRequestThread.CommentCount,
				CommentList = childMergeRequestCommentList ?? [],
			};
			if (!responseMergeRequestThreadDict.TryGetValue(allGMergeRequestThread.MergeRequestId, out var childMergeRequestThreadList))
			{
				childMergeRequestThreadList = new List<MergeRequestThread>();
				responseMergeRequestThreadDict[allGMergeRequestThread.MergeRequestId] = childMergeRequestThreadList;
			}
			childMergeRequestThreadList.Add(responseMergeRequestThread);
		}

		// レビュアーを組み立てて、親のIDでまとめる
		var responseMergeRequestReviewerDict = new Dictionary<int, List<MergeRequestReviewer>>();
		foreach (var allGMergeRequestReviewer in allGMergeRequestReviewerList)
		{
			var responseMergeRequestReviewer = new MergeRequestReviewer
			{
				Id = allGMergeRequestReviewer.Id,
				ReviewerUserId = allGMergeRequestReviewer.ReviewerUserId,
				ReviewerUsername = allGMergeRequestReviewer.ReviewerUsername,
			};
			if (!responseMergeRequestReviewerDict.TryGetValue(allGMergeRequestReviewer.MergeRequestId, out var childMergeRequestReviewerList))
			{
				childMergeRequestReviewerList = new List<MergeRequestReviewer>();
				responseMergeRequestReviewerDict[allGMergeRequestReviewer.MergeRequestId] = childMergeRequestReviewerList;
			}
			childMergeRequestReviewerList.Add(responseMergeRequestReviewer);
		}

		// アサインを組み立てて、親のIDでまとめる
		var responseMergeRequestAssigneeDict = new Dictionary<int, List<MergeRequestAssignee>>();
		foreach (var allGMergeRequestAssignee in allGMergeRequestAssigneeList)
		{
			var responseMergeRequestAssignee = new MergeRequestAssignee
			{
				Id = allGMergeRequestAssignee.Id,
				AssigneeUserId = allGMergeRequestAssignee.AssigneeUserId,
				AssigneeUsername = allGMergeRequestAssignee.AssigneeUsername,
			};
			if (!responseMergeRequestAssigneeDict.TryGetValue(allGMergeRequestAssignee.MergeRequestId, out var childMergeRequestAssigneeList))
			{
				childMergeRequestAssigneeList = new List<MergeRequestAssignee>();
				responseMergeRequestAssigneeDict[allGMergeRequestAssignee.MergeRequestId] = childMergeRequestAssigneeList;
			}
			childMergeRequestAssigneeList.Add(responseMergeRequestAssignee);
		}

		// ラベルを組み立てて、親のIDでまとめる
		var responseMergeRequestLabelDict = new Dictionary<int, List<MergeRequestLabel>>();
		foreach (var allGMergeRequestLabel in allGMergeRequestLabelList)
		{
			var responseMergeRequestLabel = new MergeRequestLabel
			{
				Id = allGMergeRequestLabel.Id,
				LabelName = allGMergeRequestLabel.LabelName,
			};
			if (!responseMergeRequestLabelDict.TryGetValue(allGMergeRequestLabel.MergeRequestId, out var childMergeRequestLabelList))
			{
				childMergeRequestLabelList = new List<MergeRequestLabel>();
				responseMergeRequestLabelDict[allGMergeRequestLabel.MergeRequestId] = childMergeRequestLabelList;
			}
			childMergeRequestLabelList.Add(responseMergeRequestLabel);
		}

		foreach (var targetGMergeRequest in targetGMergeRequestList)
		{
			responseMergeRequestLabelDict.TryGetValue(targetGMergeRequest.Id, out var childMergeRequestLabelList);
			responseMergeRequestAssigneeDict.TryGetValue(targetGMergeRequest.Id, out var childMergeRequestAssigneeList);
			responseMergeRequestReviewerDict.TryGetValue(targetGMergeRequest.Id, out var childMergeRequestReviewerList);
			responseMergeRequestThreadDict.TryGetValue(targetGMergeRequest.Id, out var childMergeRequestThreadList);
			var responseMergeRequest = new MergeRequest
			{
				Id = targetGMergeRequest.Id,
				GitlabId = targetGMergeRequest.GitlabId,
				ProjectId = targetGMergeRequest.ProjectId,
				Iid = targetGMergeRequest.Iid,
				Title = targetGMergeRequest.Title,
				Description = targetGMergeRequest.Description,
				State = targetGMergeRequest.State,
				Draft = targetGMergeRequest.Draft,
				AuthorId = targetGMergeRequest.AuthorId,
				AuthorUsername = targetGMergeRequest.AuthorUsername,
				AuthorName = targetGMergeRequest.AuthorName,
				SourceBranch = targetGMergeRequest.SourceBranch,
				TargetBranch = targetGMergeRequest.TargetBranch,
				Milestone = targetGMergeRequest.Milestone,
				CreatedAt = targetGMergeRequest.CreatedAt,
				UpdatedAt = targetGMergeRequest.UpdatedAt,
				MergedAt = targetGMergeRequest.MergedAt,
				ClosedAt = targetGMergeRequest.ClosedAt,
				MergedByUsername = targetGMergeRequest.MergedByUsername,
				CommentCount = targetGMergeRequest.CommentCount,
				UpvoteCount = targetGMergeRequest.UpvoteCount,
				DownvoteCount = targetGMergeRequest.DownvoteCount,
				AdditionCount = targetGMergeRequest.AdditionCount,
				DeletionCount = targetGMergeRequest.DeletionCount,
				ChangedFileCount = targetGMergeRequest.ChangedFileCount,
				HasConflicts = targetGMergeRequest.HasConflicts,
				WebUrl = targetGMergeRequest.WebUrl,
				LabelList = childMergeRequestLabelList ?? [],
				AssigneeList = childMergeRequestAssigneeList ?? [],
				ReviewerList = childMergeRequestReviewerList ?? [],
				ThreadList = childMergeRequestThreadList ?? [],
			};

			response.MergeRequestList.Add(responseMergeRequest);
		}

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したマージリクエストのスレッドを、古い順に取得する
	/// </summary>
	public static ResponseMergeRequestGetThreadList GetThreadList(RequestMergeRequestGetThreadList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetThreadList();

		var targetGMergeRequestThreadList = GMergeRequestThread.FindByMergeRequestId(request.MergeRequestId ?? default);

		// N+1 を避けるため、ぶら下がる概念を一括取得するIDを集める
		var targetGMergeRequestThreadIdList = new List<int>();
		foreach (var targetGMergeRequestThread in targetGMergeRequestThreadList)
		{
			targetGMergeRequestThreadIdList.Add(targetGMergeRequestThread.Id);
		}

		// コメントを一括取得する
		var allGMergeRequestCommentList = GMergeRequestComment.FindByThreadIdList(targetGMergeRequestThreadIdList);
		// コメントを組み立てて、親のIDでまとめる
		var responseMergeRequestCommentDict = new Dictionary<int, List<MergeRequestComment>>();
		foreach (var allGMergeRequestComment in allGMergeRequestCommentList)
		{
			var responseMergeRequestComment = new MergeRequestComment
			{
				Id = allGMergeRequestComment.Id,
				GitlabCommentId = allGMergeRequestComment.GitlabCommentId,
				AuthorUsername = allGMergeRequestComment.AuthorUsername,
				AuthorName = allGMergeRequestComment.AuthorName,
				Body = allGMergeRequestComment.Body,
				CreatedAt = allGMergeRequestComment.CreatedAt,
				UpdatedAt = allGMergeRequestComment.UpdatedAt,
			};
			if (!responseMergeRequestCommentDict.TryGetValue(allGMergeRequestComment.ThreadId, out var childMergeRequestCommentList))
			{
				childMergeRequestCommentList = new List<MergeRequestComment>();
				responseMergeRequestCommentDict[allGMergeRequestComment.ThreadId] = childMergeRequestCommentList;
			}
			childMergeRequestCommentList.Add(responseMergeRequestComment);
		}

		foreach (var targetGMergeRequestThread in targetGMergeRequestThreadList)
		{
			responseMergeRequestCommentDict.TryGetValue(targetGMergeRequestThread.Id, out var childMergeRequestCommentList);
			var responseMergeRequestThread = new MergeRequestThread
			{
				Id = targetGMergeRequestThread.Id,
				GitlabThreadId = targetGMergeRequestThread.GitlabThreadId,
				CreatedAt = targetGMergeRequestThread.CreatedAt,
				FirstAuthorUsername = targetGMergeRequestThread.FirstAuthorUsername,
				FilePath = targetGMergeRequestThread.FilePath,
				LineNumber = targetGMergeRequestThread.LineNumber,
				Resolvable = targetGMergeRequestThread.Resolvable,
				Resolved = targetGMergeRequestThread.Resolved,
				CommentCount = targetGMergeRequestThread.CommentCount,
				CommentList = childMergeRequestCommentList ?? [],
			};

			response.ThreadList.Add(responseMergeRequestThread);
		}

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したスレッドのコメントを、古い順に取得する
	/// </summary>
	public static ResponseMergeRequestGetCommentList GetCommentList(RequestMergeRequestGetCommentList request)
	{
		using var transaction = DatabaseTransaction.Begin();
		var response = new ResponseMergeRequestGetCommentList();

		var targetGMergeRequestCommentList = GMergeRequestComment.FindByThreadId(request.ThreadId ?? default);
		foreach (var targetGMergeRequestComment in targetGMergeRequestCommentList)
		{
			var responseMergeRequestComment = new MergeRequestComment
			{
				Id = targetGMergeRequestComment.Id,
				GitlabCommentId = targetGMergeRequestComment.GitlabCommentId,
				AuthorUsername = targetGMergeRequestComment.AuthorUsername,
				AuthorName = targetGMergeRequestComment.AuthorName,
				Body = targetGMergeRequestComment.Body,
				CreatedAt = targetGMergeRequestComment.CreatedAt,
				UpdatedAt = targetGMergeRequestComment.UpdatedAt,
			};

			response.CommentList.Add(responseMergeRequestComment);
		}

		transaction.Commit();
		return response;
	}

	/// <summary>
	/// 指定したプロジェクトの、前回から更新されたマージリクエストを GitLab から取り込む
	/// </summary>
	public static async Task ImportMergeRequestList(RequestMergeRequestImportMergeRequestList request, CancellationToken cancellationToken)
	{
		using var transaction = DatabaseTransaction.Begin();
		var targetGMergeRequest = GMergeRequest.FindLatestByProjectId(request.ProjectId ?? default);
		var fetchedMergeRequestList = await GitLabImporter.FetchMergeRequestListAsync(targetGMergeRequest?.UpdatedAt, request.ProjectId ?? default, cancellationToken);

		var fetchedMergeRequestKeyList = new List<int>();
		foreach (var fetchedMergeRequest in fetchedMergeRequestList)
		{
			fetchedMergeRequestKeyList.Add(fetchedMergeRequest.GitlabId!.Value);
		}

		var currentGMergeRequestList = GMergeRequest.FindByGitlabIdList(fetchedMergeRequestKeyList);
		var currentGMergeRequestDict = new Dictionary<int, GMergeRequest>();
		var currentGMergeRequestIdList = new List<int>();
		foreach (var currentGMergeRequest in currentGMergeRequestList)
		{
			currentGMergeRequestDict[currentGMergeRequest.GitlabId] = currentGMergeRequest;
			currentGMergeRequestIdList.Add(currentGMergeRequest.Id);
		}

		var saveGMergeRequestList = new List<GMergeRequest>();
		foreach (var fetchedMergeRequest in fetchedMergeRequestList)
		{
			if (currentGMergeRequestDict.TryGetValue(fetchedMergeRequest.GitlabId!.Value, out var updateGMergeRequest))
			{
				updateGMergeRequest.GitlabId = fetchedMergeRequest.GitlabId!.Value;
				updateGMergeRequest.ProjectId = fetchedMergeRequest.ProjectId!.Value;
				updateGMergeRequest.Iid = fetchedMergeRequest.Iid!.Value;
				updateGMergeRequest.Title = fetchedMergeRequest.Title!;
				updateGMergeRequest.Description = fetchedMergeRequest.Description ?? "";
				updateGMergeRequest.State = fetchedMergeRequest.State!.Value;
				updateGMergeRequest.Draft = fetchedMergeRequest.Draft ?? default;
				updateGMergeRequest.AuthorId = fetchedMergeRequest.AuthorId!.Value;
				updateGMergeRequest.AuthorUsername = fetchedMergeRequest.AuthorUsername!;
				updateGMergeRequest.AuthorName = fetchedMergeRequest.AuthorName ?? "";
				updateGMergeRequest.SourceBranch = fetchedMergeRequest.SourceBranch ?? "";
				updateGMergeRequest.TargetBranch = fetchedMergeRequest.TargetBranch ?? "";
				updateGMergeRequest.Milestone = fetchedMergeRequest.Milestone ?? "";
				updateGMergeRequest.CreatedAt = fetchedMergeRequest.CreatedAt!.Value;
				updateGMergeRequest.UpdatedAt = fetchedMergeRequest.UpdatedAt ?? default;
				updateGMergeRequest.MergedAt = fetchedMergeRequest.MergedAt;
				updateGMergeRequest.ClosedAt = fetchedMergeRequest.ClosedAt;
				updateGMergeRequest.MergedByUsername = fetchedMergeRequest.MergedByUsername ?? "";
				updateGMergeRequest.CommentCount = fetchedMergeRequest.CommentCount ?? default;
				updateGMergeRequest.UpvoteCount = fetchedMergeRequest.UpvoteCount ?? default;
				updateGMergeRequest.DownvoteCount = fetchedMergeRequest.DownvoteCount ?? default;
				updateGMergeRequest.AdditionCount = fetchedMergeRequest.AdditionCount;
				updateGMergeRequest.DeletionCount = fetchedMergeRequest.DeletionCount;
				updateGMergeRequest.ChangedFileCount = fetchedMergeRequest.ChangedFileCount;
				updateGMergeRequest.HasConflicts = fetchedMergeRequest.HasConflicts ?? default;
				updateGMergeRequest.WebUrl = fetchedMergeRequest.WebUrl ?? "";
				saveGMergeRequestList.Add(updateGMergeRequest);
			}
			else
			{
				saveGMergeRequestList.Add(GMergeRequest.CreateDefault(fetchedMergeRequest.GitlabId!.Value, fetchedMergeRequest.ProjectId!.Value, fetchedMergeRequest.Iid!.Value, fetchedMergeRequest.Title!, fetchedMergeRequest.Description ?? "", fetchedMergeRequest.State!.Value, fetchedMergeRequest.Draft ?? default, fetchedMergeRequest.AuthorId!.Value, fetchedMergeRequest.AuthorUsername!, fetchedMergeRequest.AuthorName ?? "", fetchedMergeRequest.SourceBranch ?? "", fetchedMergeRequest.TargetBranch ?? "", fetchedMergeRequest.Milestone ?? "", fetchedMergeRequest.CreatedAt!.Value, fetchedMergeRequest.UpdatedAt ?? default, fetchedMergeRequest.MergedAt, fetchedMergeRequest.ClosedAt, fetchedMergeRequest.MergedByUsername ?? "", fetchedMergeRequest.CommentCount ?? default, fetchedMergeRequest.UpvoteCount ?? default, fetchedMergeRequest.DownvoteCount ?? default, fetchedMergeRequest.AdditionCount, fetchedMergeRequest.DeletionCount, fetchedMergeRequest.ChangedFileCount, fetchedMergeRequest.HasConflicts ?? default, fetchedMergeRequest.WebUrl ?? ""));
			}
		}

		AbstractEntity.SaveAll(DbContextScope.GlobalContext, saveGMergeRequestList);

		var currentGMergeRequestThreadIdList = GMergeRequestThread.FindIdListByMergeRequestIdList(currentGMergeRequestIdList);
		GMergeRequestComment.DeleteByThreadIdList(currentGMergeRequestThreadIdList);
		GMergeRequestLabel.DeleteByMergeRequestIdList(currentGMergeRequestIdList);
		GMergeRequestAssignee.DeleteByMergeRequestIdList(currentGMergeRequestIdList);
		GMergeRequestReviewer.DeleteByMergeRequestIdList(currentGMergeRequestIdList);
		GMergeRequestThread.DeleteByMergeRequestIdList(currentGMergeRequestIdList);
		var saveGMergeRequestLabelList = new List<GMergeRequestLabel>();
		var saveGMergeRequestAssigneeList = new List<GMergeRequestAssignee>();
		var saveGMergeRequestReviewerList = new List<GMergeRequestReviewer>();
		var saveGMergeRequestThreadList = new List<GMergeRequestThread>();
		var saveGMergeRequestThreadPairList = new List<(GMergeRequestThread Entity, MergeRequestThread Fetched)>();
		for (var index = 0; index < fetchedMergeRequestList.Count; index++)
		{
			var fetchedMergeRequest = fetchedMergeRequestList[index];
			var currentGMergeRequest = saveGMergeRequestList[index];
			foreach (var fetchedMergeRequestLabel in fetchedMergeRequest.LabelList)
			{
				saveGMergeRequestLabelList.Add(GMergeRequestLabel.CreateDefault(currentGMergeRequest.Id, fetchedMergeRequestLabel.LabelName!));
			}
			foreach (var fetchedMergeRequestAssignee in fetchedMergeRequest.AssigneeList)
			{
				saveGMergeRequestAssigneeList.Add(GMergeRequestAssignee.CreateDefault(currentGMergeRequest.Id, fetchedMergeRequestAssignee.AssigneeUserId!.Value, fetchedMergeRequestAssignee.AssigneeUsername!));
			}
			foreach (var fetchedMergeRequestReviewer in fetchedMergeRequest.ReviewerList)
			{
				saveGMergeRequestReviewerList.Add(GMergeRequestReviewer.CreateDefault(currentGMergeRequest.Id, fetchedMergeRequestReviewer.ReviewerUserId!.Value, fetchedMergeRequestReviewer.ReviewerUsername!));
			}
			foreach (var fetchedMergeRequestThread in fetchedMergeRequest.ThreadList)
			{
				var createGMergeRequestThread = GMergeRequestThread.CreateDefault(currentGMergeRequest.Id, fetchedMergeRequestThread.GitlabThreadId!, fetchedMergeRequestThread.CreatedAt!.Value, fetchedMergeRequestThread.FirstAuthorUsername!, fetchedMergeRequestThread.FilePath ?? "", fetchedMergeRequestThread.LineNumber, fetchedMergeRequestThread.Resolvable ?? default, fetchedMergeRequestThread.Resolved ?? default, fetchedMergeRequestThread.CommentCount ?? default);
				saveGMergeRequestThreadList.Add(createGMergeRequestThread);
				saveGMergeRequestThreadPairList.Add((createGMergeRequestThread, fetchedMergeRequestThread));
			}
		}

		AbstractEntity.SaveAll(DbContextScope.GlobalContext, saveGMergeRequestLabelList);
		AbstractEntity.SaveAll(DbContextScope.GlobalContext, saveGMergeRequestAssigneeList);
		AbstractEntity.SaveAll(DbContextScope.GlobalContext, saveGMergeRequestReviewerList);
		AbstractEntity.SaveAll(DbContextScope.GlobalContext, saveGMergeRequestThreadList);

		var saveGMergeRequestCommentList = new List<GMergeRequestComment>();
		foreach (var saveGMergeRequestThreadPair in saveGMergeRequestThreadPairList)
		{
			foreach (var fetchedMergeRequestComment in saveGMergeRequestThreadPair.Fetched.CommentList)
			{
				saveGMergeRequestCommentList.Add(GMergeRequestComment.CreateDefault(saveGMergeRequestThreadPair.Entity.Id, fetchedMergeRequestComment.GitlabCommentId!.Value, fetchedMergeRequestComment.AuthorUsername!, fetchedMergeRequestComment.AuthorName ?? "", fetchedMergeRequestComment.Body ?? "", fetchedMergeRequestComment.CreatedAt!.Value, fetchedMergeRequestComment.UpdatedAt ?? default));
			}
		}

		AbstractEntity.SaveAll(DbContextScope.GlobalContext, saveGMergeRequestCommentList);

		transaction.Commit();
	}

}
