using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_projectの設定クラス
/// </summary>
public class GProjectConfiguration : IEntityTypeConfiguration<GProject>
{
	public void Configure(EntityTypeBuilder<GProject> builder)
	{
		builder.ToTable("g_project");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.ProjectId).HasColumnName("project_id");
		builder.Property(entity => entity.ProjectName).HasColumnName("project_name");
		builder.Property(entity => entity.ProjectPath).HasColumnName("project_path");
		builder.Property(entity => entity.WebUrl).HasColumnName("web_url");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
