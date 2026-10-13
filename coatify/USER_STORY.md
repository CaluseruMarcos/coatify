# Coatify – .NET Learning Tasks | Roadmap


## Ziel

Baue ein kleines IoT-Backend mit:

* C#
* .NET
* ASP.NET Core
* Entity Framework Core
* PostgreSQL

Später folgen Blazor und Azure.

## Ist-Stand (Codeprüfung vom 13.10.2026)

Die folgenden Angaben beschreiben implementiertes Verhalten, nicht nur vorhandene Dateien oder Dokumentation. „Teilweise“ bedeutet, dass die zugehörigen Akzeptanzkriterien noch nicht vollständig erfüllt sind.

| Bereich | Stand | Konkrete Lücke |
| --- | --- | --- |
| .NET/Solution und Clean Architecture | Teilweise | Eine baubare Solution und Ordner für die Schichten sind da; getrennte Projekte und gerichtete Project References fehlen. |
| Domain, DTOs, Service und DI | Teilweise | Vier Modelle, Geräte-DTOs, Service und Registrierungen sind da; `User` ist nur eine ID, und der Service benutzt weiterhin `CoatifyContext` direkt. |
| Repository | Teilweise | Interface und Klasse existieren; mehrere Methoden werfen `NotImplementedException`, und die Geräteoperationen laufen nicht über das Repository. |
| Minimal APIs für Geräte und Messwerte | Teilweise | Geräteoperationen sind gemappt; Pfade, Listenverhalten und DELETE-Status weichen von Aufgabe 5 ab. Ein Messwerte-Endpunkt fehlt. |
| Validierung, Logging und Fehlerbehandlung | Teilweise | Pflichtfelder und Gerätetyp werden teilweise geprüft; Duplikate verlassen sich auf den DB-Index. `ILogger` und ein zentraler Handler sind da, aber `GetDevice` dereferenziert beim Fehlen `null` und führt so zu 500 statt 404. |
| PostgreSQL/EF Core | Teilweise | Context, lokale Compose-Datenbank, Beziehungen, Indizes und `InitialCreate` sind da; die zweite Beispielmigration (`Location`) und ein geprüfter Neuaufbau aus Migrationen fehlen. |
| xUnit und Testdatenbank | Teilweise | xUnit und `coatify_test` sind da; mehrere Tests greifen direkt auf EF Core zu, nutzen gemeinsame Daten und prüfen Service/API-Fälle nicht ausreichend. |
| Blazor | Teilweise | `Coatify.Web` ist als .NET-10-Blazor-WebAssembly-Projekt angelegt; Home, Navigation und NotFound sind noch Template-Seiten. Das Projekt ist noch nicht Teil der Solution und ruft die Coatify-API nicht auf. |
| Entra ID und Rollen | Noch nicht umgesetzt | Keine Anmeldung oder Autorisierungsregeln im Code. |
| Docker/Azure-Deployment und Registry | Noch nicht umgesetzt | Nur PostgreSQL läuft lokal in Compose; kein API-Dockerfile und keine Azure-Bereitstellung. |
| Managed Identity, Key Vault, Networking, Azure PostgreSQL, Cosmos DB, IoT Hub, Worker, Application Insights, Grafana, Bicep, Azure DevOps, Artifacts | Noch nicht umgesetzt | Keine entsprechenden Ressourcen, Konfigurationen oder Pipelines im Repository. |
| Architektur- und Bereitstellungsdokumentation | Teilweise | README erklärt lokalen Start; Cloud-Architektur, Kosten, Bereinigung und reproduzierbarer Deploy fehlen. |

Die Reihenfolge ist: Aufgaben 1–16 für Backend und Tests schließen, danach Oberfläche und Authentifizierung, dann **eine** günstige Azure-Betriebsvariante. Weitere Cloud-Dienste sind zeitlich begrenzte Übungen.

---

# 1. Projektstruktur

## User Story

**Als Entwickler möchte ich mein Projekt sauber strukturieren, damit die Verantwortlichkeiten getrennt sind.**

### Aufgabe

Erstelle:

```text
Coatify.sln

src/
├── Coatify.Domain
├── Coatify.Application
├── Coatify.Infrastructure
└── Coatify.Api
```

Abhängigkeiten:

```text
Api → Application → Domain
Infrastructure → Application → Domain
```

### Fertig wenn

* [ ] Solution funktioniert
* [ ] Alle Projekte bauen
* [ ] Project References stimmen
* [ ] Domain kennt keine anderen Projekte
* [ ] Application kennt Infrastructure nicht

---

# 2. Domain Model

## User Story

**Als System möchte ich industrielle Geräte abbilden können.**

### Aufgabe

Erstelle:

```text
Device
DeviceType
Measurement
User
```

### Device

```text
Id
Name
SerialNumber
DeviceType
Status
CreatedAt
```

### DeviceType

```text
Id
Name
Description
```

### Measurement

```text
Id
DeviceId
Timestamp
Temperature
Pressure
PowerConsumption
```

### Fertig wenn

* [ ] Relationships sind logisch modelliert
* [ ] IDs verwenden `Guid`
* [ ] Domain enthält keine ASP.NET-Core-Abhängigkeiten
* [ ] Domain enthält keine EF-Core-Abhängigkeiten

---

# 3. Application Layer

## User Story

**Als Benutzer möchte ich Geräte verwalten können.**

### Aufgabe

Erstelle:

```text
CreateDeviceRequest
UpdateDeviceRequest
DeviceResponse

IDeviceService
DeviceService
```

Implementiere:

```text
CreateDevice()
GetDevice()
GetDevices()
UpdateDevice()
DeleteDevice()
```

### Fertig wenn

* [ ] Business Logic liegt im Application Layer
* [ ] DTOs werden verwendet
* [ ] Methoden sind `async`
* [ ] Application verwendet Interfaces
* [ ] Ungültige Geräte werden abgelehnt

---

# 4. Dependency Injection

## User Story

**Als Entwickler möchte ich Dependencies über Dependency Injection bereitstellen, damit Klassen ihre Abhängigkeiten nicht selbst erstellen müssen.**

## Ziel

Lerne und implementiere **Constructor Injection** mit dem .NET Dependency-Injection-System.

Die Anwendung soll folgende Abhängigkeiten automatisch auflösen können:

```text
IDeviceService → DeviceService
IDeviceRepository → DeviceRepository
```

## Aufgabe

### 1. Repository-Interface erstellen

Erstelle in `Coatify.Application`:

```text
Interfaces/
└── IDeviceRepository.cs
```

```csharp
public interface IDeviceRepository
{
}
```

Das Interface darf zunächst leer sein.

### 2. Repository implementieren

Erstelle in `Coatify.Infrastructure`:

```text
Repositories/
└── DeviceRepository.cs
```

```csharp
public class DeviceRepository : IDeviceRepository
{
}
```

`Coatify.Infrastructure` benötigt dafür eine Project Reference auf `Coatify.Application`.

### 3. Dependency in `DeviceService` injizieren

`DeviceService` soll das Repository **nicht selbst erstellen**.

❌ Nicht:

```csharp
public class DeviceService : IDeviceService
{
    private readonly DeviceRepository _repository = new DeviceRepository();
}
```

✅ Stattdessen:

```csharp
public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _repository;

    public DeviceService(IDeviceRepository repository)
    {
        _repository = repository;
    }
}
```

Damit bekommt `DeviceService` sein Repository von außen.

### 4. Dependencies in `Program.cs` registrieren

In `Coatify.Api/Program.cs`:

```csharp
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();
```

Damit weiß .NET:

```text
IDeviceService
      ↓
DeviceService

IDeviceRepository
      ↓
DeviceRepository
```

### 5. DI-Auflösung testen

Beim Start der Anwendung soll .NET `DeviceService` automatisch erstellen können.

Der Ablauf:

```text
DeviceService
      ↓
benötigt IDeviceRepository
      ↓
DI-System sucht Registrierung
      ↓
DeviceRepository
      ↓
DeviceService wird erstellt
```

## Projektstruktur

Nach diesem Schritt sollte die Struktur ungefähr so aussehen:

```text
Coatify
├── Coatify.Domain
│
├── Coatify.Application
│   ├── DTOs
│   ├── Interfaces
│   │   ├── IDeviceService.cs
│   │   └── IDeviceRepository.cs
│   └── Services
│       └── DeviceService.cs
│
├── Coatify.Infrastructure
│   └── Repositories
│       └── DeviceRepository.cs
│
└── Coatify.Api
    └── Program.cs
```

## Fertig wenn

* `IDeviceRepository` existiert
* `DeviceRepository` implementiert `IDeviceRepository`
* `DeviceService` erhält `IDeviceRepository` über den Constructor
* Kein `new DeviceRepository()` in `DeviceService`
* `IDeviceService → DeviceService` ist registriert
* `IDeviceRepository → DeviceRepository` ist registriert
* Anwendung startet ohne DI-Fehler
* `DeviceService` kann vom DI-System erstellt werden

> **Hinweis:** Der Controller kommt erst in Schritt 5. Dort wird `IDeviceService` ebenfalls per Constructor Injection in den Controller injiziert.


---

# 5. ASP.NET Core API

## User Story

**Als API-Nutzer möchte ich Geräte über HTTP verwalten.**

### Aufgabe

Implementiere:

```http
GET    /api/devices
GET    /api/devices/{id}
POST   /api/devices
PUT    /api/devices/{id}
DELETE /api/devices/{id}

GET    /api/devices/{id}/measurements
```

### HTTP Status Codes

```text
GET existing     → 200
GET not found    → 404

POST valid       → 201
POST invalid     → 400

PUT existing     → 200
PUT not found    → 404

DELETE existing  → 204
DELETE not found → 404
```

### Fertig wenn

* [ ] Alle Endpoints funktionieren
* [ ] Controller enthält möglichst wenig Business Logic
* [ ] DTOs werden verwendet
* [ ] HTTP Status Codes sind korrekt

---

# 6. Validation

## User Story

**Als API-Nutzer möchte ich verständliche Fehler bei ungültigen Eingaben erhalten.**

### Aufgabe

Validiere:

```text
Name
SerialNumber
DeviceType
```

Beispiele:

```text
Name darf nicht leer sein
SerialNumber darf nicht leer sein
SerialNumber darf nicht doppelt vorkommen
```

### Fertig wenn

* [ ] Ungültige Requests → 400
* [ ] Fehlermeldungen sind verständlich
* [ ] Validation ist sauber organisiert

---

# 7. Logging

## User Story

**Als Entwickler möchte ich wichtige Vorgänge nachvollziehen können.**

### Aufgabe

Verwende:

```csharp
ILogger<T>
```

Logge zum Beispiel:

```text
Device creation started
Device created
Device not found
Device updated
Device deleted
```

### Fertig wenn

* [ ] `ILogger<T>` wird verwendet
* [ ] Kein `Console.WriteLine()` für Application Logging
* [ ] Passende Log Levels werden verwendet

---

# 8. Error Handling

## User Story

**Als API-Nutzer möchte ich konsistente Fehlermeldungen erhalten.**

### Aufgabe

Implementiere eine zentrale Fehlerbehandlung.

```text
Request
  ↓
Controller
  ↓
Application
  ↓
Exception
  ↓
Global Error Handler
  ↓
HTTP Response
```

Beispiel:

```json
{
  "status": 404,
  "message": "Device not found"
}
```

### Fertig wenn

* [ ] Exceptions werden zentral behandelt
* [ ] Keine unnötigen try/catch-Blöcke in jedem Controller
* [ ] Keine Stack Traces an den Client

---

# 9. Repository Layer

## User Story

**Als Entwickler möchte ich Datenzugriff von der Business Logic trennen.**

### Aufgabe

Erstelle:

```text
IDeviceRepository
DeviceRepository
```

Architektur:

```text
Application
    ↓
IDeviceRepository
    ↓
Infrastructure
    ↓
EF Core
```

### Fertig wenn

* [ ] Application kennt nur das Interface
* [ ] Infrastructure implementiert das Interface
* [ ] Datenbankzugriffe liegen in Infrastructure

---

# 10. PostgreSQL + EF Core

## User Story

**Als Benutzer möchte ich, dass meine Geräte dauerhaft gespeichert werden.**

### Aufgabe

Integriere:

```text
Entity Framework Core
PostgreSQL
DbContext
```

Erstelle:

```text
CoatifyDbContext
```

mit:

```text
Devices
DeviceTypes
Measurements
Users
```

### Fertig wenn

* [ ] PostgreSQL läuft lokal
* [ ] EF Core funktioniert
* [ ] Daten werden gespeichert
* [ ] Neustart der API löscht keine Daten

---

# 11. Relationships

## User Story

**Als System möchte ich Geräte und Messwerte miteinander verbinden.**

### Beziehungen

```text
DeviceType
    1
    │
    └── * Device
             │
             └── * Measurement
```

### Fertig wenn

* [ ] Primary Keys definiert
* [ ] Foreign Keys definiert
* [ ] EF Core Relationships konfiguriert
* [ ] `Measurement.DeviceId` → `Device.Id`

---

# 12. Entity Configuration

## User Story

**Als Entwickler möchte ich mein Datenmodell sauber konfigurieren.**

### Aufgabe

Erstelle:

```text
Configurations/
├── DeviceConfiguration
├── DeviceTypeConfiguration
├── MeasurementConfiguration
└── UserConfiguration
```

Konfiguriere:

* Tabellen
* Required Fields
* String Lengths
* Relationships
* Constraints
* Indexes

---

# 13. Indexes

## User Story

**Als Entwickler möchte ich häufige Datenbankabfragen optimieren.**

### Aufgabe

Erstelle mindestens:

```text
Unique Index → SerialNumber
Index        → DeviceId
Index        → Timestamp
```

### Fertig wenn

* [ ] SerialNumber ist eindeutig
* [ ] DeviceId hat einen Index
* [ ] Timestamp hat einen Index

---

# 14. Migrations

## User Story

**Als Entwickler möchte ich Datenbankänderungen versionieren.**

### Aufgabe

Erste Migration:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Danach:

```text
Device
+ Location
```

Neue Migration erstellen.

### Fertig wenn

* [ ] Migrationen liegen im Repository
* [ ] Datenbank kann aus Migrationen erstellt werden
* [ ] Änderungen erzeugen neue Migrationen

---

# 15. Testing

## User Story

**Als Entwickler möchte ich sicherstellen, dass meine Business Logic funktioniert.**

### Aufgabe

Teste `DeviceService`.

Teste:

```text
CreateDevice
GetDevice
GetDevices
UpdateDevice
DeleteDevice
```

Auch Fehlerfälle:

```text
Device not found
Invalid device
Duplicate SerialNumber
```

### Fertig wenn

* [ ] Erfolgsfälle getestet
* [ ] Fehlerfälle getestet
* [ ] Tests laufen automatisch
* [ ] Unit Tests benötigen keine echte PostgreSQL-Datenbank

---

# 16. Abschlussaufgabe

## User Story

**Als Coatify-Administrator möchte ich ein Gerät registrieren und anschließend dessen Messwerte abrufen können.**

### Gerät erstellen

```http
POST /api/devices
```

```json
{
  "name": "Machine 01",
  "serialNumber": "COAT-001",
  "deviceTypeId": "..."
}
```

Flow:

```text
HTTP Request
    ↓
Controller
    ↓
Validation
    ↓
Application Service
    ↓
Domain
    ↓
Repository
    ↓
EF Core
    ↓
PostgreSQL
    ↓
Response DTO
```

### Messwerte abrufen

```http
GET /api/devices/{id}/measurements
```

Flow:

```text
HTTP Request
    ↓
Controller
    ↓
Application
    ↓
Repository
    ↓
EF Core
    ↓
PostgreSQL
    ↓
Measurement DTO
```

---

# 17. Lokales Backend abschließen

## User Story

**Als API-Nutzer möchte ich Geräte und Messwerte mit verlässlichen Antworten verwalten und abrufen.**

### Noch offene Aufgaben aus 1–16

1. Teile die vorhandenen Ordner in die geplanten Projekte auf. Domain bleibt frei von ASP.NET Core und EF Core; Application verwendet Repository-Interfaces, Infrastructure implementiert sie. Ersetze die unvollständige `IDeviceRepository`/`DeviceRepository`-Implementierung und den direkten EF-Core-Zugriff im `DeviceService` vollständig, ohne zwei parallele Datenzugriffswege zu behalten.
2. Vereinheitliche die vorhandenen Minimal-API-Routen mit Aufgabe 5: `/api/devices` und `/api/devices/{id}` für alle Geräteoperationen. Lege für GET ohne `ids` ein dokumentiertes Listenverhalten fest. Ergänze `GET /api/devices/{id}/measurements` mit Response-DTO; halte die HTTP-Mappings schlank.
3. Ergänze Validierung für leere/zu lange Werte, nicht existierenden Gerätetyp, doppelte Seriennummer sowie gültige Updates. Behandle eine Verletzung des Unique-Index auch bei konkurrierenden Requests verständlich. Korrigiere `GetDevice` bei fehlender ID auf 404 und DELETE bei Erfolg auf 204. Nutze strukturierte `ILogger<T>`-Meldungen; protokolliere keine Passwörter oder vollständigen Verbindungszeichenfolgen.
4. Prüfe die Modellkonfiguration gegen die fachlichen Regeln. Ergänze die in Aufgabe 14 vorgesehene `Location` und erstelle dafür eine zweite Migration. Teste den Schemaaufbau aus allen Migrationen auf einer leeren Datenbank; verwende `MigrateAsync()` und nicht `EnsureCreatedAsync()` für diese Datenbank.

### Fertig wenn

* [ ] Alle Projekte bauen mit den vorgesehenen Abhängigkeiten; `DeviceService` kennt nur das Repository-Interface.
* [ ] POST, GET, PUT und DELETE liefern für Erfolg, ungültige Eingabe, Duplikat und fehlende ID die in Aufgabe 5 genannten Codes.
* [ ] Der Messwerte-Endpunkt liefert für ein vorhandenes Gerät seine Messwerte und für eine fehlende ID 404.
* [ ] Ein frischer PostgreSQL-Stand entsteht nur aus Migrationen; ein erneuter Migrationslauf ist ohne Fehler möglich.

---

# 18. Tests vervollständigen und isolieren

## User Story

**Als Entwickler möchte ich Backend-Änderungen wiederholt testen können, ohne von vorhandenen Daten abzuhängen.**

### Aufgabe

Ergänze echte `DeviceService`-Tests für CRUD, Validierung, Duplikate und fehlende Geräte. Ergänze HTTP-Integrationstests für Routen, Statuscodes und Messwerte. Halte `coatify_test` von `coatify` getrennt; jeder datenbankbasierte Test bekommt einen eigenen `CoatifyContext` und legt seine benötigten Datensätze selbst an. Ersetze Tests, die `Devices.First()` auf der gemeinsamen Datenbank verwenden oder nur EF Core statt des Service/API-Verhaltens prüfen. Bereite feste IDs wiederholbar vor oder verwende eindeutige IDs und Seriennummern.

### Fertig wenn

* [ ] `dotnet build Coatify.Tests/Coatify.Tests.csproj` ist erfolgreich.
* [ ] Service- und HTTP-Tests prüfen Erfolgs- und Fehlerfälle mit konkreten Assertions; ein leerer grüner Test zählt nicht.
* [ ] Datenbanktests laufen auf einer frischen `coatify_test` und ein zweites Mal ohne Datenkonflikte.
* [ ] Reine Geschäftsregeln haben Unit Tests ohne PostgreSQL; Datenzugriff und HTTP werden separat mit PostgreSQL geprüft.

---

# 19. Blazor-Oberfläche

`Coatify.Web` existiert bereits als eigenständiges Blazor-WebAssembly-Projekt mit `App.razor`, `Home.razor`, `NotFound.razor`, `MainLayout.razor` und `NavMenu.razor`. Diese Dateien sind ein Startpunkt, aber noch keine Fachoberfläche. Die folgenden Stories sind in der Reihenfolge zu bearbeiten; sie setzen die passenden Backend-Aufgaben aus 17 und 18 voraus. Blazor greift über HTTP auf die API zu, niemals direkt auf `CoatifyContext` oder PostgreSQL.

## 19.1 Web-Projekt und API-Verbindung vorbereiten

### User Story

**Als Entwickler möchte ich Web und API gemeinsam starten und bauen können, damit ich die Oberfläche gegen echte Endpunkte entwickle.**

### Aufgabe

Füge `Coatify.Web` zur Solution hinzu. Richte eine zentrale, per DI verwendete HTTP-Client-Klasse für Geräte und Messwerte ein. Die Basisadresse der API muss zwischen lokaler Entwicklung und Veröffentlichung konfigurierbar sein; der derzeitige `HttpClient` in `Coatify.Web/Program.cs` verwendet die Adresse der Web-App. Für zwei lokale Ports braucht die API eine gezielte CORS-Freigabe für den Web-Origin. Kläre das Listenverhalten von `GET /api/devices` und vereinheitliche zuerst die Gerätepfade in Aufgabe 17. Teile Request-/Response-Verträge nur dann in ein kleines gemeinsames Projekt aus, wenn dadurch keine Server- oder EF-Core-Abhängigkeiten im Browser landen.

### Fertig wenn (DoD)

* [ ] `dotnet build coatify.sln` baut auch `Coatify.Web`; Web und API starten lokal mit dokumentierten URLs.
* [ ] Der API-Client hat eine einzige konfigurierbare Basisadresse; keine lokale URL ist in einer Razor-Seite fest codiert.
* [ ] Ein erfolgreicher GET erreicht die echte Coatify-API; ein Ausfall wird als Fehler sichtbar, nicht als leere Liste.
* [ ] CORS erlaubt nur die benötigten lokalen Origins; der Browser zeigt keinen CORS-Fehler.

## 19.2 Navigation, Startseite und unbekannte Seiten

### User Story

**Als Benutzer möchte ich die Geräteverwaltung leicht finden und bei einem falschen Link zurückfinden.**

### Aufgabe

Ersetze die Platzhalter in `Home.razor`, `NavMenu.razor` und `MainLayout.razor` durch Coatify-Texte und Links zur Geräteliste. Gestalte `NotFound.razor` mit einer verständlichen Meldung und einem Link zurück zur Startseite. Unterscheide eine unbekannte Web-Route von einem `404` für eine Geräte-ID: Die erste zeigt `NotFound.razor`, die zweite einen Fehlerzustand der Detailseite.

### Fertig wenn (DoD)

* [ ] Startseite und Menü führen mit Blazor-Navigation zur Geräteliste; der Menüpunkt zeigt den aktiven Zustand.
* [ ] Eine unbekannte URL zeigt die NotFound-Seite mit funktionierendem Rückweg.
* [ ] Die Menübedienung funktioniert per Tastatur und auf schmalem Bildschirm; Template-Bezeichnungen wie „Coatify.Web“/„About“ sind ersetzt.

## 19.3 Geräteliste

### User Story

**Als Administrator möchte ich vorhandene Geräte sehen und eines zur Detailansicht öffnen.**

### Aufgabe

Erstelle eine Listenseite mit Gerätename, Status und einem Link zum Gerät. `GET /api/devices` muss dafür eine normale Liste ohne vorher bekannte IDs liefern; die aktuelle API fordert `ids` und kann diese Story so noch nicht erfüllen. Zeige Laden, keine Geräte und API-Fehler getrennt an. Ein erneuter Aufruf soll den aktuellen Stand zeigen.

### Fertig wenn (DoD)

* [ ] Die Seite zeigt die vom API-Response gelieferten Geräte und öffnet per Link die richtige ID.
* [ ] Eine leere Datenbank zeigt „Noch keine Geräte“; ein Netzwerk-/Serverfehler zeigt eine Fehlermeldung und eine Wiederholen-Aktion.
* [ ] Nach Anlegen oder Löschen ist die Liste ohne Browser-Neustart aktuell.

## 19.4 Gerätedetails und Messwerte

### User Story

**Als Administrator möchte ich ein Gerät und seine Messwerte sehen, um seinen Zustand zu prüfen.**

### Aufgabe

Erstelle eine Detailroute mit Geräte-ID. Lade Gerät und Messwerte über `GET /api/devices/{id}` und `GET /api/devices/{id}/measurements`. Der Messwerte-Endpunkt und ein passendes Response-DTO fehlen aktuell noch im Backend. Zeige Zeitstempel und Einheiten für Temperatur, Druck und Stromverbrauch verständlich an; sortiere Messwerte nachvollziehbar, zum Beispiel neueste zuerst. Behaupte keine Live-Aktualisierung, solange kein entsprechender Mechanismus gebaut wurde.

### Fertig wenn (DoD)

* [ ] Ein existierendes Gerät zeigt die richtigen Stammdaten und nur seine Messwerte.
* [ ] Ein Gerät ohne Messwerte zeigt einen eigenen Leerzustand; eine unbekannte Geräte-ID zeigt einen 404-Zustand mit Rückweg zur Liste.
* [ ] API-Ausfall, Laden und ungültige ID sind erkennbar; Zeitstempel und Maßeinheiten werden konsistent dargestellt.

## 19.5 Gerät anlegen

### User Story

**Als Administrator möchte ich ein Gerät im Browser registrieren können.**

### Aufgabe

Baue ein Formular für `Name`, `SerialNumber`, `Status` und `DeviceTypeId`, passend zu `CreateDeviceRequest`. Für die Auswahl eines Gerätetyps braucht die UI einen lesbaren Gerätetyp-Endpunkt oder eine ausdrücklich dokumentierte, lokal vorbereitete Auswahl; eine frei erfundene ID ist keine nutzbare Lösung. Prüfe Pflichtfelder im Formular und zeige Validierungsantworten der API zusätzlich an. Sende `POST /api/devices` erst nach gültiger Eingabe.

### Fertig wenn (DoD)

* [ ] Ein gültiger Request erstellt genau ein Gerät; nach `201 Created` wird dessen Detailansicht geöffnet oder die Liste aktualisiert.
* [ ] Leere Felder, ungültiger Gerätetyp und doppelte Seriennummer werden verständlich angezeigt; das Formular bleibt mit Eingaben erhalten.
* [ ] Während des Sendens ist doppeltes Absenden verhindert; ein API-Fehler wird nicht als Erfolg dargestellt.

## 19.6 Gerät bearbeiten und löschen

### User Story

**Als Administrator möchte ich Stammdaten ändern und ein Gerät bewusst entfernen können.**

### Aufgabe

Fülle das Bearbeitungsformular mit den aktuellen Werten und sende `PUT /api/devices/{id}`. Beachte, dass `DeviceResponse` derzeit nur ID, Name und Status enthält: Für `SerialNumber` und `DeviceTypeId` braucht die UI einen dafür geeigneten Detail-Response. Ergänze eine Löschaktion mit Bestätigung und rufe `DELETE /api/devices/{id}` auf. Die API soll bei Erfolg `204 No Content` liefern.

### Fertig wenn (DoD)

* [ ] Vorhandene Werte sind sichtbar; nach Speichern zeigt die Detailseite die vom Backend bestätigten Werte.
* [ ] Bei ungültigen Daten oder `404` bleibt das Formular erhalten und zeigt eine passende Meldung.
* [ ] Abbrechen löscht nichts; nach bestätigtem `204` ist das Gerät aus Liste und Detailnavigation entfernt.

## 19.7 Qualität und Bereitstellung der Oberfläche

### User Story

**Als Entwickler möchte ich die wichtigsten Browser-Abläufe nach Änderungen verlässlich prüfen und die UI gemeinsam mit der API veröffentlichen.**

### Aufgabe

Teste mindestens Navigation/NotFound, Liste mit Leer- und Fehlerzustand, Anlegen mit Validierungsfehler, Detail mit/ohne Messwerte sowie Bearbeiten/Löschen gegen kontrollierte Testdaten. Dokumentiere lokale Starts und den Build. Plane das Veröffentlichen der Blazor-WebAssembly-Dateien über dieselbe ASP.NET-Core-App wie die API (Aufgabe 22), einschließlich Fallback für direkte Aufrufe von UI-Routen. Authentifizierung und Rollen aus Aufgabe 20 kommen danach hinzu; Browser-Validierung ersetzt keine serverseitige Prüfung.

### Fertig wenn (DoD)

* [ ] Automatisierte UI-/Komponententests decken die genannten Zustände ab; ein manueller Durchlauf prüft den gesamten Gerätefluss im Browser.
* [ ] `dotnet build coatify.sln` und ein Release-Publish der Web-App gelingen; lokale Startschritte sind dokumentiert.
* [ ] Nach dem Publish funktionieren Startseite, direkte Detail-URL, NotFound und API-Aufrufe unter einer gemeinsamen HTTPS-Adresse.
* [ ] Keine Datenbank-Zugangsdaten oder Tokens sind im WebAssembly-Paket enthalten; Browsercode enthält nur öffentliche Konfiguration.

---

# 20. Anmeldung und Rollen mit Microsoft Entra ID

## User Story

**Als Betreiber möchte ich Schreibzugriffe auf Administratoren begrenzen.**

### Aufgabe

Registriere UI und API im vorhandenen Entra-Tenant, konfiguriere Anmeldung und Tokenprüfung für die Minimal APIs. Definiere App-Rollen wie `Coatify.Reader` und `Coatify.Admin`; GET ist lesbar, POST/PUT/DELETE nur für Admins. Prüfe `401` ohne gültiges Token und `403` ohne erforderliche Rolle. Verwende keine P1/P2-Funktionen wie Conditional Access oder Entra-Custom-Roles als Voraussetzung. Entra-App-Rollen sind von Azure-RBAC-Rollen für Ressourcen zu unterscheiden.

### Fertig wenn

* [ ] Ein angemeldeter Reader kann lesen, aber nicht schreiben; ein Admin kann beide Aktionen ausführen.
* [ ] UI und API verwenden dokumentierte Redirect-URIs, Scopes und Rollen; Secrets liegen nicht im Repository.
* [ ] Die Konfiguration funktioniert lokal und lässt sich für die Azure-URL wiederholen.

**Azure-Rahmen:** Dienst: Microsoft Entra ID Free; Tarif: Free; Region: bestehender Tenant, Datenresidenz des Tenants gesondert prüfen; geschätzte Zusatzkosten: 0 für die hier genutzten Free-Funktionen; Einschränkungen: App-Registrierungsrechte und Rollenzuweisung im Tenant müssen vorhanden sein, P1/P2 ist ausgeschlossen. [Entra-Lizenzübersicht](https://learn.microsoft.com/en-us/entra/fundamentals/licensing), [App-Registrierung](https://learn.microsoft.com/en-us/entra/identity-platform/howto-create-service-principal-portal).

---

# 21. Kostengünstige Azure-Betriebsvariante vorbereiten

## User Story

**Als Student möchte ich Coatify mit begrenztem Guthaben betreiben und die laufenden Kosten vorab verstehen.**

### Aufgabe

Plane **eine Hauptvariante**: ASP.NET-Core-API plus Blazor-WebAssembly-Dateien als Code-Deploy auf **Azure App Service Free F1 (Windows, .NET 10)**, dazu **ein** Azure Database for PostgreSQL Flexible Server im **Burstable B1ms**-Tarif mit minimal zulässigem Speicher (laut aktueller Compute-Dokumentation 32 GiB), ohne Hochverfügbarkeit. Beide Ressourcen sollen in **West Europe** liegen, sofern Subscription-Policy, F1-Kapazität und B1ms-SKU das dort erlauben. Prüfe diese drei Punkte im Student-Abonnement vor dem Anlegen; falls einer scheitert, wähle eine gemeinsam verfügbare EU-Region und aktualisiere Kosten und Dokumentation. Die Datenbank enthält nur wenige Geräte und Messwerte; eine zweite Cloud-Testdatenbank ist nicht nötig.

Prüfe im Azure Sponsorships Portal das **tatsächlich verbleibende** Guthaben und Ablaufdatum. Erstelle im [Azure-Preiskalkulator](https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/pricing-calculator) eine aktuelle West-Europe-Schätzung für 730 Stunden B1ms, 32 GiB Speicher, Sicherungen, Datenverkehr und eventuelle Log-Daten. Lege eine persönliche monatliche Kostenobergrenze fest und richte Warnungen vor ihrem Erreichen ein. **Eine Budgetwarnung stoppt keine Ressource.** Plane deshalb zusätzlich eine regelmäßige manuelle Kostenkontrolle und einen konkreten Löschtermin.

### Fertig wenn

* [ ] Subscription, Region, SKU-Verfügbarkeit, Restguthaben und Ablaufdatum sind überprüft und dokumentiert.
* [ ] Eine getrennte Kostenaufstellung für App, Datenbank, Speicher, Backups, Logs und ausgehenden Traffic liegt vor; unklare Posten bleiben ausdrücklich „unbekannt“.
* [ ] Budgetwarnungen und ein Verfahren zum Stoppen/Löschen sind beschrieben; nach dem Übungszeitraum werden Cloud-Ressourcen und temporäre Daten entfernt.

**Azure-Rahmen:** Dienste: App Service F1 und PostgreSQL Flexible Server Burstable B1ms; Region: geplant West Europe, nur nach Verfügbarkeitsprüfung; geschätzte Kosten: App Service **0** innerhalb F1, PostgreSQL **laufend kostenpflichtig und Betrag für West Europe noch unbekannt** (Compute 730 h + mindestens 32 GiB Speicher + ggf. Backup/Traffic). Als grober Microsoft-Referenzwert zeigt die [PostgreSQL-Preisseite](https://azure.microsoft.com/en-us/pricing/details/postgresql/flexible-server/) etwa **12,41 USD/Monat allein für B1ms-Compute** in ihrer angezeigten Beispielpreislage; das ist **kein bestätigter West-Europe- oder Student-Preis**. F1 hat u. a. ein [CPU-Limit von 60 Minuten/Tag und 1 GiB Speicher](https://learn.microsoft.com/en-us/azure/azure-resource-manager/management/azure-subscription-service-limits#app-service-limits); bei Überschreitung ist die Hauptvariante nicht dauerhaft nutzbar. Das allgemeine 12-Monats-Gratisangebot für PostgreSQL eines **Azure Free Account** wird hier nicht als Vorteil des Student-Abonnements angenommen. [Azure for Students](https://learn.microsoft.com/en-us/azure/education-hub/faq), [PostgreSQL-Compute](https://learn.microsoft.com/en-us/azure/postgresql/compute-storage/concepts-compute), [Budgetwarnungen](https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-acm-create-budgets).

---

# 22. Hauptvariante bereitstellen und wiederherstellen

## User Story

**Als Betreiber möchte ich Coatify reproduzierbar online stellen und die Datenbank sicher verbinden.**

### Aufgabe

Erstelle einen reproduzierbaren Code-Deploy für App Service F1 mit .NET 10. Setze `ConnectionStrings__Coatify` als Azure-Konfiguration, erzwinge TLS zur PostgreSQL-Datenbank und verwende einen eigenen Datenbankbenutzer mit passenden Rechten. Führe EF-Core-Migrationen als getrennten Bereitstellungsschritt aus; `MigrateAsync()` darf nicht mit `EnsureCreatedAsync()` kombiniert werden. Konfiguriere PostgreSQL-Firewallregeln nur für die erforderlichen ausgehenden App-Adressen und einen zeitlich begrenzten Admin-Zugang. `0.0.0.0`/„alle Azure-Dienste“ ist keine enge Freigabe, da damit auch andere Azure-Kunden den Server erreichen können. Prüfe HTTP- und Datenbankzugriff nach Deploy und nach App-Neustart.

### Fertig wenn

* [ ] Die Azure-URL liefert die UI; autorisierte API-Aufrufe funktionieren über HTTPS.
* [ ] Migrationen laufen auf einer frischen Azure-Datenbank und bei erneutem Deploy ohne Fehler.
* [ ] Ein Gerät bleibt nach Neustart erhalten; Verbindungsdaten erscheinen nicht in Git, Logs oder Build-Artefakten.
* [ ] Deploy, Rollback, Datenexport und vollständige Ressourcenbereinigung sind als Schritte dokumentiert.

**Azure-Rahmen:** Dienste/Tarife: dieselben App Service F1 und PostgreSQL B1ms aus Aufgabe 21, keine zweite Dauerumgebung; Region: dieselbe geprüfte EU-Region; geschätzte Zusatzkosten: **0** für den App-Deploy innerhalb F1, Datenbank und möglicher Traffic wie Aufgabe 21, genaue Summe unbekannt. Einschränkungen: F1-Quoten, ausgehende App-Adressen können sich ändern und müssen kontrolliert werden; öffentliche PostgreSQL-Firewall benötigt enge Regeln und TLS. [ASP.NET-Core-Deploy](https://learn.microsoft.com/en-us/azure/app-service/quickstart-dotnetcore), [PostgreSQL-Firewall](https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/security-firewall-rules).

---

# 23. Infrastruktur und Dokumentation reproduzierbar machen

## User Story

**Als Entwickler möchte ich dieselbe kleine Umgebung mit überprüfbaren Parametern aufbauen und wieder entfernen können.**

### Aufgabe

Erstelle Bicep für Resource Group, F1-App, B1ms-Datenbank und benötigte Einstellungen. Nutze Parameter für Namen, Region, SKU und Secret-Referenzen; keine Passwörter oder Student-IDs im Template. Beschreibe in einem Architekturdiagramm den Weg Browser → UI/API → PostgreSQL, die Rollen, Vertrauensgrenzen, Kostenstellen und die lokalen Testdaten. Dokumentiere einmaliges Provisionieren, Migration, Deploy, Prüfung, Export und Löschen. Führe einen Bicep-What-if-Lauf vor einer tatsächlichen Bereitstellung aus.

### Fertig wenn

* [ ] Templates sind ohne gespeicherte Secrets versionierbar und ein What-if zeigt die erwarteten Ressourcen.
* [ ] Die Anleitung nennt die konkrete Region, Tarife und Löschbefehle; ein zweiter Aufbau benötigt keine undokumentierten Portal-Klicks.
* [ ] Ein Architekturdiagramm und die Kostenannahmen stimmen mit der Hauptvariante überein.

**Azure-Rahmen:** Dienst: Azure Resource Manager/Bicep; Tarif: kein separater Diensttarif; Region: Zielregion der Ressourcen aus Aufgabe 21; geschätzte Zusatzkosten: **0** für Templates/What-if, die im Template erzeugten Ressourcen kosten wie Aufgabe 21. Einschränkung: ein What-if ist noch kein Kapazitäts- oder Preisnachweis. [Bicep-What-if](https://learn.microsoft.com/en-us/azure/azure-resource-manager/bicep/deploy-what-if).

---

# 24. Optionale Cloud-Übungen nach dem Haupt-Deploy

Diese Übungen sind **nicht Teil des Dauerbetriebs**. Vor jeder Übung: Verfügbarkeit im eigenen Student-Abonnement, konkrete EU-Region, Tages-/Monatspreis und verbleibendes Guthaben prüfen. Ressourcen in eine eigene Übungs-Resource-Group legen, Endzeit notieren, danach löschen und die Löschung kontrollieren. Wenn ein Tarif oder eine Berechtigung fehlt, die Lernabsicht lokal oder mit der genannten günstigen Alternative erfüllen.

## 24.1 Docker, Container Registry und alternatives Hosting

**Ziel:** Ein API-Dockerfile bauen, lokal mit PostgreSQL aus Compose prüfen und Image-Tags verstehen. Danach höchstens kurzzeitig **Azure Container Registry Basic** und **Azure Container Apps Consumption** bereitstellen, ein Image per Managed Identity ziehen, HTTP prüfen und beide Dienste wieder löschen. Der laufende App-Service-Deploy bleibt die einzige Betriebsvariante. `az containerapp up --source` kann automatisch eine kostenpflichtige Registry und einen Log-Analytics-Workspace anlegen; ihre Kosten vor dem Befehl einplanen.

**Fertig wenn:** lokaler Image-Build und API-Smoke-Test funktionieren; Registry-Push und Container-Apps-Pull sind einmal nachgewiesen; temporäre Ressourcen sind entfernt.

**Azure-Rahmen:** Dienste/Tarife: ACR Basic (**Tagesgebühr**, 10 GiB inklusive) und Container Apps Consumption (180.000 vCPU-s, 360.000 GiB-s, 2 Mio. Requests pro Monat frei auf Subscription-Ebene); Region: dieselbe geprüfte EU-Region; geschätzte Kosten: Container Apps **0 innerhalb der Freikontingente**, ACR **>0 für jeden Nutzungstag, genaue West-Europe-Rate unbekannt**, Log-Ingestion/Netzwerk ggf. zusätzlich. Einschränkungen: Freikontingente sind geteilt, Container Apps mit VNet können mehr kosten, ACR Basic unterstützt keine Private Endpoints. [Container-Apps-Abrechnung](https://learn.microsoft.com/en-us/azure/container-apps/billing), [ACR-Tarife](https://learn.microsoft.com/en-us/azure/container-registry/container-registry-skus), [Managed-Identity-Pull](https://learn.microsoft.com/en-us/azure/container-apps/managed-identity-image-pull), [Container-App-up](https://learn.microsoft.com/en-us/azure/container-apps/containerapp-up).

## 24.2 Managed Identity, Key Vault und Networking

**Ziel:** Eine systemseitige Managed Identity für die App aktivieren, den PostgreSQL-Secret-Wert in **Key Vault Standard** ablegen und mit minimalen RBAC-Rechten abrufen. Danach Zugriff und Rotation testen. Vergleiche die vorhandene enge Firewall mit einem privaten Netzwerk auf dem Papier; ein VNet/Private Endpoint ist nur eine separate befristete Übung nach Preisprüfung. Lerne, warum `0.0.0.0` als PostgreSQL-Firewallregel alle Azure-Abonnements einbezieht.

**Fertig wenn:** die App liest das Secret über ihre Identity, ein unberechtigter Principal nicht; nach Rotation läuft sie weiter; alle Übungsregeln und Ressourcen sind entfernt.

**Azure-Rahmen:** Dienste/Tarife: Managed Identity (kein separater Tarif), Key Vault Standard (transaktionsabhängig), optional VNet/Private Endpoint (nutzungsabhängig); Region: App/Key Vault in derselben geprüften EU-Region, Tenant wie in Aufgabe 20; geschätzte Kosten: Identity **0**, Key Vault **gering, aber genaue Rate unbekannt**, Private Endpoint und Datenverkehr **unbekannt und vor Erstellung zu kalkulieren**. Einschränkungen: RBAC- und Netzwerkberechtigungen nötig; Private Endpoint ist nicht Bestandteil der günstigen Hauptvariante. [Key-Vault-Preise](https://azure.microsoft.com/en-us/pricing/details/key-vault/), [App-Service-Key-Vault-Referenzen](https://learn.microsoft.com/en-us/azure/app-service/app-service-key-vault-references), [PostgreSQL-Firewall](https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/security-firewall-rules).

## 24.3 Telemetrie, Application Insights und Grafana

**Ziel:** Für kurze Zeit Application Insights mit einem Log-Analytics-Workspace anbinden, eine fehlerhafte Anfrage und eine langsame Anfrage nachvollziehen, Sampling und kurze Aufbewahrung einstellen. Eine kleine Visualisierung mit **Azure Monitor dashboards with Grafana** ausprobieren, falls die Funktion im Portal verfügbar ist. **Azure Managed Grafana Standard** nicht anlegen: Die frühere kostenlose Essential-Stufe kann für neue Workspaces nicht mehr erstellt werden; die Lernabsicht „Dashboards verstehen“ erfüllt die Azure-Monitor-Integration.

**Fertig wenn:** Request, Fehler und Latenz im Dashboard sichtbar sind; Ingestionsmenge und Aufbewahrung geprüft und die Übungsressourcen gelöscht sind.

**Azure-Rahmen:** Dienste/Tarife: Application Insights + Log Analytics nutzungsbasiert, Azure Monitor dashboards with Grafana ohne eigenen Workspace-Tarif; Region: dieselbe geprüfte EU-Region; geschätzte Kosten: **0, falls die 5 GiB/Monat freie Log-Analytics-Menge des Billing Accounts nicht aufgebraucht ist**, sonst Ingestion/Retention nach Tarif, genauer Betrag unbekannt. Einschränkungen: Freimenge wird geteilt, Diagnoseexporte/Alerts können zusätzliche Kosten auslösen; Azure Managed Grafana Standard ist kostenpflichtig. [Application-Insights-Abrechnung](https://learn.microsoft.com/en-us/azure/azure-monitor/app/application-insights-faq), [Grafana-Vergleich](https://learn.microsoft.com/en-us/azure/azure-monitor/visualize/visualize-grafana-overview), [Managed-Grafana-Tarife](https://learn.microsoft.com/en-us/azure/managed-grafana/overview).

## 24.4 IoT Hub, Messaging und Worker

**Ziel:** Erst lokal Messwerte per Simulator und Worker erzeugen und in PostgreSQL speichern. Danach optional einen **IoT Hub F1** für wenige Testgeräte kurz anlegen, Device-to-Cloud-Nachrichten empfangen und idempotent speichern. Die ursprüngliche Lernabsicht „entkoppelte Nachrichtenverarbeitung“ kann bei fehlender F1-Verfügbarkeit lokal über eine einfache Queue-Abstraktion gezeigt werden; zusätzliche Event-Hubs-/Service-Bus-Instanzen sind nicht vorausgesetzt.

**Fertig wenn:** ein wiederholter Messwert nicht doppelt gespeichert wird, fehlerhafte Nachrichten protokolliert werden und der F1-Hub nach dem Versuch gelöscht ist.

**Azure-Rahmen:** Dienst/Tarif: IoT Hub F1; Region: dieselbe EU-Region nur falls dort für das Student-Abonnement verfügbar; geschätzte Kosten: Hub **0 innerhalb F1**, Worker lokal **0**, ausgehender Traffic/zusätzliche Senken **unbekannt**. Einschränkungen: höchstens ein F1-Hub pro Subscription, 8.000 Nachrichten/Tag und höchstens 500 Geräte; F1 ist für Test/Evaluation gedacht und die tatsächliche Anlegbarkeit ist nicht verifiziert. [IoT-Hub-Limits](https://learn.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-quotas-throttling), [Hub-Erstellung](https://learn.microsoft.com/en-us/azure/iot-hub/create-hub).

## 24.5 Cosmos DB als befristete Datenmodell-Übung

**Ziel:** Einige Messwerte als Dokumente in **Azure Cosmos DB Free Tier** modellieren und Abfragen mit PostgreSQL vergleichen. Danach das Cosmos-Konto löschen. PostgreSQL bleibt die einzige dauerhafte Datenbank; keine Doppelhaltung im Hauptbetrieb.

**Fertig wenn:** Partition Key, RU-Verbrauch und ein Vergleich einer Messwertabfrage dokumentiert sind; die temporären Daten sind entfernt.

**Azure-Rahmen:** Dienst/Tarif: Cosmos DB provisioned throughput mit aktiviertem Free Tier; Region: dieselbe geprüfte EU-Region; geschätzte Kosten: **0 bis 1.000 RU/s und 25 GiB**, darüber kostenpflichtig und Höhe unbekannt. Einschränkungen: nur ein Free-Tier-Konto pro Subscription, Free Tier muss bei Erstellung gewählt werden und gilt nicht für Serverless; Verfügbarkeit im Student-Abonnement vorab prüfen. [Cosmos-Free-Tier](https://learn.microsoft.com/en-us/azure/cosmos-db/free-tier).

## 24.6 Azure DevOps, CI/CD und Artifacts

**Ziel:** Eine Azure-DevOps-Pipeline mit Restore, Build, Unit Tests und gefilterten Integrationstests entwerfen; Deploy erst nach Prüfung auslösen. Ein kleines internes Paket oder Build-Artefakt in Azure Artifacts veröffentlichen und wiederherstellen. Wenn für die Organisation kein kostenloser Microsoft-hosted Parallel Job freigeschaltet ist, Pipeline-YAML lokal validieren und den Deploy weiterhin manuell ausführen; keine bezahlten Parallel Jobs oder Lizenzen buchen.

**Fertig wenn:** YAML und benötigte Variablen/Identitäten dokumentiert sind, ein verfügbarer kostenloser Agent die Checks ausführt oder die fehlende Freigabe ausdrücklich festgehalten ist, und Artifacts-Speicher kontrolliert/geleert wurde.

**Azure-Rahmen:** Dienste/Tarife: Azure DevOps Basic (erste fünf Nutzer frei), Pipelines Free Grant (wenn zugeteilt: ein Microsoft-hosted Job/1.800 Minuten pro Monat), Azure Artifacts (2 GiB pro Organisation frei); Region: Azure-DevOps-Organisationsstandort gesondert wählen/prüfen, Deployziel wie Aufgabe 21; geschätzte Kosten: **0 innerhalb der genannten Kontingente**, darüber oder ohne Grant nicht eingeplant. Einschränkungen: Free Grant kann für neue Organisationen eine Freischaltung erfordern; Azure DevOps wird nicht als zusätzliche Cloud-Laufzeit genutzt. [DevOps-Abrechnung](https://learn.microsoft.com/en-us/azure/devops/organizations/billing/billing-faq), [Parallel Jobs](https://learn.microsoft.com/en-us/azure/devops/pipelines/licensing/concurrent-jobs), [Artifacts-Limits](https://learn.microsoft.com/en-us/azure/devops/artifacts/reference/limits).

---

## Azure-for-Students-Kostenkontrolle und offene Annahmen

Das Student-Angebot nennt **100 USD Guthaben für 12 Monate**; das ist kein monatliches Freikontingent und kein dauerhaft kostenloser Betrieb. Das **verbleibende** Guthaben ist hier unbekannt und wird nicht geschätzt. Bei Verbrauch oder Ablauf wird die Subscription ohne Upgrade deaktiviert; diese Roadmap setzt kein Pay-as-you-go-Upgrade und keine Zusatzlizenz voraus. [Angebotsbedingungen](https://learn.microsoft.com/en-us/azure/education-hub/faq), [Guthaben prüfen](https://learn.microsoft.com/en-us/azure/cost-management-billing/manage/azurestudents-subscription-disabled).

| Kostenposten der einzigen Hauptvariante | Vorläufige Monatsannahme | Vor Bereitstellung zu klären |
| --- | --- | --- |
| App Service F1 mit API und statischer Blazor-UI | 0 innerhalb F1-Quoten | .NET-10/F1-Angebot und CPU-Nutzung in der gewählten Region |
| PostgreSQL Flexible Server B1ms, 32 GiB | **Unbekannt für West Europe**; Microsoft zeigt ca. 12,41 USD/Monat als nicht regionsbestätigten Compute-Referenzwert, plus Speicher | Aktuelle Subscription-Rate, SKU, Stunden, Speicher und Backup im Preiskalkulator |
| Netzwerk, Backups über Inklusivmenge, zusätzliche Logs | Unbekannt; für kleine Datenmenge vermutlich gering, aber keine Null-Garantie | Region, ausgehende GiB, Backup- und Log-Mengen |
| **Gesamt** | **Noch nicht belastbar bezifferbar** | Preisexport mit Datum und tatsächlich verbleibendes Studentenguthaben getrennt dokumentieren |

Vor jeder optionalen Übung: Budgetwarnungen prüfen, geschätzte Tageskosten und Endzeit notieren. Nach jeder Übung die eigene Resource Group samt Datenträgern, Logs, Registry, Secrets und Netzwerkressourcen löschen. „Server stoppen“ beendet bei PostgreSQL nur die Compute-Abrechnung; Speicher kann weiter kosten. [PostgreSQL-Überblick](https://learn.microsoft.com/en-us/azure/postgresql/flexible-server/overview), [Budgetwarnungen](https://learn.microsoft.com/en-us/azure/cost-management-billing/costs/tutorial-acm-create-budgets).

---

# Definition of Done

* [ ] .NET Solution
* [ ] Domain
* [ ] Application
* [ ] Infrastructure
* [ ] ASP.NET Core API
* [ ] DTOs
* [ ] Validation
* [ ] Dependency Injection
* [ ] Logging
* [ ] Error Handling
* [ ] Repository
* [ ] PostgreSQL
* [ ] EF Core
* [ ] Relationships
* [ ] Foreign Keys
* [ ] Indexes
* [ ] Migrations
* [ ] Unit Tests
* [ ] CRUD funktioniert
* [ ] Measurements Endpoint funktioniert
* [ ] Service- und API-Tests laufen wiederholt mit isolierter Testdatenbank
* [ ] Blazor-Oberfläche und Entra-Rollen sind lokal geprüft
* [ ] Azure-Preisprüfung und Restguthaben sind vor dem Deploy dokumentiert
* [ ] Die einzige Azure-Hauptvariante ist reproduzierbar bereitgestellt und bereinigt
* [ ] Optionale Cloud-Übungen sind nach ihrem Test wieder abgebaut
