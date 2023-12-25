import os
import shutil
from glob import glob
from pathlib import Path

import yaml
from invoke import task

REPO = Path(os.path.dirname(__file__))
LANG = REPO / "lang"


def get_version():
    manifest = (REPO / "extension.yaml").read_text()
    return yaml.safe_load(manifest)["Version"]


@task
def build(ctx):
    ctx.run("dotnet build src -c Release")


@task
def pack(ctx, toolbox="~/AppData/Local/Playnite/Toolbox.exe"):
    target = REPO / "dist/raw"
    if target.exists():
        shutil.rmtree(str(target))
    os.makedirs(str(target))
    for file in glob(str(REPO / "src/bin/Release/net462/*")):
        shutil.copy(file, target)

    toolbox = Path(toolbox).expanduser()
    ctx.run('"{}" pack "{}" dist'.format(toolbox, target))
    for file in glob(str(REPO / "dist/*.pext")):
        if "_" in file:
            shutil.move(file, str(REPO / "dist/ludusavi-playnite-v{}.pext".format(get_version())))

    shutil.make_archive(str(REPO / "dist/ludusavi-playnite-v{}".format(get_version())), "zip", str(target))


@task
def deploy(ctx, target="~/AppData/Roaming/Playnite/Extensions"):
    target = Path(target).expanduser() / "mtkennerly.ludusavi"
    if target.exists():
        print(f"Deleting: {target}")
        shutil.rmtree(str(target))
    print(f"Creating: {target}")
    target.mkdir()

    for file in glob(str(REPO / "src/bin/Release/net462/*")):
        shutil.copy(file, target)


@task
def style(ctx):
    ctx.run("dotnet format src")


@task
def lang(ctx, jar="/opt/crowdin-cli/crowdin-cli.jar"):
    ctx.run(f'java -jar "{jar}" pull --export-only-approved')

    mapping = {}
    for file in LANG.glob("*.ftl"):
        if "en-US.ftl" in file.name:
            continue
        content = file.read_text("utf8")
        if content not in mapping:
            mapping[content] = set()
        mapping[content].add(file)

    for group in mapping.values():
        if len(group) > 1:
            for file in group:
                file.unlink()
