#!/usr/bin/env bash
# Publishes a self-contained linux-x64 build and stages an installable tree
# (with a .desktop entry and icon). If appimagetool is on PATH, also builds an
# AppImage into dist/.
set -euo pipefail

cd "$(dirname "$0")/.."

VERSION="${1:-$(git describe --tags --always 2>/dev/null || echo dev)}"
RID="linux-x64"
CONFIG="Release"
APP="ODExplorer.UI.Avalonia"
OUT="dist/odexplorer-${VERSION}-${RID}"

echo "==> Publishing ${APP} (${RID}, self-contained)..."
dotnet publish "src/${APP}/${APP}.csproj" -c "$CONFIG" -r "$RID" --self-contained true -o "${OUT}/publish"

echo "==> Staging desktop integration..."
mkdir -p "${OUT}/usr/share/applications" "${OUT}/usr/share/icons/hicolor/64x64/apps"
cp packaging/odexplorer.desktop "${OUT}/usr/share/applications/"
cp src/ODExplorer.UI.Avalonia/Assets/od-explorer-icon-64.png "${OUT}/usr/share/icons/hicolor/64x64/apps/odexplorer.png"

echo "==> Layout:"
echo "    ${OUT}/publish          self-contained app + runtime"
echo "    ${OUT}/usr/share        .desktop entry and icon (install to /usr/share)"

# Optional AppImage build (requires appimagetool: https://github.com/AppImage/AppImageKit)
if command -v appimagetool >/dev/null 2>&1; then
    echo "==> Building AppImage..."
    APPDIR="$(mktemp -d)/ODExplorer.AppDir"
    mkdir -p "${APPDIR}/usr/bin"
    cp -a "${OUT}/publish/." "${APPDIR}/usr/bin/"
    cp packaging/AppRun "${APPDIR}/AppRun"
    chmod +x "${APPDIR}/AppRun"
    cp packaging/odexplorer.desktop "${APPDIR}/odexplorer.desktop"
    cp src/ODExplorer.UI.Avalonia/Assets/od-explorer-icon-64.png "${APPDIR}/odexplorer.png"
    appimagetool "${APPDIR}" "dist/ODExplorer-${VERSION}-${RID}.AppImage"
    echo "==> dist/ODExplorer-${VERSION}-${RID}.AppImage"
else
    echo "==> appimagetool not found; skipping AppImage (self-contained tree built anyway)."
fi

echo "Done."
