-- GitLab から取り込んだマージリクエスト1件
CREATE TABLE IF NOT EXISTS g_merge_request (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    gitlab_id INTEGER NOT NULL,
    project_id INTEGER NOT NULL,
    iid INTEGER NOT NULL,
    title TEXT NOT NULL,
    description TEXT NOT NULL,
    state INTEGER NOT NULL,
    draft INTEGER NOT NULL,
    author_id INTEGER NOT NULL,
    author_username TEXT NOT NULL,
    author_name TEXT NOT NULL,
    source_branch TEXT NOT NULL,
    target_branch TEXT NOT NULL,
    milestone TEXT NOT NULL,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL,
    merged_at TEXT NULL,
    closed_at TEXT NULL,
    merged_by_username TEXT NOT NULL,
    comment_count INTEGER NOT NULL,
    upvote_count INTEGER NOT NULL,
    downvote_count INTEGER NOT NULL,
    addition_count INTEGER NULL,
    deletion_count INTEGER NULL,
    changed_file_count INTEGER NULL,
    has_conflicts INTEGER NOT NULL,
    web_url TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE UNIQUE INDEX IF NOT EXISTS uq_g_merge_request_01 ON g_merge_request (gitlab_id);
CREATE INDEX IF NOT EXISTS idx_g_merge_request_01 ON g_merge_request (project_id, author_id);

-- マージリクエストに付いたラベル1つ
CREATE TABLE IF NOT EXISTS g_merge_request_label (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    merge_request_id INTEGER NOT NULL,
    label_name TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_g_merge_request_label_01 ON g_merge_request_label (merge_request_id);

-- マージリクエストに割り当てられた人1人
CREATE TABLE IF NOT EXISTS g_merge_request_assignee (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    merge_request_id INTEGER NOT NULL,
    assignee_user_id INTEGER NOT NULL,
    assignee_username TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_g_merge_request_assignee_01 ON g_merge_request_assignee (merge_request_id);

-- マージリクエストのレビューを頼まれた人1人
CREATE TABLE IF NOT EXISTS g_merge_request_reviewer (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    merge_request_id INTEGER NOT NULL,
    reviewer_user_id INTEGER NOT NULL,
    reviewer_username TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_g_merge_request_reviewer_01 ON g_merge_request_reviewer (merge_request_id);

-- マージリクエストのコメントのスレッド1つ。コードへのコメントなら、ファイルと行を持つ
CREATE TABLE IF NOT EXISTS g_merge_request_thread (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    merge_request_id INTEGER NOT NULL,
    gitlab_thread_id TEXT NOT NULL,
    created_at TEXT NOT NULL,
    first_author_username TEXT NOT NULL,
    file_path TEXT NOT NULL,
    line_number INTEGER NULL,
    resolvable INTEGER NOT NULL,
    resolved INTEGER NOT NULL,
    comment_count INTEGER NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_g_merge_request_thread_01 ON g_merge_request_thread (merge_request_id);

-- スレッドの中のコメント1件。GitLab が自動で書く記録は含めない
CREATE TABLE IF NOT EXISTS g_merge_request_comment (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    thread_id INTEGER NOT NULL,
    gitlab_comment_id INTEGER NOT NULL,
    author_username TEXT NOT NULL,
    author_name TEXT NOT NULL,
    body TEXT NOT NULL,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IF NOT EXISTS idx_g_merge_request_comment_01 ON g_merge_request_comment (thread_id);

-- GitLab から取り込んだプロジェクト1つ
CREATE TABLE IF NOT EXISTS g_project (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    project_id INTEGER NOT NULL,
    project_name TEXT NOT NULL,
    project_path TEXT NOT NULL,
    web_url TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE UNIQUE INDEX IF NOT EXISTS uq_g_project_01 ON g_project (project_id);

-- 集計に数えるバッジ1つ。コメント本文に [design-badge] と書いたときの design の部分
CREATE TABLE IF NOT EXISTS g_badge (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    badge_name TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE UNIQUE INDEX IF NOT EXISTS uq_g_badge_01 ON g_badge (badge_name);

-- 集計から除くマージリクエストの、タイトルの先頭の文字1つ
CREATE TABLE IF NOT EXISTS g_excluded_title (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    title_prefix TEXT NOT NULL,
    create_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    update_date TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE UNIQUE INDEX IF NOT EXISTS uq_g_excluded_title_01 ON g_excluded_title (title_prefix);
