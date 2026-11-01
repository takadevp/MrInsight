using App.Common;

namespace MrInsight;

public class GitLabStatusException(string message, object context) : AppException(message, context, 400)
{
}
