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

## Development
Please refer to [CONTRIBUTING.md](./CONTRIBUTING.md).
