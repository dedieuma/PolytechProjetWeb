# TP 3

## Objectifs :

- Mettre en place une Base de Donnée avec EF Core
- Lier les opérations CRUD avec la base de donnée
- (Bonus) Ajouter au démarrage de l'appli une routine pour ajouter automatiquement des pokémons par défaut


Nous allons utiliser **Entity Framework Core**

Entity FrameWork Core est un outil implémentant l'Objet Relationship Mapping (ORM). On dit plus simplement que EF Core est un ORM.

Un ORM permet de faire un lien entre des objets dans le sens programmation orienté objet, et des entités dans le sens base de donnée. 
Ainsi, une `class` en dotnet peut facilement être convertie en une `table` SQL.

Enfin, cela élimine le besoin d'écrire à la main les requêtes SQL, l'outil ayant à sa disposition des moyens d'effectuer une requête à partir de directives propres à dotnet.

> ⚠️ Ne confondez pas `Entity Framework Core` de `Entity Framework`. De la même manière que Microsoft a voulu réécrire le .NET Framework vers .NET Core, les ingénieurs de Microsoft ont réécri Entity Framework (aussi appelé EF6) vers Entity Framework Core. Ils n'ont pas les mêmes designs et leur utilisation se fait différemment.

## (1) Mettre en place un DbContext

Démarrez un terminal à la racine du projet. Entrez les commandes

````
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
````

Dans le `Program.cs`, ajoutez la ligne 

````csharp
builder.Services.AddSwaggerGen(); // existant

builder.Services.AddDbContext<PokemonContext>(options => options.UseSqlite("Data Source=pokemons.db")); // à ajouter

var app = builder.Build(); // existant
````

`PokemonContext` ne compile pas, nous allons s'en occuper.

Créez un fichier `Repositories/PokemonContext.cs` : 

````csharp
using Microsoft.EntityFrameworkCore;

public class PokemonContext : DbContext
{
    public PokemonContext(DbContextOptions<PokemonContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Pokemon> Pokemons { get; set; } = default!;
}
````

> 💡 Un `DbContext` sert à faire une session avec la base de donnée. C'est par ce fichier que EF Core va faire des query et sauvegarder les instances des entités.

> 💡 `public DbSet<Pokemon> Pokemons` permet de déclarer à EF Core que notre Model `Pokemon.cs` et ses propriétés doit être mappé à une table SQL, qui aura (par défaut) le nom... Pokemons.

Ajoutons des données à notre Base, via EF Core.

Tout d'abord, rendons notre liste de Pokémons `static` pour pouvoir y accéder depuis l'extérieur de la classe.

`Services/PokemonsSources.cs` : 

````csharp
public static List<Pokemon> Pokemons = new List<Pokemon>
....
````

Revenez à `Repositories/PokemonContext.cs`, ajoutez :

````csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Pokemon>()
        .HasData(PokemonsSources.Pokemons);
}
````

# (2) Faire un GET pour récupérer les Pokémons de la BDD

Créons un nouveau controller, `Controllers/PokemonDbController.cs` :

````csharp
using Microsoft.AspNetCore.Mvc;

namespace PokeAPIPolytech.Controllers;

[ApiController]
[Route("[controller]")]
public class PokemonsDbController : ControllerBase
{
    private readonly ILogger<PokemonsController> _logger;
    private readonly IPokemonsDbSources _pokemonsDbSources;

    public PokemonsDbController(
        ILogger<PokemonsController> logger,
        IPokemonsDbSources pokemonsDbSources)
    {
        _logger = logger;
        _pokemonsDbSources = pokemonsDbSources;
    }

    [HttpGet("All")]
    public IEnumerable<Pokemon> GetAllPokemons()
    {
        return _pokemonsDbSources.GetAll();
    }
}
````

Créons le service `Services/IPokemonsDbSources.cs`

````csharp
public interface IPokemonsDbSources
{
    IEnumerable<Pokemon> GetAll();
}
````

Puis le service qui l'implémente, `Services/PokemonsDbSources.cs`

````csharp
public class PokemonsDbSources : IPokemonsDbSources
{
    public IEnumerable<Pokemon> GetAll()
    {
        throw new NotImplementedException();
    }
}
````

N'oublions pas de le rajouter dans le dictionaire d'injection de dépendances :

`Program.cs`

````csharp
builder.Services.AddSingleton<IPokemonsSources, PokemonsSources>(); //existant
builder.Services.AddScoped<IPokemonsDbSources, PokemonsDbSources>();
````

Le service est prêt, nous pouvons lui injecter notre DbContext et remplir la méthode `GetAll()`

````csharp
using Microsoft.EntityFrameworkCore;

public class PokemonsDbSources : IPokemonsDbSources
{
    private readonly PokemonContext _dbContext;

    public PokemonsDbSources(
        PokemonContext context
    )
    {
        this._dbContext = context;
    }

    public IEnumerable<Pokemon> GetAll()
    {
        return this._dbContext.Pokemons
            .FromSql($"SELECT * FROM Pokemons")
            .ToList();
    }
}
````


