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
