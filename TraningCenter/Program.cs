
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TraningCenter.Data;
using TraningCenter.Entity;

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
    .LogTo(Console.WriteLine,LogLevel.Information)
    .EnableSensitiveDataLogging()
    .Options;

using var context = new AppDbContext(options);

//test result

try {
    var connectionResult = context.Database.CanConnect();
    if (connectionResult) {
        Console.WriteLine("\nConnection Sql Server has been successfully\n");
    }
    else {
        Console.WriteLine("Opps, there are some error connections with your database");
    }
}catch(Exception ex){
    Console.WriteLine("Error be as: {0}", ex.Message);
}

ShowInfoBESTWAY(context);

static void ShowInfoBESTWAY(AppDbContext context)
{
    try
    {
        //Projection Way [ Best way to show specific data ]
        var query = context.Students.Select(std => new {
            std.FirstName,
            std.LastName,
            TotalCourses = std.Enrollment.Count
        });
        var result = query.ToList();
        Console.WriteLine("\nBest way to show data: \n");
        foreach(var std in result) {
            Console.WriteLine("Name: {0} {1} and total number of courses: {2} ",std.FirstName, std.LastName,std.TotalCourses);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void ShowRelatedInfo(AppDbContext context) {
    try {
        //We want to know student name with number of enrollment courses !
        var query = context.Students.Include(std => std.Enrollment); // left join
        var students = query.ToList();
        Console.WriteLine("\ncurrent status\n");
        foreach (var student in students) {
            Console.WriteLine("Full Name: {0} {1} - count: {2}", student.FirstName, student.LastName,student.Enrollment.Count);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}


static void GetStudentStatus(AppDbContext context){
    try {
        var query = context.Students
            .Select(std => std.Status)
            .Distinct();

        // group by query and query chain
        var qry = context.Students.
            GroupBy(std => std.Status)
            .Where(std => std.Count() > 6)
            .Select(res => new {
                Status = res.Key,
                Total = res.Count()
            })
            .Distinct()
            .OrderByDescending(std => std.Status);


        var students = qry.ToList();

        Console.WriteLine("\ncurrent status\n");
        foreach (var student in students) {
            Console.WriteLine("Status: {0} - total status of it: {1} ",student.Status, student.Total);
        }
        
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}


//be as a good way [  when you want to use count  ] rather bad way where about get all student list then do count
static void TotalStudent(AppDbContext context) {
    try {
        var query = context.Students.Where(std => std.Status == "Active");
        var total = query.Count();
        Console.WriteLine("Total student: {0}",total);
    }
    catch (Exception ex) {
        Console.WriteLine(ex.Message);
    }
}


static void IsActiveStudent(AppDbContext context) {
    try
    {
        var query = context.Students
            .Where(std => std.Status == "Active");

        var check = query.Any();
        //Console.WriteLine(student);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void QueryChain(AppDbContext context){
    try
    {
        var std = context.Students
            .Where(std => std.Status == "Active")
            .Select(std => new { std.FirstName, std.StudentId,std.PhoneNumber })
            .OrderByDescending(std => std.StudentId);

        var students = std.ToList();

        Console.WriteLine("getting all student info\n");
        foreach (var student in students) {
            Console.WriteLine("{0} - {1} - {2}", student.StudentId, student.FirstName, student.PhoneNumber);
        }

    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void ProjectionGettingData(AppDbContext context){
    try
    {
        var std = context.Students.Select(std => new {
            std.FirstName,std.PhoneNumber,std.StudentId
        });

        var students = std.ToList();

        if (std == null) {
            Console.WriteLine("Sorry, there are no students with this requirements");
            return;
        }
        else {
            Console.WriteLine("getting all student info\n");
            foreach (var student in students){
                Console.WriteLine("{0} - {1} - {2}", student.StudentId, student.FirstName,student.PhoneNumber);
            }
        }
    }
    catch (Exception ex){
        Console.WriteLine(ex.Message);
    }
}

static void FindStudentByID(AppDbContext context){
    try
    {
        var std = context.Students.Find(1);
        if (std == null)
        {
            Console.WriteLine("Sorry, there are no students with this requirements");
            return;
        }
        else
        {
            Console.WriteLine("Student info: ID: {0} - Full Name: {1}  {2} - Email: {3} ", std.StudentId, std.FirstName, std.LastName, std.Email);
        }
    }
    catch (Exception ex){
        Console.WriteLine(ex.Message);
    }
}

static void GetStudentDataFirstOrDefault(AppDbContext context)
{
    try {
        var std = context.Students.FirstOrDefault(std => std.FirstName.Length >= 5);
        if (std == null){
            Console.WriteLine("Sorry, there are no students with this requirements");
            return;
        }
        else {
            Console.WriteLine("Student info: ID: {0} - Full Name: {1}  {2} - Email: {3} ", std.StudentId, std.FirstName, std.LastName, std.Email);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void GetActiveStudent(AppDbContext context){
    try
    {
        var query = context.Students.
            Where(std => std.Status == "Active")
           .OrderBy(std => std.StudentId);

        var students = query.ToList();

        Console.WriteLine("Active Students: \n");
        foreach (var student in students){
            Console.WriteLine("Id: {0} - Full Name: {1} {2} - Is Active: {3}" , student.StudentId, student.FirstName , student.LastName, student.Status);
        }
        Console.WriteLine("\nTotal Active Student: {0} \n", students.Count);
    }
    catch (Exception ex){
        Console.WriteLine(ex.Message);
    }
}

static void ShowStudentInfo(AppDbContext context)
{
    try
    {
        var student = context.Students.OrderBy(s => s.StudentId);
        GeneratedSQLQuery("Student", student.ToQueryString());//15ms

        var students = student.ToList();

        if (students.Count() == 0 ){
            Console.WriteLine("There are no students, you can add one !");
            return;
        }

        Console.WriteLine("Student Info:\n");
        foreach (var info in students){
            Console.WriteLine("Id: {0}, Name: {1} {2} Email: {3} register at: {4} date of birth: {5} current status: {6} phone number: {7}",info.StudentId,info.FirstName,info.LastName,info.Email,info.RegisteredAt,info.DateOfBirth, info.Status, info.PhoneNumber);
        }

        Console.WriteLine("\nTotal student: {0}",students.Count());
    }
    catch (Exception ex){
        Console.WriteLine(ex.Message);
    }
}

static void LoggerExample(AppDbContext context){

    try
    {

        var query = context.Students.
            Where(std => std.Status == "Active")
           .OrderBy(std => std.StudentId);

        Console.WriteLine("\nquery to string - logger - result to show main executed query: {0}\n", query.ToQueryString()); //2ms

        var students = query.ToList();
        Console.WriteLine("Returned Rows: {0}", students.Count());
    }
    catch (Exception ex){
        Console.WriteLine(ex.Message);
    }
}

static void GeneratedSQLQuery(string tableName, string query){
    Console.WriteLine("\nsql query for {0}\n",tableName);
    Console.WriteLine("query be as: {0}\n",query);
}
