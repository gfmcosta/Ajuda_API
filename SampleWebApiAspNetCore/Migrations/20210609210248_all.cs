using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SampleWebApiAspNetCore.Migrations
{
    public partial class all : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Funcao",
                columns: table => new
                {
                    IdFuncao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcao", x => x.IdFuncao);
                });

            migrationBuilder.CreateTable(
                name: "Utilizador",
                columns: table => new
                {
                    IdUtilizador = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizador", x => x.IdUtilizador);
                });

            migrationBuilder.CreateTable(
                name: "Funcionario",
                columns: table => new
                {
                    IdFuncionario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtilizador = table.Column<int>(type: "int", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sexo = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Nacionalidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataNasc = table.Column<DateTime>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Funcao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funcionario", x => x.IdFuncionario);
                    table.ForeignKey(
                        name: "FK_Funcionario_Funcao",
                        column: x => x.Funcao,
                        principalTable: "Funcao",
                        principalColumn: "IdFuncao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Funcionario_Utilizador",
                        column: x => x.IdUtilizador,
                        principalTable: "Utilizador",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Paciente",
                columns: table => new
                {
                    IdPaciente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtilizador = table.Column<int>(type: "int", nullable: true),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sexo = table.Column<string>(type: "char(10)", unicode: false, fixedLength: true, maxLength: 10, nullable: false),
                    Telemovel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nacionalidade = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DataNasc = table.Column<DateTime>(type: "date", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CC = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    NIF = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paciente", x => x.IdPaciente);
                    table.ForeignKey(
                        name: "FK_Paciente_Utilizador",
                        column: x => x.IdUtilizador,
                        principalTable: "Utilizador",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Marcacao",
                columns: table => new
                {
                    IdMarcacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPaciente = table.Column<int>(type: "int", nullable: false),
                    IdFuncionario = table.Column<int>(type: "int", nullable: true),
                    IdTecnico = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "date", nullable: false),
                    Hora = table.Column<TimeSpan>(type: "time", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    QRCODE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Relatorio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ultima_atualizacao = table.Column<DateTime>(type: "datetime", nullable: false),
                    TecnicoNavigationIdFuncionario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marcacao", x => x.IdMarcacao);
                    table.ForeignKey(
                        name: "FK_Marcacao_Funcionario",
                        column: x => x.IdFuncionario,
                        principalTable: "Funcionario",
                        principalColumn: "IdFuncionario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marcacao_Funcionario_TecnicoNavigationIdFuncionario",
                        column: x => x.TecnicoNavigationIdFuncionario,
                        principalTable: "Funcionario",
                        principalColumn: "IdFuncionario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Marcacao_Paciente",
                        column: x => x.IdPaciente,
                        principalTable: "Paciente",
                        principalColumn: "IdPaciente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Funcionario_Funcao",
                table: "Funcionario",
                column: "Funcao");

            migrationBuilder.CreateIndex(
                name: "IX_Funcionario_IdUtilizador",
                table: "Funcionario",
                column: "IdUtilizador",
                unique: true,
                filter: "[IdUtilizador] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Marcacao_IdFuncionario",
                table: "Marcacao",
                column: "IdFuncionario");

            migrationBuilder.CreateIndex(
                name: "IX_Marcacao_IdPaciente",
                table: "Marcacao",
                column: "IdPaciente");

            migrationBuilder.CreateIndex(
                name: "IX_Marcacao_TecnicoNavigationIdFuncionario",
                table: "Marcacao",
                column: "TecnicoNavigationIdFuncionario");

            migrationBuilder.CreateIndex(
                name: "IX_Paciente_IdUtilizador",
                table: "Paciente",
                column: "IdUtilizador",
                unique: true,
                filter: "[IdUtilizador] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Marcacao");

            migrationBuilder.DropTable(
                name: "Funcionario");

            migrationBuilder.DropTable(
                name: "Paciente");

            migrationBuilder.DropTable(
                name: "Funcao");

            migrationBuilder.DropTable(
                name: "Utilizador");
        }
    }
}
