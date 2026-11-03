using System;

namespace App.Common
{
	/// <summary>
	/// アプリケーションの例外クラス
	/// </summary>
	public class AppException : Exception
	{
		// Unity のように nullable を有効にしていない生成先に写すと、注釈が CS8632 の警告になるので抑える
#pragma warning disable CS8632
		/// <summary>
		/// 追加情報
		/// </summary>
		public object? Context { get; }
#pragma warning restore CS8632

		/// <summary>
		/// HTTPステータスコード。既定の500は実装のバグを表す想定外のエラー。
		/// クライアントの入力・通信状況に起因する想定内のエラーは400を指定する
		/// </summary>
		public int StatusCode { get; }

		// 上の Context と同じ理由で抑える
#pragma warning disable CS8632
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="message">利用者に表示するメッセージ</param>
		/// <param name="context">追加情報</param>
		/// <param name="statusCode">HTTPステータスコード。既定は500</param>
		/// <example>
		/// throw new AppException("カテゴリが見つかりません。", new { id, values });
		/// throw new AppException("ナンスが見つかりません。", new { nonce }, statusCode: 400);
		/// </example>
		public AppException(string message, object? context = null, int statusCode = 500) : base(message)
#pragma warning restore CS8632
		{
			Context = context;
			StatusCode = statusCode;
		}
	}
}
