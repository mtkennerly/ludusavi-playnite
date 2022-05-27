import os
import shutil
from glob import glob
from pathlib import Path

import yaml
from invoke import task

REPO = Path(os.path.dirname(__file__))


def get_version():
    manifest = (REPO / "extension.yaml").read_text()
    return yaml.safe_load(manifest)["Version"]


@task
def build(ctx):
    if not (REPO / "vendor" / "Linguini").exists():
        ctx.run("git clone git@github.com:Ygg01/Linguini.git vendor/Linguini")
        with ctx.cd("vendor/Linguini"):
            ctx.run("git checkout 9a7939b00c3ca68df19b45afc0e39bad05c0b483")

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
def deploy(ctx, target="~/AppData/Local/Playnite/Extensions"):
    target = Path(target).expanduser() / "Ludusavi"
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
