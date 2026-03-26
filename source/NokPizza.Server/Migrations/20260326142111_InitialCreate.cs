using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NokPizza.Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DietaryConstraints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DietaryConstraints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PizzaOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PizzaOrderConstraints",
                columns: table => new
                {
                    PizzaOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConstraintId = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaOrderConstraints", x => new { x.PizzaOrderId, x.ConstraintId });
                    table.ForeignKey(
                        name: "FK_PizzaOrderConstraints_DietaryConstraints_ConstraintId",
                        column: x => x.ConstraintId,
                        principalTable: "DietaryConstraints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PizzaOrderConstraints_PizzaOrders_PizzaOrderId",
                        column: x => x.PizzaOrderId,
                        principalTable: "PizzaOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DietaryConstraints",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Vegan" },
                    { 2, "Vegetarian" },
                    { 3, "Gluten" },
                    { 4, "Dairy" },
                    { 5, "Nuts" },
                    { 6, "Fish" },
                    { 7, "Shellfish" },
                    { 8, "Eggs" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PizzaOrderConstraints_ConstraintId",
                table: "PizzaOrderConstraints",
                column: "ConstraintId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PizzaOrderConstraints");

            migrationBuilder.DropTable(
                name: "DietaryConstraints");

            migrationBuilder.DropTable(
                name: "PizzaOrders");
        }
    }
}
