using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PetCare360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_CLINICA",
                columns: table => new
                {
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_CLINICA = table.Column<string>(type: "NVARCHAR2(150)", maxLength: 150, nullable: false),
                    CNPJ = table.Column<string>(type: "NVARCHAR2(18)", maxLength: 18, nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CLINICA", x => x.ID_CLINICA);
                });

            migrationBuilder.CreateTable(
                name: "TB_TUTOR",
                columns: table => new
                {
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_TUTOR = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "NVARCHAR2(14)", maxLength: 14, nullable: false),
                    EMAIL = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    TELEFONE = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    ENDERECO = table.Column<string>(type: "NVARCHAR2(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_TUTOR", x => x.ID_TUTOR);
                });

            migrationBuilder.CreateTable(
                name: "TB_PET",
                columns: table => new
                {
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_PET = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    ESPECIE = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    RACA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DT_NASCIMENTO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    PESO = table.Column<decimal>(type: "DECIMAL(6,2)", precision: 6, scale: 2, nullable: true),
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_PET", x => x.ID_PET);
                    table.ForeignKey(
                        name: "FK_TB_PET_TB_TUTOR_ID_TUTOR",
                        column: x => x.ID_TUTOR,
                        principalTable: "TB_TUTOR",
                        principalColumn: "ID_TUTOR",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TB_CONSULTA",
                columns: table => new
                {
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    DT_CONSULTA = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DESCRICAO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    DIAGNOSTICO = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: false),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_CONSULTA", x => x.ID_CONSULTA);
                    table.ForeignKey(
                        name: "FK_TB_CONSULTA_TB_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "TB_CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TB_CONSULTA_TB_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "TB_PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_MEDICAMENTO",
                columns: table => new
                {
                    ID_MEDICAMENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_MEDICAMENTO = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DOSAGEM = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    FREQUENCIA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DT_INICIO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_FIM = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_MEDICAMENTO", x => x.ID_MEDICAMENTO);
                    table.ForeignKey(
                        name: "FK_TB_MEDICAMENTO_TB_CONSULTA_ID_CONSULTA",
                        column: x => x.ID_CONSULTA,
                        principalTable: "TB_CONSULTA",
                        principalColumn: "ID_CONSULTA");
                    table.ForeignKey(
                        name: "FK_TB_MEDICAMENTO_TB_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "TB_PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TB_VACINA",
                columns: table => new
                {
                    ID_VACINA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NM_VACINA = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    FABRICANTE = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    DT_APLICACAO = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    DT_PROXIMA_DOSE = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_CONSULTA = table.Column<int>(type: "NUMBER(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_VACINA", x => x.ID_VACINA);
                    table.ForeignKey(
                        name: "FK_TB_VACINA_TB_CONSULTA_ID_CONSULTA",
                        column: x => x.ID_CONSULTA,
                        principalTable: "TB_CONSULTA",
                        principalColumn: "ID_CONSULTA");
                    table.ForeignKey(
                        name: "FK_TB_VACINA_TB_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "TB_PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_CLINICA_CNPJ",
                table: "TB_CLINICA",
                column: "CNPJ",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONSULTA_ID_CLINICA",
                table: "TB_CONSULTA",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_CONSULTA_ID_PET",
                table: "TB_CONSULTA",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_TB_MEDICAMENTO_ID_CONSULTA",
                table: "TB_MEDICAMENTO",
                column: "ID_CONSULTA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_MEDICAMENTO_ID_PET",
                table: "TB_MEDICAMENTO",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_TB_PET_ID_TUTOR",
                table: "TB_PET",
                column: "ID_TUTOR");

            migrationBuilder.CreateIndex(
                name: "IX_TB_TUTOR_CPF",
                table: "TB_TUTOR",
                column: "CPF",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_TUTOR_EMAIL",
                table: "TB_TUTOR",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TB_VACINA_ID_CONSULTA",
                table: "TB_VACINA",
                column: "ID_CONSULTA");

            migrationBuilder.CreateIndex(
                name: "IX_TB_VACINA_ID_PET",
                table: "TB_VACINA",
                column: "ID_PET");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_MEDICAMENTO");

            migrationBuilder.DropTable(
                name: "TB_VACINA");

            migrationBuilder.DropTable(
                name: "TB_CONSULTA");

            migrationBuilder.DropTable(
                name: "TB_CLINICA");

            migrationBuilder.DropTable(
                name: "TB_PET");

            migrationBuilder.DropTable(
                name: "TB_TUTOR");
        }
    }
}
