public interface IPokeApi
{
    Task<Pokemon> GetByName(string name);
}