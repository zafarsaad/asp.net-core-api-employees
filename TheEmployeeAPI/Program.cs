using Microsoft.AspNetCore.Mvc;

var employees = new List<Employee>
{
    new Employee { Id = 1, FirstName = "John", LastName = "Doe", SocialSecurityNumber = "110-20-3030" },
    new Employee { Id = 2, FirstName = "Jane", LastName = "Doe", SocialSecurityNumber = "110-20-3031" }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var employeeRoute = app.MapGroup("/employees");

employeeRoute.MapGet(string.Empty, () =>
{
    foreach (var employee in employees)
    {
        employee.SocialSecurityNumber = null; // We don't want to return the SSN
    }
    return employees;
});

employeeRoute.MapGet("{id:int}", (int id) =>
{
    var employee = employees.SingleOrDefault(e => e.Id == id);
    if (employee == null)
    {
        return Results.NotFound();
    }
    employee.SocialSecurityNumber = null; // We don't want to return the SSN
    return Results.Ok(employee);
});

employeeRoute.MapPut("{id}", ([FromBody] Employee employee, int id) =>
{
    var existingEmployee = employees.SingleOrDefault(e => e.Id == id);
    if (existingEmployee == null)
    {
        return Results.NotFound();
    }

    // existingEmployee.FirstName = employee.FirstName;
    // existingEmployee.LastName = employee.LastName;
    existingEmployee.Address1 = employee.Address1;
    existingEmployee.Address2 = employee.Address2;
    existingEmployee.City = employee.City;
    existingEmployee.State = employee.State;
    existingEmployee.ZipCode = employee.ZipCode;
    existingEmployee.PhoneNumber = employee.PhoneNumber;
    existingEmployee.Email = employee.Email;

    return Results.Ok(existingEmployee);
});

employeeRoute.MapPost(string.Empty, (Employee employee) =>
{
    employee.Id = employees.Max(e => e.Id) + 1; // We're not using a database, so we need to manually assign an ID
    employees.Add(employee);
    return Results.Created($"/employees/{employee.Id}", employee);
});

app.UseHttpsRedirection();

app.Run();

public partial class Program { }