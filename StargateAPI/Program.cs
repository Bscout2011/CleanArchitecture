using Microsoft.EntityFrameworkCore;
using StargateAPI.Business.Commands;
using StargateAPI.Business.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowLocalhost4200",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        }
    );
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StargateContext>(
    options => options.UseSqlite(builder.Configuration.GetConnectionString("StarbaseApiDatabase"))
);

builder.Services.AddMediatR(cfg =>
{
    cfg.AddRequestPreProcessor<CreateAstronautDutyPreProcessor>();
    cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly);
});

var app = builder.Build();

// Use CORS policy
app.UseCors("AllowLocalhost4200");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
