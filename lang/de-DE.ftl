ludusavi = Ludusavi
file-filter-executable = Ausführbare Datei
button-browse = Durchsuchen
button-open = Öffnen
button-yes = Ja
button-yes-remembered = Ja, immer
button-no = Nein
button-no-remembered = Nein, niemals
label-launch = Starten
badge-failed = FEHLGESCHLAGEN
badge-ignored = IGNORIERT
needs-custom-entry =
    { $total-games ->
        [one] Dieses Spiel erfordert
       *[other] Einige Spiele erfordern
    } einen passenden benutzerdefinierten Eintrag in { ludusavi }.

## Backup

back-up-specific-game =
    .confirm = Speicherstände für { $game } sichern?
    .on-success = Speicherstände für { $game } ({ $processed-size } gesichert)
    .on-unchanged = Für { $game } gibt es nichts neues zum sichern
    .on-empty = Keine Speicherstand Daten zum sichern gefunden für { $game }
    .on-failure = Speicherstände gesichert für { $game } ({ $processed-size } von { $total-size }), aber einige Speicherstände sind fehlgeschlagen
# Defers to `back-up-specific-game.*`.
back-up-last-game = Speicherstände für das zuletzt gespielte Spiel sichern
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Sichere Speicherdaten für ausgewählte Spiele
    .confirm = Speicherstände für { $total-games } ausgewählte Spiele sichern?
back-up-all-games = Speicherstände für alle Spiele sichern
    .confirm = Speicherstände von allen Spielen sichern die Ludusavi finden kann?
    .on-success = Speicherstände für { $processed-games } Spiele ({ $processed-size }) gesichert; Klicke für vollständige Liste
    .on-failure = Speicherstände für { $processed-games } von { $total-games } Spielen ({ $processed-size } von { $total-size }) gesichert, aber einige sind fehlgeschlagen; Klicke für vollständige Liste
back-up-during-play-on-success = { $total-backups } Sicherungen während { $game } spielen
back-up-during-play-on-failure = { $total-backups } Sicherungen während { $game } spielen, von denen { $failed-backups } fehlgeschlagen sind

## Restore

restore-specific-game =
    .confirm = Speicherstände für { $game } wiederherstellen?
    .on-success = Speicherstände wiederhergestellt für { $game } ({ $processed-size })
    .on-unchanged = Nichts Neues für { $game } zum wiederherstellen
    .on-empty = Keine Speicherstand Daten zum wiederherstellen gefunden für { $game }
    .on-failure = Speicherstände wiederhergestellt für { $game } ({ $processed-size } von { $total-size }), aber einige Sicherungen sind fehlgeschlagen
# Defers to `restore-specific-game.*`.
restore-last-game = Speicherstand für das zuletzt gespielte Spiel wiederherstellen
# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Speicherdaten für ausgewählte Spiele wiederherstellen
    .confirm = Speicherstände für { $total-games } ausgewählte Spiele wiederherstellen?
restore-all-games = Speicherdaten für alle Spiele wiederherstellen
    .confirm = Speicherstände von allen Spielen wiederherstellen die Ludusavi finden kann?
    .on-success = Speicherstände für { $processed-games } Spiele ({ $processed-size }) wiederherstellen; Klicke für vollständige Liste
    .on-failure = Speicherstände für { $processed-games } von { $total-games } Spielen ({ $processed-size } von { $total-size }) wiederhergestellt, aber einige sind fehlgeschlagen; Klicke für vollständige Liste

## Tags

add-tag-for-selected-games = Tag: "{ $tag }" - Für ausgewählte Spiele hinzufügen
    .confirm =
        Füge "{ $tag }" Tag für { $total-games } ausgewählt { $total-games ->
            [one] Spiel
           *[other] Spiele
        } und entferne alle widersprüchlichen Tags?
remove-tag-for-selected-games = Tag: "{ $tag }" - Für ausgewählte Spiele entfernen
    .confirm =
        Entferne "{ $tag }" Tag für { $total-games } ausgewählt { $total-games ->
            [one] Spiel
           *[other] Spiele
        } und entferne alle widersprüchlichen Tags?

## Generic errors

operation-still-pending = { ludusavi } arbeitet noch an einer vorherigen Anfrage. Bitte versuche es erneut, sobald eine Benachrichtigung erscheint, dass es fertig ist.
no-game-played-yet = Du hast noch nichts in dieser Sitzung gespielt.
unable-to-run-ludusavi = Kann { ludusavi } nicht ausführen.
cannot-open-folder = Ordner kann nicht geöffnet werden.
unable-to-synchronize-with-cloud = Synchronisierung mit der Cloud nicht möglich.
cloud-synchronize-conflict = Ihre lokalen und Cloud-Backups befinden sich in Konflikt. Öffnen Sie Ludusavi und führen Sie einen Upload oder Download durch, um dies zu beheben.

## Settings

config-executable-path = Name oder vollständiger Pfad der Ludusavi-Programmdatei:
config-backup-path = Den vollständigen Pfad zum Verzeichnis zum Speichern von Sicherungen überschreiben:
config-override-backup-format = Backup-Format überschreiben.
config-override-backup-compression = Backup-Kompression überschreiben.
config-override-backup-retention = Override backup retention.
config-full-backup-limit = Maximale Sicherungen pro Spiel:
config-differential-backup-limit = Max differential backups per full backup:
config-do-backup-on-game-stopped = Back up save data for a game after playing it
config-do-restore-on-game-starting = Also restore save data for a game before playing it
config-ask-backup-on-game-stopped = Ask first instead of doing it automatically
config-only-backup-on-game-stopped-if-pc = Only do this for PC games
config-retry-unrecognized-game-with-normalization = If not found, retry by normalizing the title
config-add-suffix-for-non-pc-game-names = Look up non-PC games by adding this suffix to their names (requires custom entry):
config-retry-non-pc-games-without-suffix = If not found with the suffix, then try again without it
config-do-platform-backup-on-non-pc-game-stopped = Back up save data by platform name after playing non-PC games (requires custom entry)
config-do-platform-restore-on-non-pc-game-starting = Also restore save data by platform name before playing non-PC games
config-ask-platform-backup-on-non-pc-game-stopped = Ask first instead of doing it automatically
config-do-backup-during-play = Back up games on an interval during play, if they would also be backed up after play without asking
config-ignore-benign-notifications = Only show notifications on failure
config-tag-games-with-backups = Automatically tag games with backups as "{ $tag }"
config-tag-games-with-unknown-save-data = Automatically tag games with unknown save data as "{ $tag }"
label-minutes = Minutes:
option-simple = Simple
option-none = Keine

## Miscellaneous

initial-setup-required = Ludusavi does not seem to be installed. Please download it and then follow the plugin setup instructions.
upgrade-prompt = Installiere Ludusavi { $version } oder neuer, um die beste Erfahrung zu erzielen. Klicke hier, um die neueste Version anzuzeigen.
unrecognized-game = Ludusavi erkennt { $game } nicht
look-up-as-other-title = Look up with another title
look-up-as-normal-title = Look up with default title
open-backup-directory = Open backup directory
