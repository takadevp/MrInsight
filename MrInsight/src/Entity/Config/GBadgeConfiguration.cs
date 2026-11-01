using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_badgeの設定クラス
/// </summary>
public class GBadgeConfiguration : IEntityTypeConfiguration<GBadge>
{
	public void Configure(EntityTypeBuilder<GBadge> builder)
	{
		builder.ToTable("g_badge");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.BadgeName).HasColumnName("badge_name");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
