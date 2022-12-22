using Models;

public interface IPokemonsDbSources
{
    IEnumerable<Pokemon> GetAll();
    IEnumerable<Ability> GetAllAbilities();
    Pokemon GetByName(string name);
    Pokemon Insert(CreatePokemonDto dto);
}