## Development
### Requirements
* .NET Core SDK with the `dotnet` CLI
* Python 3.5+

Run this to install additional dependencies:

```
dotnet tool install -g dotnet-format
pip install invoke pre-commit
```

Then activate pre-commit hooks:

```
pre-commit install
```

### Commands
* Build:
  * Release: `invoke build`
  * Debug: `invoke build --debug`
* Auto-format code:
  * `invoke style`
* Deploy to Playnite extensions folder:
  * Default location (`~/AppData/Local/Playnite/Extensions/LudusaviPlaynite`): `invoke deploy`
  * Custom location (`<custom-folder>/LudusaviPlaynite`): `invoke deploy --target <custom-folder>`

You can chain `invoke` commands, such as: `invoke build deploy`.
