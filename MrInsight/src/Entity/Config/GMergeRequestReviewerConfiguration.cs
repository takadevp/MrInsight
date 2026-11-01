using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_merge_request_reviewerの設定クラス
/// </summary>
public class GMergeRequestReviewerConfiguration : IEntityTypeConfiguration<GMergeRequestReviewer>
{
	public void Configure(EntityTypeBuilder<GMergeRequestReviewer> builder)
	{
		builder.ToTable("g_merge_request_reviewer");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.MergeRequestId).HasColumnName("merge_request_id");
		builder.Property(entity => entity.ReviewerUserId).HasColumnName("reviewer_user_id");
		builder.Property(entity => entity.ReviewerUsername).HasColumnName("reviewer_username");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
