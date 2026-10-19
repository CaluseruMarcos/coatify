# Hinweise für Coding-Agents

Coatify ist ein C#/.NET-Lernprojekt. Der Projektinhaber kommt von TypeScript und baut Teile der Anwendung und Tests bewusst selbst. Erkläre neue C#-, xUnit- und EF-Core-Konzepte in einfachen, konkreten Schritten; TypeScript-Vergleiche sind hilfreich. Wenn nach Hinweisen gefragt wird, lass die Übung beim Projektinhaber. Wenn eine Umsetzung beauftragt wird, führe sie aus und prüfe sie.

## Projektkontext

- `coatify/` enthält die ASP.NET-Core-Anwendung. `Program.cs` startet `Api.run`; dort werden `CoatifyContext` und `DeviceService` registriert.
- `DeviceService` greift derzeit direkt über EF Core auf `CoatifyContext` zu. `DeviceRepository` ist noch ein Grundgerüst. Behaupte in Tests oder Dokumentation nicht, dass die Geräteoperationen bereits über das Repository laufen.
- `coatify/Migrations/` enthält die EF-Core-Migrationen. Bei Änderungen am Datenmodell prüfe, ob eine neue Migration erforderlich ist. Nutze für migrationsbasierte Datenbanken `MigrateAsync()`; kombiniere es nicht mit `EnsureCreatedAsync()`.
- `Coatify.Web/` ist ein separates Blazor-WebAssembly-Frontend. Das Web-Client-Projekt darf nicht direkt auf das Backend-Projekt `coatify/` verweisen; für gemeinsame DTOs/Response-Modelle nutzt man ein eigenes Shared/Contracts-Projekt oder klar getrennte API-Modelle.
- `Coatify.Tests/` enthält xUnit-Tests. `README.md` und `Coatify.Tests/README.md` beschreiben Start und Testumgebung.

## Datenbanken und Tests

- `compose.yaml` stellt lokales PostgreSQL auf `127.0.0.1:1234` bereit. `coatify` ist die Entwicklungsdatenbank; die Tests verwenden `coatify_test`. Halte diese Datenbanken getrennt.
- `Coatify.Tests/run-postgres-tests.sh` startet PostgreSQL und ruft `dotnet test` auf. Die Tests verändern `coatify_test` dauerhaft; sie setzen die Datenbank nicht zurück.
- Gib jedem datenbankbasierten Test einen eigenen `CoatifyContext`. Lege benötigte Datensätze im Test selbst an oder richte sie gezielt ein, damit der Test auf einer frischen Testdatenbank funktioniert. Verwende eindeutige Schlüssel und Seriennummern oder wiederholbar vorbereitete feste IDs.
- Einige bestehende Tests wählen das erste Gerät der gemeinsamen Testdatenbank aus und ändern oder löschen es. Berücksichtige das bei Änderungen und behandle einen grünen, aber leeren Test nicht als Verhaltensnachweis.

## Prüfen

- Für C#-Änderungen: `dotnet build Coatify.Tests/Coatify.Tests.csproj`.
- Für betroffene Datenbanktests: `bash Coatify.Tests/run-postgres-tests.sh --filter FullyQualifiedName~TestName` oder den passenden `dotnet test`-Filter. Prüfe bei Datenbanktests auch einen zweiten Lauf, wenn feste Testdaten erneut verwendet werden.
- Berichte knapp, was geändert wurde, was tatsächlich geprüft wurde und welche Tests wegen fehlender Datenbank oder Umgebung nicht laufen konnten.
