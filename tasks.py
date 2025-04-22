import datetime as dt
import os
import re
import shutil
import sys
from glob import glob
from pathlib import Path

import yaml
from invoke import task

REPO = Path(os.path.dirname(__file__))
LANG = REPO / "lang"


def get_version():
    manifest = (REPO / "extension.yaml").read_text()
    return yaml.safe_load(manifest)["Version"]


def replace_pattern_in_file(file: Path, old: str, new: str, count: int = 1):
    content = file.read_text("utf-8")
    updated = re.sub(old, new, content, count=count)
    file.write_text(updated, "utf-8")


def confirm(prompt: str):
    response = input(f"Confirm by typing '{prompt}': ")
    if response.lower() != prompt.lower():
        sys.exit(1)


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


@task
def prerelease(ctx, new_version, update_lang=True):
    date = dt.datetime.now().strftime("%Y-%m-%d")

    replace_pattern_in_file(
        REPO / "extension.yaml",
        'Version: .+',
        f'Version: {new_version}',
    )

    replace_pattern_in_file(
        REPO / "CHANGELOG.md",
        "## Unreleased",
        f"## v{new_version} ({date})",
    )

    if update_lang:
        lang(ctx)


@task
def release(ctx):
    version = get_version()

    confirm(f"release {version}")

    ctx.run(f'git commit -m "Release v{version}"')
    ctx.run(f'git tag v{version} -m "Release"')
    ctx.run("git push")
    ctx.run(f"git push origin tag v{version}")
