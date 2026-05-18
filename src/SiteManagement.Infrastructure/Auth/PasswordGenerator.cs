using System.Security.Cryptography;
using SiteManagement.Application.Abstractions.Auth;

namespace SiteManagement.Infrastructure.Auth;

/// <summary>
/// Cryptographically random password generator that satisfies the Identity
/// password policy (lower, upper, digit; configurable length). Used by the
/// admin-driven resident registration flow.
/// </summary>
public sealed class PasswordGenerator : IPasswordGenerator
{
    private const string Lowercase = "abcdefghijkmnopqrstuvwxyz";  // l excluded
    private const string Uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";   // I, O excluded
    private const string Digits = "23456789";                       // 0, 1 excluded
    private const int Length = 12;

    /// <inheritdoc />
    public string Generate()
    {
        // Guarantee at least one char from each required class so the
        // result passes IdentityPasswordPolicy in one shot.
        var chars = new char[Length];
        chars[0] = Pick(Lowercase);
        chars[1] = Pick(Uppercase);
        chars[2] = Pick(Digits);

        const string AllClasses = Lowercase + Uppercase + Digits;
        for (var i = 3; i < Length; i++)
        {
            chars[i] = Pick(AllClasses);
        }

        // Fisher-Yates shuffle so the guaranteed chars don't always sit at
        // the start of the output.
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }

    private static char Pick(string pool) => pool[RandomNumberGenerator.GetInt32(pool.Length)];
}
