using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_excluded_titleの設定クラス
/// </summary>
public class GExcludedTitleConfiguration : IEntityTypeConfiguration<GExcludedTitle>
{
	public void Configure(EntityTypeBuilder<GExcludedTitle> builder)
	{
		builder.ToTable("g_excluded_title");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.TitlePrefix).HasColumnName("title_prefix");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
