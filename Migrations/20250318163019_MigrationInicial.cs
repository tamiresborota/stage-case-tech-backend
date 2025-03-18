using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CaseStage.Migrations
{
    /// <inheritdoc />
    public partial class MigrationInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Areas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    ProcessoPaiId = table.Column<Guid>(type: "TEXT", nullable: true),
                    AreaId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Processos_Areas_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Processos_Processos_ProcessoPaiId",
                        column: x => x.ProcessoPaiId,
                        principalTable: "Processos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessosDetalhes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProcessoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessosDetalhes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessosDetalhes_Processos_ProcessoId",
                        column: x => x.ProcessoId,
                        principalTable: "Processos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosProcesso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProcessoDetalheId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosProcesso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosProcesso_ProcessosDetalhes_ProcessoDetalheId",
                        column: x => x.ProcessoDetalheId,
                        principalTable: "ProcessosDetalhes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FerramentasProcesso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProcessoDetalheId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FerramentasProcesso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FerramentasProcesso_ProcessosDetalhes_ProcessoDetalheId",
                        column: x => x.ProcessoDetalheId,
                        principalTable: "ProcessosDetalhes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResponsaveisProcesso",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProcessoDetalheId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsaveisProcesso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponsaveisProcesso_ProcessosDetalhes_ProcessoDetalheId",
                        column: x => x.ProcessoDetalheId,
                        principalTable: "ProcessosDetalhes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosProcesso_ProcessoDetalheId",
                table: "DocumentosProcesso",
                column: "ProcessoDetalheId");

            migrationBuilder.CreateIndex(
                name: "IX_FerramentasProcesso_ProcessoDetalheId",
                table: "FerramentasProcesso",
                column: "ProcessoDetalheId");

            migrationBuilder.CreateIndex(
                name: "IX_Processos_AreaId",
                table: "Processos",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Processos_ProcessoPaiId",
                table: "Processos",
                column: "ProcessoPaiId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessosDetalhes_ProcessoId",
                table: "ProcessosDetalhes",
                column: "ProcessoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ResponsaveisProcesso_ProcessoDetalheId",
                table: "ResponsaveisProcesso",
                column: "ProcessoDetalheId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosProcesso");

            migrationBuilder.DropTable(
                name: "FerramentasProcesso");

            migrationBuilder.DropTable(
                name: "ResponsaveisProcesso");

            migrationBuilder.DropTable(
                name: "ProcessosDetalhes");

            migrationBuilder.DropTable(
                name: "Processos");

            migrationBuilder.DropTable(
                name: "Areas");
        }
    }
}
