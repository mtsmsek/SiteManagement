using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SiteManagement.Infrastructure.Persistence.Migrations
{
    /// <summary>
    /// No-op migration. We mapped the npgsql <c>xmin</c> system column as an
    /// EF concurrency token on Sites, Blocks and Residents — but xmin is a
    /// Postgres-managed system column that always exists, so EF must NOT
    /// emit AddColumn/DropColumn against it. The scaffolder didn't honour
    /// npgsql's "skip system columns" rule for shadow properties, so the
    /// generated SQL is suppressed here while keeping the model snapshot
    /// entry needed to track schema state.
    /// </summary>
    public partial class ConcurrencyTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // intentionally empty — xmin is a Postgres system column
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // intentionally empty — xmin is a Postgres system column
        }
    }
}
