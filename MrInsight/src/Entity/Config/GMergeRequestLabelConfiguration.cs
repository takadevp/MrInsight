using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_merge_request_labelの設定クラス
/// </summary>
public class GMergeRequestLabelConfiguration : IEntityTypeConfiguration<GMergeRequestLabel>
{
	public void Configure(EntityTypeBuilder<GMergeRequestLabel> builder)
	{
		builder.ToTable("g_merge_request_label");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.MergeRequestId).HasColumnName("merge_request_id");
		builder.Property(entity => entity.LabelName).HasColumnName("label_name");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
