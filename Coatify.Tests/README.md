# PostgreSQL-Tests

Im Repository-Verzeichnis starten:

```bash
bash Coatify.Tests/run-postgres-tests.sh
```

Das Skript startet den PostgreSQL-Container aus `compose.yaml` und führt `dotnet test` aus. Die Vorbereitung der separaten Datenbank `coatify_test` und der Testdaten ist noch deine Aufgabe. `GetDeviceTest` ist deshalb vorerst als übersprungen markiert.

Der Container stellt PostgreSQL unter `127.0.0.1:1234` bereit. Benutzer und Passwort sind `coatify`. Die von `compose.yaml` angelegte Datenbank `coatify` ist die Entwicklungsdatenbank; verwende für Tests eine eigene Datenbank, zum Beispiel `coatify_test`.

Wenn du einen anderen PostgreSQL-Server verwendest, setze `COATIFY_TEST_ADMIN_CONNECTION`, damit das Skript den Docker-Container nicht startet. Die Verbindung zu diesem Server musst du dann selbst in deiner Testvorbereitung verwenden.
