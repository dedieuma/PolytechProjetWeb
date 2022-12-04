using Microsoft.AspNetCore.Mvc;

namespace PokeAPIPolytech.Controllers;

[ApiController]
[Route("[controller]")]
public class PokeApiController : ControllerBase
{
    private readonly ILogger<PokeApiController> _logger;
    private readonly IPokeApi _pokeAPI;



    public PokeApiController(
        ILogger<PokeApiController> logger,
        IPokeApi pokeAPI)
    {
        _logger = logger;
        this._pokeAPI = pokeAPI;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetByName(string name){
        Pokemon pokemon = await _pokeAPI.GetByName(name);

        return pokemon == null
        ? NotFound()
        : Ok(pokemon);
    }
}
