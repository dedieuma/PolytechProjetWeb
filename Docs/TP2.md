# TP 2

POKEMONS !

## Objectifs

- Créer un CRUD sur les pokémons

> 💡 Create, Read, Update, Delete

Nous allons écrire une API REST permettant de manipuler et gérer des pokémons.

C'est ce service que vous allez devoir réutiliser lors des cours sur le Frontend. Nous allons définir les pokémons et comment les manipuler, tandis que la (future) application front se chargera de mettre une UI pour interagir avec les actions que vous allez définir.

## (0) Suppression de la WeatherApp

Ouvrez la WebAPI dotnet que vous avez généré au TP précédent.

Supprimez `Controllers/WeatherForecaseController.cs` et `WeatherForecast.cs`.

---

## (1) Définition d'un pokémon

Avant d'exposer des endpoint API Rest, il nous faut définir un pokémon.

Créez un fichier `Models/Pokemon.cs`

````csharp
public class Pokemon 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PokemonType Type { get; set; } 
    public string Description { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;

}
````

puis une class `Models/PokemonType.cs`

````csharp
public enum PokemonType 
{    
    Normal,
    Fighting,
    Flying,
    Poison,
    Ground,
    Rock,
    Bug,
    Ghost,
    Steel,
    Fire,
    Water,
    Grass,
    Electric,
    Psychic,
    Ice,
    Dragon,
    Dark,
    Fairy
}
````

> 💡 Un `enum` est, pour simplifier, une liste de constantes définies sous un seul et même type. **En dotnet :** c'est aussi une extension du type primitif `int`

---

## (2) Création d'une méthode GET

Créez un fichier `Controllers/PokemonsController.cs`

````csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PokemonsController : ControllerBase
{
    private readonly ILogger<PokemonsController> _logger;

    private IEnumerable<Pokemon> Pokemons = new List<Pokemon>
    {
        new Pokemon 
        {
            Id = 1,
            Name = "Bulbasaur",
            Description = "A strange seed was planted on its back at birth. The plant sprouts and grows with this POKéMON.",
            Type = PokemonType.Grass,
            PictureUrl = "https://img.pokemondb.net/artwork/large/bulbasaur.jpg"
        },
        new Pokemon 
        {
            Id = 2,
            Name = "Charmander",
            Description = "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.",
            Type = PokemonType.Fire,
            PictureUrl = "https://img.pokemondb.net/artwork/large/charmander.jpg"
        },
        new Pokemon 
        {
            Id = 3,
            Name = "Squirtle",
            Description = "After birth, its back swells and hardens into a shell. Powerfully sprays foam from its mouth.",
            Type = PokemonType.Water,
            PictureUrl = "https://img.pokemondb.net/artwork/large/squirtle.jpg"
        }
    };

    public PokemonsController(ILogger<PokemonsController> logger)
    {
        _logger = logger;
    }

    [HttpGet("All")]
    public IEnumerable<Pokemon> GetAllPokemons()
    {
        return Pokemons;
    }
}
````

Faites un `dotnet run`

**Q1 : que fait une méthode GET ?**

**Q2 : que fait cette classe ?**

**Q3 : Pourquoi, dans les résultats de la requête, le type est un numéro ?**

**Q4 : comment l'url de la requête est préfixée par /Pokemons ?**

Faisons dorénavant une autre méthode GET, permettant de récupérer un pokémon par son ID passé en paramètre.

````csharp
[HttpGet("{id}")]
public Pokemon? GetPokemonById(int id)
{
    return Pokemons.FirstOrDefault(pok => pok.Id == id);
}
````

> 💡 `Pokemons.FirstOrDefault()` est du Linq

**Q5 : qu'est-ce que cela renvoie ?**

**Q6 : que se passe-t-il si vous entrez un Id qui n'est pas dans le tableau de base ? En quoi est-ce invalide ?**

**Q7 : changez le `[HttpGet("{id}")]` en `[HttpGet("{idd}")]`. Que se passe-t-il sur la requête ? Que se passe-t-il sur l'url en particulier ?**

> 💡 Ok c'est bien, mais j'aimerais bien renvoyer le bon HTTP status code si jamais on entre un ID invalide.

Mettons à jour la méthode :

````csharp
[HttpGet("{id}")]
public IActionResult GetPokemonById(int id)
{
    var pokemon = Pokemons.FirstOrDefault(pok => pok.Id == id);

    return pokemon == default
    ? NotFound()
    : Ok(pokemon);
}
````

**Q8 : que se passe-t-il à présent lorsque l'on met un ID invalide ? Pourquoi renvoyons nous ce Status Code en particulier ?**

---

## (3) Création d'un autre controller

De la même manière que le controller que l'on vient de construire, faites un nouveau controller qui permet d'exposer les Types de pokemons via une méthode GET.

---

## (4) Création d'une méthode POST

Reprenez `PokemonsController.cs`

Ajoutez une méthode POST.

````csharp
[HttpPost]
public Pokemon CreatePokemon(CreatePokemonDto createPokemonDto)
{
    var pokemon = new Pokemon
    {
        Id = createPokemonDto.Id,
        Name = createPokemonDto.Name,
        Description = createPokemonDto.Description,
        PictureUrl = createPokemonDto.PictureUrl,
        Type = createPokemonDto.Type
    };

    Pokemons = Pokemons.Append(pokemon);

    return pokemon;
}
````

✍️ Créez la classe `Dtos/CreatePokemonDto.cs` de façon à ce que cela compile.

Testez de faire un POST avec un nouveau pokemon, puis refaites un GET All.
Le nouveau pokemon n'apparaît pas !

> 💡 C'est parce que la liste est "recréée" à chaque fois que le serveur traite une requête...

Pour régler cela, il faut exporter la liste des Pokemons dans un service à part, qui sera singleton.

**Q9 : qu'est-ce qu'un singleton ?**

Créez deux fichiers : `Services/IPokemonsSources.cs` et `Services/PokemonSources.cs`

````csharp
public interface IPokemonsSources 
{
    IEnumerable<Pokemon> GetAll();

    Pokemon Add(Pokemon pokemon);
}
````

````csharp
public class PokemonsSources : IPokemonsSources
{
    private List<Pokemon> pokemons = new List<Pokemon>
    {
        new Pokemon
        {
            Id = 1,
            Name = "Bulbasaur",
            Description = "A strange seed was planted on its back at birth. The plant sprouts and grows with this POKéMON.",
            Type = PokemonType.Grass,
            PictureUrl = "https://img.pokemondb.net/artwork/large/bulbasaur.jpg"
        },
        new Pokemon
        {
            Id = 2,
            Name = "Charmander",
            Description = "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.",
            Type = PokemonType.Fire,
            PictureUrl = "https://img.pokemondb.net/artwork/large/charmander.jpg"
        },
        new Pokemon
        {
            Id = 3,
            Name = "Squirtle",
            Description = "After birth, its back swells and hardens into a shell. Powerfully sprays foam from its mouth.",
            Type = PokemonType.Water,
            PictureUrl = "https://img.pokemondb.net/artwork/large/squirtle.jpg"
        }   
    };

    public Pokemon Add(Pokemon pokemon)
    {
        pokemons.Add(pokemon);
        return pokemon;
    }

    public IEnumerable<Pokemon> GetAll()
    {
        return pokemons;
    }
}
````

N'oubliez pas de supprimer la liste initiale qui était dans le controller.

Dans le `Program.cs`, ajoutez :

````csharp
builder.Services.AddSwaggerGen(); //existant

builder.Services.AddSingleton<IPokemonsSources, PokemonsSources>(); //à ajouter

var app = builder.Build(); //existant
````

Dans `PokemonsController`, ajoutez :

````csharp
    private readonly ILogger<PokemonsController> _logger; //existant
    private readonly IPokemonsSources _pokemonsSources; //à ajouter

    public PokemonsController(
        ILogger<PokemonsController> logger,
        IPokemonsSources pokemonsSources)
    {
        _logger = logger;
        _pokemonsSources = pokemonsSources;
    }
````

✍️ mettez à jour la classe pour que cela compile à nouveau.

Lancez le service. Est-ce que cela fonctionne comme attendu ?

**Q10 : que se passe-t-il si je rajoute via POST un pokémon qui a le même Id qu'un autre ? Pouvez-vous rajouter de la logique pour faire en sorte que cela ne se produise pas ?**

---

## (5) Creation d'une méthode PUT

Allons mettre à jour un pokémon.

````csharp
[HttpPut("{pokemonId}")]
public Pokemon? UpdatePokemon(int pokemonId, UpdatePokemonDto updatePokemonDto)
{   
    var pokemon = new Pokemon
    {
        Name = updatePokemonDto.Name,
        Description = updatePokemonDto.Description,
        PictureUrl = updatePokemonDto.PictureUrl,
        Type = updatePokemonDto.Type
    };

    return _pokemonsSources.Update(pokemonId, pokemon);
}
````

✍️ Ecrivez la méthode Update dans PokemonsSources, de même que la classe UpdatePokemonDto

✍️ De la même manière que la méthode GET by id, pouvez-vous modifier la méthode pour que le controller renvoie Not Found si jamais l'Id entré est invalide ?

---

## (6) Creation d'une méthode DELETE

Complétez comme il faut ce début d'endpoint DELETE :

````csharp
[HttpDelete("pokemonId")]
public void DeletePokemon(int pokemonId)
{
    // à vous
}
````

> 💡 N'oubliez pas de faire en sorte que la méthode renvoie NotFound si jamais l'id est invalide...

Bravo, vous avez implémenté votre premier CRUD !

---

**Q11 : Si je lance le serveur, que je rajoute un pokemon, que j'éteins le serveur, que je le rallume, et que je fais un GetAll, que se passe-t-il ? Que faudrait t-il faire pour résoudre le problème ?**

---

## (7) Changement de la définition d'un pokemon

Modifiez la définition d'un pokemon en rajoutant/modifiant ce que vous voulez. Mettez à jour le controlleur.

## (8) [Bonus étalé sur le TP 2 et TP 3] Se brancher à PokéAPI

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
