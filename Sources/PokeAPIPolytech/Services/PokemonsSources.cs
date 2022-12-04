
namespace PokeAPIPolytech.Services;
public class PokemonsSources : IPokemonsSources
{
    private List<Pokemon> pokemons = new List<Pokemon>();

    public Pokemon Add(Pokemon pokemon)
    {
        pokemons.Add(pokemon);
        return pokemon;
    }

    public IEnumerable<Pokemon> GetAll()
    {
        if (!pokemons.Any()){
            pokemons = InitPokemons();
        }

        return pokemons;
    }

    private List<Pokemon> InitPokemons() => new List<Pokemon>
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
}
