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

builder.Services.AddDbContext<PokemonContext>(options => options.UseSqlite("pokemons.db")); // à ajouter

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

