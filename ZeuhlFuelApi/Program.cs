var builder = WebApplication.CreateBuilder(args);

// Add CORS policy to allow requests from any origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Replace with your Angular app's URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Later, use the following terminal commands with redirection:
// dotnet dev-certs https --trust
// dotnet run --launch-profile "https"
// app.UseHttpsRedirection();

// Use the defined CORS policy
app.UseCors("AllowAngular");

// Hello world endpoint
app.MapGet("/", () => new { message = "Hello from ASP.NET Core Web API!" })
   .WithName("GetHello");

app.Run();
