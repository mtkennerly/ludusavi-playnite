ludusavi = Ludusavi
file-filter-executable = Исполняемый файл
button-browse = Обзор
button-open = Открыть
button-yes = Да
button-yes-remembered = Да, всегда
button-no = Нет
button-no-remembered = Нет, никогда
label-launch = Запустить
badge-failed = ОШИБКА
badge-ignored = ИГНОРИРОВАНО
needs-custom-entry =
    { $total-games ->
        [one] Эта игра требует
       *[other] Некоторые игры требуют
    } соответствующей пользовательской записи в { ludusavi }.

## Backup

back-up-specific-game =
    .confirm = Создать резервную копию { $game }?
    .on-success = Создана резервная копия { $game } ({ $processed-size })
    .on-unchanged = Ничего нового для резервного копирования у { $game }
    .on-empty = Не найдено данных { $game } для создания резервной копии
    .on-failure = Сохранена резервная копия { $game } ({ $processed-size } из { $total-size }), но часть сохранить не удалось
# Defers to `back-up-specific-game.*`.
back-up-last-game = Резервное копирование данных для последней игры
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Резервное копирование данных для выбранных игр
    .confirm = Сохранять резервные копии данных для { $total-games } выбранных игр?
back-up-all-games = Резервное копирование данных для всех игр
    .confirm = Резервные копии данных для всех игр, которые Ludusavi может найти?
    .on-success = Резервное копирование сохранений для { $processed-games } игр ({ $processed-size }); нажмите для просмотра списка
    .on-failure = Резервированы сохранения { $processed-games } из { $total-games } игр ({ $processed-size } из { $total-size }), но некоторые из них не удались; нажмите на кнопку для просмотра списка
back-up-during-play-on-success = Triggered { $total-backups } backups while playing { $game }
back-up-during-play-on-failure = Triggered { $total-backups } backups while playing { $game }, of which { $failed-backups } failed

## Restore

restore-specific-game =
    .confirm = Восстановить сохраненные данные для { $game }?
    .on-success = Восстановленные сохранения для { $game } ({ $processed-size })
    .on-unchanged = Ничего нового для восстановления для { $game }
    .on-empty = Не найдено данных для восстановления у { $game }
    .on-failure = Восстановлены сохранения для { $game } ({ $processed-size } из { $total-size }), но некоторые сохранения не удалось
# Defers to `restore-specific-game.*`.
restore-last-game = Restore save data for last game played
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Restore save data for selected games
    .confirm = Restore save data for { $total-games } selected games?
restore-all-games = Restore save data for all games
    .confirm = Restore save data for all games that Ludusavi can find?
    .on-success = Restored saves for { $processed-games } games ({ $processed-size }); click for full list
    .on-failure = Restored saves for { $processed-games } of { $total-games } games ({ $processed-size } of { $total-size }), but some failed; click for full list

## Tags

add-tag-for-selected-games = Тег: "{ $tag }" - Добавить для выбранных игр
    .confirm =
        Добавить тег "{ $tag }" для { $total-games } выбранных { $total-games ->
            [one] игры
           *[other] игр
        } и удалить любые конфликтующие теги?
remove-tag-for-selected-games = Tag: "{ $tag }" - Remove for selected games
    .confirm =
        Remove "{ $tag }" tag for { $total-games } selected { $total-games ->
            [one] game
           *[other] games
        } and remove any conflicting tags?

## Generic errors

operation-still-pending = { ludusavi } все еще работает над предыдущим запросом. Пожалуйста, попробуйте снова, когда вы увидите уведомление, что оно завершено.
no-game-played-yet = Вы еще не играли ни во что в этой сессии.
unable-to-run-ludusavi = Не удается запустить { ludusavi }.
cannot-open-folder = Не удается открыть папку.

## Settings

config-executable-path = Имя или полный путь к исполняемому файлу Ludusavi:
config-backup-path = Полный путь к каталогу для хранения резервных копий:
config-override-backup-format = Переопределить формат резервной копии.
config-override-backup-compression = Переопределить сжатие резервной копии.
config-override-backup-retention = Override backup retention.
config-full-backup-limit = Max full backups per game:
config-differential-backup-limit = Max differential backups per full backup:
config-do-backup-on-game-stopped = Сохранять резервные копии после игры
config-do-restore-on-game-starting = Также восстановить данные для игры перед игрой
config-ask-backup-on-game-stopped = Спросить сначала вместо автоматического выполнения
config-only-backup-on-game-stopped-if-pc = Сделать это только для ПК игр
config-retry-unrecognized-game-with-normalization = If not found, retry by normalizing the title
config-add-suffix-for-non-pc-game-names = Look up non-PC games by adding this suffix to their names (requires custom entry):
config-retry-non-pc-games-without-suffix = If not found with the suffix, then try again without it
config-do-platform-backup-on-non-pc-game-stopped = Back up save data by platform name after playing non-PC games (requires custom entry)
config-do-platform-restore-on-non-pc-game-starting = Also restore save data by platform name before playing non-PC games
config-ask-platform-backup-on-non-pc-game-stopped = Ask first instead of doing it automatically
config-do-backup-during-play = Back up games on an interval during play, if they would also be backed up after play without asking
config-ignore-benign-notifications = Only show notifications on failure
config-tag-games-with-backups = Automatically tag games with backups as "{ $tag }"
label-minutes = Минут(ы):
option-simple = Simple
option-none = Нет

## Miscellaneous

upgrade-prompt = Install Ludusavi { $version } or newer for the best experience. Click to view the latest release.
unrecognized-game = Ludusavi does not recognize { $game }
look-up-as-other-title = Look up with another title
look-up-as-normal-title = Look up with default title
