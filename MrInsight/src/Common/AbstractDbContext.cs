using Microsoft.EntityFrameworkCore;

namespace App.Common;

/// <summary>
/// DB接続の基底クラス。
/// 接続先の DB と型変換はアプリの ApplicationDbContext が継承して決める
/// </summary>
public abstract class AbstractDbContext : DbContext
{
	/// <summary>
	/// 設定。
	/// 継承先は base を呼び、接続先を足す
	/// </summary>
	/// <param name="optionsBuilder">接続とクエリの設定を受け取るビルダー</param>
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
	}

	/// <summary>
	/// 保存時の処理
	/// </summary>
	/// <returns>書き込んだ行数</returns>
	/// <exception cref="AppException">トランザクションが開始されていない場合</exception>
	public sealed override int SaveChanges()
	{
		ValidateBeginTransaction();
		SetTimestamps();

		return base.SaveChanges();
	}

	/// <summary>
	/// 保存時の処理
	/// </summary>
	/// <param name="cancellationToken">中断を伝えるトークン</param>
	/// <returns>書き込んだ行数</returns>
	/// <exception cref="AppException">トランザクションが開始されていない場合</exception>
	public sealed override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
	{
		ValidateBeginTransaction();
		SetTimestamps();

		return base.SaveChangesAsync(cancellationToken);
	}

	/// <summary>
	/// トランザクションが開始されていることを確認する
	/// </summary>
	/// <exception cref="AppException">トランザクションが開始されていない場合</exception>
	private void ValidateBeginTransaction()
	{
		if (Database.CurrentTransaction == null)
		{
			throw new AppException("トランザクションが開始されていません。保存する前にトランザクションを開始してください。");
		}
	}

	/// <summary>
	/// 作成日時・更新日時を設定する
	/// </summary>
	private void SetTimestamps()
	{
		var entryList = ChangeTracker.Entries<AbstractEntity>();
		var now = DateTime.Now;

		foreach (var entry in entryList)
		{
			if (entry.State == EntityState.Added)
			{
				entry.Entity.CreateDate = now;
				entry.Entity.UpdateDate = now;
			}
			else if (entry.State == EntityState.Modified)
			{
				entry.Entity.UpdateDate = now;
			}
		}
	}
}
