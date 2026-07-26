# Coding Standards

Language
- C++20

Formatting
- clang-format

Naming
- Classes: PascalCase
- Functions: camelCase
- Variables: snake_case
- Constants: kCamelCase

General
- Prefer RAII.
- Avoid raw owning pointers.
- Use std::unique_ptr by default.
- Keep UI and business logic separate.
- No global variables.
- No Windows-specific APIs.
- Every public class should have a clear responsibility.