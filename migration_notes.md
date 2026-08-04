Migration Handoff Note — ODExplorer core extraction + UI adapter implementation

Status (2026-07-31T21:47:00-04:00):
- Branch: migration/extract-core-from-windows-app (MERGED into main). Extracted core, removed WPF deps, added adapter interfaces + NoOp defaults.
- Branch: migration/implement-ui-adapters (MERGED into main). Implemented UI adapter services (Dispatcher, Audio, Notifications, OdUtils, PlatformPaths); wired in App startup.
- Core (ODExplorer.Core) builds successfully (net8.0).
- UI (ODExplorer.UI.Avalonia) builds successfully (net8.0), references ODExplorer.Core.
- TestApp runs: SettingsStore.LoadSettings() with NoOp/shell adapters.
- CI workflow at .github/workflows/ci.yml builds both projects and runs TestApp on ubuntu-latest. All checks passing.

Files changed (complete list):

**Core extraction (migration/extract-core-from-windows-app):**
- Added:
  - src/ODExplorer.Core/Adapters/IOdUtilsAdapter.cs (minimal interface + NoOp)
  - src/ODExplorer.Core/Adapters/IEliteJournalReaderAdapter.cs (CoreJournalEntry DTO)
  - src/ODExplorer.Core/Adapters/INotificationAdapter.cs (NotificationModel + NoOp)
  - src/ODExplorer.Core/Adapters/IPlatformPaths.cs (paths interface + NoOp)
  - src/ODExplorer.Core/Audio/IAudioPlayer.cs (play/stop/status)
  - src/ODExplorer.Core/Audio/IAudioPlayerProvider.cs (static Current provider)
  - src/ODExplorer.Core/Stubs/* (minimal compile-time stubs for ODUtils/EliteJournalReader types)
  - src/TestApp/* (minimal console app; wires NoOp adapters, loads SettingsStore)
  - .github/workflows/ci.yml (builds on ubuntu-latest: restore, build, run TestApp)
- Modified:
  - src/ODExplorer.Core/Models/PropertyChangeNotify.cs (DispatcherHelper/IDispatcher)
  - src/ODExplorer.Core/Extensions/CollectionExtentions.cs (use DispatcherHelper instead of WPF dispatcher)
  - src/ODExplorer.Core/Extensions/DataGridExtentions.cs (UI placeholder)
  - src/ODExplorer.Core/Models/GridSize.cs (GridLength/GridUnitType core enums)
  - src/ODExplorer.Core/Models/MessageBoxEventArgsAsync.cs (core enums for MessageBox)
  - src/ODExplorer.Core/Stores/NotificationStore.cs (core facade, uses INotificationAdapter)
  - src/ODExplorer.Core/Stores/SettingsStore.cs (no System.Windows; DB provider is dynamic)
  - src/ODExplorer.Core/ODExplorer.Core.csproj (no WPF packages; excluded host-only code)

**UI adapter implementation (migration/implement-ui-adapters):**
- Added:
  - src/ODExplorer.UI.Avalonia/Services/DispatcherAdapter.cs (implements ODExplorer.Models.IDispatcher; uses Avalonia.Threading.Dispatcher.UIThread)
  - src/ODExplorer.UI.Avalonia/Services/AudioPlayer.cs (implements IAudioPlayer; uses paplay/aplay fallback for Linux)
  - src/ODExplorer.UI.Avalonia/Services/OdUtilsAdapter.cs (implements IOdUtilsAdapter; parses GalacticRegions enum)
  - src/ODExplorer.UI.Avalonia/Services/NotificationAdapter.cs (implements INotificationAdapter; uses notify-send on Linux)
  - src/ODExplorer.UI.Avalonia/Services/PlatformPaths.cs (implements IPlatformPaths; uses Assembly location + env paths)
- Modified:
  - src/ODExplorer.UI.Avalonia/App.axaml.cs (OnFrameworkInitializationCompleted wires adapters: sets DispatcherHelper.Current, IAudioPlayerProvider.Current)
  - src/ODExplorer.UI.Avalonia/ODExplorer.UI.Avalonia.csproj (added ProjectReference to ODExplorer.Core; target net8.0; qualified IDispatcher to avoid Avalonia conflict)

Build & Test Commands (local development):
- dotnet restore
- dotnet build ODExplorer.sln -c Release (or build individual projects)
- dotnet run --project src/TestApp/TestApp.csproj -c Release (verify core + adapters)
- dotnet run --project src/ODExplorer.UI.Avalonia/ODExplorer.UI.Avalonia.csproj -c Release (run Avalonia app; may require desktop runtime)

Testing steps for the next agent (what to validate after cloning):
1) Clone and fetch latest main:
   git clone https://github.com/bossmanbutts/odexplorer-linux.git
   cd odexplorer-linux
   git fetch origin main
   git checkout main (or main/master depending on default branch name)
2) Restore and build solution:
   dotnet restore
   dotnet build ODExplorer.sln -c Release
   Expected: both ODExplorer.Core and ODExplorer.UI.Avalonia build successfully.
3) Run TestApp (core + adapter smoke test):
   dotnet run --project src/TestApp/TestApp.csproj -c Release
   Expected output: "Settings loaded successfully (no-op provider)."
4) Optionally run Avalonia UI:
   dotnet run --project src/ODExplorer.UI.Avalonia/ODExplorer.UI.Avalonia.csproj -c Release
   Expected: window appears (may be minimal since WPF views not yet ported).
5) Check CI:
   Visit https://github.com/bossmanbutts/odexplorer-linux/actions
   Verify latest main branch build passed (should show green checkmarks).

What the next agent should do (step-by-step):

**Phase 1: Verify state (immediate)**
1) Clone and build (see Testing steps above).
2) Run CI locally and via Actions.
3) Run TestApp and verify output.
4) If any build/run error: investigate and fix surgically.

**Phase 2: Replace NoOp adapters and stubs (medium-term)**
1) Replace IOdUtilsAdapter NoOp with real implementation (or vendor minimal ODUtils types from source library if available).
2) Replace INotificationAdapter shell invocation with a robust cross-platform notification library (consider libappindicator3-dev for Linux, or Avalonia's built-in toast).
3) Replace IAudioPlayer shell invocation with a real audio library (recommend ManagedBass or LibVLCSharp for cross-platform support).
4) Remove Stubs/ directory and reconcile types with actual external libraries (ODUtils, EliteJournalReader).
5) Rebuild, test, commit: "feat(adapters): replace NoOp implementations with real cross-platform libraries".

**Phase 3: Port remaining ViewModel/Store code to UI layer (medium-term)**
1) Audit remaining ViewModels in src/ODExplorer.Core/ViewModels/ for WPF/UI dependencies.
2) Move UI-specific ViewModels into src/ODExplorer.UI.Avalonia/ViewModels/ or refactor to use INotificationAdapter/IDialogService/etc.
3) Remove any remaining System.Windows references from core.
4) Rebuild and test.
5) Commit: "refactor: migrate remaining ViewModel UI logic to UI layer".

**Phase 4: Port WPF Views to Avalonia (long-term)**
1) Port Views/*.xaml (and handlers) to ODExplorer.UI.Avalonia/Views/*.axaml.
2) Rewrite converters to Avalonia equivalents.
3) Replace WPF commands with Avalonia RoutedCommands or ReactiveUI.
4) Incrementally test each view.

**Phase 5: Harden CI and documentation (ongoing)**
1) Extend CI to run unit tests if tests exist.
2) Add solution-wide build step (currently only Core + TestApp).
3) Update README.md with build instructions, architecture notes, and porting checklist.
4) Create ARCHITECTURE.md describing the three-layer structure (Core, UI, Host adapters).

TODOs / Known issues (next agent fill these in as they work):
- [ ] Replace shell-based audio (paplay/aplay) with real library (ManagedBass or LibVLCSharp).
- [ ] Replace shell-based notifications (notify-send) with libappindicator3-dev or Avalonia UI.
- [ ] Reconcile stubs (src/ODExplorer.Core/Stubs/*) with actual ODUtils/EliteJournalReader/ToastNotifications packages.
- [ ] Port Views/*.xaml from WPF to Avalonia.
- [ ] Test on actual Linux desktop (desktop environment, GNOME/KDE/etc.) to verify notifications/audio work end-to-end.
- [ ] Consider packaging strategy (dotnet publish + AppImage, Flatpak, or snap) for distribution.

Notes & rationale:
- Option C (adapter/wrapper pattern) was chosen to keep core platform-agnostic and small.
- Small in-repo stubs were added only to satisfy compilation and must be reconciled with source libraries (ODUtils, EliteJournalReader) later.

Where to find artifacts in this session:
- Bundle (if needed): /home/milo/Projects/odexplorer-linux/migration-extract-core-from-windows-app.bundle
- Local repo: /home/milo/Projects/odexplorer-linux

Contact points and resources for next agent:
- Core branch (merged): migration/extract-core-from-windows-app → see git log for commit details
- UI adapters branch (merged): migration/implement-ui-adapters → see git log for commit details
- CI workflow path: .github/workflows/ci.yml
- Test app path: src/TestApp/Program.cs + src/TestApp/TestApp.csproj
- Core interfaces: src/ODExplorer.Core/Adapters/ and src/ODExplorer.Core/Audio/
- UI adapter implementations: src/ODExplorer.UI.Avalonia/Services/
- App wiring: src/ODExplorer.UI.Avalonia/App.axaml.cs (OnFrameworkInitializationCompleted)
- Stubs (TODO: reconcile): src/ODExplorer.Core/Stubs/

Key design decisions (rationale):
- Option C (adapter/wrapper pattern) was chosen to keep core platform-agnostic, isolated from WPF, and testable with NoOp defaults.
- Shell-based fallbacks (paplay, notify-send) were used to avoid adding heavy native deps; next agent should replace with real libraries.
- Small in-repo stubs were added only to satisfy compilation; they must be reconciled with actual external libraries (ODUtils, EliteJournalReader) for production use.
- DispatcherHelper.Current is a static singleton; next agent may refactor to dependency injection if core becomes part of a larger DI container.

Keep this file updated as changes progress. Include exact git commands and CI logs for traceability.

Recent porting snapshot (what was just done)
- Replaced several WPF UI types in core ViewModels with UI-agnostic models/events:
  - StarSystemViewModel: removed System.Windows.ContextMenu/ToolTip and introduced MenuItemModel + MenuItems list
  - CartoDetailsViewModel: replaced blocking ODMessageBox.Show calls with MessageBoxRequester.Request (async event + callbacks)
  - NotificationStore: now calls OdUtilsAdapterProvider.Current.CopyToClipboard when available
- Added OdUtilsAdapterProvider static hook so the UI can provide clipboard/OpenUrl helpers at runtime
- Implemented concrete UI adapters in ODExplorer.UI.Avalonia/Services:
  - DispatcherAdapter, AudioPlayer (shell fallback), NotificationAdapter (notify-send), OdUtilsAdapter (clipboard/open-url), PlatformPaths
- Wired adapters in App startup and hooked MessageBoxRequester to show a non-blocking toast (temporary)
- Verified solution builds and Avalonia window opens; TestApp runs successfully

Instructions for the next agent (no repo access)
- To reproduce locally (commands to run on your machine):
  1) Fetch the latest main branch and adapter branch (if using bundles provided in this session):
     - If using bundles: git fetch /path/to/migration-implement-ui-adapters-v2.bundle migration/implement-ui-adapters:refs/heads/migration/implement-ui-adapters
     - Otherwise: git clone https://github.com/bossmanbutts/odexplorer-linux.git && cd odexplorer-linux && git fetch && git checkout main
  2) Build & run smoke tests:
     - dotnet restore
     - dotnet build ODExplorer.sln -c Release
     - dotnet run --project src/TestApp/TestApp.csproj -c Release  # expect "Settings loaded successfully"
     - dotnet run --project src/ODExplorer.UI.Avalonia/ODExplorer.UI.Avalonia.csproj -c Release  # optional GUI smoke test

- If you cannot push changes from this session, apply changes by fetching the bundle and pushing from your machine; then open PRs via the GitHub web UI.

Exact files to inspect first (for review or further porting):
- Core (focus on removing UI deps):
  - src/ODExplorer.Core/ViewModels/** (look for remaining System.Windows and convert as done above)
  - src/ODExplorer.Core/Stores/NotificationStore.cs
  - src/ODExplorer.Core/Models/MessageBoxEventArgsAsync.cs
  - src/ODExplorer.Core/Models/MessageBoxRequester.cs
  - src/ODExplorer.Core/Adapters/* and src/ODExplorer.Core/Audio/*
- UI adapter implementations:
  - src/ODExplorer.UI.Avalonia/Services/* (DispatcherAdapter, OdUtilsAdapter, NotificationAdapter, AudioPlayer, PlatformPaths)
  - src/ODExplorer.UI.Avalonia/App.axaml.cs (wiring)

Priority next tasks (for an autonomous agent without repo access):
1) Grep for remaining System.Windows, RoutedEventArgs, ContextMenu, ToolTip, MessageBox, DataGrid, ToastNotifications, NAudio, System.Drawing. Produce a prioritized list of files by frequency and complexity.
2) For each high-priority file, create a small transformation plan (1–3 edits) to replace UI types with core-friendly models/events or move UI logic to the UI project. Example: replace ContextMenu => MenuItemModel list (as done), replace MessageBox.Show => MessageBoxRequester.Request.
3) Implement an interactive MessageBox dialog in Avalonia (UI): subscribe to MessageBoxRequester.Requested and show a dialog that executes callbackYes/callbackNo appropriately.
4) Replace shell-based notification/audio with proper cross-platform libraries in UI; leave TODOs linking to recommended libs (ManagedBass, LibVLCSharp, libappindicator/Avalonia toast).
5) Remove stubs (src/ODExplorer.Core/Stubs/*) once actual libraries are vendored or adapters are implemented; update csproj references if needed.
6) Update README.md and add ARCHITECTURE.md describing adapter pattern, how to wire adapters, and test instructions.

Handoff and traceability guidance (how to operate without repo access):
- Use the provided bundle(s) in this session if direct git push/pull isn't possible. Bundles created in this session:
  - /home/milo/Projects/odexplorer-linux/migration-implement-ui-adapters-v2.bundle
  - /home/milo/Projects/odexplorer-linux/migration-implement-ui-adapters.bundle
  - /home/milo/Projects/odexplorer-linux/migration-extract-core-from-windows-app.bundle
- Always run dotnet restore then explicit dotnet build ODExplorer.sln -c Release (don’t rely on default dotnet commands in a folder with multiple sln files)
- When opening PRs use GitHub web UI (compare main...branch) and paste migration_notes.md content as PR body; include the checklist and TODOs above

When you finish work, update migration_notes.md with:
- exact git commit SHAs, files changed, CI run URLs, and any runtime test outputs (console logs)
- short developer notes on why each UI-related change was made and next action items

Recent update (automated porting step):
- Replaced remaining System.Windows dispatcher calls with DispatcherHelper across ViewModels (SpanshViewModel, MainViewModel, CartographicViewModel, SystemBodyViewModel and others).
- Introduced ODExplorer.Models.Visibility enum to replace System.Windows.Visibility in core (used by SettingsViewModel, OrganicViewModel).
- Replaced WPF ToolTip property in SystemBodyViewModel with ToolTipText (string) and moved complex tooltip controls to the UI layer.
- Replaced WindowState usage to rely on existing project stub (avoided duplicate definitions).
- Verified: dotnet build ODExplorer.sln -c Release succeeded locally (ODExplorer.Core and ODExplorer.UI.Avalonia built).

Files touched in this step:
- src/ODExplorer.Core/Models/Visibility.cs (added)
- src/ODExplorer.Core/ViewModels/ViewVMs/SpanshViewModel.cs (replaced dispatcher calls)
- src/ODExplorer.Core/ViewModels/ViewVMs/MainViewModel.cs (replaced dispatcher calls)
- src/ODExplorer.Core/ViewModels/ViewVMs/SettingsViewModel.cs (remove System.Windows usage; use Visibility enum)
- src/ODExplorer.Core/ViewModels/ViewVMs/CartographicViewModel.cs (replaced dispatcher calls)
- src/ODExplorer.Core/ViewModels/ViewVMs/OrganicViewModel.cs (use Visibility from ODExplorer.Models)
- src/ODExplorer.Core/ViewModels/ModelVMs/SystemBodyViewModel.cs (ToolTip -> ToolTipText; replaced dispatcher call)
- src/ODExplorer.Core/ViewModels/ModelVMs/WindowPositionViewModel.cs (now uses ODExplorer.Models.WindowState stub)
- src/ODExplorer.Core/ViewModels/ModelVMs/OrganicScanItemViewModel.cs (removed System.Windows using)

Notes and next actions:
- Some ViewModels still reference WPF collection/view types (ListCollectionView, CollectionViewSource) — these require a more involved refactor or a UI-side wrapper. Prioritize moving heavy view code into ODExplorer.UI.Avalonia/ViewModels.
- Implement an Avalonia MessageBox dialog to subscribe to MessageBoxRequester.Requests (next high-priority UI task).
- Run full test on a desktop Linux environment to verify notifications, audio, and clipboard functions from the UI adapters.

Latest handoff update (2026-07-31 23:05 -04:00)
- Paused because credits are low and the remaining work is a broad stub/compile cleanup.
- The immediate blocker is the remaining missing core/stub types needed to finish Core ViewModel compilation.
- Re-add the dropped import in `src/ODExplorer.Core/ViewModels/ModelVMs/StarSystemViewModel.cs`: `using ODUtils.Models;`
- The next agent should treat this note as the source of truth because it does not have repo access.

What the next agent should do first
1) Rebuild `ODExplorer.sln -c Release` and capture the current unique CS errors.
2) Fix the missing `StarSystemViewModel.cs` import.
3) Expand `src/ODExplorer.Core/Stubs/` for the remaining types:
   - `ODUtils.Models` (`StarSystem`, `SystemBody`, `DataState`, `DiscoveryStatus`, `PlanetClass`, `StarType`, `PlanetMaterial`, `OrganicVariant`, etc.)
   - `EliteJournalReader` (`OrganicScanItem`, `OrganicScanStage`, `OrganicScanState`, `PlanetRing`, and related enums)
   - Minimal `ODExplorer.Stores` facades for the store types still referenced by ViewModels
4) Rebuild until `ODExplorer.Core` compiles cleanly again.

What you can do now
- If you want to keep pushing locally, run `dotnet build ODExplorer.sln -c Release` and paste the current errors back here.
- If you want to hand this off cleanly, leave the branch as-is and let the next agent start from this updated note.
- If you resume later, the most useful thing to capture is the exact build output after the next stub pass.

Latest handoff update (2026-08-03, build fixed)
- Installed .NET SDK locally at ~/.dotnet (dotnet was not on PATH in this environment):
  - SDK 8.0.423 (channel 8.0) and SDK 10.0.302 (channel 10.0) via /tmp/dotnet-install.sh.
  - IMPORTANT: The solution must be built with the .NET 10 SDK. Avalonia 12.1.0 analyzers require Roslyn >= 4.14; the .NET 8 SDK (Roslyn 4.11) fails to run Avalonia.Generators, so `InitializeComponent` is NOT generated and the UI project fails with CS0103. With .NET 10 SDK the full solution builds with 0 errors.
- Fixed all 29 compile errors in ODExplorer.Core (Core had been failing to compile since the stub pass):
  - Stores/SettingsStore.cs: `ActiveViewModel.Cartographic` -> `ActiveViewModel.Carto` (enum member is `Carto`).
  - Stubs/StoreStubs.cs (SpanshCsvStore): `ParseCSV`/`ForceParseCSV` now return `bool` (match the real Store at Stores/SpanshCsvStore.cs; ViewModel did `if (csv)`).
  - Stubs/StoreStubs.cs (ExplorationDataStore): `OrganicScanItems` is now `List<SystemBody>` (was `ObservableCollection<OrganicScanItemViewModel>`), matching the real store; fixes `body.OrganicScanItems` in OrganicViewModel.
  - Stubs/StoreStubs.cs (OrganicCheckListDataStore): `OrganicScanItems` is now `Dictionary<string, List<OrganicChecklistItem>>` (matches real store); removed the mis-typed `OrganicScanItemsCollection` shim.
  - ViewModels/ModelVMs/StarSystemViewModel.cs: `mats |= material.Name` -> `mats |= material.Name_AsMaterial` (PlanetMaterial |= string invalid; bridge type exposes Name_AsMaterial).
  - Stubs/ODUtilsMissingStubs.cs (JournalCommander): added 5-arg ctor `(int id, string name, string? journalPath, string? lastFile, bool isHidden)` (matches real usage; also kept parameterless ctor).
  - Stubs/ODUtilsMissingStubs.cs (OdExplorerDatabaseProvider): added `GetIgnoredSystems(int cmdrId)` -> `List<IgnoredSystem>` and `DeleteCommander(int commanderID)` -> `Task`.
- Verification:
  - `dotnet build ODExplorer.sln -c Release` => Build succeeded, 0 errors (36 warnings remain, mostly CS0067 unused events + nullable warnings in stubs/ViewModels).
  - `dotnet run --project src/TestApp/TestApp.csproj -c Release` => "Settings loaded successfully (no-op provider)."
- Remaining warnings to clean up (non-blocking): CS0105 duplicate `using ODExplorer.Models` in several ViewModels; CS8622/CS8604 nullability; CS0067 unused stub events.

Latest handoff update (2026-08-03, gitignore + interactive MessageBox dialog)
- Added `.gitignore` (root) covering bin/obj, build dirs, IDE files, *.bundle, OS junk. This repo previously tracked ~315 bin/obj build artifacts (no .gitignore existed) causing binary-churn commits like the earlier `merge` commit.
- Ran `git rm -r --cached` on all tracked `bin/`/`obj/` files (315 files staged for deletion, NOT committed). bin/obj are now ignored; re-add + commit when ready.
- Implemented interactive Avalonia MessageBox dialog replacing the temporary toast handler:
  - src/ODExplorer.UI.Avalonia/Views/MessageBoxWindow.axaml + .axaml.cs — window with message TextBlock and a code-built button row. Maps core `MessageBoxButton` (OK / OKCancel / YesNo / YesNoCancel) to buttons; Yes/OK invoke `CallbackYes`, No invokes `CallbackNo`, Cancel closes without callback. Public parameterless ctor added so the XAML resource is runtime-loadable (fixes AVLN3001).
  - src/ODExplorer.UI.Avalonia/Services/MessageBoxService.cs — static `Show(Window? owner, args)` that posts to `Dispatcher.UIThread` and shows the dialog (modal via owner, or modeless if no owner).
  - src/ODExplorer.UI.Avalonia/App.axaml.cs — `MessageBoxRequester.Requested` handler now shows the dialog owned by `desktop.MainWindow` instead of the toast; removed now-unused `notifier`/`paths` locals.
- Build: `dotnet build ODExplorer.sln -c Release` => Build succeeded, 0 errors, no AVLN warnings.
- NOT runtime-verified: this environment is headless (no X11/Xvfb), app fails at XOpenDisplay. Dialog must be smoke-tested on a real Linux desktop. Next UI steps: Port MainWindow/Views so the app actually navigates to ViewModels (the dialog only fires once ViewModels are reachable); SettingsViewModel routes via `NavigationViewModel.InvokeMessageBox` -> `MainViewModel.OnMessageBoxRequested` (currently unsubscribed in Core) so that path still needs UI wiring.

Latest handoff update (2026-08-03, CI fix)
- Root cause of CI failure: `src/TestApp` is NOT part of `ODExplorer.sln` (sln = Core + UI.Avalonia only). CI ran `dotnet build ODExplorer.sln` (never built TestApp) then `dotnet run --project src/TestApp/TestApp.csproj --no-build` (no output to run). It had only ever passed because committed bin/obj artifacts included a prebuilt TestApp binary; once the gitignore cleanup removed those, the latent bug surfaced ("No such file or directory .../bin/Release/net8.0/TestApp").
- Also found: `dotnet restore ODExplorer.sln` does NOT restore TestApp either (NETSDK1004 on `--no-restore` build).
- Fix (`.github/workflows/ci.yml`): added `Restore TestApp` (`dotnet restore src/TestApp/TestApp.csproj`) and real `Build TestApp` (`dotnet build ... --no-restore`) steps before the existing `Run TestApp --no-build` step.
- Verified locally from clean TestApp bin/obj: restore -> build sln -> build TestApp -> run --no-build all succeed; TestApp prints "Settings loaded successfully (no-op provider)."
- CAVEAT: CI pins `dotnet-version: '8.0.x'`. Locally, the .NET 8 SDK 8.0.423 could NOT compile the UI project (Avalonia 12.1.0 analyzers need Roslyn >= 4.14; CS9057/CS0103 InitializeComponent). The user's CI Build step passed, implying the runner's 8.0.x patch resolves a newer Roslyn. If a future CI run fails at the Build step with CS0103/CS9057 in ODExplorer.UI.Avalonia, bump CI to `dotnet-version: '10.0.x'` (verified working locally).

Latest handoff update (2026-08-03, app shell + nav bar + Settings view port)
- Smoke-tested on a real Linux desktop (Wayland): window opens, LoadingView renders with GitHub/PayPal buttons that open the browser. Shell confirmed working.
- Added `Views/ViewLocator.cs` (convention `IDataTemplate`: `XViewModel` -> `Views/XView`; fallback shows "view not ported yet" placeholder). Registered in `App.axaml` Application.DataTemplates.
- `MainWindow.axaml`/`.cs`: DockPanel shell with top nav bar + `ContentControl` bound to `MainViewModel.CurrentViewModel`. Nav buttons bind `NavigateToView` with `CommandParameter={x:Static models:ActiveViewModel.X}` (Carto/ExoBiology/Settings/DisplaySettings/Spansh/EdAstro).
- `App.axaml.cs` composition root: builds full store graph from stubs, wires all 8 `OdNavigationService<T>` factories (lazy, circular refs handled with `NavigationViewModel?`/`mainViewModel!`), creates MainViewModel, navigates to LoadingView at startup.
- `Views/SettingsView.axaml`/`.cs` (commander-focused; NOT the full 6-control WPF port): commander ListBox (Name/JournalPath/LastFile/Hidden checkbox), details + Change Logs Folder/Reset Last File/Save/Delete/Scan Directory buttons, Minimise-to-tray, links (GitHub/PayPal/EDSM/Spansh/EdAstro), DB reset, scanning overlay (indeterminate ProgressBar instead of ODUtils LoadingSpinner). Folder pickers use `TopLevel.StorageProvider.OpenFolderPickerAsync` -> `vm.OnSetNewDir/OnScanNewDirectory`.
- Stub tweaks for testability: `JournalParserStore.IsLive => true` (unlocks shell `UiEnabled` gate so nav works before real store wiring) and `OdExplorerDatabaseProvider.GetAllJournalCommanders` returns one "Test Commander" (Id 1) so the Settings list is populated. SelectedCommanderID defaults to 0, so the row must be clicked to enable commander commands.
- `SettingsViewModel.OnSetNewDir` changed `internal` -> `public` (WPF had it internal because view was same-assembly; Avalonia view is a separate assembly).
- Build: `dotnet build ODExplorer.sln -c Release` => Build succeeded, 0 errors (36 pre-existing stub warnings).
- Avalonia 12.1.0 API gotchas recorded (IMPORTANT for all future view ports):
  - `Visibility` enum is GONE from Avalonia 12 (was `Avalonia.Controls.Visibility` in 11). `Visual` now has `IsVisible` (bool). Use `IsVisible="{Binding ...}"` + a converter mapping `ODExplorer.Models.Visibility` -> bool (see `Converters/VisibilityConverter.cs`).
  - `TopLevel.StorageProvider` + `FolderPickerOpenOptions` + `StorageProviderExtensions.TryGetLocalPath(IStorageItem)` confirmed present in 12.1.0.
  - Compiled bindings require `x:DataType` on the root element AND on each `DataTemplate` (AVLN2100 otherwise).
  - Namespace-resolution gotcha: inside `namespace ODExplorer.UI.Avalonia.Views`, writing `Avalonia.Layout.HorizontalAlignment` fails (resolves `ODExplorer.UI.Avalonia.Layout`). Add `using Avalonia.Layout;` and use the short name.
- NOT yet smoke-tested: SettingsView + nav bar (needs desktop run; headless env can't launch UI). Expected: nav bar buttons enable, Settings shows the Test Commander row, clicking it enables Save/Delete/etc., folder pickers open a GTK dialog.
- Next steps: port remaining views (CartographicView, OrganicView, SpanshView, EdAstroView, DisplaySettingsView, CartoDetailsView); wire real JournalParserStore/DB stores; then polish nav bar visuals + tray + scaling per original MainWindow.xaml.

Latest handoff update (2026-08-03, smoke-test round 2 fixes)
- User smoke-test findings (real desktop) + fixes:
  1. "Black on black, hard to read" -> App.axaml now `RequestedThemeVariant="Dark"` (matches original dark-styled app; system was light so FluentTheme rendered light text on our hardcoded dark surfaces). Also set explicit `Foreground="White"` on the SettingsView scanning-overlay TextBlocks.
  2. "Clicking Exobiology crashes" -> KeyNotFoundException `'$Codex_Ent_Aleoids_Genus_Name;'` at OrganicViewModel.cs:135. Stub `OrganicCheckListDataStore.OrganicScanItems` dictionary was empty but the VM indexes it by genus codex. Fixed: stub ctor now pre-populates all 16 genus keys (Aleoids/Bacterial/Cactoid/Clypeus/Conchas/Electricae/Fonticulus/Fumerolas/Fungoids/Osseus/Recepta/Shrubs/Stratum/Tubus/Tussocks + "Other") with empty `List<OrganicChecklistItem>` (matching real store behavior). `CheckInRegion` already returns [] for empty lists.
  3. "Settings stuck on permanent loading bar" -> App.axaml.cs settingsService factory used `new SettingsViewModel(...)` but the original app uses `SettingsViewModel.CreateViewModel(...)` which kicks off `_ = vm.Initialise()` -> `LoadCommanders()` -> sets `ScanningWindowVisibility=Collapsed` + `IsLoaded=true`. Switched factory to `CreateViewModel`.
- Pre-checked remaining view ctors for stub-safety with empty stub stores (all safe, no further changes): CartographicViewModel (CurrentSystem null, guarded), SpanshViewModel (GetCurrentContainer null, guarded), EdAstroViewModel (EdAstroPois empty, early return), DisplaySettingsViewModel (commands + NotificationSettings.Clone), CartoDetailsViewModel (Build*Systems iterate empty stores; BuildIgnoreSystems uses stub GetIgnoredSystems -> empty).
- Build: `dotnet build ODExplorer.sln -c Release` => Build succeeded, 0 errors (36 pre-existing stub warnings).
- For user: commit+push, then on desktop run `dotnet run --project src/ODExplorer.UI.Avalonia -c Release` and re-test: dark theme readable everywhere; Settings overlay disappears and commander row is selectable; Exobiology nav shows the "view not ported yet" placeholder instead of crashing.

Latest handoff update (2026-08-03, milestone: functional in-memory stores + tray)
- Made the stub stores FUNCTIONAL (in-memory) so the Settings flows actually do something:
  - `Stubs/StoreStubs.cs` `JournalParserStore`: now takes `(IOdToolsDatabaseProvider? databaseProvider, SettingsStore? settingsStore)` ctor. `ReadNewDirectory(path)` scans the folder's `*.log` journal files, parses the `LoadGame` event (Newtonsoft JObject) for the commander name, registers/updates the commander in the provider, sets `SelectedCommanderID`, refreshes the list, and raises `OnJournalStoreStatusChange`. `UpdateCommanders()` reloads commanders from the provider + raises `OnCommandersUpdated`. `ReadNewCommander(id)` selects the commander and fires "Ready — Selected Commander X" / "No Commanders Found". `ResetDataBase(provider)` clears commander data.
  - `Stubs/ODUtilsMissingStubs.cs` `OdExplorerDatabaseProvider`: now holds an in-memory `List<JournalCommander>` with real `AddCommander` (upsert), `GetAllJournalCommanders`, `GetCommander`, `DeleteCommander`, `ClearCommanders`, plus `DeleteCommanderAsync`/`ResetDataBaseAsync`. (Removed the hardcoded "Test Commander" sample row.)
- Tray icon (minimise-to-tray): confirmed Avalonia 12.1.0 `TrayIcon` API via reflection: `TrayIcon.IconsProperty` attached prop on `Application` (`TrayIcon.GetIcons/SetIcons`), instance props Icon/WindowIcon, Menu/NativeMenu, ToolTipText, IsVisible; `NativeMenuItem.Click` is `EventHandler`; `WindowIcon(string)` ctor exists. Docs: tray supported on Windows/macOS/Ubuntu (some Linux DEs need SNI/AppIndicator support).
  - Generated PNG icons (32/64) via a dependency-free Python PNG encoder into `src/ODExplorer.UI.Avalonia/Assets/od-explorer-icon-{32,64}.png` (orange-ringed target style, no external tooling).
  - `ODExplorer.UI.Avalonia.csproj`: added `<ItemGroup><AvaloniaResource Include="Assets\**" /></ItemGroup>`.
  - `App.axaml`: added `TrayIcon.Icons` attached property with `NativeMenu` (Show OD Explorer / Quit) using `Icon="/Assets/od-explorer-icon-64.png"`.
  - `App.axaml.cs`: `OnTrayShow` (Show + Normal + Activate), `OnTrayQuit` (calls `MainWindow.RequestQuit()` else `desktop.Shutdown()`).
  - `MainWindow.axaml.cs`: subscribes `Window.Closing`; when `MainViewModel.SettingsStore.MinimiseToTray` is true it sets `e.Cancel = true` + `Hide()` (close-to-tray). Added `RequestQuit()` (sets flag + Close) so the tray Quit bypasses the hide. `WindowClosingEventArgs` derives from `CancelEventArgs` (Cancel is settable).
  - `App.axaml.cs` composition: `new JournalParserStore(databaseProvider, settingsStore)` so the scanner has the provider/settings to work with.
- Build: `dotnet build ODExplorer.sln -c Release` => Build succeeded, 0 errors, 0 warnings (Avalonia analyzers clean).
- Expected behavior for the user's next smoke test:
  - Scan Directory now REGISTERS a commander (folder picker -> LoadingView -> "Registered Commander X" -> Settings list updates). Selecting it enables Save/Delete. (Scanning real data still inert — Exo/Carto stores are stubs.)
  - Delete Commander removes the selected commander from the list.
  - Reset Database clears commanders and bounces to LoadingView.
  - Ticking Minimise-to-tray + closing the window hides it to the tray; tray "Show" restores, tray "Quit" exits. NOTE: on Linux the tray icon needs an SNI-capable tray (KDE fine; GNOME requires the AppIndicator extension). If the tray doesn't show, close-to-tray still hides the window (recover via tray or kill process).
- Next steps (unchanged scope): port remaining views (CartographicView, OrganicView, SpanshView, EdAstroView, DisplaySettingsView, CartoDetailsView); wire real JournalParserStore/DB stores (needs EliteJournalReader/ODUtils reconstruction, not on NuGet); then polish nav visuals + scaling per original MainWindow.xaml.

Latest handoff update (2026-08-03, window sizing fix)
- User smoke test: journal locate works, minimise-to-tray works. Issue: "when log is pulled up, the window needs to be manually resized to show all menus" (Settings view content clipped below the window's bottom edge).
- Root cause: the main `ContentControl` did not stretch its content, so the Settings `ScrollViewer` was never height-constrained and never scrolled; also the Settings content (~735px tall) exceeded the 700px window.
- Fixes:
  - `MainWindow.axaml`: `ContentControl` now has `HorizontalContentAlignment="Stretch" VerticalContentAlignment="Stretch"` (view fills the content area so inner ScrollViewers work); default window size bumped 1100x700 -> 1200x800.
  - `Views/SettingsView.axaml`: compacted commander section so it fits at default size without clipping — ListBox `MaxHeight` 260->170, detail button grid rows 50->42, buttons/TextBoxes 40/34->34 high.
- Build: `dotnet build ODExplorer.sln -c Release` => Build succeeded, 0 errors, 0 warnings.
