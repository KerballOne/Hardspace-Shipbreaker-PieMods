#!/usr/bin/env bash
#
# Packages built BepInEx plugins into per-mod release zips (and one combined zip).
#
# Usage: build/package.sh [Config] [Version]
#   Config  - MSBuild configuration to package (default: Release)
#   Version - string embedded in the zip file names (default: dev)
#
# Each zip mirrors the game's Content/ folder, so a user just extracts it into
#   .../Hardspace Shipbreaker/   (the folder that contains BepInEx/)
#
set -euo pipefail

CONFIG="${1:-Release}"
VERSION="${2:-dev}"
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT="$ROOT/dist"
STAGE="$OUT/staging"

# Mods that ship in releases (TestProj / TestProjectPatcher are excluded on purpose).
MODS=(
  CustomPostersAndStickers
  DebugMenu
  EndGameDebtModifier
  FOVAdjuster
  ModdedShipLoader
  PieChallengeMode
  ShiftRecorder
  UpgradeCostAdjuster
)

bindir() { echo "$ROOT/$1/bin/$CONFIG/net48"; }

rm -rf "$OUT"
mkdir -p "$STAGE"

# --- Base packaging: the plugin dll + its settings.json ---------------------
for mod in "${MODS[@]}"; do
  dest="$STAGE/$mod/BepInEx/plugins/$mod"
  mkdir -p "$dest"

  dll="$(bindir "$mod")/$mod.dll"
  [ -f "$dll" ] || { echo "ERROR: missing build output: $dll" >&2; exit 1; }
  cp "$dll" "$dest/"

  [ -f "$ROOT/$mod/settings.json" ] && cp "$ROOT/$mod/settings.json" "$dest/"
done

# --- Per-mod extras ---------------------------------------------------------
# ModdedShipLoader ships a BepInEx preloader patcher alongside the plugin.
patchdest="$STAGE/ModdedShipLoader/BepInEx/patchers/ModdedShipLoaderPatcher"
mkdir -p "$patchdest"
cp "$(bindir ModdedShipLoaderPatcher)/ModdedShipLoaderPatcher.dll" "$patchdest/"
cp "$ROOT/ModdedShipLoaderPatcher/TypesToModify.txt" "$patchdest/"

# ShiftRecorder needs its bundled video-capture runtime deps to actually record.
srdest="$STAGE/ShiftRecorder/BepInEx/plugins/ShiftRecorder"
cp "$ROOT/ShiftRecorder/externals/VideoCaptureLib.dll" "$srdest/"
cp "$ROOT/ShiftRecorder/externals/ffmpeg.exe" "$srdest/"

# --- Zip each mod + a combined "all mods" bundle ----------------------------
for mod in "${MODS[@]}"; do
  ( cd "$STAGE/$mod" && zip -qr "$OUT/${mod}-${VERSION}.zip" BepInEx )
  echo "packaged  $mod -> dist/${mod}-${VERSION}.zip"
done

combined="$OUT/_combined"
mkdir -p "$combined"
for mod in "${MODS[@]}"; do cp -a "$STAGE/$mod/." "$combined/"; done
( cd "$combined" && zip -qr "$OUT/PieMods-AllMods-${VERSION}.zip" BepInEx )
echo "packaged  <all mods> -> dist/PieMods-AllMods-${VERSION}.zip"

rm -rf "$combined" "$STAGE"
echo
echo "Artifacts in dist/:"
ls -1 "$OUT"
