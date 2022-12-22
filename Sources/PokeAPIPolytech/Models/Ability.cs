namespace Models;

public class Ability
{
    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Pokemon> Pokemons { get; set; }
}