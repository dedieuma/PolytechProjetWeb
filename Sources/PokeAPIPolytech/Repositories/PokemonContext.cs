
using Microsoft.EntityFrameworkCore;

public class PokemonContext : DbContext
{
    public PokemonContext(DbContextOptions<PokemonContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Pokemon> Pokemons { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var data = PokemonsSources.Pokemons.Append(new Pokemon
        {
            Id = 10,
            Name = "Caterpie",
            Description = "Its short feet are tipped with suction pads that enable it to tirelessly climb slopes and walls.",
            Type = PokemonType.Bug,
            PictureUrl = "https://img.pokemondb.net/artwork/large/caterpie.jpg"
        });

        modelBuilder.Entity<Pokemon>()
            .HasData(data);
    }
}