# MrInsight

GitLabのマージリクエストを取り込み、レビュー内容を集計するWindowsのアプリです。

## セットアップ

1. リポジトリのルートで`build.cmd`を実行します。
2. `app/MrInsight.exe`・`app/settings.json`が作成されます。
3. `app/settings.json`に GitLab のURLとアクセストークンを書きます。
4. `app/MrInsight.exe`を起動します。

ビルドし直しても`settings.json`は上書きされません。

### 設定

```json
{
  "GitLabUrl": "",
  "GitLabAccessToken": "",
  "UseSample": false
}
```

| 項目 | 意味 |
|---|---|
| `GitLabUrl` | GitLabのURLです。 |
| `GitLabAccessToken` | GitLabのアクセストークンです。読むだけなので`read_api`の権限で足ります |
| `UseSample` | `true`にするとGitLabに接続せず、`sample/gitlab.json`の見本のデータを取り込みます。DB も見本用の別のファイルになり、ウィンドウの題名が「MrInsight 見本」になります |

### データの置き場所

設定・DB・ログは、exe の隣に置きます。取り込んだデータは`database/MrInsight.db`に、見本のときは`MrInsight-sample.db`に入ります。
DBのファイルを消せば、次に起動したときに空のDBが作られます。

| 置き場所 | 中身 |
|---|---|
| `settings.json` | 設定 |
| `database/` | 取り込んだデータ |
| `log/` | GitLabとの通信の記録 |

## バッジの書き方

レビューのコメント本文に`[<バッジ名>-badge]`と書きます。バッジ名に使えるのは半角英数字と`-`・`_`です。

```
[naming-badge] 変数名 tmpList では中身が分からないので、orderLineList のように何の一覧かを書いてください
```

1つのコメントに複数のバッジを書けます。同じバッジを何度書いても1件と数えます。
集計に数えるのは「バッジ」タブで登録したバッジだけです。

## クイックスタート

1. 「マージリクエスト」タブで「プロジェクトの取り込み」を押します。
2. プロジェクトを選び、「マージリクエストの取り込み」を押します。
3. 「バッジ」タブで「再読み込み」を押し、未登録のバッジ表から数えたいバッジを登録します。
4. 分析のタブで「再読み込み」を押し、プロジェクトを選びます。

ほかのタブで取り込みや登録をしたときは、見ているタブの「再読み込み」を押してください。
