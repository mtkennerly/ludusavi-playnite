ludusavi = Ludusavi
file-filter-executable = Executable

button-browse = Browse
button-open = Open
button-yes = Yes
button-yes-remembered = Yes, always
button-no = No
button-no-remembered = No, never

label-launch = Launch

badge-failed = FAILED
badge-ignored = IGNORED

needs-custom-entry = {$total-games ->
    [one] This game requires
    *[other] Some games require
} a matching custom entry in {ludusavi}.

## Backup

back-up-specific-game =
    .confirm = Back up save data for {$game}?
    .on-success = Backed up saves for {$game} ({$processed-size})
    .on-unchanged = Nothing new to back up for {$game}
    .on-empty = No save data found to back up for {$game}
    .on-failure = Backed up saves for {$game} ({$processed-size} of {$total-size}), but some saves failed

# Defers to `back-up-specific-game.*`.
back-up-last-game = Back up save data for last game played

# Defers to `back-up-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
back-up-selected-games = Back up save data for selected games
    .confirm = Back up save data for {$total-games} selected games?

back-up-all-games = Back up save data for all games
    .confirm = Back up save data for all games that Ludusavi can find?
    .on-success = Backed up saves for {$processed-games} games ({$processed-size}); click for full list
    .on-failure = Backed up saves for {$processed-games} of {$total-games} games ({$processed-size} of {$total-size}), but some failed; click for full list

back-up-during-play-on-success = Triggered {$total-backups} backups while playing {$game}
back-up-during-play-on-failure = Triggered {$total-backups} backups while playing {$game}, of which {$failed-backups} failed

## Restore

restore-specific-game =
    .confirm = Restore save data for {$game}?
    .on-success = Restored saves for {$game} ({$processed-size})
    .on-unchanged = Nothing new to restore for {$game}
    .on-empty = No save data found to restore for {$game}
    .on-failure = Restored saves for {$game} ({$processed-size} of {$total-size}), but some saves failed

# Defers to `restore-specific-game.*`.
restore-last-game = Restore save data for last game played

# Defers to `restore-specific-game.*` for each game individually.
# In `.confirm`, there will always be more than one game.
restore-selected-games = Restore save data for selected games
    .confirm = Restore save data for {$total-games} selected games?

restore-all-games = Restore save data for all games
    .confirm = Restore save data for all games that Ludusavi can find?
    .on-success = Restored saves for {$processed-games} games ({$processed-size}); click for full list
    .on-failure = Restored saves for {$processed-games} of {$total-games} games ({$processed-size} of {$total-size}), but some failed; click for full list

## Tags

add-tag-for-selected-games = Tag: "{$tag}" - Add for selected games
    .confirm = Add "{$tag}" tag for {$total-games} selected {$total-games ->
        [one] game
        *[other] games
    } and remove any conflicting tags?

remove-tag-for-selected-games = Tag: "{$tag}" - Remove for selected games
    .confirm = Remove "{$tag}" tag for {$total-games} selected {$total-games ->
        [one] game
        *[other] games
    } and remove any conflicting tags?

## Generic errors

operation-still-pending = {ludusavi} is still working on a previous request. Please try again when you see the notification that it's done.
no-game-played-yet = You haven't played anything yet in this session.
unable-to-run-ludusavi = Unable to run {ludusavi}.
cannot-open-folder = Cannot open folder.

## Settings

config-executable-path = Name or full path of the Ludusavi executable:
config-backup-path = Full path to directory for storing backups:
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

label-minutes = Minutes:

option-simple = Simple
option-none = None

## Miscellaneous

upgrade-prompt = Install Ludusavi {$version} or newer for the best experience. Click to view the latest release.
unrecognized-game = Ludusavi does not recognize {$game}
