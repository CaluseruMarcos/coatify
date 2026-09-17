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

Aktuell sind die Bereiche als Ordner in einem gemeinsamen ASP.NET-Core-Projekt organisiert:

```text
coatify.sln
coatify/
├── Api/              # HTTP-Endpunkte und Startkonfiguration
├── Application/      # Services, DTOs und Interfaces
├── Domain/           # Geräte, Gerätetypen, Messwerte und Benutzer
├── Infrastructure/   # Grundlage für den späteren Datenzugriff
├── Program.cs        # Einstiegspunkt
└── USER_STORY.md     # Lernaufgaben und Abnahmekriterien
```

Im Lernplan ist vorgesehen, diese Bereiche in separate Projekte aufzuteilen. Die Domain soll unabhängig von ASP.NET Core und EF Core bleiben. Die Application-Schicht soll den Datenzugriff über Interfaces ansprechen, deren Implementierungen in Infrastructure liegen.

## Aktueller Stand

Das Projekt befindet sich im Aufbau und verwendet .NET 10 mit ASP.NET Core. Erste Geräte-Endpunkte, Datenmodelle, DTOs sowie Service- und Repository-Grundgerüste sind vorhanden.

Die Geräteoperationen arbeiten derzeit mit einfachen Antwortobjekten. Es gibt noch keine dauerhafte Speicherung und keine echte Suche nach gespeicherten Geräten. Das Repository ist bisher ein Grundgerüst. Auch die vollständige Validierung, zentrale Fehlerbehandlung, strukturiertes Logging und Unit Tests gehören zu den weiteren Lernschritten.

EF Core und PostgreSQL sind geplant. Blazor für eine Benutzeroberfläche und Azure sollen später folgen.

## Lokal starten

Voraussetzung ist das **.NET-10-SDK**. Im Ordner mit `coatify.sln` ausführen:

```bash
dotnet run --project coatify/coatify.csproj --launch-profile http
```

Die API ist anschließend unter `http://localhost:5105` erreichbar. Ein erster Aufruf ist:

```text
http://localhost:5105/api/devices
```

Aktuell wird keine Datenbank zum Starten benötigt. Es gibt noch keine Weboberfläche; die Endpunkte lassen sich über den Browser für GET-Anfragen oder über curl, Postman und Riders HTTP-Client testen.

## Lernplan

Die einzelnen Aufgaben, User Stories und Kriterien für den Abschluss stehen in [USER_STORY.md](coatify/USER_STORY.md). Sie beschreiben den angestrebten Ausbau und sind nicht gleichbedeutend mit bereits fertig implementierten Funktionen.

Das Projekt soll helfen, die einzelnen Bausteine eines Backends zu verstehen und selbst umzusetzen. Mit jedem Lernschritt wächst aus dem einfachen API-Grundgerüst eine Anwendung mit nachvollziehbarer Struktur, geprüften Eingaben und dauerhaft gespeicherten Daten.
