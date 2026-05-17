using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using FluentAssertions;
using SiteManagement.Application.Shared.Resources;
using SiteManagement.Application.Shared.Validation;

namespace SiteManagement.ArchitectureTests;

/// <summary>
/// Guards the resource layer: every <see cref="ErrorMessageKeys"/> and
/// <see cref="ValidationMessages"/> constant must have a matching entry in
/// <em>both</em> the Turkish and English resource files. A missing entry
/// would silently fall back to the raw key string at runtime — these tests
/// fail the build instead, so the drift never reaches a user.
/// </summary>
public class ResourceIntegrityTests
{
    private const string TrCulture = "tr";
    private const string EnCulture = "en";

    /// <summary>Every key declared in <see cref="ErrorMessageKeys"/> must exist in tr + en resources.</summary>
    [Theory]
    [InlineData(TrCulture)]
    [InlineData(EnCulture)]
    public void ErrorMessageKeys_AllResolveInCulture(string culture)
    {
        // arrange
        var keys = StringConstantsOf(typeof(ErrorMessageKeys));

        // act
        var missing = keys.Where(k => !ResourceHasKey(typeof(ErrorMessages), culture, k)).ToArray();

        // assert
        missing.Should().BeEmpty(
            "ErrorMessages.{0}.resx is missing keys: {1}",
            culture,
            string.Join(", ", missing));
    }

    /// <summary>Every key declared in <see cref="ValidationMessages"/> must exist in tr + en resources.</summary>
    [Theory]
    [InlineData(TrCulture)]
    [InlineData(EnCulture)]
    public void ValidationMessageKeys_AllResolveInCulture(string culture)
    {
        // arrange
        var keys = StringConstantsOf(typeof(ValidationMessages));

        // act
        var missing = keys.Where(k => !ResourceHasKey(typeof(ValidationMessages), culture, k)).ToArray();

        // assert
        missing.Should().BeEmpty(
            "ValidationMessages.{0}.resx is missing keys: {1}",
            culture,
            string.Join(", ", missing));
    }

    /// <summary>Both resource files must declare the same key set (no drift between cultures).</summary>
    [Fact]
    public void ErrorMessages_TurkishAndEnglishHaveSameKeySet()
    {
        // arrange + act + assert
        AssertSameKeySet(typeof(ErrorMessages), "ErrorMessages");
    }

    /// <summary>Same drift check for the validation resources.</summary>
    [Fact]
    public void ValidationMessages_TurkishAndEnglishHaveSameKeySet()
    {
        // arrange + act + assert
        AssertSameKeySet(typeof(ValidationMessages), "ValidationMessages");
    }

    /// <summary>True when the marker's <c>.{culture}.resx</c> contains the given key.</summary>
    private static bool ResourceHasKey(Type markerType, string culture, string key)
    {
        var rm = new ResourceManager(markerType);
        var ci = new CultureInfo(culture);
        return !string.IsNullOrEmpty(rm.GetString(key, ci));
    }

    /// <summary>Reads every key for a marker in a given culture using the ResourceManager API.</summary>
    private static HashSet<string> ReadKeys(Type markerType, string culture)
    {
        var rm = new ResourceManager(markerType);
        var ci = new CultureInfo(culture);
        // tryParents = false ensures we get only the culture-specific resx, not the neutral fallback.
        using var set = rm.GetResourceSet(ci, createIfNotExists: true, tryParents: false)
            ?? throw new InvalidOperationException(
                $"Resource set not found for {markerType.FullName} ({culture}).");

        return set
            .Cast<DictionaryEntry>()
            .Select(e => (string)e.Key)
            .ToHashSet(StringComparer.Ordinal);
    }

    /// <summary>Compares the key sets of the tr and en resource files.</summary>
    private static void AssertSameKeySet(Type markerType, string baseName)
    {
        var trKeys = ReadKeys(markerType, TrCulture);
        var enKeys = ReadKeys(markerType, EnCulture);

        var onlyInTr = trKeys.Except(enKeys).ToArray();
        var onlyInEn = enKeys.Except(trKeys).ToArray();

        onlyInTr.Should().BeEmpty(
            "{0}.tr.resx has keys missing in en.resx: {1}",
            baseName, string.Join(", ", onlyInTr));
        onlyInEn.Should().BeEmpty(
            "{0}.en.resx has keys missing in tr.resx: {1}",
            baseName, string.Join(", ", onlyInEn));
    }

    /// <summary>Reflects all <c>public const string</c> values declared on a type.</summary>
    private static IEnumerable<string> StringConstantsOf(Type type)
        => type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f is { IsLiteral: true, IsInitOnly: false } && f.FieldType == typeof(string))
            .Select(f => (string)f.GetRawConstantValue()!);
}
