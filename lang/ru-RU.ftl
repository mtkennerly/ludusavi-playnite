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
    } соответствующей пользовательской записи в Ludusavi.

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
back-up-during-play-on-success = Запущено { $total-backups } резервных копий во время игры в { $game }
back-up-during-play-on-failure = Запущено { $total-backups } резервных копий во время игры в { $game }, из которых { $failed-backups } не удались

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

operation-still-pending = Ludusavi все еще работает над предыдущим запросом. Пожалуйста, попробуйте снова, когда вы увидите уведомление, что оно завершено.
no-game-played-yet = Вы еще не играли ни во что в этой сессии.
unable-to-run-ludusavi = Не удается запустить Ludusavi.
cannot-open-folder = Не удается открыть папку.
unable-to-synchronize-with-cloud = Не удалось синхронизировать с облаком.
cloud-synchronize-conflict = Ваши локальные и облачные резервные копии конфликтуют. Выполните закачку или загрузку, чтобы исправить это.

## Settings

config-executable-path = Имя или полный путь к исполняемому файлу Ludusavi:
config-backup-path = Переопределить полный путь к каталогу для хранения резервных копий:
config-override-backup-format = Переопределить формат резервной копии.
config-override-backup-compression = Переопределить сжатие резервной копии.
config-override-backup-retention = Переопределить путь сохранения резервной копии.
config-full-backup-limit = Макс. кол-во резервных копий к игре:
config-differential-backup-limit = Максимальное количество дифференциальных резервных копий за полную резервную копию:
config-do-backup-on-game-stopped = Сохранять резервные копии после игры
config-do-restore-on-game-starting = Также восстановить данные для игры перед игрой
config-ask-backup-on-game-stopped = Спросить сначала вместо автоматического выполнения
config-only-backup-on-game-stopped-if-pc = Сделать это только для ПК игр
config-retry-unrecognized-game-with-normalization = Если не найдено, попробуйте нормализировать заголовок
config-add-suffix-for-non-pc-game-names = Искать не-ПК, добавив этот суффикс к их именам (требует пользовательской записи):
config-retry-non-pc-games-without-suffix = Если не найден с суффиксом, попробуйте снова без него
config-do-platform-backup-on-non-pc-game-stopped = Резервное копирование данных по названию платформы после воспроизведения не-PC игр (требуется пользовательская запись)
config-do-platform-restore-on-non-pc-game-starting = Восстановить данные по названию платформы перед проигрыванием не-PC игр
config-ask-platform-backup-on-non-pc-game-stopped = Спросить сначала вместо автоматического выполнения
config-do-backup-during-play = Резервное копирование игр с интервалом во время игры, если бы они также были скопированы после воспроизведения без запроса
config-ignore-benign-notifications = Показывать уведомления только при ошибке
config-tag-games-with-backups = Автоматически помечать игры с резервными копиями как "{ $tag }"
config-tag-games-with-unknown-save-data = Автоматически помечать игры с неизвестными сохраненными данными как "{ $tag }"
config-check-app-update = Проверять обновления Ludusavi автоматически
config-ask-when-multiple-games-are-running = Require confirmation when multiple games are running
label-minutes = Минут(ы):
option-simple = Простой
option-none = Нет

## Miscellaneous

initial-setup-required = Ludusavi не установлен. Пожалуйста, загрузите его и следуйте инструкциям по установке плагина.
upgrade-prompt = Установите Ludusavi { $version } или новее для лучшей работы. Нажмите для просмотра последней версии.
upgrade-available = Ludusavi { $version } доступен. Нажмите, чтобы посмотреть список изменений.
unrecognized-game = Ludusavi не распознает { $game }
look-up-as-other-title = Искать с другим заголовком
look-up-as-normal-title = Искать с названием по умолчанию
open-backup-directory = Открыть каталог для резервной копий
