using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// マージリクエストに付いたラベル1つ
/// </summary>
public class GMergeRequestLabel : AbstractEntity
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
	/// ラベル名
	/// </summary>
	public required string LabelName { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.MergeRequestLabels.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.MergeRequestLabels.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.MergeRequestLabels.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GMergeRequestLabel CreateDefault(int mergeRequestId, string labelName)
	{
		var entity = new GMergeRequestLabel
		{
			MergeRequestId = mergeRequestId,
			LabelName = labelName,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// マージリクエストIDで取得する
	/// </summary>
	public static List<GMergeRequestLabel> FindByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestLabelList = DbContextScope.GlobalContext.MergeRequestLabels
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.OrderBy(entity => entity.Id)
			.ToList();
		return targetGMergeRequestLabelList;
	}

	/// <summary>
	/// マージリクエストIDでまとめて削除する
	/// </summary>
	public static void DeleteByMergeRequestIdList(List<int> mergeRequestIdList)
	{
		var targetGMergeRequestLabelList = DbContextScope.GlobalContext.MergeRequestLabels
			.Where(entity => mergeRequestIdList.Contains(entity.MergeRequestId))
			.ToList();

		if (targetGMergeRequestLabelList.Count > 0)
		{
			DeleteAll(DbContextScope.GlobalContext, targetGMergeRequestLabelList);
		}
	}

}
