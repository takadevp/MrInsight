using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_merge_request_assigneeの設定クラス
/// </summary>
public class GMergeRequestAssigneeConfiguration : IEntityTypeConfiguration<GMergeRequestAssignee>
{
	public void Configure(EntityTypeBuilder<GMergeRequestAssignee> builder)
	{
		builder.ToTable("g_merge_request_assignee");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.MergeRequestId).HasColumnName("merge_request_id");
		builder.Property(entity => entity.AssigneeUserId).HasColumnName("assignee_user_id");
		builder.Property(entity => entity.AssigneeUsername).HasColumnName("assignee_username");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
