using coatify.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace Coatify.Tests;
using Npgsql;
// TODO: Hier kannst du die Vorbereitung deiner PostgreSQL-Testdatenbank bauen.
// Verwende den vorhandenen CoatifyContext aus dem Anwendungsprojekt.


public class TestDatabase
{
    
    private string _connectionString= "Host=127.0.0.1;Port=1234;Database=coatify_test;Username=coatify;Password=coatify";
    
    
  
    public async Task<string> ConnectToDatabase()
    {
        
        await using var connection = CreateContext();
       var canConnect = await connection.Database.CanConnectAsync();
       if (canConnect)
       {
              return "Verbindung zur Testdatenbank erfolgreich.";
       }
        
       await connection.Database.MigrateAsync();
       return "Verbindung zur Testdatenbank nicht da - Datenbank wurde erstellt.";


      
    
       
        
       // Hier kannst du deine Testdatenbank vorbereiten, z.B. Tabellen erstellen, Testdaten einfügen usw.
    }

    public  CoatifyContext CreateContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<CoatifyContext>();
        optionsBuilder.UseNpgsql(_connectionString);

        return new CoatifyContext(optionsBuilder.Options);
    }

    public async Task CreateTestDatabase()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        // Beispiel: Erstellen einer Testtabelle
        var createTableCommand = new NpgsqlCommand(
            "CREATE TABLE IF NOT EXISTS test_table (id SERIAL PRIMARY KEY, name TEXT);", connection);
        await createTableCommand.ExecuteNonQueryAsync();

        // Beispiel: Einfügen von Testdaten
        var insertDataCommand = new NpgsqlCommand(
            "INSERT INTO test_table (name) VALUES ('Test1'), ('Test2'), ('Test3');", connection);
        await insertDataCommand.ExecuteNonQueryAsync();
    }
}
