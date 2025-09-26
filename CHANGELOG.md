## Unreleased

* Added:
  * The context menu now indicates when a backup is locked.
    ([Contributed by darklinkpower](https://github.com/mtkennerly/ludusavi-playnite/pull/96))
  * The context menu now lets you open Ludusavi to add/edit a game's custom entry.
    This requires Ludusavi v0.30.0 or newer.
  * The context menu now lets you (un)lock a backup and edit its comment.
    This requires Ludusavi v0.30.0 or newer.
  * Updated translations, including a new translation for Turkish.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.17.1 (2025-05-07)

* Fixed:
  * With the `Require confirmation when multiple games are running` option enabled,
    the confirmation dialog contained "always" and "never" options
    that didn't make sense in context.
    These are now removed.
  * With the `Require confirmation when multiple games are running` option enabled,
    the confirmation could trigger by mistake when other plugins had cancelled a game launch.
* Changed:
  * Updated translations, including a new translation for French.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.17.0 (2025-04-22)

* Added:
  * New option: `Require confirmation when multiple games are running`.
    This can be useful if you play multiple games at the same time that share save data
    (particularly for platform-based backups),
    so you can prevent automatic backup/restore when more than one game is running.
* Fixed:
  * If two games had the same plugin source and game ID,
    then the wrong game could get backed up.
* Changed:
  * Updated translations, including a new translation for Arabic and Indonesian.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.16.0 (2024-08-23)

* Added:
  * You can now open the backup directories for multiple selected games at once.
  * The plugin will notify you when an update for Ludusavi is available.
    This requires Ludusavi v0.25.0 or newer.
    The check occurs at most once per 24 hours.
* Fixed:
  * The plugin now uses a buffered update when changing tags.
    This should avoid issues where other plugins may respond to each tag change individually.
    ([Contributed by Jeshibu](https://github.com/mtkennerly/ludusavi-playnite/pull/80))
* Changed:
  * Updated translations, including a new partial Norwegian translation.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.15.0 (2024-06-08)

* Added:
  * In a game's context menu, backup comments are now displayed,
    if any have been added in Ludusavi.
    If a backup is from another OS than Windows, that is also indicated.
  * In a game's context menu, you can now open that game's backup directory.
    This requires Ludusavi v0.24.0 or newer.
  * On startup, if Ludusavi cannot be found, then an error popup is displayed.
    This should help users who did not realize they had to install Ludusavi itself
    and who also missed the error notification.
* Fixed:
  * In some cases where Playnite and Ludusavi used different names for a game,
    the game's context menu would not list its backups.
    This now works properly in more cases if you use Ludusavi v0.24.0 or newer.
* Changed:
  * The recommended version of Ludusavi is now v0.24.0. You can download the latest release here:
    https://github.com/mtkennerly/ludusavi/releases
  * In a game's context menu, backups are now sorted newest to oldest.

## v0.14.0 (2024-04-27)

* Changed:
  * Updated handling for the `restore` command for Ludusavi v0.23.0+,
    which no longer returns an error when there is no data to restore.
  * Updated translations, including a new German translation.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.13.2 (2023-08-11)

* Fixed:
  * Crash when changing plugin settings if some games had a null name.

## v0.13.1 (2023-08-10)

* Fixed:
  * Possible crash when saving plugin settings if the config file contained certain null values.
* Changed:
  * Updated translations.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.13.0 (2023-06-22)

* Added:
  * Ludusavi 0.18.0+: The plugin will show a notification if cloud sync fails or has a conflict.
  * You can now choose not to override the backup directory and defer to Ludusavi's own configuration.
  * There is an option to automatically tag games with unknown save data.
* Changed:
  * Ludusavi 0.18.0+: The plugin no longer sets the `--merge` flag, since it has been deprecated.
    It is still set for older Ludusavi versions.
  * Ludusavi 0.18.0+: The plugin now sets `--try-manifest-update` instead of `--try-update`.
  * Updated translations, including new translations for Japanese and Russian.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))
* Fixed:
  * The during-play backup feature did not check if a backup was already running before starting a new one.

## v0.12.1 (2023-03-19)

* Fixed:
  * If a game had multiple platforms listed in its metadata,
    then it would not trigger any automatic backups before/during/after play
    (manual backups worked as expected).
    The plugin now properly handles games with multiple platforms.
  * When distinguishing PC and non-PC games,
    the plugin only considered the first platform listed in the game details.
    It now considers all platforms listed.
  * On confirmation prompts, the plugin would preemptively say
    "this game requires a matching custom entry" for non-PC games.
    However, that assumption isn't always true.
    Now, the plugin just adds this message onto the error notification when Ludusavi doesn't recognize a game.
  * If you had both game- and platform-based automatic backups enabled after play,
    then the game backup might not be done yet when the platform backup was attempted,
    resulting in the platform backup aborting.
    The plugin now waits until the game backup is done before attempting the platform backup.
* Changed:
  * The recommended version of Ludusavi is now 0.16.0. You can download the latest release here:
    https://github.com/mtkennerly/ludusavi/releases
  * Updated translations.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.12.0 (2023-02-28)

* Added:
  * An option to automatically tag games with backups. This requires Ludusavi 0.14.0 or newer.
  * An option to look up games under a different name.
    This allows you to resolve Playnite/Ludusavi naming discrepancies without creating a custom entry in Ludusavi.

    Right now, this is simply a text box where you can enter the desired lookup name.
    Once Ludusavi 0.16.0 is released, it will allow the plugin to show a searchable list of Ludusavi's known titles.
* Fixed:
  * Games could not be backed up if they had Unicode characters in their title (e.g., "Ninja Gaiden Σ").
  * With older versions of Ludusavi, the plugin would report that there was nothing new to back up,
    even if it did make a new backup.

## v0.11.1 (2022-11-16)

* Fixed:
  * Crash when trying to open a nonexistent folder from the settings window.
* Changed:
  * Updated translations.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.11.0 (2022-10-29)

* Added:
  * The plugin now indicates when there was nothing new to back up or restore.
    When doing a full scan, the expanded list also indicates new (+) or updated (Δ) for each game.
    This is enabled if you have Ludusavi 0.14.0 or newer.
  * When backing up or restoring a specific game,
    the plugin now indicates when Ludusavi knows it by a different name.
    Example: `Backed up saves for Playnite Title (↪ Ludusavi Title)`.
    This is enabled if you have Ludusavi 0.14.0 or newer.
  * An option to look up games by a normalization of their titles.
    For example, this means the plugin can retry "Some Game: Special Edition (2022)" as just "Some Game"
    if Ludusavi didn't recognize the original title.
    If the normalized title produces multiple matches, the plugin chooses the first one.
    This setting only takes effect if you have Ludusavi 0.14.0 or newer.
  * Plugin settings to override the backup format, compression, and retention.
    These will still default to whatever you have configured in Ludusavi itself,
    but now you can override it from Playnite.
    These settings only take effect if you have Ludusavi 0.14.0 or newer.
  * On startup, the plugin will show a one-time notification
    if a newer version of Ludusavi is recommended for the best experience.
* Changed:
  * When Ludusavi 0.14.0 or newer is installed, the plugin now uses the `find` command to look up games.
    As a side effect, a game's Steam ID (if applicable) is now checked before its title rather than after.
  * For compatibility with Ludusavi 0.14.0, the plugin now passes `--force` to the backup command.
    This remains compatible with older versions as well.

## v0.10.2 (2022-09-12)

* Fixed:
  * The extension's context menu section did not appear when selecting a single game
    if the installed Ludusavi version did not support multi-backup.

## v0.10.1 (2022-09-11)

* Added:
  * Updated translations.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))
* Fixed:
  * Some errors were logged at info level.

## v0.10.0 (2022-09-01)

* Added:
  * Support for multiple backups per game. This requires Ludusavi 0.12.0 or newer.
    You can open Ludusavi to configure how many backups to keep.
    The feature will be disabled if an older version is detected.
  * Support for automatic backups on an interval while playing a game.
  * Translation for Italian.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))
* Fixed:
  * Disabled fallback restoration by Steam ID for Ludusavi versions older than
    0.12.0 to avoid a defect in those versions.
  * The plugin settings window now has a horizontal scroll bar when necessary.

## v0.9.0 (2022-08-13)

* Added:
  * Completed the translation for Simplified Chinese.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))

## v0.8.1 (2022-07-11)

* Fixed:
  * Handling of games whose name begins with a hyphen.
    Previously, the plugin would not have been able to run Ludusavi for such games.
  * Handling of games whose name contains quotation marks.
    Previously, depending on the quote placement, the plugin may not have found
    anything to back up for such games.

## v0.8.0 (2022-07-07)

* Added:
  * Translations for Simplified Chinese (partial), Polish, Brazilian Portuguese, and Spanish.
    (Thanks to contributors on the [Crowdin project](https://crowdin.com/project/ludusavi-playnite))
* Changed:
  * The plugin can now recognize when a game is on PC or Steam in more situations.
    Specifically, it now checks the game's platform name and source plugin ID as well,
    instead of just the platform specification ID and source name.

## v0.7.0 (2022-05-27)

* Changed:
  * Localization now uses [Project Fluent](https://projectfluent.org) instead of pure C# code internally.
    Although English currently remains the only language supported, this change
    should make it easier for other people to contribute. If you'd like to help,
    [check out the new Crowdin project](https://crowdin.com/project/ludusavi-playnite).
  * The `ludusavi-skip` tag is now `[Ludusavi] Skip`.
    If you were using the old tag, it will be automatically renamed for you.
  * Removed support for [erri120's extension update notifier](https://github.com/erri120/Playnite.Extensions)
    since Playnite now natively provides this functionality.
* Added:
  * Game-specific context menu entries to add or remove Ludusavi-related tags.
  * Option to restore save data for a game before playing it.
  * Option to remember your choice when asked if you want to back up a game.
  * Several new tags to tweak behavior for individual games:
    * `[Ludusavi] Game: backup`
    * `[Ludusavi] Game: backup and restore`
    * `[Ludusavi] Game: no backup`
    * `[Ludusavi] Game: no restore`
    * `[Ludusavi] Platform: backup`
    * `[Ludusavi] Platform: backup and restore`
    * `[Ludusavi] Platform: no backup`
    * `[Ludusavi] Platform: no restore`
* Fixed:
  * When doing a backup or restore of specific games via their context menu,
    the plugin did not check whether another backup/restore was already in progress.

## v0.6.1 (2022-01-14)

* Fixed:
  * Crash when a game in Playnite did not have any platforms set.

## v0.6.0 (2021-10-08)

* Changed:
  * Updated for Playnite 9 and dropped support for earlier Playnite versions.
    ([Contributed by sharkusmanch](https://github.com/mtkennerly/ludusavi-playnite/pull/19))

## v0.5.1 (2020-11-16)

* Fixed:
  * Unresponsiveness when backing up after playing a game if its total backup
    size was very large (multiple GiB). The backup would still complete, but
    Playnite would lock up until it finished.
  * Error message in any backup/restore when the total file size was very large.
    The backup/restore itself still worked, but it could not be reported properly.

## v0.5.0 (2020-11-01)

* Added:
  * Support for per-platform custom entries.
  * A note in the confirmation popups about needing custom entries for non-PC games.
  * Vertical scrollbar in settings when the window is too small.
* Fixed:
  * The confirmation popup when selecting multiple games now indicates the
    platform suffix (if any) that will be used when looking up each game.

## v0.4.0 (2020-10-14)

* Added:
  * Game-specific context menu entry to back up or restore the selected games.
  * Detection for Playnite's active language. Although the plugin does not yet
    have any translations, this will enable translations in the future.
* Changed:
  * Updated for Playnite 8 and dropped support for Playnite 7.
  * Main menu items are now grouped under `Extensions` > `Ludusavi`.
    Previously, they were directly inside of `Extensions` and just had
    a "Ludusavi:" prefix on each label.

## v0.3.0 (2020-10-08)

* Added:
  * Support for [erri120's extension update notifier](https://github.com/erri120/Playnite.Extensions#extensions-updater).
  * Ability to ignore specific games by tagging them as `ludusavi-skip`.
  * Option to only show notifications on failure.

## v0.2.0 (2020-08-03)

[Ludusavi v0.7.0 or newer](https://github.com/mtkennerly/ludusavi/releases) is now required.

* Added:
  * After doing a backup or restore of all games, you can click the notification
    for a full list of all games processed.
  * "Browse" button for Ludusavi executable file.
  * "Browse" and "open" buttons for backup directory.
* Fixed:
  * Some error conditions reported by Ludusavi were not handled properly.
* Changed:
  * By default, the plugin no longer tries to back up non-PC games when you
    finish playing them, but you can configure the plugin to do so if you like.
  * When doing backups, the plugin now asks Ludusavi to try updating its manifest
    so that you can benefit from the latest updates without opening Ludusavi itself.
    If the update doesn't work (such as when your Internet is down), the plugin
    will still operate as normal; a connection is only required for the very first
    download of the manifest.
  * File size units are now adjusted based on the size, rather than always using MiB.
  * The default backup directory now writes out the user folder in full rather
    than using the `~` placeholder (although that is still supported).
  * You can now dismiss notifications by clicking on them.

## v0.1.0 (2020-07-29)

* Initial release.
