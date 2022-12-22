using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeAPIPolytech.Migrations
{
    /// <inheritdoc />
    public partial class Abilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Abilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Abilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AbilityPokemon",
                columns: table => new
                {
                    AbilitiesId = table.Column<int>(type: "INTEGER", nullable: false),
                    PokemonsId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbilityPokemon", x => new { x.AbilitiesId, x.PokemonsId });
                    table.ForeignKey(
                        name: "FK_AbilityPokemon_Abilities_AbilitiesId",
                        column: x => x.AbilitiesId,
                        principalTable: "Abilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AbilityPokemon_Pokemons_PokemonsId",
                        column: x => x.PokemonsId,
                        principalTable: "Pokemons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Abilities",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1, "shield-dust" });

            migrationBuilder.InsertData(
                table: "AbilityPokemon",
                columns: new[] { "AbilitiesId", "PokemonsId" },
                values: new object[] { 1, 10 });

            migrationBuilder.CreateIndex(
                name: "IX_AbilityPokemon_PokemonsId",
                table: "AbilityPokemon",
                column: "PokemonsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbilityPokemon");

            migrationBuilder.DropTable(
                name: "Abilities");
        }
    }
}
