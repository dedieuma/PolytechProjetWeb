
using Microsoft.EntityFrameworkCore;
using Models;

public class PokemonContext : DbContext
{
    public PokemonContext(DbContextOptions<PokemonContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Pokemon> Pokemons { get; set; } = default!;

    public DbSet<Ability> Abilities { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var dataPokemon = PokemonsSources.Pokemons.Append(new Pokemon
        {
            Id = 10,
            Name = "Caterpie",
            Description = "Its short feet are tipped with suction pads that enable it to tirelessly climb slopes and walls.",
            Type = PokemonType.Bug,
            PictureUrl = "https://img.pokemondb.net/artwork/large/caterpie.jpg"
        });

        modelBuilder.Entity<Pokemon>()
            .HasData(dataPokemon);

        var dataAbilities = new List<Ability>{
            new Ability{
                Id = 1,
                Name = "shield-dust"
            }
        };

        modelBuilder.Entity<Ability>()
            .HasData(dataAbilities);

        modelBuilder.Entity<Pokemon>()
            .HasMany(pokemon => pokemon.Abilities)
            .WithMany(ab => ab.Pokemons)
            .UsingEntity(abPok => abPok.HasData(new { PokemonsId = 10, AbilitiesId = 1 }));
            
    }
}