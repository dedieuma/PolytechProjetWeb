public interface IPokemonsDbSources
{
    IEnumerable<Pokemon> GetAll();
    Pokemon GetById(int id);
    Pokemon Insert(CreatePokemonDto dto);
}