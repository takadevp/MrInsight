using Microsoft.EntityFrameworkCore;

namespace App.Common;

/// <summary>
/// エンティティの基底クラス
/// </summary>
public abstract class AbstractEntity
{
	/// <summary>
	/// 作成日時
	/// </summary>
	public DateTime CreateDate { get; set; }

	/// <summary>
	/// 更新日時
	/// </summary>
	public DateTime UpdateDate { get; set; }

	/// <summary>
	/// 状態
	/// </summary>
	protected EntityState _state = EntityState.Detached;

	/// <summary>
	/// 複数のエンティティをまとめて保存する
	/// </summary>
	/// <param name="context">保存先のDB接続</param>
	/// <param name="entityList">保存するエンティティの並び</param>
	public static void SaveAll(AbstractDbContext context, IEnumerable<AbstractEntity> entityList)
	{
		foreach (var entity in entityList)
		{
			if (entity._state == EntityState.Added)
			{
				context.Add(entity);
			}
			else
			{
				context.Update(entity);
			}
		}

		context.SaveChanges();
	}

	/// <summary>
	/// 複数のエンティティをまとめて削除する
	/// </summary>
	/// <param name="context">削除先のDB接続</param>
	/// <param name="entityList">削除するエンティティの並び</param>
	public static void DeleteAll(AbstractDbContext context, IEnumerable<AbstractEntity> entityList)
	{
		context.RemoveRange(entityList);
		context.SaveChanges();
	}
}
