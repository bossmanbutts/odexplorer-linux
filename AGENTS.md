# ODExplorer Linux

## Project Mission

ODExplorer Linux is a native Qt6/C++ reimplementation of the original Windows OD Explorer for Elite Dangerous.

The objective of this project is not to redesign, modernize, or reinterpret the original application.

The objective is to faithfully reproduce its behavior while using native Qt/C++ architecture.

When complete, the Linux version should behave indistinguishably from the original application from the user's perspective.

---

# Project Philosophy

Behavioral compatibility is the highest priority.

The original OD Explorer has already solved the application's behavior and workflow.

Whenever possible, existing behavior should be ported rather than reimplemented.

Do not invent new behavior where an original implementation already exists.

Do not preserve incorrect ports simply because they currently exist.

---

# Repository Authority

Repositories are ordered by authority.

## 1. BossmanButts/odexplorer-linux

The implementation to modify.

Contains the Linux/Qt codebase.

## 2. WarmedxMints/OD-Explorer

Canonical source for:

- application behavior
- workflow
- UI interactions
- journal event handling
- exploration state
- feature implementation

## 3. EliteDangerousCore

Canonical source for:

- Elite Dangerous game mechanics
- exploration value calculations
- scan rewards
- mapping rewards
- first discovery logic
- first mapped logic
- efficiency bonuses
- stellar body values
- exploration mathematics

When repositories disagree:

Application behavior follows OD-Explorer.

Game calculations follow EliteDangerousCore.

The Linux implementation should adapt these behaviors to Qt without changing them.

---

# Development Strategy

The project should not be developed by adding new behavior.

Instead, each subsystem should be audited against the original implementation and restored until behavioral parity is achieved.

Every subsystem follows the same workflow:

1. Locate the Linux implementation.
2. Locate the original OD-Explorer implementation.
3. Trace authoritative calculations into EliteDangerousCore where applicable.
4. Compare behavior.
5. Identify divergences.
6. Replace incorrect implementations with faithful ports.
7. Verify behavior before continuing.

---

# Current Priority

The current focus is the exploration value subsystem.

Previous attempts manually ported portions of the calculation logic.

Those implementations should not be assumed correct.

Treat the current implementation as provisional.

Audit the entire value calculation pipeline and replace incorrect logic with faithful ports from the reference repositories.

Do not patch individual formulas unless they are confirmed to match the original implementation.

The goal is not "correct-looking" values.

The goal is identical behavior.

---

# General Rules

Never rewrite algorithms from memory.

Never simplify calculations.

Never modernize behavior.

Never perform unrelated cleanup.

Never rename project concepts without explicit instruction.

Keep changes localized.

Keep commits reviewable.

One logical change per commit whenever practical.

If uncertain, inspect the reference repositories before making changes.

Reading source code is always preferred over reasoning from memory.

---

# Definition of Success

The project is complete when each subsystem has been verified against the original OD Explorer and behaves the same under equivalent journal events and gameplay scenarios.

Behavioral parity—not implementation similarity—is the definition of success.