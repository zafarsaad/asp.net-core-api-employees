using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TheEmployeeAPI;

var employees = new List<Employee>
{
    new Employee { Id = 1, FirstName = "John", LastName = "Doe",
            Benefits = new List<EmployeeBenefits>
        {
            new EmployeeBenefits { BenefitType = BenefitType.Health, Cost = 100 },
            new EmployeeBenefits { BenefitType = BenefitType.Dental, Cost = 50 }
        } },
    new Employee { Id = 2, FirstName = "Jane", LastName = "Doe" }
};

var employeeRespository = new EmployeeRepository();
foreach (var e in employees)
{
    employeeRespository.Create(e);
}


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "TheEmployeeAPI.xml"));
});
// builder.Services.AddSingleton<IRepository<Employee>, EmployeeRepository>(); // previously we had factory instantiate type
builder.Services.AddSingleton<IRepository<Employee>>(employeeRespository);
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<FluentValidationFilter>();
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }