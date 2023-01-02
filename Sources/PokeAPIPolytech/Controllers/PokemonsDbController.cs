using Microsoft.AspNetCore.Mvc;
using Models;

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

    [HttpGet("Pokemons/All")]
    public IEnumerable<Pokemon> GetAllPokemons()
    {
        return _pokemonsDbSources.GetAll();
    }

    [HttpGet("Pokemons/{name}")]
    public ActionResult<Pokemon> GetPokemonByName(string name)
    {
        var pokemon = _pokemonsDbSources.GetByName(name);

        if (pokemon == null)
        {
            return NotFound();
        }

        return Ok(pokemon);
    }

    [HttpGet("Abilities/All")]
    public IEnumerable<Ability> GetAllAbilities()
    {
        return _pokemonsDbSources.GetAllAbilities();
    }

    [HttpPost]
    public ActionResult<Pokemon> InsertPokemon(CreatePokemonDto dto)
    {
        if (_pokemonsDbSources.GetAll().Any(pokemon => pokemon.Id == dto.Id))
        {
            return BadRequest();
        }

        var pokemon = _pokemonsDbSources.Insert(dto);

        return Ok(pokemon);
    }
}
