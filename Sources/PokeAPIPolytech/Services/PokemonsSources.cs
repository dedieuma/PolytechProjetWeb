public class PokemonsSources : IPokemonsSources
{
    public static List<Pokemon> Pokemons = new List<Pokemon>
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

    public Pokemon Add(Pokemon pokemon)
    {
        Pokemons.Add(pokemon);
        return pokemon;
    }

    public void DeletePokemon(int pokemonId)
    {
        Pokemons.RemoveAll(pok => pok.Id == pokemonId);
    }

    public IEnumerable<Pokemon> GetAll()
    {
        return Pokemons;
    }

    public Pokemon? Update(int pokemonId, Pokemon pokemon)
    {
        var retrivedPokemon = Pokemons.FirstOrDefault(pok => pok.Id == pokemonId);

        if (retrivedPokemon == null){
            return null;
        }

        retrivedPokemon.Description = pokemon.Description;
        retrivedPokemon.Name = pokemon.Name;
        retrivedPokemon.PictureUrl = pokemon.PictureUrl;
        retrivedPokemon.Type = pokemon.Type;
        return retrivedPokemon;
    }
}
