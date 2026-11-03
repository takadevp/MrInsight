using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// 集計から除くマージリクエストの、タイトルの先頭の文字1つ
/// </summary>
public class GExcludedTitle : AbstractEntity
{
	/// <summary>
	/// ID
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// タイトルがこの文字で始まるマージリクエストを除く
	/// </summary>
	public required string TitlePrefix { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.ExcludedTitles.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.ExcludedTitles.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.ExcludedTitles.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GExcludedTitle CreateDefault(string titlePrefix)
	{
		var entity = new GExcludedTitle
		{
			TitlePrefix = titlePrefix,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// 全件取得する
	/// </summary>
	public static List<GExcludedTitle> GetAll()
	{
		var targetGExcludedTitleList = DbContextScope.GlobalContext.ExcludedTitles
			.OrderBy(entity => entity.TitlePrefix)
			.ToList();
		return targetGExcludedTitleList;
	}

	/// <summary>
	/// タイトルの先頭で先頭の1件を取得する
	/// </summary>
	public static GExcludedTitle? FindFirstByTitlePrefix(string titlePrefix)
	{
		var targetGExcludedTitle = DbContextScope.GlobalContext.ExcludedTitles
			.Where(entity => entity.TitlePrefix == titlePrefix)
			.FirstOrDefault();
		return targetGExcludedTitle;
	}

	/// <summary>
	/// IDで先頭の1件を取得する
	/// </summary>
	public static GExcludedTitle? FindById(int id)
	{
		var targetGExcludedTitle = DbContextScope.GlobalContext.ExcludedTitles
			.Where(entity => entity.Id == id)
			.FirstOrDefault();
		return targetGExcludedTitle;
	}

}
