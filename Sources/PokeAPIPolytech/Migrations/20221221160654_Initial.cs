using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PokeAPIPolytech.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pokemons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PictureUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pokemons", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Pokemons",
                columns: new[] { "Id", "Description", "Name", "PictureUrl", "Type" },
                values: new object[,]
                {
                    { 1, "A strange seed was planted on its back at birth. The plant sprouts and grows with this POKéMON.", "Bulbasaur", "https://img.pokemondb.net/artwork/large/bulbasaur.jpg", 11 },
                    { 2, "Obviously prefers hot places. When it rains, steam is said to spout from the tip of its tail.", "Charmander", "https://img.pokemondb.net/artwork/large/charmander.jpg", 9 },
                    { 3, "After birth, its back swells and hardens into a shell. Powerfully sprays foam from its mouth.", "Squirtle", "https://img.pokemondb.net/artwork/large/squirtle.jpg", 10 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pokemons");
        }
    }
}
