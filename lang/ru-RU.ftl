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
restore-last-game = Восстановить сохранённые данные для последней игры
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Восстановить сохраненные данные для выбранных игр
    .confirm = Восстановить сохранённые данные для { $total-games } выбранных игр?
restore-all-games = Восстановить сохраненные данные для всех игр
    .confirm = Восстановить данные для всех игр, которые Ludusavi сможет найти?
    .on-success = Восстановленные сохранения для { $processed-games } игр ({ $processed-size }); нажмите для просмотра полного списка
    .on-failure = Восстановлены сохранения для { $processed-games } из { $total-games } игр ({ $processed-size } из { $total-size }), но некоторые из них не восстановились; нажмите для просмотра полного списка

## Tags

add-tag-for-selected-games = Тег: "{ $tag }" - Добавить для выбранных игр
    .confirm =
        Добавить тег "{ $tag }" для { $total-games } выбранных { $total-games ->
            [one] игры
           *[other] игр
        } и удалить любые конфликтующие теги?
remove-tag-for-selected-games = Тег: "{ $tag }" - Удалить для выбранных игр
    .confirm =
        Убрать тег "{ $tag }" для { $total-games } выбранных { $total-games ->
            [one] игры
           *[other] игр
        } и удалить любые конфликтующие теги?

## Generic errors

operation-still-pending = { ludusavi } все еще работает над предыдущим запросом. Пожалуйста, попробуйте снова, когда вы увидите уведомление, что оно завершено.
no-game-played-yet = Вы еще не играли ни во что в этой сессии.
unable-to-run-ludusavi = Не удается запустить { ludusavi }.
cannot-open-folder = Не удается открыть папку.
unable-to-synchronize-with-cloud = Unable to synchronize with cloud.
cloud-synchronize-conflict = Your local and cloud backups are in conflict. Open Ludusavi and perform an upload or download to resolve this.

## Settings

config-executable-path = Имя или полный путь к исполняемому файлу Ludusavi:
config-backup-path = Полный путь к каталогу для хранения резервных копий:
config-override-backup-format = Переопределить формат резервной копии.
config-override-backup-compression = Переопределить сжатие резервной копии.
config-override-backup-retention = Override backup retention.
config-full-backup-limit = Макс. кол-во резервных копий к игре:
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
config-ask-platform-backup-on-non-pc-game-stopped = Спросить сначала вместо автоматического выполнения
config-do-backup-during-play = Back up games on an interval during play, if they would also be backed up after play without asking
config-ignore-benign-notifications = Показывать уведомления только при ошибке
config-tag-games-with-backups = Автоматически помечать игры с резервными копиями как "{ $tag }"
label-minutes = Минут(ы):
option-simple = Простой
option-none = Нет

## Miscellaneous

upgrade-prompt = Установите Ludusavi { $version } или новее для лучшей работы. Нажмите для просмотра последней версии.
unrecognized-game = Ludusavi не распознает { $game }
look-up-as-other-title = Look up with another title
look-up-as-normal-title = Look up with default title
