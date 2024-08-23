ludusavi = Ludusavi
file-filter-executable = Plik wykonywalny
button-browse = Przeglądaj
button-open = Otwórz
button-yes = Tak
button-yes-remembered = Tak, zawsze
button-no = Nie
button-no-remembered = Nie, nigdy
label-launch = Uruchom
badge-failed = PRZEGRANY
badge-ignored = IGNOROWANY
needs-custom-entry =
    { $total-games ->
        [one] Ta gra wymaga
        *[other] Niektóre gry wymagają
    } pasującego wpisu w Ludusavi.

## Backup

back-up-specific-game =
    .confirm = Kopia zapasowa danych dla { $game }?
    .on-success = Stworzono kopię zapasową dla { $game } ({ $processed-size })
    .on-unchanged = Nothing new to back up for { $game }
    .on-empty = Nie znaleziono zapisów dla { $game }
    .on-failure = Przywrócono zapisy dla { $game } ({ $processed-size } z { $total-size }), ale niektóre się nie powiodły
# Defers to `back-up-specific-game.*`.
back-up-last-game = Stworzono kopię zapasową dla ostatnio granej gry
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Utwórz kopię zapasową danych dla zaznaczonych gier
    .confirm = Stworzyć kopię zapasową dla { $total-games } zaznaczonych gier?
back-up-all-games = Stwórz kopię zapasową dla wszystkich gier
    .confirm = Stwórz kopię zapasową dla wszystkich gier, które może znaleźć Ludusavi?
    .on-success = Stworzono kopię zapasową dla { $processed-games } gier ({ $processed-size }); Naciśnij, aby pokazać całą listę
    .on-failure = Stworzono kopię zapasową dla { $processed-games } z { $total-games } gier ({ $processed-size } z { $total-size }), ale niektóre się nie powiodły; kliknij, aby pokazać pełną listę
back-up-during-play-on-success = Triggered { $total-backups } backups while playing { $game }
back-up-during-play-on-failure = Triggered { $total-backups } backups while playing { $game }, of which { $failed-backups } failed

## Restore

restore-specific-game =
    .confirm = Przywrócić zapisy dla { $game }?
    .on-success = Przywrócono zapisy dla { $game } ({ $processed-size })
    .on-unchanged = Nothing new to restore for { $game }
    .on-empty = Nie znaleziono zapisów do przywrócenia dla { $game }
    .on-failure = Przywrócono zapisy dla { $game } ({ $processed-size } z { $total-size }), ale niektóre się nie powiodły
# Defers to `restore-specific-game.*`.
restore-last-game = Przywróć zapisy dla ostatnio granej gry
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Przywróc zapisy dla zaznaczonych gier
    .confirm = Przywrócić zapisy dla { $total-games }?
restore-all-games = Przywróć zapisany dla wszystkich gier
    .confirm = Przywrócić zapisy dla wszystkich gier, które Ludusavi może znaleźć?
    .on-success = Przywrócono zapisy dla { $processed-games } gier ({ $processed-size }); Naciśnij, aby pokazać całą listę
    .on-failure = Przywrócono zapisy dla { $processed-games } z { $total-games } gier ({ $processed-size } z { $total-size }), ale niektóre nie powiodło się; kliknij aby uzyskać pełną listę

## Tags

add-tag-for-selected-games = Tag: "{ $tag }" - Dodaj do wybranych gier
    .confirm =
        Dodaj tag "{ $tag }" dla { $total-games ->
            [one] wybranej gry
           *[other] { $total-games } wybranych gier
        } i usunąć jakiekolwiek konfliktujące tagi?
remove-tag-for-selected-games = Tag: "{ $tag }" - Usuń dla wybranych gier
    .confirm =
        Dodaj tag "{ $tag }" dla { $total-games ->
            [one] wybranej gry
           *[other] { $total-games } wybranych gier
        } i usunąć jakiekolwiek konfliktujące tagi?

## Generic errors

operation-still-pending = Ludusavi nadal pracuje nad poprzednim żądaniem. Spróbuj ponownie, gdy zobaczysz powiadomienie o jego zakończeniu.
no-game-played-yet = Nie zagrałeś jeszcze nic w tej sesji.
unable-to-run-ludusavi = Nie udało się uruchomić Ludusavi.
cannot-open-folder = Cannot open folder.
unable-to-synchronize-with-cloud = Unable to synchronize with cloud.
cloud-synchronize-conflict = Kopia lokalna różni się od tej w chmurze. Otwórz Ludusavi i wyślij lub pobierz odpowiednią wersję, aby rozwiązać ten problem.

## Settings

config-executable-path = Nazwa lub pełna ścieżka pliku wykonywalnego Ludusavi:
config-backup-path = Zastąp pełną ścieżkę katalogu do przechowywania kopii zapasowych:
config-override-backup-format = Override backup format.
config-override-backup-compression = Override backup compression.
config-override-backup-retention = Override backup retention.
config-full-backup-limit = Max full backups per game:
config-differential-backup-limit = Max differential backups per full backup:
config-do-backup-on-game-stopped = Utwórz kopię zapasową dla gry po jej graniu
config-do-restore-on-game-starting = Przywróć również zapisy dla gry przed jej graniem
config-ask-backup-on-game-stopped = Zapytaj, zamiast zrobić to automatycznie
config-only-backup-on-game-stopped-if-pc = Tylko dla gier PC
config-retry-unrecognized-game-with-normalization = If not found, retry by normalizing the title
config-add-suffix-for-non-pc-game-names = Wyszukaj gry non-PC, dodając ten sufiks do ich nazw (wymaga wpisu niestandardowego):
config-retry-non-pc-games-without-suffix = Jeśli nie znaleziono z sufiksem, spróbuj ponownie bez niego
config-do-platform-backup-on-non-pc-game-stopped = Kopia zapasowa danych po nazwie platformy po graniu w gry non-PC (wymaga wpisu niestandardowego)
config-do-platform-restore-on-non-pc-game-starting = Przywróć również dane po nazwie platformy przed graniem w gry non-PC
config-ask-platform-backup-on-non-pc-game-stopped = Zapytaj, zamiast tego automatycznie
config-do-backup-during-play = Back up games on an interval during play, if they would also be backed up after play without asking
config-ignore-benign-notifications = Pokaż powiadomienia tylko przy niepowodzeniu
config-tag-games-with-backups = Automatically tag games with backups as "{ $tag }"
config-tag-games-with-unknown-save-data = Automatically tag games with unknown save data as "{ $tag }"
config-check-app-update = Check for Ludusavi updates automatically
label-minutes = Minutes:
option-simple = Prosty
option-none = Brak

## Miscellaneous

initial-setup-required = Ludusavi does not seem to be installed. Please download it and then follow the plugin setup instructions.
upgrade-prompt = Install Ludusavi { $version } or newer for the best experience. Click to view the latest release.
upgrade-available = Ludusavi { $version } is now available. Click to view the release notes.
unrecognized-game = Ludusavi does not recognize { $game }
look-up-as-other-title = Look up with another title
look-up-as-normal-title = Look up with default title
open-backup-directory = Open backup directory
