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
