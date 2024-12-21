using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace TheEmployeeAPI.Tests;

public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private int _employeeIdForAddressTest;

    public BasicTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;

        var repo = _factory.Services.GetRequiredService<IRepository<Employee>>();
        var employee = new Employee { FirstName = "John", LastName = "Doe", Address1 = "123 Main Street" };
        repo.Create(employee);
        _employeeIdForAddressTest = repo.GetAll().First().Id;
    }

    [Fact]
    public async Task GetAllEmployees_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/employees");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetEmployeeById_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        // var response = await client.GetAsync($"/employees/{_employeeIdForAddressTest}"); // may not be needed here
        var response = await client.GetAsync("/employees/1");

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateEmployee_ReturnsCreatedResult()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsJsonAsync("/employees", new Employee { FirstName = "Johhn", LastName = "Does", SocialSecurityNumber = "123-34-5555" });

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateEmployee_ReturnsBadRequestResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidEmployee = new CreateEmployeeRequest(); // Empty object to trigger validation errors

        // Act
        var response = await client.PostAsJsonAsync("/employees", invalidEmployee);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("FirstName", problemDetails.Errors.Keys);
        Assert.Contains("LastName", problemDetails.Errors.Keys);
        Assert.Contains("First name is required.", problemDetails.Errors["FirstName"]);
        Assert.Contains("'Last Name' must not be empty.", problemDetails.Errors["LastName"]);
    }

    [Fact]
    public async Task UpdateEmployee_ReturnsOkResult()
    {
        var client = _factory.CreateClient();
        var response = await client.PutAsJsonAsync("/employees/1", new Employee
        {
            FirstName = "John",
            LastName = "Doe",
            Address1 = "123 Main St"
        });

        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task UpdateEmployee_ReturnsBadRequestWhenAddress()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidEmployee = new UpdateEmployeeRequest(); // Empty object to trigger validation errors

        // Act
        var response = await client.PutAsJsonAsync($"/employees/{_employeeIdForAddressTest}", invalidEmployee);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("Address1", problemDetails.Errors.Keys);
    }

    // Currently not handling this, returning 500 with our custom

    // [Fact]
    // public async Task UpdateEmployee_ReturnsNotFoundForNonExistantEmployee()
    // {
    //     var client = _factory.CreateClient();
    //     var response = await client.PutAsJsonAsync("/employees/9999", new Employee { FirstName = "Jasohn", LastName = "Doe", SocialSecurityNumber = "123-34-5555" });

    //     Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    // }
}