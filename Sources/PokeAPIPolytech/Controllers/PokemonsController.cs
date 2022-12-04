using Microsoft.AspNetCore.Mvc;
using PokeAPIPolytech.Services;

namespace PokeAPIPolytech.Controllers;

[ApiController]
[Route("[controller]")]
public class PokemonsController : ControllerBase
{
    private readonly ILogger<PokemonsController> _logger;
    private readonly IPokemonsSources _pokemonsSources;



    public PokemonsController(
        ILogger<PokemonsController> logger,
        IPokemonsSources pokemonsSources)
    {
        _logger = logger;
        _pokemonsSources = pokemonsSources;
    }

    [HttpGet("All")]
    public IEnumerable<Pokemon> GetAllPokemons()
    {
        return _pokemonsSources.GetAll();
    }

    [HttpGet("{id}")]
    public IActionResult GetPokemonById(int id)
    {
        var pokemon = _pokemonsSources.GetAll().FirstOrDefault(pok => pok.Id == id);

        return pokemon == default
        ? NotFound()
        : Ok(pokemon);
    }

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

        _pokemonsSources.Add(pokemon);

        return pokemon;
    }
}
