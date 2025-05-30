ludusavi = Ludusavi
file-filter-executable = Kjørbar fil
button-browse = Søk
button-open = Åpne
button-yes = Ja
button-yes-remembered = Ja, alltid
button-no = Nei
button-no-remembered = Nei, aldri
label-launch = Start
badge-failed = FEIL
badge-ignored = IGNORERT
needs-custom-entry =
    { $total-games ->
        [one] Dette spillet krever
       *[other] Noen spill krever
    } en matchende egendefinert oppføring i Ludusavi.

## Backup

back-up-specific-game =
    .confirm = Sikkerhetskopier spill-lagringsdata for { $game }?
    .on-success = Sikkerhetskopierte spill-lagringsdata for { $game } ({ $processed-size })
    .on-unchanged = Ingenting nytt å sikkerhetskopiere for { $game }
    .on-empty = Ingen lagringsdata å sikkerhetskopiere ble funnet for { $game }
    .on-failure = Sikkerhetskopierte lagringsdata for { $game } ({ $processed-size } av { $total-size }), men noen filer feilet
# Defers to `back-up-specific-game.*`.
back-up-last-game = Sikkerhetskopier lagringsdata for siste spill som ble spilt
# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Sikkerhetskopier lagringsdata for valgte spill
    .confirm = Back up save data for { $total-games } selected games?
back-up-all-games = Back up save data for all games
    .confirm = Back up save data for all games that Ludusavi can find?
    .on-success = Backed up saves for { $processed-games } games ({ $processed-size }); click for full list
    .on-failure = Backed up saves for { $processed-games } of { $total-games } games ({ $processed-size } of { $total-size }), but some failed; click for full list
back-up-during-play-on-success = Triggered { $total-backups } backups while playing { $game }
back-up-during-play-on-failure = Triggered { $total-backups } backups while playing { $game }, of which { $failed-backups } failed

## Restore

restore-specific-game =
    .confirm = Restore save data for { $game }?
    .on-success = Restored saves for { $game } ({ $processed-size })
    .on-unchanged = Nothing new to restore for { $game }
    .on-empty = No save data found to restore for { $game }
    .on-failure = Restored saves for { $game } ({ $processed-size } of { $total-size }), but some saves failed
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

add-tag-for-selected-games = Tag: "{ $tag }" - Add for selected games
    .confirm =
        Add "{ $tag }" tag for { $total-games } selected { $total-games ->
            [one] game
           *[other] games
        } and remove any conflicting tags?
remove-tag-for-selected-games = Tag: "{ $tag }" - Remove for selected games
    .confirm =
        Remove "{ $tag }" tag for { $total-games } selected { $total-games ->
            [one] game
           *[other] games
        } and remove any conflicting tags?

## Generic errors

operation-still-pending = Ludusavi is still working on a previous request. Please try again when you see the notification that it's done.
no-game-played-yet = You haven't played anything yet in this session.
unable-to-run-ludusavi = Unable to run Ludusavi.
cannot-open-folder = Cannot open folder.
unable-to-synchronize-with-cloud = Unable to synchronize with cloud.
cloud-synchronize-conflict = Your local and cloud backups are in conflict. Open Ludusavi and perform an upload or download to resolve this.

## Settings

config-executable-path = Name or full path of the Ludusavi executable:
config-backup-path = Override full path to directory for storing backups:
config-override-backup-format = Override backup format.
config-override-backup-compression = Override backup compression.
config-override-backup-retention = Override backup retention.
config-full-backup-limit = Max full backups per game:
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
config-check-app-update = Check for Ludusavi updates automatically
config-ask-when-multiple-games-are-running = Require confirmation when multiple games are running
label-minutes = Minutes:
option-simple = Simple
option-none = None

## Miscellaneous

initial-setup-required = Ludusavi does not seem to be installed. Please download it and then follow the plugin setup instructions.
upgrade-prompt = Install Ludusavi { $version } or newer for the best experience. Click to view the latest release.
upgrade-available = Ludusavi { $version } is now available. Click to view the release notes.
unrecognized-game = Ludusavi does not recognize { $game }
look-up-as-other-title = Look up with another title
look-up-as-normal-title = Look up with default title
open-backup-directory = Open backup directory
