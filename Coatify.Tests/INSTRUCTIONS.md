1.
Verbindung herstellen. Erstelle in DbContext.cs eine Hilfsklasse, zum Beispiel TestDatabase – nenne sie nicht DbContext, denn so heißt bereits die EF-Klasse. Verbinde dich zunächst mit PostgreSQL auf 127.0.0.1:1234. Ein erstes Erfolgskriterium ist: OpenAsync() läuft ohne Fehler.
2.
Eigene Datenbank sicherstellen. Prüfe, ob coatify_test existiert, und erstelle sie nur, falls sie fehlt. Verwende coatify nicht für Tests. Hinweis: Von unserem vorherigen Testlauf kann coatify_test auf deinem Rechner bereits existieren; „existiert schon“ ist also ein erwarteter Fall.
3.
CoatifyContext einrichten. Erzeuge DbContextOptions<CoatifyContext> mit UseNpgsql(...) und der Verbindung zu coatify_test. Übergib diese Optionen an new CoatifyContext(options). Du schreibst damit keinen zweiten EF-Context, sondern verwendest deine vorhandene Klasse.
4.
Tabellen anlegen. Rufe auf dem Context context.Database.MigrateAsync() auf. Das führt die Migrationen aus dem Anwendungsprojekt aus.
5.
Ein Testgerät speichern. Lege zuerst einen DeviceType, dann ein Device mit fester Guid an und rufe SaveChangesAsync() auf. Denke beim zweiten Lauf daran, vorhandene Einträge zu erkennen, damit du sie nicht doppelt einfügst.
6.
GetDeviceTest vervollständigen. Entferne Skip, rufe GetDevice mit der festen ID auf und prüfe mit Assert.Equal zuerst nur den Namen. Danach kannst du ID und Status ergänzen. bash Coatify.Tests/run-postgres-tests.sh startet weiterhin PostgreSQL und xUnit. Das Skript erstellt die Testdatenbank jetzt nicht für dich – genau diesen Teil baust du in C#. Der erste gute Zwischenstand ist Schritt 1: eine erfolgreiche Verbindung, noch ohne Fixture, Migration oder Testdaten.
