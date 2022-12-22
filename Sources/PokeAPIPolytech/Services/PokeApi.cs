using Dtos;

public class PokeApi : IPokeApi
{
    private readonly HttpClient _client;

    public PokeApi(HttpClient client)
    {
        _client = client;
    }

    public async Task<Pokemon> GetByName(string name)
    {
        _client.BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon/");

        var result = await _client.GetAsync(name);

        var root = await result.Content.ReadFromJsonAsync<Root>();

        return new Pokemon{
            Id = root.id,
            Name = root.name
        };
    }
}