using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// 集計に数えるバッジ1つ。
/// コメント本文に [design-badge] と書いたときの design の部分
/// </summary>
public class GBadge : AbstractEntity
{
	/// <summary>
	/// ID
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// バッジ名
	/// </summary>
	public required string BadgeName { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.Badges.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.Badges.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.Badges.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GBadge CreateDefault(string badgeName)
	{
		var entity = new GBadge
		{
			BadgeName = badgeName,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// 全件取得する
	/// </summary>
	public static List<GBadge> GetAll()
	{
		var targetGBadgeList = DbContextScope.GlobalContext.Badges
			.OrderBy(entity => entity.BadgeName)
			.ToList();
		return targetGBadgeList;
	}

	/// <summary>
	/// バッジ名で先頭の1件を取得する
	/// </summary>
	public static GBadge? FindFirstByBadgeName(string badgeName)
	{
		var targetGBadge = DbContextScope.GlobalContext.Badges
			.Where(entity => entity.BadgeName == badgeName)
			.FirstOrDefault();
		return targetGBadge;
	}

	/// <summary>
	/// IDで先頭の1件を取得する
	/// </summary>
	public static GBadge? FindById(int id)
	{
		var targetGBadge = DbContextScope.GlobalContext.Badges
			.Where(entity => entity.Id == id)
			.FirstOrDefault();
		return targetGBadge;
	}

}
