import os
import shutil
from glob import glob
from pathlib import Path

from invoke import task

REPO = Path(os.path.dirname(__file__))


@task
def build(ctx, debug=False):
    if debug:
        ctx.run("dotnet build")
    else:
        ctx.run("dotnet build -c Release")


@task
def deploy(ctx, target="~/AppData/Local/Playnite/Extensions"):
    target = Path(target).expanduser() / "LudusaviPlaynite"
    if target.exists():
        print(f"Deleting: {target}")
        shutil.rmtree(str(target))
    print(f"Creating: {target}")
    target.mkdir()

    for file in glob(str(REPO / "bin/Release/net462/*")):
        shutil.copy(file, target)


@task
def style(ctx):
    ctx.run("dotnet format")
