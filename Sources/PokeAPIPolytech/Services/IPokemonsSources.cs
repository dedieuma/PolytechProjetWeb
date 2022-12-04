public interface IPokemonsSources{

    IEnumerable<Pokemon> GetAll();

    Pokemon Add(Pokemon pokemon);
    Pokemon? Update(int pokemonId, Pokemon pokemon);
}