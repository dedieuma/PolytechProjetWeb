# TP 3

## Objectifs :

- Mettre en place une Base de Données avec EF Core
- Lier les opérations CRUD avec la base de données

Nous allons utiliser **Entity Framework Core**.

Entity FrameWork Core est un outil implémentant l'Objet Relationship Mapping (ORM). On dit plus simplement que EF Core est un ORM.

Un ORM permet de faire un lien entre des objets dans le sens programmation orienté objet, et des entités dans le sens base de données. 
Ainsi, une `class` en dotnet peut facilement être convertie en une `table` SQL.

![](img/efcore.jfif)

Enfin, cela élimine le besoin d'écrire à la main les requêtes SQL, l'outil ayant à sa disposition des moyens d'effectuer une requête à partir de directives propres à dotnet.

> ⚠️ Ne confondez pas `Entity Framework Core` de `Entity Framework`. De la même manière que Microsoft a voulu réécrire le .NET Framework vers .NET Core, les ingénieurs de Microsoft ont réécri Entity Framework (aussi appelé EF6) vers Entity Framework Core. Entity Framework Core est la nouvelle version d'Entity Framework après EF6. Ils n'ont pas les mêmes designs et leur utilisation se fait différemment.

## (1) Mettre en place un DbContext

Démarrez un terminal à la racine du projet. 

Installer le package `EntityFrameworkCore` à l'aide de la commande suivante : 

````
dotnet add package Microsoft.EntityFrameworkCore
````
Installer ensuite le package `Sqlite`, un léger fournisseur de base de données.
```
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Dans le `Program.cs`, enregistrer un contexte de base de données. Ajoutez la ligne 

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

> 💡 Un `DbContext` sert à faire une session avec la base de données. C'est par ce fichier que EF Core va faire des query et sauvegarder les instances des entités.

> 💡 `public DbSet<Pokemon> Pokemons` permet de déclarer à EF Core que notre Model `Pokemon.cs` et ses propriétés doivent être mappés à une table SQL, qui aura (par défaut) le nom... Pokemons.

Ajoutons des données à notre (future) Base, via EF Core.

Tout d'abord, rendons notre liste de Pokémons `static` pour pouvoir y accéder depuis l'extérieur de la classe. 

> 💡 Pour rappel, quand une variable est rendue `static`, une seule et même copie de cette variable est créée. Les variables `static` sont accédées avec le nom de la classe, et donc une instance de la classe n'est pas requise. Par exemple, la variable static Pokemons sera accessible comme ceci : `PokemonsSources.Pokemons`   

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

> 💡 La méthode `OnModelCreating(ModelBuilder modelBuilder)` est utilisée par le framework EF Core. A un moment donné dans l'éxécution, le framework appelle cette méthode, et éxécute les directives qui y sont présentes. Par défaut, la méthode ne contient rien, mais on peut l'`override` pour indiquer que des opérations sont à faire. 

> 💡 Ce genre de comportement d'une méthode, de pouvoir surcharger un comportement mais que cela reste optionnel, est caractéristique d'une méthode avec le mot clé `virtual` : c'est une méthode qui possède un comportement par défaut, mais qui peut être réécrit par la classe qui étend (définition exposée, implémentation requise). Les autres mot-clés de comportement sont `abstract` ou encore `interface`.

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

Le service est prêt, nous pouvons lui injecter notre `DbContext` et remplir la méthode `GetAll()`

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
            .FromSqlRaw($"SELECT * FROM Pokemons")
            .ToList();
    }
}
````

Lancez le service, et tentez de faire un `GetAll()` avec le controller `PokemonsDbController`.

Vous devriez avoir une erreur : 

````
Microsoft.Data.Sqlite.SqliteException (0x80004005): SQLite Error 1: 'no such table: Pokemons'.
   at Microsoft.Data.Sqlite.SqliteException.ThrowExceptionForRC(Int32 rc, sqlite3 db)

   ....
````

> ⚠️ Effectivement, pour le moment, nous n'avons fait que définir le lien EF Core entre la table SQL Pokemon et la classe .NET `Pokemon`. La table `Pokemons` elle, n'a pas encore été créée. Pour comparer, il manque les directives SQL `CREATE TABLE Pokemons`...

## (3) Migration de la Base de données

EF Core vient avec un module dit de **migration** : il permet de faire évoluer le schéma de la base de données au cours du temps et du développement.

De la même manière que nous avions défini comment doit être le lien entre classe .NET et SQL pour pouvoir faire des requêtes, EF Core permet de générer automatiquement le code permettant de définir le schéma d'une base de données, et de la faire évoluer dans le temps.

![](img/ef%20core%20migrations.png)

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

Les deux derniers fichiers sont des fichiers internes à EF, oubliez-les.

En revanche ouvrez le premier fichier : il contient deux méthodes `Up` et `Down`

> 💡 L'idée derrière ces méthodes est de pouvoir avancer ou reculer dans une suite de migrations : mettre à jour une base de données depuis le début jusqu'à une migration N va demander à EF d'exécuter toutes les méthodes `Up` jusqu'à la migration N ciblée (incluse).
> Dans l'autre sens, demander à EF de revenir en arrière vers une migration plus ancienne va exécuter les commandes `Down`.

> 💡 Si on regarde dans le détail les méthodes, on voit bien qu'il y a des directives en dotnet `CreateTable` ou `DropTable`, ce qui va nous permettre de faire évoluer le schéma de notre BD !

> ⚠️ Il est fortement conseillé de ne pas toucher directement au code des fichiers sous le dossier `Migrations`, mais plutôt d'utiliser l'outil `dotnet ef`. Les fichiers sont du code généré.

Nous avons notre script de migration, appliquons-le à notre BD. La commande à exécuter est la suivante : 

> `dotnet ef database update`

Cela va mettre à jour la BD vers la migration la plus récente.

> 💡 Si on voulait cibler une migration partiulière, la commande aurait été `dotnet ef database update "MaMigration"` (sans la date, juste le nom)

> 💡 Pour cibler la première migration, et en particulier les `Down`, la commande est `dotnet ef database update 0`

Vous avez peut être remarqué la création d'un nouveau fichier : `pokemons.db` à la racine de votre projet. C'est votre base de données... Elle n'est pas lisible par un humain mais c'est dedans où les tables et les données sont définies.

> 💡 En temps normal, un projet professionnel utilise un vrai moteur de base de données, mais c'est complexe à mettre en place dans le cadre des TP Polytech.

Réessayez de relancer le service, et de requêter des nouveaux pokémons : cela devrait fonctionner !

## (4) Insérer un nouveau pokémon en Base de données

Créez un nouvel endpoint avec le verbe POST dans `PokemonsDbController.cs`.

Faites en sorte qu'il appelle la méthode `Insert()` de `PokemonsDbSources.cs`

`PokemonsDbSources.cs` : 

````csharp
public Pokemon Insert(CreatePokemonDto dto)
{
    var pokemon = new Pokemon
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        PictureUrl = dto.PictureUrl,
        Type = dto.Type
    };

    var query = "INSERT INTO Pokemons (Id, Description, Name, PictureUrl, Type) VALUES ('"+pokemon.Id+"', '"+pokemon.Description+"', '"+pokemon.Name+"', '"+pokemon.PictureUrl+"', '"+pokemon.PictureUrl+"')";

    this._dbContext.Pokemons
        .FromSqlRaw(query)
        .ToList();

    return pokemon;
}
````

Testez pour voir si ça fonctionne.

## (5) Récupérer un pokémon par nom

Créez un nouvel endpoint dans `PokemonsDbController.cs`, qui permettra de récupérer un pokémon par nom.

Puis dans `PokemonsDbSources.cs` : 

````csharp
public Pokemon GetByName(string name)
{
    var query = "SELECT * FROM Pokemons WHERE Name = '"+name+"'";

    return this._dbContext.Pokemons
        .FromSqlRaw(query)
        .ToList()
        .FirstOrDefault();
}
````

## (6) Relation Many to Many

Un pokémon possède une liste d'attaques.
Une attaque peut être apprise par plusieurs pokémons différents.

Nous sommes dans le cas d'une relation many-to-many (ou N-to-N). Nous pouvons définir cette relation dans EF Core, et une migration fera le reste dans le schéma de la base de données.

Créons une nouvelle entité, `Models/Ability.cs`

````csharp
namespace Models;

public class Ability
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Pokemon> Pokemons { get; set; }
}
````

> 💡 Ici, le mot clé `virtual` permet à la collection `Pokemons` d'être substitué dans une classe dérivant de Ability. 

> 💡 Notez que le mot clé `virtual` n'est plus requis dans les versions récentes de EF Core. Cependant, je préfère le garder, car cela montre à la future personne relisant le code que cette `ICollection` est en réalité une liste de Pokémons venant d'une autre table SQL. Ability ne possède pas dans sa table une liste de Pokémons.

Ajoutons une ligne dans `Pokemons.cs`

````csharp
    public virtual ICollection<Ability> Abilities { get; set; }
````

Rajoutons un DbSet dans `PokemonContext.cs`

````csharp
    public DbSet<Ability> Abilities { get; set; } = default!;
````

Ajoutez dans le `OnModelCreating()`

````csharp
var dataAbilities = new List<Ability>{
    new Ability{
        Id = 1,
        Name = "shield-dust"
    }
};

modelBuilder.Entity<Ability>()
    .HasData(dataAbilities);

modelBuilder.Entity<Pokemon>()
    .HasMany(pokemon => pokemon.Abilities)
    .WithMany(ab => ab.Pokemons)
    .UsingEntity(abPok => abPok
        .HasData(new 
            { PokemonsId = 10, AbilitiesId = 1 }
        )
    );
           
````
> 💡 Cela ajoute la compétence "shield-dust" au pokémon "Caterpie".

> 💡 Une relation Many-to-Many utilise une table d'association entre deux entités. C'est toujours le cas ici, mais EF Core nous le cache. Le `.UsingEntity(abPok => abPok...` configure des données que possédera la table d'association.

Ptit `dotnet ef migrations add "Abilities"` into `dotnet ef database update`

(N'hésitez pas à aller voir du côté de la migration créée...)

## (7) GET Abilities

Créez une méthode GET pour récupérer toutes les Abilities de la base de données

> 💡 Il est judicieux de modifier les méthodes GET existantes, par exemple de `[HttpGet("All")]` vers `[HttpGet("Pokemons/All")]`

> Vous avez probablement `null` en résultat sur la liste des pokémons, nous allons nous en occuper plus tard...

## (8) Et si on tentait un truc

Lancez votre service.

Sur l'endpoint permettant de récupérer un pokémon par nom, dans le champ demandé par swagger, au lieu de mettre votre nom ciblé, mettez plutôt

> `Bulbasaur';drop table Abilities--`

Maintenant, refaites en Get All Abilities.

**Que s'est-t'il passé ?**

Pour réparer, allez dans le fichier de migration `[date]_Abilities.cs`, commentez la ligne 

````csharp
            migrationBuilder.DropTable(
                name: "Abilities");
````

Effectuez

> `dotnet ef database update "Initial"`

Décommentez la ligne, puis

> `dotnet ef database update`

## (9) Utilisation de Linq-to-SQL au lieu de SQL brut

Jusqu'à présent, nous avons effectué des requêtes SQL Pur pour faire nos manipulations. 

Non seulement c'est laborieux à écrire, mais c'est aussi soumis à des bugs et des failles de sécurité catastrophiques.

Heureusement, EF Core est encore là pour nous (que ferions-nous sans lui ?). 

EF Core propose ce que l'on appelle des projections "Linq To SQL". Le principe est d'utiliser des directives Linq sur notre DbContext pour faire nos query SQL. EF Core se chargera de convertir la directive Linq en requête SQL, nous enlevant la charge de rédiger du SQL.

> 💡 Linq ('Link' ou 'Lin-kiou') est une librairie officielle permettant de manipuler des listes au sens large. Or, un résultat d'une query SQL reste une sorte de liste (nom officiel : `IQueryable`) : une requête `SELECT` renvoie une liste de lignes d'une table.

> 💡 Il s'utilise de cette manière : `maListe.MonOpérationLinq()`. Consultez <https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.firstordefault?view=net-7.0>

> 💡 EF Core contient des providers Linq-To-SQL pour tous les moteurs de base de données populaires. Aussi, il propose des providers pour des bases de données non-relationnelles. Ainsi, vous pouvez aussi utiliser EF Core pour manipuler des collections Mongo, par exemple.

Nous allons réécrire nos query SQL brut en Linq-to-SQL.

> Si vous le souhaitez, vous pouvez renommer les méthodes actuelles en autre chose, si vous voulez garder l'ancienne version.

`PokemonsDbSources.cs` : 

````csharp
public IEnumerable<Pokemon> GetAll()
{
    return this._dbContext.Pokemons
        .ToList();
}

public IEnumerable<Ability> GetAllAbilities()
{
    return this._dbContext.Abilities
        .ToList();
}

public Pokemon GetByName(string name)
{
    return this._dbContext.Pokemons
        .FirstOrDefault(pokemon => pokemon.Name.Equals(name));
}
````

... c'est tout pour les `SELECT` !

Si vous regardez dans la console lors des appels aux endpoints, on voit quelles sont les requêtes effectuées par EF Core.

Pour la méthode `Insert()`, c'est un peu différent :

````csharp
public Pokemon Insert(CreatePokemonDto dto)
{
    var pokemon = new Pokemon
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        PictureUrl = dto.PictureUrl,
        Type = dto.Type
    };

    this._dbContext.Pokemons
        .Add(pokemon);

    this._dbContext.SaveChanges();

    return pokemon;
}
````

La méthode `this._dbContext.SaveChanges();` est importante, et est en lien avec un paradigme de EF Core. Faire `this._dbContext.Pokemons.Add(pokemon);` marque l'entité `pokemon` comme étant "à ajouter" par EF Core. Tant que `this._dbContext.SaveChanges();` n'est pas appelé, l'entité ne sera pas sauvegardé dans la base.

C'est un équivalent aux commit des bases de données.

## (10) Includes

Lors du GetAllPokemons(), la liste des Abilities reste toujours à null.

C'est dû au fait que les entités Pokémons et Abilities se situent dans deux tables SQL différentes... ce qui veut en théorie dire qu'il faut requêter deux fois la base de données, une pour la table Pokémon, et une pour la table Abilities. Et c'est sans compter les tables d'associations, qui permettent de faire du Many-to-Many !

C'est pour cela que, par défaut, les requêtes Linq-to-SQL ne vont pas aller chercher les données d'autres tables que celle visée de base. Cependant, on peut indiquer que l'on veut faire un lien avec une autre table.

Modifions les méthodes : 

````csharp
public IEnumerable<Pokemon> GetAll()
{
    return this._dbContext.Pokemons
        .Include(pokemon => pokemon.Abilities)
        .ToList();
}

public IEnumerable<Ability> GetAllAbilities()
{
    return this._dbContext.Abilities
        .Include(ability => ability.Pokemons)
        .ToList();
}

public Pokemon GetByName(string name)
{
    return this._dbContext.Pokemons
        .Include(pokemon => pokemon.Abilities)
        .FirstOrDefault(pokemon => pokemon.Name.Equals(name));
}
````

Ajoutez aussi une ligne dans le `Program.cs`

````csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
````

Démarrez le service. Grâce à la directive `Include()`, EF Core va effectuer des requêtes de liaison sur la Base de données. Quelle type de liaison fait EF Core ?

## (11) Implémentez un update et un delete d'un Pokémon sur la Base de données

Regardez les méthodes disponibles sur le `_dbContext` pour faire cela, et implémentez aussi des nouveaux endpoints à votre controlleur.

Une fois que c'est fait, vous avez votre CRUD de Pokémon qui est sauvegardé dans une Base !

---
> ☠️ Comme nous avons pu le constater, EF Core est un outil puissant. Il mâche beaucoup le travail de modélisation de la base de données, les débutants en modélisation peuvent facilement le manipuler pour créer une base de données relationelle. 

> Cependant, il est important dans le travail d'un ingénieur de comprendre ce que l'outil fait et crée. Nous avons la responsabilité de la création du schéma de la base, si quelque chose fonctionne mal c'est de notre ressort d'analyser et de trouver la réponse au problème. Chose impossible à faire si nous n'avons pas les notions de modélisation de base de données.
---

## (12) [Bonus étalé sur le TP 2 et TP 3] Se brancher à PokéAPI

Nous avons défini en local des pokémons. Mais un service Web existe déjà, qui expose tous les pokémons ! Il s'agit de <https://pokeapi.co/>

Ajoutez un nouveau controller : `PokeApiController.cs`. Et un nouveau service : `PokeApi.cs` (et `IPokeApi.cs`)

````csharp
using Microsoft.AspNetCore.Mvc;

namespace PokeAPIPolytech.Controllers;

[ApiController]
[Route("[controller]")]
public class PokeApiController : ControllerBase
{
    private readonly ILogger<PokeApiController> _logger;
    private readonly IPokeAPI _pokeAPI;

    public PokeApiController(
        ILogger<PokeApiController> logger,
        IPokeAPI pokeAPI)
    {
        _logger = logger;
        this._pokeAPI = pokeAPI;
    }
}
````

`Program.cs` :

````csharp
builder.Services.AddScoped<IPokeApi, PokeApi>();

builder.Services.AddHttpClient();
````

`PokeApi.cs`

````csharp
public class PokeApi : IPokeApi
{
    private readonly HttpClient _client;

    public PokeApi(HttpClient client)
    {
        _client = client;
    }
}
````

Utilisez ce dont vous avez vu dans ce TP pour créer un endpoint GET GetByPokemonName(), qui ira chercher sur PokeApi le pokémon correspondant au nom entré.

> 💡 HttpClient est l'outil dotnet permettant de faire des requêtes HTTP dans le code.

> 💡 Vous pouvez vous aider de sites comme <https://json2csharp.com/> pour convertir un fichier JSON en classe dotnet. C'est particulièrement utile pour convertir un résultat d'une requête HTTP (qui est en JSON) en classes typées dotnet.

## (13) Pokémon Favoris

Implémentez une nouvelle entité permettant de mettre en favori des pokémons. Créez un nouveau controlleur et un nouveau service permettant d'ajouter/d'enlever un pokémon des favoris.

## (14) Encore plus ?

Sujet libre. Ajoutez au service ce que vous voulez, mais pensez à bien mettre des commentaires pour ma compréhension.