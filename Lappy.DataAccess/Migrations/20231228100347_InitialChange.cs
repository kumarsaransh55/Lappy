using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lappy.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScreenSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Colour = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HardDiskSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CpuModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RamMemory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialFeatures = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GraphicsCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    ListPrice10 = table.Column<double>(type: "float", nullable: false),
                    ListPrice25 = table.Column<double>(type: "float", nullable: false),
                    ListPrice100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, 1, "Gaming" },
                    { 2, 2, "Productivity" },
                    { 3, 3, "Business" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "Colour", "CpuModel", "Description", "GraphicsCard", "HardDiskSize", "ListPrice", "ListPrice10", "ListPrice100", "ListPrice25", "ModelName", "OS", "RamMemory", "ScreenSize", "SpecialFeatures" },
                values: new object[] { 1, "ASUS", "Quiet Blue", "Core i3", "Processor: IntelCore i3-1215U Processor 1.2 GHz (10M Cache, up to 4.4 GHz, 6 cores)\r\nMemory: 8GB DDR4 RAM on board | Storage: 512GB M.2 NVMe PCIe 3.0 SSD\r\nDisplay: 15.6-inch, FHD (1920 x 1080) 16:9 aspect ratio, 250nits, 60Hz refresh rate, Anti-glare display, 45% NTSC\r\nGraphics: Integrated Intel UHD Graphics\r\nOperating System: Pre-installed Windows 11 Home with Lifetime Validity | Software Included: Pre-installed Office Home and Student with Lifetime Validity | McAfee (1 Year)\r\nDesign: 1.99 ~ 1.99 cm | 1.70 kg | Thin and Light Laptop | 42WHrs, 3S1P, 3-cell Li-ion | Up to 6 hours battery life ;Note: Battery life depends on conditions of usage\r\nKeyboard: Backlit Chiclet Keyboard | 1.4mm Key Travel", "Integrated", "512 GB", 40990.0, 39000.0, 35000.0, 37000.0, "ASUS Vivobook 15", "Windows 11 Home", "8 GB", "15.6 Inches", "Backlit Keyboard, Anti Glare Coating" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
