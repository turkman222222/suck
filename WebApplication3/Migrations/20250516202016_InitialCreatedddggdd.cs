using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication3.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreatedddggdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "compl",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kompl_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_compl", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cveta",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cvet_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cveta", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Marks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name_marka = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Marks", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rol",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    rol_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rol", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "salonch",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    salon = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_salonch", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "strana",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    strana_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_strana", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id_user = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    mail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rol_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id_user);
                    table.ForeignKey(
                        name: "FK_user_rol_rol_id",
                        column: x => x.rol_id,
                        principalTable: "rol",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Carss",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_marki = table.Column<int>(type: "int", nullable: false),
                    id_str = table.Column<int>(type: "int", nullable: false),
                    god_poiz = table.Column<int>(type: "int", nullable: false),
                    id_cvet = table.Column<int>(type: "int", nullable: false),
                    id_salona = table.Column<int>(type: "int", nullable: false),
                    id_kompl = table.Column<int>(type: "int", nullable: false),
                    image = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    price = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carss", x => x.id);
                    table.ForeignKey(
                        name: "FK_Carss_Marks_id_marki",
                        column: x => x.id_marki,
                        principalTable: "Marks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carss_compl_id_kompl",
                        column: x => x.id_kompl,
                        principalTable: "compl",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carss_cveta_id_cvet",
                        column: x => x.id_cvet,
                        principalTable: "cveta",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carss_salonch_id_salona",
                        column: x => x.id_salona,
                        principalTable: "salonch",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Carss_strana_id_str",
                        column: x => x.id_str,
                        principalTable: "strana",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "bron",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_car = table.Column<int>(type: "int", nullable: false),
                    id_usr = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bron", x => x.id);
                    table.ForeignKey(
                        name: "FK_bron_Carss_id_car",
                        column: x => x.id_car,
                        principalTable: "Carss",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bron_user_id_usr",
                        column: x => x.id_usr,
                        principalTable: "user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "izbr",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    car_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_izbr", x => x.id);
                    table.ForeignKey(
                        name: "FK_izbr_Carss_car_id",
                        column: x => x.car_id,
                        principalTable: "Carss",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_izbr_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id_user",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_bron_id_car",
                table: "bron",
                column: "id_car");

            migrationBuilder.CreateIndex(
                name: "IX_bron_id_usr",
                table: "bron",
                column: "id_usr");

            migrationBuilder.CreateIndex(
                name: "IX_Carss_id_cvet",
                table: "Carss",
                column: "id_cvet");

            migrationBuilder.CreateIndex(
                name: "IX_Carss_id_kompl",
                table: "Carss",
                column: "id_kompl");

            migrationBuilder.CreateIndex(
                name: "IX_Carss_id_marki",
                table: "Carss",
                column: "id_marki");

            migrationBuilder.CreateIndex(
                name: "IX_Carss_id_salona",
                table: "Carss",
                column: "id_salona");

            migrationBuilder.CreateIndex(
                name: "IX_Carss_id_str",
                table: "Carss",
                column: "id_str");

            migrationBuilder.CreateIndex(
                name: "IX_izbr_car_id",
                table: "izbr",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_izbr_user_id",
                table: "izbr",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_rol_id",
                table: "user",
                column: "rol_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bron");

            migrationBuilder.DropTable(
                name: "izbr");

            migrationBuilder.DropTable(
                name: "Carss");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "Marks");

            migrationBuilder.DropTable(
                name: "compl");

            migrationBuilder.DropTable(
                name: "cveta");

            migrationBuilder.DropTable(
                name: "salonch");

            migrationBuilder.DropTable(
                name: "strana");

            migrationBuilder.DropTable(
                name: "rol");
        }
    }
}
