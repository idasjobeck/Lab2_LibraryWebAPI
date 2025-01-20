using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab2_LibraryWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedCheckConstraintOnBookAvailableQty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Book_AvailableQty",
                table: "Books",
                sql: "AvailableQty <= TotalQty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Book_AvailableQty",
                table: "Books");
        }
    }
}
