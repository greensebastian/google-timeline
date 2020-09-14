using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataAccess.Migrations
{
    public partial class AddIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimelineDataId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatitudeE7 = table.Column<int>(nullable: false),
                    LongitudeE7 = table.Column<int>(nullable: false),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PlaceId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimelineData",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimelineData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationVisits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemanticType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LocationId = table.Column<int>(nullable: false),
                    DbActivitySegmentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationVisits_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceVisits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationVisitId = table.Column<int>(nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    Confidence = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CenterLatE7 = table.Column<int>(nullable: false),
                    CenterLngE7 = table.Column<int>(nullable: false),
                    DbPlaceVisitId = table.Column<int>(nullable: true),
                    TimelineDataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceVisits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceVisits_PlaceVisits_DbPlaceVisitId",
                        column: x => x.DbPlaceVisitId,
                        principalTable: "PlaceVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceVisits_LocationVisits_LocationVisitId",
                        column: x => x.LocationVisitId,
                        principalTable: "LocationVisits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceVisits_TimelineData_TimelineDataId",
                        column: x => x.TimelineDataId,
                        principalTable: "TimelineData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Waypoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LatitudeE7 = table.Column<int>(nullable: false),
                    LongitudeE7 = table.Column<int>(nullable: false),
                    DbActivitySegmentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Waypoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActivitySegments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartWaypointId = table.Column<int>(nullable: true),
                    EndWaypointId = table.Column<int>(nullable: true),
                    StartDateTime = table.Column<DateTime>(nullable: false),
                    EndDateTime = table.Column<DateTime>(nullable: false),
                    Confidence = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ActivityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Distance = table.Column<int>(nullable: false),
                    TimelineDataId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivitySegments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivitySegments_Waypoints_EndWaypointId",
                        column: x => x.EndWaypointId,
                        principalTable: "Waypoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivitySegments_Waypoints_StartWaypointId",
                        column: x => x.StartWaypointId,
                        principalTable: "Waypoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ActivitySegments_TimelineData_TimelineDataId",
                        column: x => x.TimelineDataId,
                        principalTable: "TimelineData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TimelineDataId",
                table: "AspNetUsers",
                column: "TimelineDataId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySegments_EndWaypointId",
                table: "ActivitySegments",
                column: "EndWaypointId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySegments_StartWaypointId",
                table: "ActivitySegments",
                column: "StartWaypointId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivitySegments_TimelineDataId",
                table: "ActivitySegments",
                column: "TimelineDataId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationVisits_DbActivitySegmentId",
                table: "LocationVisits",
                column: "DbActivitySegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationVisits_LocationId",
                table: "LocationVisits",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceVisits_DbPlaceVisitId",
                table: "PlaceVisits",
                column: "DbPlaceVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceVisits_LocationVisitId",
                table: "PlaceVisits",
                column: "LocationVisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceVisits_TimelineDataId",
                table: "PlaceVisits",
                column: "TimelineDataId");

            migrationBuilder.CreateIndex(
                name: "IX_Waypoints_DbActivitySegmentId",
                table: "Waypoints",
                column: "DbActivitySegmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TimelineData_TimelineDataId",
                table: "AspNetUsers",
                column: "TimelineDataId",
                principalTable: "TimelineData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationVisits_ActivitySegments_DbActivitySegmentId",
                table: "LocationVisits",
                column: "DbActivitySegmentId",
                principalTable: "ActivitySegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Waypoints_ActivitySegments_DbActivitySegmentId",
                table: "Waypoints",
                column: "DbActivitySegmentId",
                principalTable: "ActivitySegments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TimelineData_TimelineDataId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivitySegments_Waypoints_EndWaypointId",
                table: "ActivitySegments");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivitySegments_Waypoints_StartWaypointId",
                table: "ActivitySegments");

            migrationBuilder.DropTable(
                name: "PlaceVisits");

            migrationBuilder.DropTable(
                name: "LocationVisits");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Waypoints");

            migrationBuilder.DropTable(
                name: "ActivitySegments");

            migrationBuilder.DropTable(
                name: "TimelineData");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TimelineDataId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TimelineDataId",
                table: "AspNetUsers");
        }
    }
}
