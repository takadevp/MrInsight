using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_merge_requestの設定クラス
/// </summary>
public class GMergeRequestConfiguration : IEntityTypeConfiguration<GMergeRequest>
{
	public void Configure(EntityTypeBuilder<GMergeRequest> builder)
	{
		builder.ToTable("g_merge_request");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.GitlabId).HasColumnName("gitlab_id");
		builder.Property(entity => entity.ProjectId).HasColumnName("project_id");
		builder.Property(entity => entity.Iid).HasColumnName("iid");
		builder.Property(entity => entity.Title).HasColumnName("title");
		builder.Property(entity => entity.Description).HasColumnName("description");
		builder.Property(entity => entity.State).HasColumnName("state");
		builder.Property(entity => entity.Draft).HasColumnName("draft");
		builder.Property(entity => entity.AuthorId).HasColumnName("author_id");
		builder.Property(entity => entity.AuthorUsername).HasColumnName("author_username");
		builder.Property(entity => entity.AuthorName).HasColumnName("author_name");
		builder.Property(entity => entity.SourceBranch).HasColumnName("source_branch");
		builder.Property(entity => entity.TargetBranch).HasColumnName("target_branch");
		builder.Property(entity => entity.Milestone).HasColumnName("milestone");
		builder.Property(entity => entity.CreatedAt).HasColumnName("created_at");
		builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at");
		builder.Property(entity => entity.MergedAt).HasColumnName("merged_at");
		builder.Property(entity => entity.ClosedAt).HasColumnName("closed_at");
		builder.Property(entity => entity.MergedByUsername).HasColumnName("merged_by_username");
		builder.Property(entity => entity.CommentCount).HasColumnName("comment_count");
		builder.Property(entity => entity.UpvoteCount).HasColumnName("upvote_count");
		builder.Property(entity => entity.DownvoteCount).HasColumnName("downvote_count");
		builder.Property(entity => entity.AdditionCount).HasColumnName("addition_count");
		builder.Property(entity => entity.DeletionCount).HasColumnName("deletion_count");
		builder.Property(entity => entity.ChangedFileCount).HasColumnName("changed_file_count");
		builder.Property(entity => entity.HasConflicts).HasColumnName("has_conflicts");
		builder.Property(entity => entity.WebUrl).HasColumnName("web_url");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
