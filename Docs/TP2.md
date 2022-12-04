# TP 2

POKEMONS !

## Objectifs : 

- Créer un CRUD sur les pokémons

> 💡 Create, Read, Update, Delete

Nous allons écrire une API REST permettant de manipuler et gérer des pokémons.

C'est ce service que vous allez devoir réutiliser lors des cours sur le Frontend. Nous allons définir les pokémons et comment les manipuler, tandis que la (future) application front se chargera de mettre une UI pour interagir avec les actions que vous allez définir.

## (0) Suppression de la WeatherApp

Ouvrez la WebAPI dotnet que vous avez généré au TP précédent.

Supprimez Controllers/WeatherForecaseController.cs et WeatherForecast.cs.

## (1) Définition d'un pokémon

Avant d'exposer des endpoint API Rest, il nous faut définir un pokémon.

Créez un fichier Models/Pokemon.cs

````csharp
public class Pokemon{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public PokemonType Type { get; set; } 
    public string Description { get; set; } = string.Empty;
    public string PictureUrl { get; set; } = string.Empty;

}
````

puis une class Models/PokemonType.cs

````csharp
public enum PokemonType{
    
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

> 💡 Un `enum` est, pour simplifier, une liste de constantes définies sous un seul et même type. **En dotnet :** c'est une extension du type primitif `int`

## (2) Création d'une méthode GET

Créez un fichier Controllers/PokemonsController.cs

````csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PokemonsController : ControllerBase
{
    private readonly ILogger<PokemonsController> _logger;

    private IEnumerable<Pokemon> Pokemons = new List<Pokemon>
    {
        new Pokemon{
            Id = 1,
            Name = "Bulbasaur",
            Description = "A strange seed was planted on its back at birth. The plant sprouts and grows with this POKéMON.",
            Type = PokemonType.Grass,
            PictureUrl = "https://img.pokemondb.net/artwork/large/bulbasaur.jpg"
        },
            new Pokemon{
                Id = 2,
                Name = "Charmander",
                Description = "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.",
                Type = PokemonType.Fire,
                PictureUrl = "https://img.pokemondb.net/artwork/large/charmander.jpg"
        },
            new Pokemon{
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
public Pokemon? GetPokemonById(int id){
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

**Q8 : que se passe-t-il à présent lorsque l'on met un ID invalide ?**

## (3) Création d'un autre controller

De la même manière que le controller que l'on vient de construire, faites un nouveau controller qui permet d'exposer les Types de pokemons via une méthode GET.

## (4) Création d'une méthode POST

Reprenez PokemonsController.cs

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

