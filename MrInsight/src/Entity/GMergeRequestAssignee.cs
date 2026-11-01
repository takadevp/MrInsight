using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// マージリクエストに割り当てられた人1人
/// </summary>
public class GMergeRequestAssignee : AbstractEntity
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
	/// GitLab のユーザーID
	/// </summary>
	public int AssigneeUserId { get; set; }

	/// <summary>
	/// アサインユーザー名
	/// </summary>
	public required string AssigneeUsername { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.MergeRequestAssignees.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.MergeRequestAssignees.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.MergeRequestAssignees.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GMergeRequestAssignee CreateDefault(int mergeRequestId, int assigneeUserId, string assigneeUsername)
	{
		var entity = new GMergeRequestAssignee
		{
			MergeRequestId = mergeRequestId,
			AssigneeUserId = assigneeUserId,
			AssigneeUsername = assigneeUsername,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// マージリクエストIDで取得する
	/// </summary>
	public static List<GMergeRequestAssignee> FindByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestAssigneeList = DbContextScope.GlobalContext.MergeRequestAssignees
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.OrderBy(entity => entity.Id)
			.ToList();
		return targetGMergeRequestAssigneeList;
	}

	/// <summary>
	/// マージリクエストIDでまとめて削除する
	/// </summary>
	public static void DeleteByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestAssigneeList = DbContextScope.GlobalContext.MergeRequestAssignees
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.ToList();

		if (targetGMergeRequestAssigneeList.Count > 0)
		{
			DeleteAll(DbContextScope.GlobalContext, targetGMergeRequestAssigneeList);
		}
	}

}
