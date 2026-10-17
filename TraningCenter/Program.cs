
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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

PaginationBestWay(context,1,5);

static void PaginationBestWay(AppDbContext context,int pageNumber, int pageSize){
    try
    {
        var query = context.Students
            .OrderBy(std => std.StudentId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        var result = query.ToList();

        Console.WriteLine("Page #{0} Data: ", pageNumber);
        foreach (var res in result){
            Console.WriteLine("Student Name: {0} {1} Id: {2}",res.FirstName, res.LastName, res.StudentId);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}


static void PriceGraterThanCourseAVG(AppDbContext context) {
    try {
        var query = context.Courses
           .Where(ces => ces.Price > context.Courses.Average(cs => cs.Price)) // sub query that be matched performance optimization
           .OrderBy(c => c.Price);
        var result = query.ToList();
        
        foreach (var res in result) {
            Console.WriteLine("Course #{0} Price: {1} Course Name: {2}",res.CourseId, res.Price, res.Title);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}


static void ShowStudentEnrollment(AppDbContext context) {
    try {
        //Inner join - SelectMany: show one collection of data rather than collection of collection where
        //in this way we avoid return unnecessary data that un used to hold memory management and performace
        var query = context.Students.SelectMany(
            std => std.Enrollment,//std.Enrollment: this way called : navigational property
            (std, enrt) => new {
                std.StudentId,
                StudentName = std.FirstName + " " + std.LastName,
                enrt.EnrollmentId,
                enrt.CourseId
            }
        ).OrderBy(std => std.StudentId);

        var result = query.ToList();

        foreach (var res in result) {
            Console.WriteLine("student #{0} name: {1} Enrollment Id: {2} Course Id: {3}\n", res.StudentId, res.StudentName, res.EnrollmentId, res.CourseId);
        }
    }
    catch (Exception ex) {
        Console.WriteLine(ex.Message);
    }
}

static void ShowStudentWithProjectInfo(AppDbContext context) {
    try {
        //we use left join when we want to show optional data
        //we build new table like student profile we want to handle optional-data
        var query =
            from std in context.Students
            join profile in context.StudentProfiles
            on std.StudentId equals profile.StudentId
            into ProfileGroupData // it will hold matched data into group called -> ProfileGroupData
            from p in ProfileGroupData.DefaultIfEmpty() // if there exist data it will return it if not it will return null
            select new {
                std.StudentId,
                StudentName = std.FirstName + " " + std.LastName,
                City = p.City != null ? p.City : "no city is added",
                County = p.Country != null ? p.Country : "no country is added"
            };

        var result = query.OrderBy(std => std.StudentId);

        foreach (var res in result) {
            Console.WriteLine("student #{0} name: {1} city: {2}", res.StudentId, res.StudentName, res.City,res.County);
        }
    }
    catch (Exception ex) {
        Console.WriteLine(ex.Message);
    }
}

static void ShowInstructorOFCourses(AppDbContext context) {

    try {
        //Inner Join Way [ beter than way from include to show specific data ]
        //Inner join between courses and instructors

        var query = context.Courses.Join(
            context.Instructors,
            course => course.InstructorId,
            instrcutor => instrcutor.InstructorId,//Don't do course.CourseId == instructor.InstructorId
            (course, instrcutor) => new {
                course.Title,
                course.Code,
                InstrcutorFullName = instrcutor.FirstName + " " + instrcutor.LastName
            }).OrderBy(cse => cse.Title);

        var result = query.ToList();

        foreach (var res in result){
            Console.WriteLine("Course: {0} instructor name: {1} code: {2}",res.Title,res.InstrcutorFullName, res.Code);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }
}

static void ShowCoursesOFStudent(AppDbContext context){
    try {

        var query = context.Students
            .Include(std => std.Enrollment)
            .ThenInclude(crs => crs.Course)
            .OrderBy(res => res.StudentId); 

        var students = query.ToList();

        foreach (var std in students) {
            Console.WriteLine("\n\nName: {1} {2} - Student ID: {0}",std.StudentId, std.FirstName, std.LastName);
            foreach (var ent in std.Enrollment) {
                Console.WriteLine("Course: {0} - Course Status: {1} - Progress Percent: {2}%", ent.Course.Title, ent.Course.Status , ent.ProgressPercent);
            }            
        }


    } catch (Exception ex){
        Console.WriteLine(ex.Message);
    }
}

static void ShowInfoBESTWAY(AppDbContext context)
{
    try
    {
        //Projection Way [ beter than way from include to show specific data ]
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
        // we use include when we want to see al columns + all data but when we want to see part of data - we want to use projection way
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

static void ShowAll(AppDbContext context) {
    LoggerExample(context);
    ShowStudentInfo(context);
    ShowCoursesOFStudent(context);
    ShowInfoBESTWAY(context);
    ShowRelatedInfo(context);
    GetStudentStatus(context);
    TotalStudent(context);
    IsActiveStudent(context);
    QueryChain(context);
    ProjectionGettingData(context);
    FindStudentByID(context);
    GetStudentDataFirstOrDefault(context);
    GetActiveStudent(context);
    ShowInstructorOFCourses(context);
    ShowStudentWithProjectInfo(context);
    ShowAll(context);
    ShowStudentEnrollment(context);
    PriceGraterThanCourseAVG(context);
}