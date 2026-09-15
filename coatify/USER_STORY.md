# Coatify – .NET Learning Tasks | Phase 1-7 


## Ziel

Baue ein kleines IoT-Backend mit:

* C#
* .NET
* ASP.NET Core
* Entity Framework Core
* PostgreSQL

Später folgen Blazor und Azure.

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
