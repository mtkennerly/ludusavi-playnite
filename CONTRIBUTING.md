## Development
### Requirements
* .NET Core SDK with the `dotnet` CLI
* Python 3.5+

Run this to install additional dependencies:

```
dotnet tool install -g dotnet-format
pip install invoke pre-commit pyyaml
```

Then activate pre-commit hooks:

```
pre-commit install
```

### Commands
* Build:
  * `invoke build`
* Auto-format code:
  * `invoke style`
* Pack for release (creates `dist` folder with `*.zip` and `*.pext`):
  * `invoke pack`
* Deploy to Playnite extensions folder:
  * Default location (`~/AppData/Local/Playnite/Extensions/Ludusavi`): `invoke deploy`
  * Custom location (`<custom-folder>/Ludusavi`): `invoke deploy --target <custom-folder>`

You can chain `invoke` commands, such as: `invoke build deploy`.
