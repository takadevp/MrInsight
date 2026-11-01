using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_merge_request_commentの設定クラス
/// </summary>
public class GMergeRequestCommentConfiguration : IEntityTypeConfiguration<GMergeRequestComment>
{
	public void Configure(EntityTypeBuilder<GMergeRequestComment> builder)
	{
		builder.ToTable("g_merge_request_comment");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.ThreadId).HasColumnName("thread_id");
		builder.Property(entity => entity.GitlabCommentId).HasColumnName("gitlab_comment_id");
		builder.Property(entity => entity.AuthorUsername).HasColumnName("author_username");
		builder.Property(entity => entity.AuthorName).HasColumnName("author_name");
		builder.Property(entity => entity.Body).HasColumnName("body");
		builder.Property(entity => entity.CreatedAt).HasColumnName("created_at");
		builder.Property(entity => entity.UpdatedAt).HasColumnName("updated_at");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
