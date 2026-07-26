# OD Explorer Linux

## Project Goal

Create a native Linux implementation of OD Explorer with feature parity to the Windows application.

Do not translate C# source code line-by-line.
Implement equivalent functionality using modern C++20 and Qt6.

## Tech Stack

- C++20
- Qt6 Widgets
- CMake
- SQLite
- Catch2

## Architecture

- UI must not contain business logic.
- UI must never parse JSON.
- Prefer composition over inheritance.
- Keep classes focused on one responsibility.
- Use RAII.
- Prefer std::unique_ptr over raw owning pointers.
- Avoid global variables.

## Style

- Classes: PascalCase
- Methods: camelCase
- Variables: snake_case

## Code Quality

- Write clear code instead of clever code.
- Prefer standard library algorithms.
- Minimize heap allocations.
- Every class should be easy to unit test.

## Important

Match the behavior of the Windows application wherever practical.