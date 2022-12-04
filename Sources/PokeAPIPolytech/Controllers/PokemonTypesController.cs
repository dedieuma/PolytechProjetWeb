using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class PokemonTypesController : ControllerBase
{
    [HttpGet]
    public IEnumerable<PokemonType> GetAllTypes()
    {
        return Enum.GetValues<PokemonType>();
    }
}