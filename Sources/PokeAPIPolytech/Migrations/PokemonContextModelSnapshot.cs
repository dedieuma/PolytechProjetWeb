﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace PokeAPIPolytech.Migrations
{
    [DbContext(typeof(PokemonContext))]
    partial class PokemonContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.1");

            modelBuilder.Entity("Pokemon", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PictureUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Pokemons");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Description = "A strange seed was planted on its back at birth. The plant sprouts and grows with this POKéMON.",
                            Name = "Bulbasaur",
                            PictureUrl = "https://img.pokemondb.net/artwork/large/bulbasaur.jpg",
                            Type = 11
                        },
                        new
                        {
                            Id = 2,
                            Description = "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.",
                            Name = "Charmander",
                            PictureUrl = "https://img.pokemondb.net/artwork/large/charmander.jpg",
                            Type = 9
                        },
                        new
                        {
                            Id = 3,
                            Description = "After birth, its back swells and hardens into a shell. Powerfully sprays foam from its mouth.",
                            Name = "Squirtle",
                            PictureUrl = "https://img.pokemondb.net/artwork/large/squirtle.jpg",
                            Type = 10
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
