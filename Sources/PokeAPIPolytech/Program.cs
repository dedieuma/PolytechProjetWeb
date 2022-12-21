using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PokemonContext>(options => options.UseSqlite("pokemons.db"));

builder.Services.AddSingleton<IPokemonsSources, PokemonsSources>();
builder.Services.AddScoped<IPokeApi, PokeApi>();

builder.Services.AddHttpClient();

builder.Services.AddCors();


//builder.Services.AddDbContext<>(options => options.UseSqlite(builder.Configuration.GetConnectionString("dbConnectionString")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(
    options => options
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
