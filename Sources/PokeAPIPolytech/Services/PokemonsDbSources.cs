using Microsoft.EntityFrameworkCore;
using Models;

public class PokemonsDbSources : IPokemonsDbSources
{
    private readonly PokemonContext _dbContext;

    public PokemonsDbSources(
        PokemonContext context
    )
    {
        this._dbContext = context;
    }

    public IEnumerable<Pokemon> GetAll()
    {
        return this._dbContext.Pokemons
            .Include(pokemon => pokemon.Abilities)
            .ToList();
    }

    public IEnumerable<Ability> GetAllAbilities()
    {
        return this._dbContext.Abilities
            .Include(ability => ability.Pokemons)
            .ToList();
    }

    public Pokemon GetByName(string name)
    {
        return this._dbContext.Pokemons
            .Include(pokemon => pokemon.Abilities)
            .FirstOrDefault(pokemon => pokemon.Name.Equals(name));
    }

    public Pokemon Insert(CreatePokemonDto dto)
    {
        var pokemon = new Pokemon
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            PictureUrl = dto.PictureUrl,
            Type = dto.Type
        };

        this._dbContext.Pokemons
            .Add(pokemon);

        this._dbContext.SaveChanges();

        return pokemon;
    }
}