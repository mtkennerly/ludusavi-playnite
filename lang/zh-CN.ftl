ludusavi = Ludusavi
file-filter-executable = 可执行文件
button-browse = 浏览
button-open = 打开
button-yes = 是
button-yes-remembered = 是，总是
button-no = 否
button-no-remembered = 否，永不
label-launch = 启动
badge-failed = 已失败
badge-ignored = 已忽略
needs-custom-entry =
    { $total-games ->
        [one] 该游戏需要的
       *[other] 某些游戏需要的
    } 某个匹配的自定义条目于 { ludusavi }.

## Backup

back-up-specific-game =
    .confirm = 要备份 { $game } 的存档数据吗？
    .on-success = 已备份 { $game } 的存档数据。（{ $processed-size }）
    .on-unchanged = { $game } 存档无变化，无需备份
    .on-empty = 没有找到 { $game } 可以备份的存档数据
    .on-failure = 已备份 { $game } 的存档（{ $total-size } 中的 { $processed-size }），但有些存档失败了
# Defers to `back-up-specific-game.*`.
back-up-last-game = 为最近游玩的游戏备份存档
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = 为所选游戏备份存档
    .confirm = 要备份 { $total-games } 所选游戏的存档数据吗？
back-up-all-games = 为所有游戏备份存档
    .confirm = 要备份Ludusavi能找到的所有游戏的存档吗？
    .on-success = 已备份游戏 { $processed-games } 的存档（{ $processed-size }）；点击查看完整列表
    .on-failure = 已备份 { $total-games } 里面 { $processed-games } 的存档（{ $total-size } 中的 { $processed-size }），但有些失败了；点击查看完整列表
back-up-during-play-on-success = 在游玩 { $game } 的时候触发了 { $total-backups } 备份，备份成功!
back-up-during-play-on-failure = 在游玩 { $game } 的时候触发了 { $total-backups } 备份, 但 { $failed-backups } 备份失败了!

## Restore

restore-specific-game =
    .confirm = 要恢复 { $game } 的存档数据吗？
    .on-success = 已恢复 { $game } 的存档数据。（{ $processed-size }）
    .on-unchanged = { $game } 已是最新存档，无需恢复
    .on-empty = 没有找到可以恢复的 { $game } 的存档数据
    .on-failure = 已恢复 { $game } 的存档（{ $total-size } 中的 { $processed-size }），但有些存档失败了
# Defers to `restore-specific-game.*`.
restore-last-game = 为最近游玩的游戏恢复存档
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = 为所选游戏恢复存档
    .confirm = 要恢复 { $total-games } 所选游戏的存档数据吗？
restore-all-games = 为所有游戏恢复存档
    .confirm = 要恢复Ludusavi能找到的所有游戏的存档吗？
    .on-success = 已恢复 { $processed-games } 的存档（{ $processed-size }）；点击查看完整列表
    .on-failure = 已恢复 { $total-games } 里面的 { $processed-games } 的存档（{ $total-size } 中的 { $processed-size }），但有些失败了；点击查看完整列表

## Tags

add-tag-for-selected-games = 标签："{ $tag }" - 为所选游戏添加
    .confirm = 是否为 { $total-games } 个所选游戏添加标签“{ $tag }”游戏并移除一切冲突标签？
remove-tag-for-selected-games = 标签："{ $tag }" - 从所选游戏移除
    .confirm = 是否从 { $total-games } 个所选游戏移除标签“{ $tag }”游戏并移除一切冲突标签？

## Generic errors

operation-still-pending = { ludusavi } 仍在处理前一个请求。请在看到它已完成的通知后再试。
no-game-played-yet = 您还未在此次会话中游玩任何游戏。
unable-to-run-ludusavi = 无法运行 { ludusavi }。
cannot-open-folder = 无法打开目录.
unable-to-synchronize-with-cloud = Unable to synchronize with cloud.
cloud-synchronize-conflict = Your local and cloud backups are in conflict. Open Ludusavi and perform an upload or download to resolve this.

## Settings

config-executable-path = Ludusavi可执行文件的名称或完整路径：
config-backup-path = 修改备份文件存放目录的完整路径：
config-override-backup-format = 修改备份文件格式
config-override-backup-compression = 修改备份文件的压缩方式
config-override-backup-retention = 修改备份文件选项:
config-full-backup-limit = 单个游戏的最大完整备份数量:
config-differential-backup-limit = Max differential backups per full backup:
config-do-backup-on-game-stopped = 在游戏结束后备份其存档数据
config-do-restore-on-game-starting = 在游戏开始前恢复其存档数据
config-ask-backup-on-game-stopped = 在游戏结束后选择是否备份其存档数据
config-only-backup-on-game-stopped-if-pc = 仅用于 PC 游戏结束后备份存档数据
config-retry-unrecognized-game-with-normalization = 若未找到游戏, 通过规范的标题重试
config-add-suffix-for-non-pc-game-names = 通过将此后缀添加到非 PC 游戏名称后面以检查之（需要自定义条目）：
config-retry-non-pc-games-without-suffix = 若未找到后缀，则不带后缀再试一次
config-do-platform-backup-on-non-pc-game-stopped = 在游玩非 PC 游戏后按平台名称备份存档数据（需要自定义条目）
config-do-platform-restore-on-non-pc-game-starting = 在游玩非 PC 游戏前同样按平台名称备份存档数据
config-ask-platform-backup-on-non-pc-game-stopped = 在非PC游戏结束时选择是否备份平台数据
config-do-backup-during-play = 在游戏过程中每隔一段时间备份游戏，如果游戏结束后也无需询问即可备份
config-ignore-benign-notifications = 仅在失败时显示通知
config-tag-games-with-backups = 自动给游戏存档备份打上标签 "{ $tag }"
config-tag-games-with-unknown-save-data = Automatically tag games with unknown save data as "{ $tag }"
label-minutes = 分钟:
option-simple = 简单
option-none = 默认

## Miscellaneous

upgrade-prompt = 安装 Ludusavi { $version } 或者更新的版本已获得更好的体验. 点击查看最新版本.
unrecognized-game = Ludusavi 不能识别 { $game }
look-up-as-other-title = 以其他名称查找
look-up-as-normal-title = 以默认名称查找
