using System.Reflection;
using FluentAssertions;

namespace SiteManagement.E2E.Tests.Configuration;

/// <summary>
/// Pure helper tests for the platform <c>DATABASE_URL</c> → Npgsql connection
/// string converter. Lives in E2E.Tests because that project already has the
/// API assembly available; the helper itself is internal in the API project.
/// </summary>
public class DatabaseUrlExtensionsTests
{
    private static string Convert(string url)
    {
        var asm = typeof(Program).Assembly;
        var type = asm.GetType("SiteManagement.Api.Configuration.DatabaseUrlExtensions", throwOnError: true)!;
        var method = type.GetMethod("ConvertToNpgsqlConnectionString",
            BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)!;
        return (string)method.Invoke(null, [url])!;
    }

    /// <summary>Standard Railway-style URL produces the expected Npgsql connection string.</summary>
    [Fact]
    public void Convert_StandardUrl_ProducesNpgsqlConnectionString()
    {
        // arrange
        const string url = "postgresql://app_user:secret@db.railway.internal:5432/sitemgmt";

        // act
        var connectionString = Convert(url);

        // assert
        connectionString.Should().Contain("Host=db.railway.internal");
        connectionString.Should().Contain("Port=5432");
        connectionString.Should().Contain("Database=sitemgmt");
        connectionString.Should().Contain("Username=app_user");
        connectionString.Should().Contain("Password=secret");
        connectionString.Should().Contain("SSL Mode=Require");
    }

    /// <summary>Password with URL-encoded special characters round-trips correctly.</summary>
    [Fact]
    public void Convert_PasswordWithEncodedChars_IsDecoded()
    {
        // arrange (password is "p@ss/word")
        const string url = "postgresql://app_user:p%40ss%2Fword@host.example:5432/db";

        // act
        var connectionString = Convert(url);

        // assert
        connectionString.Should().Contain("Password=p@ss/word");
    }

    /// <summary>When the URL omits a port, the converter falls back to 5432.</summary>
    [Fact]
    public void Convert_NoExplicitPort_DefaultsTo5432()
    {
        // arrange
        const string url = "postgresql://u:p@host.example/db";

        // act
        var connectionString = Convert(url);

        // assert
        connectionString.Should().Contain("Port=5432");
    }
}
