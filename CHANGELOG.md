## Unreleased

[Ludusavi v0.7.0 or newer](https://github.com/mtkennerly/ludusavi/releases) is now required.

* Added:
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
    will still operate as normal.
  * File size units are now adjusted based on the size, rather than always using MiB.
  * The default backup directory now writes out the user folder in full rather
    than using the `~` placeholder (although that is still supported).

## v0.1.0 (2020-07-29)

* Initial release.
