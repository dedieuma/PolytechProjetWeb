
using Microsoft.EntityFrameworkCore;

public class PokemonContext : DbContext
{
    public PokemonContext(DbContextOptions<PokemonContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<Pokemon> Pokemons { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pokemon>()
            .HasData(PokemonsSources.Pokemons);
    }
}