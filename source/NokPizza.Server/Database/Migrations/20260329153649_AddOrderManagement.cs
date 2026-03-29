using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NokPizza.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PizzaOrderConstraints");

            migrationBuilder.DropColumn(
                name: "NumberOfPeople",
                table: "PizzaOrders");

            migrationBuilder.AddColumn<string>(
                name: "AdminPasswordHash",
                table: "PizzaOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatorEmail",
                table: "PizzaOrders",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedCreatorEmail",
                table: "PizzaOrders",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantPasswordHash",
                table: "PizzaOrders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RsvpDeadline",
                table: "PizzaOrders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "PizzaOrderParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PizzaOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaOrderParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PizzaOrderParticipants_PizzaOrders_PizzaOrderId",
                        column: x => x.PizzaOrderId,
                        principalTable: "PizzaOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PizzaOrderParticipantConstraints",
                columns: table => new
                {
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConstraintId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PizzaOrderParticipantConstraints", x => new { x.ParticipantId, x.ConstraintId });
                    table.ForeignKey(
                        name: "FK_PizzaOrderParticipantConstraints_DietaryConstraints_ConstraintId",
                        column: x => x.ConstraintId,
                        principalTable: "DietaryConstraints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PizzaOrderParticipantConstraints_PizzaOrderParticipants_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "PizzaOrderParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PizzaOrders_NormalizedCreatorEmail",
                table: "PizzaOrders",
                column: "NormalizedCreatorEmail");

            migrationBuilder.CreateIndex(
                name: "IX_PizzaOrders_RsvpDeadline",
                table: "PizzaOrders",
                column: "RsvpDeadline");

            migrationBuilder.CreateIndex(
                name: "IX_PizzaOrderParticipantConstraints_ConstraintId",
                table: "PizzaOrderParticipantConstraints",
                column: "ConstraintId");

            migrationBuilder.CreateIndex(
                name: "IX_PizzaOrderParticipants_NormalizedEmail",
                table: "PizzaOrderParticipants",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_PizzaOrderParticipants_PizzaOrderId_NormalizedEmail",
                table: "PizzaOrderParticipants",
                columns: new[] { "PizzaOrderId", "NormalizedEmail" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PizzaOrderParticipantConstraints");

            migrationBuilder.DropTable(
                name: "PizzaOrderParticipants");

            migrationBuilder.DropIndex(
                name: "IX_PizzaOrders_NormalizedCreatorEmail",
                table: "PizzaOrders");

            migrationBuilder.DropIndex(
                name: "IX_PizzaOrders_RsvpDeadline",
                table: "PizzaOrders");

            migrationBuilder.DropColumn(
                name: "AdminPasswordHash",
                table: "PizzaOrders");

            migrationBuilder.DropColumn(
                name: "CreatorEmail",
                table: "PizzaOrders");

            migrationBuilder.DropColumn(
                name: "NormalizedCreatorEmail",
                table: "PizzaOrders");

            migrationBuilder.DropColumn(
                name: "ParticipantPasswordHash",
                table: "PizzaOrders");

            migrationBuilder.DropColumn(
                name: "RsvpDeadline",
                table: "PizzaOrders");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfPeople",
                table: "PizzaOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_PizzaOrderConstraints_ConstraintId",
                table: "PizzaOrderConstraints",
                column: "ConstraintId");
        }
    }
}
