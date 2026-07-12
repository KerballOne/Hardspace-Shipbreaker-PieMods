#!/usr/bin/env bash
#
# Packages built BepInEx plugins into one folder + one zip per mod.
#
# Usage: build/package.sh [Config]      (default Config: Release)
#
# For each shipped mod this produces:
#   dist/<Mod>/BepInEx/...     - the tree (uploaded as the CI artifact <Mod>)
#   dist/<Mod>.zip             - the same tree zipped (attached to releases)
#
# The layout mirrors the game's Content/ folder, so a user just extracts into
#   .../Hardspace Shipbreaker/   (the folder that contains BepInEx/)
#
set -euo pipefail

CONFIG="${1:-Release}"
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT="$ROOT/dist"

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

# --- Base packaging: the plugin dll -----------------------------------------
for mod in "${MODS[@]}"; do
  dest="$OUT/$mod/BepInEx/plugins/$mod"
  mkdir -p "$dest"

  dll="$(bindir "$mod")/$mod.dll"
  [ -f "$dll" ] || { echo "ERROR: missing build output: $dll" >&2; exit 1; }
  cp "$dll" "$dest/"
done

# --- Per-mod extras ---------------------------------------------------------
# ModdedShipLoader ships a BepInEx preloader patcher alongside the plugin.
patchdest="$OUT/ModdedShipLoader/BepInEx/patchers/ModdedShipLoaderPatcher"
mkdir -p "$patchdest"
cp "$(bindir ModdedShipLoaderPatcher)/ModdedShipLoaderPatcher.dll" "$patchdest/"
cp "$ROOT/ModdedShipLoaderPatcher/TypesToModify.txt" "$patchdest/"

# ShiftRecorder needs its bundled video-capture runtime deps to actually record.
srdest="$OUT/ShiftRecorder/BepInEx/plugins/ShiftRecorder"
cp "$ROOT/ShiftRecorder/externals/VideoCaptureLib.dll" "$srdest/"
cp "$ROOT/ShiftRecorder/externals/ffmpeg.exe" "$srdest/"

# --- Zip each mod (pure folder name, no version suffix) ---------------------
for mod in "${MODS[@]}"; do
  ( cd "$OUT/$mod" && zip -qr "$OUT/$mod.zip" BepInEx )
  echo "packaged  $mod -> dist/$mod.zip  (+ dist/$mod/ tree)"
done

echo
echo "dist/:"
ls -1 "$OUT"
