# ODExplorer Linux

A cross-platform (Linux-first) port of **ODExplorer**, an Elite Dangerous companion
app for exploration and exobiology. It watches the game's journal in real time,
tracks discovered bodies and organic samples, predicts exobiology species on
scanned planets, shows Vista Genomics-style values, and raises toast/audio
notifications when something notable turns up.

The UI is **Avalonia** (not WPF, which the original Windows app used); the core
layer is WPF-free and platform-agnostic.

## Requirements

- .NET 8 SDK (`dotnet --version` → 8.x). The solution also builds cleanly on SDK 9/10.
- A Linux desktop with:
  - **PulseAudio or PipeWire** (for the fleet-carrier timer sound). If no audio
    server is available the app silently skips the sound.
  - **wl-copy / xclip** (Wayland/X11 clipboard) for copy-to-clipboard helpers.
- A copy of Elite Dangerous' journal folder (e.g. `~/Saved Games/Frontier Developments/Elite Dangerous`).

## Build & run

```sh
dotnet restore
dotnet build ODExplorer.sln -c Release
dotnet run --project src/ODExplorer.UI.Avalonia/ODExplorer.UI.Avalonia.csproj -c Release
```

On first launch the app asks for your journal folder (Settings → *Change Logs Folder*).

## Tests

```sh
# Console smoke suite (core pipeline + adapters, ~120 checks, no UI)
dotnet run --project src/TestApp/TestApp.csproj -c Release

# Headless Avalonia UI tests (NUnit, no display needed)
dotnet test src/ODExplorer.UI.Avalonia.Tests/ODExplorer.UI.Avalonia.Tests.csproj -c Release

# Replay a real journal folder through the production pipeline
dotnet run --project src/JournalReplay/JournalReplay.csproj -c Release -- --dir "<Saved Games>/.../Elite Dangerous"
```

`ci.yml` runs the build, the TestApp smoke suite, and the NUnit tests on every push.

## Project layout

| Project | Purpose |
| --- | --- |
| `src/ODExplorer.Core` | Platform-agnostic core: journal pipeline, stores, settings, notifications, view models, prediction engine. No WPF/UI references. |
| `src/ODExplorer.UI.Avalonia` | Avalonia desktop UI: views, host-adapter implementations (audio, clipboard, dispatcher), app wiring. |
| `src/ODExplorer.UI.Avalonia.Tests` | Headless NUnit tests (toast host, pop-out windows, WAV reader, live PulseAudio playback). |
| `src/EliteJournalReader` | Vendored real EliteJournalReader v3.7.4 (journal parsing). |
| `src/TestApp` | Console smoke test of the core pipeline + adapters. |
| `src/JournalReplay` | Headless replay of a real journal directory through the exact production pipeline. |

## Packaging

`scripts/publish.sh` produces a self-contained `linux-x64` build. See
[ARCHITECTURE.md](ARCHITECTURE.md) and `migration_notes.md` for design decisions
and the remaining porting checklist.
