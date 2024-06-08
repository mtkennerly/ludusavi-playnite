ludusavi = Ludusavi
file-filter-executable = 実行ファイル
button-browse = 参照
button-open = 開く
button-yes = はい
button-yes-remembered = 常にはい
button-no = いいえ
button-no-remembered = 常にいいえ
label-launch = 実行
badge-failed = 失敗
badge-ignored = 無効
needs-custom-entry =
    { $total-games ->
        [one] このゲームには
       *[other] ゲームによっては
    } Ludusavi のカスタムエントリが一致する必要があります。

## Backup

back-up-specific-game =
    .confirm = { $game } のセーブデータをバックアップしますか？
    .on-success = { $game } ({ $processed-size }) のセーブデータをバックアップしました
    .on-unchanged = { $game } をバックアップする新しいものはありません
    .on-empty = { $game } にバックアップするセーブデータが見つかりませんでした
    .on-failure = { $game } ({ $processed-size } のうち { $total-size }) のセーブデータをバックアップしましたが、一部失敗しました
# Defers to `back-up-specific-game.*`.
back-up-last-game = 最後にプレイしたゲームのセーブデータデータをバックアップする
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = 選択したゲームのセーブデータをバックアップする
    .confirm = { $total-games } 選択したゲームのセーブデータをバックアップしますか？
back-up-all-games = すべてのゲームのセーブデータをバックアップする
    .confirm = Ludusaviが見つけられるすべてのゲームのセーブデータをバックアップしますか?
    .on-success = { $processed-games } ゲームのセーブデータをバックアップしました ({ $processed-size }); クリックすると全リストが表示されます
    .on-failure = { $processed-games } ゲーム中 { $total-games } ゲーム ({ $processed-size } のうち { $total-size }) のセーブデータをバックアップしましたが、一部失敗しました。クリックすると全リストが表示されます
back-up-during-play-on-success = { $game } をプレイ中に { $total-backups } バックアップを実行しました
back-up-during-play-on-failure = { $game } をプレイ中に { $total-backups } バックアップを実行しました。そのうち { $failed-backups } が失敗しました

## Restore

restore-specific-game =
    .confirm = { $game } のセーブデータを復元しますか？
    .on-success = { $game } ({ $processed-size } ) のセーブデータを復元しました
    .on-unchanged = { $game } に復元する新しいものはありません
    .on-empty = { $game } に復元するセーブデータが見つかりません
    .on-failure = { $game } ({ $processed-size } のうち { $total-size }) のセーブデータを復元しましたが、一部失敗しました
# Defers to `restore-specific-game.*`.
restore-last-game = 最後にプレイしたゲームのセーブデータを復元する
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = 選択したゲームのセーブデータを復元
    .confirm = { $total-games } 選択したゲームのセーブデータを復元しますか？
restore-all-games = すべてのゲームのセーブデータを復元
    .confirm = Ludusaviが見つけられるすべてのゲームのセーブデータを復元しますか?
    .on-success = { $processed-games } ゲームのセーブデータを復元しました ({ $processed-size }); クリックすると全リストが表示されます
    .on-failure = { $processed-games } ゲーム中 { $total-games } ゲーム ({ $processed-size } のうち { $total-size }) のセーブデータを復元しましたが、一部失敗しました。クリックすると全リストが表示されます

## Tags

add-tag-for-selected-games = タグ: "{ $tag }" - 選択したゲームに追加
    .confirm =
        "{ $tag }" タグを { $total-games } 選択した{ $total-games ->
            [one] ゲーム
           *[other] ゲーム
        } に追加し、競合するタグを削除しますか？
remove-tag-for-selected-games = タグ: "{ $tag }" - 選択したゲームから削除
    .confirm =
        "{ $tag }" タグを { $total-games } 選択した{ $total-games ->
            [one] ゲーム
           *[other] ゲーム
        } から削除し、競合するタグを削除しますか？

## Generic errors

operation-still-pending = Ludusavi は実行中です。完了の通知が表示されてから、もう一度お試しください。
no-game-played-yet = このセッションではまだ何もプレイしていません。
unable-to-run-ludusavi = Ludusavi を実行できません。
cannot-open-folder = フォルダを開くことができません。
unable-to-synchronize-with-cloud = Unable to synchronize with cloud.
cloud-synchronize-conflict = Your local and cloud backups are in conflict. Open Ludusavi and perform an upload or download to resolve this.

## Settings

config-executable-path = Ludusavi 実行ファイルの名前またはフルパス:
config-backup-path = バックアップを保存するディレクトリへのフルパスを選択する:
config-override-backup-format = バックアップ形式を選択する。
config-override-backup-compression = バックアップの圧縮形式を選択する。
config-override-backup-retention = バックアップの保持を有効にする。
config-full-backup-limit = ゲームごとの最大バックアップ数:
config-differential-backup-limit = バックアップあたりの差分バックアップの最大数:
config-do-backup-on-game-stopped = ゲームをプレイした後にセーブデータをバックアップする
config-do-restore-on-game-starting = ゲームをプレイする前にセーブデータを復元する
config-ask-backup-on-game-stopped = 自動で実行する前に確認する
config-only-backup-on-game-stopped-if-pc = PCゲームでのみ行う
config-retry-unrecognized-game-with-normalization = 見つからない場合は、タイトルを正規化して再試行します
config-add-suffix-for-non-pc-game-names = Look up non-PC games by adding this suffix to their names (requires custom entry):
config-retry-non-pc-games-without-suffix = If not found with the suffix, then try again without it
config-do-platform-backup-on-non-pc-game-stopped = PC以外のゲームをプレイした後、プラットフォーム名でデータをバックアップする (カスタム入力が必要)
config-do-platform-restore-on-non-pc-game-starting = PC以外のゲームをプレイする前に、プラットフォーム名でセーブデータを復元する
config-ask-platform-backup-on-non-pc-game-stopped = 自動で実行する前に確認する
config-do-backup-during-play = プレイ中もセーブデータをバックアップし、プレイ後も自動でバックアップする
config-ignore-benign-notifications = 失敗の通知のみ表示
config-tag-games-with-backups = バックアップしたゲームに "{$tag}" のタグを自動的に付ける
config-tag-games-with-unknown-save-data = Automatically tag games with unknown save data as "{ $tag }"
label-minutes = 分:
option-simple = Simple
option-none = None

## Miscellaneous

initial-setup-required = Ludusavi does not seem to be installed. Please download it and then follow the plugin setup instructions.
upgrade-prompt = 最新のLudesavi {$version} が利用可能です。 クリックすると最新のリリースが表示されます。
unrecognized-game = Ludusaviは {$game} を認識していません
look-up-as-other-title = 別のタイトルで検索
look-up-as-normal-title = デフォルトのタイトルで検索
open-backup-directory = Open backup directory
