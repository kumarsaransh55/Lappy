using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lappy.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class added_TransactionId_field : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "OrderHeaders");
        }
    }
}
