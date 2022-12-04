namespace PokeAPIPolytech.Services;

public interface IPokemonsSources{

    IEnumerable<Pokemon> GetAll();

    Pokemon Add(Pokemon pokemon);
}