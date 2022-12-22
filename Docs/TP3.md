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
        var data = PokemonsSources.Pokemons.Append(new Pokemon
        {
            Id = 10,
            Name = "Caterpie",
            Description = "Its short feet are tipped with suction pads that enable it to tirelessly climb slopes and walls.",
            Type = PokemonType.Bug,
            PictureUrl = "https://img.pokemondb.net/artwork/large/caterpie.jpg"
        });

    modelBuilder.Entity<Pokemon>()
        .HasData(data);
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

Lancez le service, et tentez de faire un `GetAll()` avec le bon controller.

Vous devriez avoir une erreur : 

````
Microsoft.Data.Sqlite.SqliteException (0x80004005): SQLite Error 1: 'no such table: Pokemons'.
   at Microsoft.Data.Sqlite.SqliteException.ThrowExceptionForRC(Int32 rc, sqlite3 db)

   ....
````

> ⚠️ Effectivement, il manque quelque chose... La base de donnée n'a pas été définie : toutes les commandes chiantes du style '`CREATE TABLE Pokemons`'... Pour le moment, nous n'avons que défini le lien EF Core entre la table SQL Pokemon et la classe .NET Pokemon... mais si la table n'existe pas nous n'allons pas aller loin.

## (3) Migration de la Base de donnée

EF Core vient avec un module dit de **migration** : il permet de faire évoluer le schéma de la base de donnée au cours du temps et du dévelopement.

De la même manière que nous avions défini comment doit être le lien entre classe .NET et SQL pour pouvoir faire des queries, EF Core permet de générer automatiquement le code permettant de définir le schéma d'une base de donnée, et de la faire évoluer dans le temps.

Tout d'abord, ajoutons un package nécessaire

> `dotnet add package Microsoft.EntityFrameworkCore.Design`

Il faut maintenant installer l'outil `dotnet ef` :

> `dotnet tool install --global dotnet-ef`

Vérifiez que l'outil est installé avec 

> `dotnet ef`

Pour créer notre première migration, il faut utiliser la commande

> `dotnet ef migrations add "Initial"`

Regardez dans vos dossiers : un nouveau dossier nommé Migrations a dû être créé.

3 Fichiers sont présents : 
- [date]_Initial.cs
- [date]_Initial.Designer.cs
- PokemonContextModelSnapshot.cs

Les deux derniers fichiers sont des fichiers internes à EF, oubliez les.

En revanche ouvrez le premier fichier : il contient deux méthodes `Up` et `Down`

> 💡 L'idée derrière ces méthodes est de pouvoir avancer ou reculer dans une suite de migrations : mettre à jour une base de donnée depuis le début jusqu'à une migration N va demander à EF d'exécuter toutes les méthodes `Up` jusqu'à la migration N ciblée (incluse).
> Dans l'autre sens, demander à EF de revenir en arrière vers une migration plus ancienne va éxécuter les commandes `Down`.

> 💡 Si on regarde dans le détail les méthodes, on voit bien qu'il y a des directives en dotnet `CreateTable` ou `DropTable`, ce qui va nous permettre de faire évoluer le schéma de notre BD !

> ⚠️ Il est fortement conseillé de ne pas toucher directement au code des fichiers sous le dossier `Migrations`, mais plutôt d'utiliser l'outil `dotnet ef`. Les fichiers sont du code généré.

On a notre migration, il faut encore l'appliquer à notre BD.

La commande à faire est 

> `dotnet ef database update`

Cela va mettre à jour la BD vers la migration la plus récente.

> 💡 Si on voulait cibler une migration partiulière, la commande aurait été `dotnet ef database update "MaMigration"` (sans la date, juste le nom)

> 💡 Pour cibler la première migration, et en particulier les `Down`, la commande est `dotnet ef database update 0`

Vous avez peut être remarqué la création d'un nouveau fichier : `pokemons.db` à la racine de votre projet. C'est votre base de donnée... Elle n'est pas lisible par un humain mais c'est dedans où les tables et les données sont définies.

> 💡 En temps normal, un projet professionnel utilise un vrai moteur de base de donnée, mais c'est complexe à mettre en place dans le cadre des TP Polytech.

Réessayez de relancer le service, et de requêter des nouveaux pokémons : cela devrait fonctionner !