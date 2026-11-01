using App.Common;
using Microsoft.EntityFrameworkCore;

namespace MrInsight;

/// <summary>
/// GitLab から取り込んだプロジェクト1つ
/// </summary>
public class GProject : AbstractEntity
{
	/// <summary>
	/// ID
	/// </summary>
	public int Id { get; init; }

	/// <summary>
	/// GitLab のプロジェクトのID
	/// </summary>
	public int ProjectId { get; set; }

	/// <summary>
	/// プロジェクト名
	/// </summary>
	public required string ProjectName { get; set; }

	/// <summary>
	/// グループを含めたパス
	/// </summary>
	public required string ProjectPath { get; set; }

	/// <summary>
	/// GitLab の画面のURL
	/// </summary>
	public required string WebUrl { get; set; }

	/// <summary>
	/// 保存する
	/// </summary>
	public void Save()
	{
		if (_state == EntityState.Added)
		{
			DbContextScope.GlobalContext.Projects.Add(this);
		}
		else
		{
			DbContextScope.GlobalContext.Projects.Update(this);
		}

		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// 削除する
	/// </summary>
	public void Delete()
	{
		DbContextScope.GlobalContext.Projects.Remove(this);
		DbContextScope.GlobalContext.SaveChanges();
	}

	/// <summary>
	/// デフォルト値で生成する
	/// </summary>
	public static GProject CreateDefault(int projectId, string projectName, string projectPath, string webUrl)
	{
		var entity = new GProject
		{
			ProjectId = projectId,
			ProjectName = projectName,
			ProjectPath = projectPath,
			WebUrl = webUrl,
		};
		entity._state = EntityState.Added;
		return entity;
	}

	/// <summary>
	/// プロジェクトIDで取得する
	/// </summary>
	public static List<GProject> FindByProjectIdList(List<int> projectIdList)
	{
		var targetGProjectList = DbContextScope.GlobalContext.Projects
			.Where(entity => projectIdList.Contains(entity.ProjectId))
			.OrderBy(entity => entity.ProjectPath)
			.ToList();
		return targetGProjectList;
	}

	/// <summary>
	/// 全件取得する
	/// </summary>
	public static List<GProject> GetAll()
	{
		var targetGProjectList = DbContextScope.GlobalContext.Projects
			.OrderBy(entity => entity.ProjectPath)
			.ToList();
		return targetGProjectList;
	}

}
