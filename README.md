# Coatify

Coatify ist ein Lernprojekt, mit dem Schritt für Schritt ein kleines IoT-Backend für industrielle Geräte entsteht. Ziel ist es, C# und .NET praktisch zu lernen und zu verstehen, wie eine HTTP-Anfrage durch die verschiedenen Teile einer Anwendung bis zur Datenbank und zurück läuft.

## Was soll die Anwendung können?

Coatify soll Geräte verwalten und deren Messwerte bereitstellen. Ein Gerät besitzt zum Beispiel einen Namen, eine Seriennummer, einen Gerätetyp und einen Status. Dazu gehören Messwerte wie Temperatur, Druck und Stromverbrauch mit einem Zeitstempel.

Das angestrebte Ergebnis: Ein Administrator kann ein Gerät über die API registrieren, später wieder abrufen, bearbeiten oder löschen und seine Messwerte einsehen. Die Daten sollen dauerhaft in PostgreSQL gespeichert werden und nach einem Neustart erhalten bleiben.

## Was wird dabei gelernt?

- **C# und .NET:** Klassen, Interfaces, GUIDs, Collections und asynchrone Methoden mit `Task` und `async`/`await`.
- **ASP.NET Core:** HTTP-Endpunkte erstellen, Parameter entgegennehmen und passende Statuscodes zurückgeben.
- **Klare Verantwortlichkeiten:** HTTP-Verarbeitung, Geschäftslogik, Datenmodelle und Datenzugriff voneinander trennen.
- **Dependency Injection:** Abhängigkeiten über Konstruktoren bereitstellen und vom DI-System auflösen lassen.
- **DTOs und Validierung:** Ein- und Ausgaben definieren und ungültige Daten verständlich ablehnen.
- **Datenhaltung:** Repository-Schnittstellen, Entity Framework Core, PostgreSQL, Beziehungen, Indizes und Migrationen einsetzen.
- **Qualität und Nachvollziehbarkeit:** Logging, zentrale Fehlerbehandlung und Unit Tests für Erfolgs- und Fehlerfälle ergänzen.

## Aufbau

Aktuell sind die Anwendungsbereiche als Ordner in einem ASP.NET-Core-Projekt organisiert. Daneben gibt es ein eigenes xUnit-Testprojekt:

```text
coatify.sln
compose.yaml           # Lokaler PostgreSQL-Container
coatify/
├── Api/              # HTTP-Endpunkte und Startkonfiguration
├── Application/      # Services, DTOs und Interfaces
├── Domain/           # Geräte, Gerätetypen, Messwerte und Benutzer
├── Infrastructure/   # EF-Core-Context und Datenzugriff
├── Migrations/       # Versionierte Datenbankschema-Änderungen
├── Program.cs        # Einstiegspunkt
└── USER_STORY.md     # Lernaufgaben und Abnahmekriterien
Coatify.Tests/         # xUnit-Tests und Testdatenbank-Helfer
AGENTS.md              # Projekthinweise für Coding-Agents
```

Im Lernplan ist vorgesehen, diese Bereiche in separate Projekte aufzuteilen. Die Domain soll unabhängig von ASP.NET Core und EF Core bleiben. Die Application-Schicht soll den Datenzugriff über Interfaces ansprechen, deren Implementierungen in Infrastructure liegen.

## Aktueller Stand

Das Projekt befindet sich im Aufbau und verwendet .NET 10, ASP.NET Core, EF Core und PostgreSQL. Geräte-Endpunkte für Anlegen, Lesen, Aktualisieren und Löschen sind vorhanden. `DeviceService` greift dafür auf `CoatifyContext` zu; das Repository ist noch ein Grundgerüst.

Die erste EF-Core-Migration beschreibt das Datenbankschema. Das Testprojekt enthält unter anderem einen Datenbanktest für `CreateDevice` und zwei Testfälle für `DeviceService.GetDevice`. Weitere Tests und Teile der Fehlerbehandlung sind noch im Aufbau.

Blazor für eine Benutzeroberfläche und Azure sollen später folgen.

## Lokal starten

Voraussetzungen sind das **.NET-10-SDK** und **Docker Compose**. Im Ordner mit `coatify.sln` zunächst PostgreSQL starten und die Verbindungszeichenfolge setzen:

```bash
docker compose up -d --wait postgres
export ConnectionStrings__Coatify='Host=127.0.0.1;Port=1234;Database=coatify;Username=coatify;Password=coatify'
```

Auf einer neuen Datenbank das Schema mit den EF-Core-Migrationen erstellen:

```bash
cd coatify
dotnet tool restore
dotnet ef database update
cd ..
```

Danach die API starten:

```bash
dotnet run --project coatify/coatify.csproj --launch-profile http
```

Die API ist anschließend unter `http://localhost:5105` erreichbar. `GET /api/device/{id}` liest ein Gerät; `GET /api/devices` erwartet mindestens eine `ids`-Query-Parameter-ID. Ohne IDs liefert der Listen-Endpunkt `400 Bad Request`.

Die Verbindungsdaten oben stammen aus `compose.yaml` und sind nur für die lokale Entwicklung gedacht. Die API erstellt das Schema beim Start nicht selbst; auf einer neuen Datenbank ist der Migrationsschritt erforderlich.

Es gibt noch keine Weboberfläche. Die Endpunkte lassen sich mit curl, Postman oder Riders HTTP-Client aufrufen.

## Tests ausführen

```bash
bash Coatify.Tests/run-postgres-tests.sh
```

Das Skript startet PostgreSQL bei Bedarf und führt `dotnet test` aus. Die Tests verwenden die separate Datenbank `coatify_test` und verändern deren Daten dauerhaft. Hinweise zum Testaufbau stehen in [Coatify.Tests/README.md](Coatify.Tests/README.md).

## Lernplan

Die einzelnen Aufgaben, User Stories und Kriterien für den Abschluss stehen in [USER_STORY.md](coatify/USER_STORY.md). Sie beschreiben den angestrebten Ausbau und sind nicht gleichbedeutend mit bereits fertig implementierten Funktionen.

Das Projekt soll helfen, die einzelnen Bausteine eines Backends zu verstehen und selbst umzusetzen. Mit jedem Lernschritt wächst aus dem einfachen API-Grundgerüst eine Anwendung mit nachvollziehbarer Struktur, geprüften Eingaben und dauerhaft gespeicherten Daten.
