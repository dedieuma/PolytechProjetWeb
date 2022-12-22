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

    [HttpGet("{id}")]
    public ActionResult<Pokemon> GetPokemonById(int id)
    {
        var pokemon = _pokemonsDbSources.GetById(id);

        if (pokemon == null)
        {
            return NotFound();
        }

        return Ok(pokemon);
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
