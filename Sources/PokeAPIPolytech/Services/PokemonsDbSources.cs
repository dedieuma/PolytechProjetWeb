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
            .ToList();
    }

    public IEnumerable<Ability> GetAllAbilities()
    {
        return this._dbContext.Abilities
            .ToList();
    }

    public Pokemon GetByName(string name)
    {
        return this._dbContext.Pokemons
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

        var query = "INSERT INTO Pokemons (Id, Description, Name, PictureUrl, Type) VALUES ('"+pokemon.Id+"', '"+pokemon.Description+"', '"+pokemon.Name+"', '"+pokemon.PictureUrl+"', '"+pokemon.PictureUrl+"')";

        this._dbContext.Pokemons
            .FromSqlRaw(query)
            .ToList();

        return pokemon;
    }
}