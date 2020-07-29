# Ludusavi for Playnite
This repository contains a [Playnite](https://playnite.link) plugin to
back up your save data using [Ludusavi](https://github.com/mtkennerly/ludusavi).

## Features
* Known save locations for more than 7,000 games.
* On-demand backup and restore for all saves.
* Automatically back up a game when you finish playing it.

## Setup
### Ludusavi
Refer to the [Ludusavi project](https://github.com/mtkennerly/ludusavi)
for instructions on how to install Ludusavi itself. You'll need Ludusavi
version 0.6.0 or newer.

By default, the plugin will look for the Ludusavi executable in your `PATH`
environment variable, but you can also configure the plugin with the full
path to the executable.

### Plugin
You can download the plugin from the
[releases page](https://github.com/mtkennerly/ludusavi-playnite/releases).
There are two ways to install it:

* Using the `*.pext` file:
  * Download `ludusavi-playnite-v*.pext`
  * Drag and drop the file into Playnite
* Using the `*.zip` file:
  * Download `ludusavi-playnite-v*.zip`
  * Extract it into a subfolder of the Playnite extensions directory
    (e.g., `C:/Users/<YOUR_NAME>/AppData/Local/Playnite/Extensions/Ludusavi`)

### Configuration
The plugin provides some of its own configuration (see the screenshots below),
but you'll also need to open Ludusavi to configure it as needed. For example,
at this time, the plugin is not able to tell Ludusavi about your Playnite
library sources, so you'll need to configure Ludusavi's roots separately.

## Screenshots
### Prompt after exiting a game
> ![Screenshot of prompt after exiting a game](docs/prompt.png)

### Menu actions
> ![Screenshot of menu actions](docs/actions.png)

### Notifications
> ![Screenshot of notifications](docs/notifications.png)

### Settings
> ![Screenshot of settings](docs/settings.png)

## Other notes
* The backup and restore operations run in the background after you start them.
  You'll get a notification in Playnite when they finish. If you try to start
  another operation before the first one finishes, you'll have to wait.
* When processing a single game, the plugin will first ask Ludusavi to look it
  up by name. Sometimes, that can fail since Playnite and Ludusavi don't always
  use the same names, so if Ludusavi doesn't recognize it, and if the game is
  from Steam, then the plugin will ask again with the game's Steam ID.
  If that fails too, then you'll get a notification that no data was found.
* For backups, the plugin always sets Ludusavi's `--merge` flag. This way,
  if you back up saves for one game, it will not interfere with any backups
  you may have for another game.
* Only one set of save backups is kept for each game. If you decide that
  you'd like to archive a particular set of backups, simply copy them to an
  external drive or cloud storage.

## Development
Please refer to [CONTRIBUTING.md](./CONTRIBUTING.md).
