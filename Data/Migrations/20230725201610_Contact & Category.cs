using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactHarbor.Data.Migrations;

/// <inheritdoc />
public partial class ContactCategory : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Categories",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AppUserId = table.Column<string>(type: "text", nullable: false),
                Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Categories", x => x.Id);
                table.ForeignKey(
                    name: "FK_Categories_AspNetUsers_AppUserId",
                    column: x => x.AppUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Contacts",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                AppUserId = table.Column<string>(type: "text", nullable: false),
                FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                DateOfBirth = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Address1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                Address2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                City = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                State = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                ZipCode = table.Column<string>(type: "text", nullable: true),
                Email = table.Column<string>(type: "text", nullable: false),
                PhoneNumber = table.Column<string>(type: "text", nullable: true),
                ImageName = table.Column<string>(type: "text", nullable: true),
                ImageData = table.Column<byte[]>(type: "bytea", nullable: true),
                ImageType = table.Column<string>(type: "text", nullable: true),
                Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Contacts", x => x.Id);
                table.ForeignKey(
                    name: "FK_Contacts_AspNetUsers_AppUserId",
                    column: x => x.AppUserId,
                    principalTable: "AspNetUsers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "CategoryContact",
            columns: table => new
            {
                CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                ContactsId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CategoryContact", x => new { x.CategoriesId, x.ContactsId });
                table.ForeignKey(
                    name: "FK_CategoryContact_Categories_CategoriesId",
                    column: x => x.CategoriesId,
                    principalTable: "Categories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_CategoryContact_Contacts_ContactsId",
                    column: x => x.ContactsId,
                    principalTable: "Contacts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_Categories_AppUserId",
            table: "Categories",
            column: "AppUserId");

        migrationBuilder.CreateIndex(
            name: "IX_CategoryContact_ContactsId",
            table: "CategoryContact",
            column: "ContactsId");

        migrationBuilder.CreateIndex(
            name: "IX_Contacts_AppUserId",
            table: "Contacts",
            column: "AppUserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CategoryContact");

        migrationBuilder.DropTable(
            name: "Categories");

        migrationBuilder.DropTable(
            name: "Contacts");
    }
}
