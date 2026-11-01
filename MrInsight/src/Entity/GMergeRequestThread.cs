using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// マージリクエストのコメントのスレッド1つ。
/// コードへのコメントなら、ファイルと行を持つ
/// </summary>
public class GMergeRequestThread : AbstractEntity
{
	/// <summary>
	/// ID
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// 所属するマージリクエスト
	/// </summary>
	public int MergeRequestId { get; set; }

	/// <summary>
	/// GitLab のディスカッションのID
	/// </summary>
	public required string GitlabThreadId { get; set; }

	/// <summary>
	/// 最初のコメントが書かれた日時
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// 最初に書いた人のユーザー名
	/// </summary>
	public required string FirstAuthorUsername { get; set; }

	/// <summary>
	/// コードへのコメントなら対象のファイル。
	/// 無ければ空
	/// </summary>
	public required string FilePath { get; set; }

	/// <summary>
	/// コードへのコメントなら対象の行。
	/// 無ければ空
	/// </summary>
	public int? LineNumber { get; set; }

	/// <summary>
	/// 解決済みにできるスレッドか
	/// </summary>
	public bool Resolvable { get; set; }

	/// <summary>
	/// 解決済み
	/// </summary>
	public bool Resolved { get; set; }

	/// <summary>
	/// スレッドの中の、人が書いたコメントの数
	/// </summary>
	public int CommentCount { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.MergeRequestThreads.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.MergeRequestThreads.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.MergeRequestThreads.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GMergeRequestThread CreateDefault(int mergeRequestId, string gitlabThreadId, DateTime createdAt, string firstAuthorUsername, string filePath, int? lineNumber, bool resolvable, bool resolved, int commentCount)
	{
		var entity = new GMergeRequestThread
		{
			MergeRequestId = mergeRequestId,
			GitlabThreadId = gitlabThreadId,
			CreatedAt = createdAt,
			FirstAuthorUsername = firstAuthorUsername,
			FilePath = filePath,
			LineNumber = lineNumber,
			Resolvable = resolvable,
			Resolved = resolved,
			CommentCount = commentCount,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// マージリクエストIDで取得する
	/// </summary>
	public static List<GMergeRequestThread> FindByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestThreadList = DbContextScope.GlobalContext.MergeRequestThreads
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.OrderBy(entity => entity.CreatedAt)
			.ToList();
		return targetGMergeRequestThreadList;
	}

	/// <summary>
	/// マージリクエストIDで取得する
	/// </summary>
	public static List<GMergeRequestThread> FindByMergeRequestId(int mergeRequestId)
	{
		var targetGMergeRequestThreadList = DbContextScope.GlobalContext.MergeRequestThreads
			.Where(entity => entity.MergeRequestId == mergeRequestId)
			.OrderBy(entity => entity.CreatedAt)
			.ToList();
		return targetGMergeRequestThreadList;
	}

	/// <summary>
	/// マージリクエストIDで主キーの一覧を取得する
	/// </summary>
	public static List<int> FindIdListByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestThreadIdList = DbContextScope.GlobalContext.MergeRequestThreads
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.Select(entity => entity.Id)
			.ToList();
		return targetGMergeRequestThreadIdList;
	}

	/// <summary>
	/// マージリクエストIDでまとめて削除する
	/// </summary>
	public static void DeleteByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestThreadList = DbContextScope.GlobalContext.MergeRequestThreads
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.ToList();

		if (targetGMergeRequestThreadList.Count > 0)
		{
			DeleteAll(DbContextScope.GlobalContext, targetGMergeRequestThreadList);
		}
	}

}
