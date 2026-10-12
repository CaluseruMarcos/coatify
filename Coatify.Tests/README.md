# PostgreSQL-Tests

Im Repository-Verzeichnis starten:

```bash
bash Coatify.Tests/run-postgres-tests.sh
```

Das Skript startet den PostgreSQL-Container aus `compose.yaml` und führt `dotnet test` aus. Die Tests verwenden die separate Datenbank `coatify_test`. `TestDatabase.ConnectToDatabase()` prüft die Verbindung und führt die vorhandenen EF-Core-Migrationen aus, falls die Datenbank noch nicht erreichbar ist.

Der Container stellt PostgreSQL unter `127.0.0.1:1234` bereit. Benutzer und Passwort sind `coatify`. Die von `compose.yaml` angelegte Datenbank `coatify` ist die Entwicklungsdatenbank. `GetDeviceTest` legt zwei Geräte mit festen IDs in `coatify_test` an und prüft sie über `DeviceService.GetDevice`. `CreateDeviceTest` legt bei jedem Lauf neue Daten an. Die Tests für Aktualisieren und Löschen arbeiten derzeit direkt auf der gemeinsamen Testdatenbank; ihre Ergebnisse können deshalb von vorhandenen Daten und der Testreihenfolge abhängen.

Wenn du `COATIFY_TEST_ADMIN_CONNECTION` setzt, startet das Skript keinen Docker-Container. `TestDatabase` verwendet derzeit trotzdem seine fest eingetragene Verbindung zu `127.0.0.1:1234`; passe diese vor einem Testlauf gegen einen anderen Server an.
