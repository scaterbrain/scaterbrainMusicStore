using ScatterbrainMusic.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Scatterbrain Music API",
        Version = "v1",
        Description = "Backend API for Scatterbrain Music — Gear · Vinyl · Community"
    });
});

// Register services
builder.Services.AddSingleton<IProductService, ProductService>();
builder.Services.AddSingleton<ICartService, CartService>();

// CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Scatterbrain Music API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Needed for integration tests
public partial class Program { }
