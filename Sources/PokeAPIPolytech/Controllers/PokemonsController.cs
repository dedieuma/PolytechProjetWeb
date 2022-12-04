using Microsoft.AspNetCore.Mvc;

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
    public IActionResult CreatePokemon(CreatePokemonDto createPokemonDto)
    {

        if (_pokemonsSources.GetAll().Any(p => p.Id == createPokemonDto.Id))
        {
            return BadRequest();
        }

        var pokemon = new Pokemon
        {
            Id = createPokemonDto.Id,
            Name = createPokemonDto.Name,
            Description = createPokemonDto.Description,
            PictureUrl = createPokemonDto.PictureUrl,
            Type = createPokemonDto.Type
        };

        _pokemonsSources.Add(pokemon);

        return Ok(pokemon);
    }

    [HttpPut("{pokemonId}")]
    public IActionResult UpdatePokemon(int pokemonId, UpdatePokemonDto updatePokemonDto)
    {

        var pokemon = new Pokemon
        {
            Name = updatePokemonDto.Name,
            Description = updatePokemonDto.Description,
            PictureUrl = updatePokemonDto.PictureUrl,
            Type = updatePokemonDto.Type
        };

        var pokemonUpdated = _pokemonsSources.Update(pokemonId, pokemon);

        return pokemonUpdated == null
        ? BadRequest()
        : Ok(pokemonUpdated);
    }

    [HttpDelete("pokemonId")]
    public IActionResult DeletePokemon(int pokemonId)
    {
        var pokemon = _pokemonsSources
            .GetAll()
            .FirstOrDefault(pok => pok.Id == pokemonId);

        if (pokemon == null){
            return NotFound();
        }

        _pokemonsSources.DeletePokemon(pokemonId);
        return NoContent();
    }
}
