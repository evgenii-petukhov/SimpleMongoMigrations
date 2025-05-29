# SimpleMongoMigrations – Release Notes

---

### v1.0.16

- Updated README.md: add link to real-life usage example.

---

### v1.0.15

- Refactored `MigrationRunner` for improved testability.
- Added unit tests.

---

### v1.0.14

- Changed class access modifiers to `internal` to hide implementation details from NuGet consumers.

---

### v1.0.13

- Added asynchronous support to TransactionSupportChecker.

---

### v1.0.12

- Added support for passing an existing `MongoClient` to `MigrationEngineBuilder`.

---

### v1.0.11

- Refactored migration execution logic to use asynchronous transaction handling.

---

### v1.0.10

- Improved transaction support.

---

### v1.0.9

- Introduced a fluent API via `MigrationEngineBuilder` for flexible `MigrationEngine` configuration.
- Added transaction support with MongoDB sessions for transaction management.

---

### v1.0.8

- Updated test projects to target .NET 9.

---

### v1.0.7

- Added missing dependencies to the .NET Framework demo project.

---

### v1.0.6

- Restructured the solution and renamed projects for improved clarity.
- Refactored the `Version` constructor.
- Added unit tests.
- Configured CI to run unit tests on pushes to the master branch.

---

### v1.0.5

- Automated publishing to NuGet.

---

### v1.0.4

- Updated README.md.

---

### v1.0.3

- Updated README.md.

---

### v1.0.2

- Moved serializer registration to a static constructor.
- Updated icon, refined README, and adjusted test project target frameworks to resolve compilation warnings.

---

### v1.0.1

- Added package description, icon, and README.
- Updated `MongoDB.Driver` to version 3.4.0.

---

### v1.0.0

- Initial implementation for .NET 6 and .NET Framework 4.7.2.
