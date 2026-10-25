using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tb_bib_autor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Nacionalidade = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_bib_autor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_bib_categoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(80)", maxLength: 80, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_bib_categoria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_bib_usuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Nome = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_bib_usuario", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tb_bib_livro",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    Isbn = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    AnoPublicacao = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    AutorId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    CategoriaId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_bib_livro", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_bib_livro_tb_bib_autor_AutorId",
                        column: x => x.AutorId,
                        principalTable: "tb_bib_autor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_bib_livro_tb_bib_categoria_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "tb_bib_categoria",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tb_bib_emprestimo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DataEmprestimo = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataPrevistaDevolucao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DataDevolucao = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    UsuarioId = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    LivroId = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tb_bib_emprestimo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tb_bib_emprestimo_tb_bib_livro_LivroId",
                        column: x => x.LivroId,
                        principalTable: "tb_bib_livro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tb_bib_emprestimo_tb_bib_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "tb_bib_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_bib_emprestimo_livro_devol",
                table: "tb_bib_emprestimo",
                columns: new[] { "LivroId", "DataDevolucao" });

            migrationBuilder.CreateIndex(
                name: "IX_tb_bib_emprestimo_UsuarioId",
                table: "tb_bib_emprestimo",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IDX_bib_livro_isbn",
                table: "tb_bib_livro",
                column: "Isbn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IDX_bib_livro_titulo_autor",
                table: "tb_bib_livro",
                columns: new[] { "Titulo", "AutorId" });

            migrationBuilder.CreateIndex(
                name: "IX_tb_bib_livro_AutorId",
                table: "tb_bib_livro",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_tb_bib_livro_CategoriaId",
                table: "tb_bib_livro",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IDX_bib_usuario_email",
                table: "tb_bib_usuario",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tb_bib_emprestimo");

            migrationBuilder.DropTable(
                name: "tb_bib_livro");

            migrationBuilder.DropTable(
                name: "tb_bib_usuario");

            migrationBuilder.DropTable(
                name: "tb_bib_autor");

            migrationBuilder.DropTable(
                name: "tb_bib_categoria");
        }
    }
}
