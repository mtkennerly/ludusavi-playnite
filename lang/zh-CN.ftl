ludusavi = 录读加一-Ludusavi
file-filter-executable = 可执行文件
button-browse = 浏览
button-open = 打开
button-yes = 是
button-yes-remembered = 是，总是
button-no = 否
button-no-remembered = 否，永不
label-launch = 启动
may-need-custom-entry =
    { $total-custom-games ->
        [0] { "" }
       *[other]
            { $total-games ->
                [one] 该游戏需要的
               *[other] 某些游戏需要的
            } 某个匹配的自定义条目于 { ludusavi }.
    }

## Backup

back-up-specific-game =
    .confirm = 要备份 { $game } 的存档数据吗？ { may-need-custom-entry }
    .on-success = 已备份 { $game } 的存档数据。（{ $processed-size }）
    .on-empty = 没有找到可以备份的 { $game } 的存档数据
    .on-failure = 已备份 { $game } 的存档（{ $total-size } 中的 { $processed-size }），但有些存档失败了
# Defers to `back-up-specific-game.*`.
back-up-last-game = 为最近游玩的游戏备份存档
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = 为所选游戏备份存档
    .confirm = 要备份 { $total-games } 所选游戏的存档数据吗？ { may-need-custom-entry }
back-up-all-games = 为所有游戏备份存档
    .confirm = 要备份录读加一能找到的所有游戏的存档吗？
    .on-success = 已备份 { $processed-games } 游戏的存档（{ $processed-size }）；点击查看完整列表
    .on-failure = 已备份 { $total-games } 中的 { $processed-games } 游戏的存档（{ $total-size } 中的 { $processed-size }），但有些失败了；点击查看完整列表

## Restore

restore-specific-game =
    .confirm = 要恢复 { $game } 的存档数据吗？ { may-need-custom-entry }
    .on-success = 已恢复 { $game } 的存档数据。（{ $processed-size }）
    .on-empty = 没有找到可以恢复的 { $game } 的存档数据
    .on-failure = 已恢复 { $game } 的存档（{ $total-size } 中的 { $processed-size }），但有些存档失败了
# Defers to `restore-specific-game.*`.
restore-last-game = 为最近游玩的游戏恢复存档
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = 为所选游戏恢复存档
    .confirm = 要恢复 { $total-games } 所选游戏的存档数据吗？ { may-need-custom-entry }
restore-all-games = 为所有游戏恢复存档
    .confirm = 要恢复录读加一能找到的所有游戏的存档吗？
    .on-success = 已恢复 { $processed-games } 游戏的存档（{ $processed-size }）；点击查看完整列表
    .on-failure = 已恢复 { $total-games } 中的 { $processed-games } 游戏的存档（{ $total-size } 中的 { $processed-size }），但有些失败了；点击查看完整列表

## Tags

add-tag-for-selected-games = 标签："{ $tag }" - 为所选游戏添加
    .confirm = 是否为 { $total-games } 个所选游戏添加标签“{ $tag }”游戏并移除一切冲突标签？
remove-tag-for-selected-games = 标签："{ $tag }" - 从所选游戏移除
    .confirm = 是否从 { $total-games } 个所选游戏移除标签“{ $tag }”游戏并移除一切冲突标签？

## Generic errors

operation-still-pending = { ludusavi } 仍在处理前一请求。请在看到它已完成的通知后再试。
no-game-played-yet = 您还未在此次会话中玩任何东西。
unable-to-run-ludusavi = 无法运行 { ludusavi }。

## Full backup/restore error reporting

full-list-game-line-item =
    { $status ->
        [failed] [已失败] { $game } ({ $size })
        [ignored] [已忽略] { $game } ({ $size })
       *[success] { $game } ({ $size })
    }

## Settings

config-executable-path = 录读加一可执行文件的名称或完整路径：
config-backup-path = 存储备份的目录的完整路径：
config-do-backup-on-game-stopped = 在玩一个游戏之后备份其存档数据
config-do-restore-on-game-starting = 在玩一个游戏之前同样恢复其存档数据
config-ask-backup-on-game-stopped = “先奏后斩”
config-only-backup-on-game-stopped-if-pc = 仅用于 PC 游戏
config-add-suffix-for-non-pc-game-names = 通过将此后缀添加到非 PC 游戏名称后面以检查之（需要自定义条目）：
config-retry-non-pc-games-without-suffix = 若未找到后缀，则不带后缀再试一次
config-do-platform-backup-on-non-pc-game-stopped = 在游玩非 PC 游戏后按平台名称备份存档数据（需要自定义条目）
config-do-platform-restore-on-non-pc-game-starting = 在游玩非 PC 游戏前同样按平台名称备份存档数据
config-ask-platform-backup-on-non-pc-game-stopped = “先奏后斩”
config-ignore-benign-notifications = 仅在失败时显示通知