namespace MrInsight;

/// <summary>
/// 指定したプロジェクトのマージリクエストのうち、タイトル・本文・作成者ユーザー名に指定した文字を含み、作成日が指定した期間に入るものを、新しい順に取得するレスポンス
/// </summary>
public class ResponseMergeRequestGetProjectMergeRequestList
{
	/// <summary>
	/// マージリクエスト一覧
	/// </summary>
	public List<MergeRequest> MergeRequestList { get; set; } = [];

}
