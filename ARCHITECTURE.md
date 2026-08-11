# ODExplorer Architecture

## Overview

Three layers, all running in one process:

```
Elite Dangerous journal (JSON files)
        │
        ▼
EliteJournalReader (vendored real lib v3.7.4)
        │  typed events
        ▼
JournalParserStore ──► OrganicCheckListDataStore ──► ExplorationDataStore
        │                    │                              │
        │                    ▼                              ▼
        │              ViewModels ◄────── SettingsStore / SpanshCsvStore
        │                    │
        ▼                    ▼
Avalonia Views (axaml) ──► ToastHost / Audio / Clipboard (host adapters)
```

- **Core** (`ODExplorer.Core`) — platform-agnostic. No WPF, no shelling out.
- **UI** (`ODExplorer.UI.Avalonia`) — the Avalonia shell plus the concrete
  implementations of the host-adapter seams.
- **Vendored parser** (`EliteJournalReader`) — the real journal parsing library,
  checked in as source so no private-package dependency is needed.

## Host-adapter seams

Core stays UI-free by calling static provider singletons that the UI layer
populates at startup (`App.axaml.cs → OnFrameworkInitializationCompleted`):

| Seam | Core type | UI implementation |
| --- | --- | --- |
| Audio playback | `IAudioPlayerProvider.Current` | `AudioPlayer` (PulseAudio via `libpulse-simple` P/Invoke) |
| Clipboard / open URL | `OdUtilsAdapterProvider.Current` | `OdUtilsAdapter` (wl-copy / xclip) |
| Dispatcher | `DispatcherHelper.Current` | `DispatcherAdapter` (Avalonia `Dispatcher`) |
| MessageBox | `MessageBoxRequester.Requested` | `MessageBoxService` (Avalonia dialog) |
| Toasts | `NotificationStore.OnToast` | `ToastHost` (Avalonia corner pop-ups) |

## Data flow

1. `JournalParserStore` tails the journal directory and feeds every line through
   `JournalEventMapper`, which delegates parsing to EliteJournalReader and
   swallows malformed events.
2. `ExplorationDataStore` rebuilds the current-system body list (`Scan`,
   `FSSBodySignals`), tracks mapped/sold state, triggers EDSM background lookups
   (`EdsmApiService`, real HTTP), and raises notification events.
3. `OrganicCheckListDataStore` records scanned/sold organic species and codex
   entries; `ExoPredictionEngine` replaces "Not Predicted" placeholders with the
   species whose BioScan rules match the body's scan data.
4. View models subscribe to store events and drive the Avalonia views; toasts
   are raised through `NotificationStore` and rendered by `ToastHost`.

## Exobiology prediction

- `Stubs/ExoPredictionData.cs` — the 15 Odyssey genera, generated from the
  BioScan rulesets (see the header comment).
- `Stubs/ExoPredictionEngine.cs` — mirrors the BioScan matcher.
- `Stubs/ExoGalaxyData.cs` — galaxy regions/nebulae/guardian-zone data, ported
  but **not yet consumed**: the engine deliberately excludes galaxy-position
  rules so it under-predicts rather than wrongly predicts. Wiring region/nebula
  matching requires coordinates on the body/system model plus a
  BioScan-compatible region resolver (current `GalacticRegionsMap` stub is a
  coarse distance heuristic).

## Stubs

`ODUtils` (the original Windows-only helper library) is private and cannot be
restored as a package, so a small set of `Stubs/` files re-implements the
surface Core needs. EliteJournalReader, by contrast, is the real library vendored
in-tree. See `migration_notes.md` for the reconciliation history.

## Testing

- **TestApp** (`src/TestApp`): console smoke suite (~120 checks) exercising the
  core pipeline end-to-end with an in-memory DB: journal parse, persistence,
  settings round-trip, toasts, EDSM flow, Spansh CSV, carrier timer.
- **NUnit headless suite** (`src/ODExplorer.UI.Avalonia.Tests`): runs the real
  Avalonia view graph headlessly (no X11) — toast-host behaviour, pop-out
  windows, WAV parsing, and live PulseAudio playback (skipped if no audio server).
- **JournalReplay** (`src/JournalReplay`): replays a real journal folder through
  the exact production pipeline and asserts state sanity + DB parity.

## Build

`dotnet build ODExplorer.sln -c Release` (Core + UI + Tests) — CI runs this plus
both test suites on every push.
