using Xunit.Abstractions;

namespace Coatify.Tests;

public class DatabaseConnectionTests
{
    
    private readonly ITestOutputHelper _output;
    public DatabaseConnectionTests(ITestOutputHelper output)
    {
        _output = output;
    }
    [Fact]
    public async Task VerbindungZuPostgresFunktioniert()
    {
        string result= await new TestDatabase().ConnectToDatabase();
        _output.WriteLine(result);
    }
   
  
    
}