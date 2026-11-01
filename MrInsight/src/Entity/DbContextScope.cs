using App.Common;

namespace MrInsight;

public static class DbContextScope
{
	private static readonly AsyncLocal<ApplicationGlobalDbContext?> _globalContext = new();

	public static ApplicationGlobalDbContext GlobalContext
	{
		get => _globalContext.Value ?? throw new AppException("GlobalContextが設定されていません。DatabaseTransaction.Begin() を先に呼んでください。");
		set => _globalContext.Value = value;
	}

	public static void Clear()
	{
		_globalContext.Value = null;
	}
}
