using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cofrinho.infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Cofrinho");

            migrationBuilder.CreateTable(
                name: "Objetivos",
                schema: "Cofrinho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ImagemUrl = table.Column<string>(type: "text", nullable: true),
                    ValorAlvo = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ValorAlvo_Moeda = table.Column<string>(type: "text", nullable: false),
                    Prazo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Categoria = table.Column<string>(type: "text", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstaDeletado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Objetivos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transacoes",
                schema: "Cofrinho",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    Valor_Moeda = table.Column<string>(type: "text", nullable: false),
                    Tipo = table.Column<string>(type: "text", nullable: false),
                    DataTransacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ObjetivoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ObjetivoId1 = table.Column<Guid>(type: "uuid", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EstaDeletado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transacoes_Objetivos_ObjetivoId",
                        column: x => x.ObjetivoId,
                        principalSchema: "Cofrinho",
                        principalTable: "Objetivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Transacoes_Objetivos_ObjetivoId1",
                        column: x => x.ObjetivoId1,
                        principalSchema: "Cofrinho",
                        principalTable: "Objetivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ObjetivoId",
                schema: "Cofrinho",
                table: "Transacoes",
                column: "ObjetivoId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_ObjetivoId1",
                schema: "Cofrinho",
                table: "Transacoes",
                column: "ObjetivoId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transacoes",
                schema: "Cofrinho");

            migrationBuilder.DropTable(
                name: "Objetivos",
                schema: "Cofrinho");
        }
    }
}
