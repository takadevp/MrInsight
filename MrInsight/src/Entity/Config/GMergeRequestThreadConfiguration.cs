using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MrInsight;

/// <summary>
/// g_merge_request_threadの設定クラス
/// </summary>
public class GMergeRequestThreadConfiguration : IEntityTypeConfiguration<GMergeRequestThread>
{
	public void Configure(EntityTypeBuilder<GMergeRequestThread> builder)
	{
		builder.ToTable("g_merge_request_thread");

		builder.HasKey(entity => entity.Id);

		// カラム名のマッピング
		builder.Property(entity => entity.Id).HasColumnName("id").ValueGeneratedOnAdd();
		builder.Property(entity => entity.MergeRequestId).HasColumnName("merge_request_id");
		builder.Property(entity => entity.GitlabThreadId).HasColumnName("gitlab_thread_id");
		builder.Property(entity => entity.CreatedAt).HasColumnName("created_at");
		builder.Property(entity => entity.FirstAuthorUsername).HasColumnName("first_author_username");
		builder.Property(entity => entity.FilePath).HasColumnName("file_path");
		builder.Property(entity => entity.LineNumber).HasColumnName("line_number");
		builder.Property(entity => entity.Resolvable).HasColumnName("resolvable");
		builder.Property(entity => entity.Resolved).HasColumnName("resolved");
		builder.Property(entity => entity.CommentCount).HasColumnName("comment_count");
		builder.Property(entity => entity.CreateDate).HasColumnName("create_date");
		builder.Property(entity => entity.UpdateDate).HasColumnName("update_date");
	}
}
