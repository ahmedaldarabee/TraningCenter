
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TraningCenter.Data;

//build configuration
IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// reading connection string 
string? connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString)){
    Console.WriteLine("Opps, connections with database is fields!");
    return;
}

//configuration of EF Core now...
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlServer(connectionString)
    .Options;

using var context = new AppDbContext(options);

//test result

try {
    var connectionResult = context.Database.CanConnect();
    if (connectionResult) {
        Console.WriteLine("Connection Sql Server has been successfully");
    }
    else {
        Console.WriteLine("Opps, there are some error connections with your database");
    }
}catch(Exception ex){
    Console.WriteLine("Error be as: {0}", ex.Message);
}