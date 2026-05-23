using FluentAssertions;
using SiteManagement.Domain.Property;
using SiteManagement.Domain.Property.Exceptions;
using SiteManagement.Domain.Property.ValueObjects;
using SiteManagement.Domain.Tests.Doubles;

namespace SiteManagement.Domain.Tests.Property;

/// <summary>
/// Specifies the <see cref="Site"/> aggregate root: the single entity the
/// outside world saves and loads. Owns blocks (which own apartments) and
/// enforces block-name uniqueness inside itself.
/// </summary>
public class SiteTests
{
    private const string SampleSiteName = "Lavender Heights";
    private const string SampleSiteAddress = "Cumhuriyet Mah. No:7";

    [Fact]
    public void Create_AssignsIdAndStartsWithoutBlocks()
    {
        // arrange + act
        var site = Site.Create(SampleSiteName, SampleSiteAddress);

        // assert
        site.Id.Should().NotBe(Guid.Empty);
        site.Name.Should().Be(SampleSiteName);
        site.Address.Should().Be(SampleSiteAddress);
        site.Blocks.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_Throws(string name)
    {
        // act
        var act = () => Site.Create(name, SampleSiteAddress);

        // assert
        act.Should().Throw<InvalidSiteNameException>();
    }

    [Fact]
    public void Create_TooLongName_Throws()
    {
        // arrange
        var tooLong = new string('A', PropertyLimits.SiteNameMaxLength + 1);

        // act
        var act = () => Site.Create(tooLong, SampleSiteAddress);

        // assert
        act.Should().Throw<InvalidSiteNameException>();
    }

    [Fact]
    public void AddBlock_UniqueName_Succeeds()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);

        // act
        site.AddBlock(PropertyDoubles.SampleBlockName("A"));

        // assert
        site.Blocks.Should().ContainSingle().Which.Name.Value.Should().Be("A");
    }

    [Fact]
    public void AddBlock_DuplicateName_Throws()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);
        site.AddBlock(PropertyDoubles.SampleBlockName("A"));

        // act
        var act = () => site.AddBlock(PropertyDoubles.SampleBlockName("a"));

        // assert — case-insensitive uniqueness lives on BlockName equality.
        act.Should().Throw<DuplicateBlockNameException>();
    }

    [Fact]
    public void RemoveBlock_ExistingId_Removes()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);
        var blockId = site.AddBlock(PropertyDoubles.SampleBlockName("A")).Id;

        // act
        site.RemoveBlock(blockId);

        // assert
        site.Blocks.Should().BeEmpty();
    }

    [Fact]
    public void RemoveBlock_UnknownId_Throws()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);

        // act
        var act = () => site.RemoveBlock(Guid.NewGuid());

        // assert
        act.Should().Throw<BlockNotFoundInSiteException>();
    }

    [Fact]
    public void Blocks_CollectionIsReadOnly()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);

        // assert
        site.Blocks.Should().BeAssignableTo<IReadOnlyCollection<Block>>();
        site.Blocks.Should().NotBeAssignableTo<List<Block>>();
    }

    [Fact]
    public void Rename_UpdatesName()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);

        // act
        site.Rename("New Name");

        // assert
        site.Name.Should().Be("New Name");
    }

    [Fact]
    public void GetBlock_ExistingId_ReturnsBlock()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);
        var blockId = site.AddBlock(PropertyDoubles.SampleBlockName("A")).Id;

        // act
        var found = site.GetBlock(blockId);

        // assert
        found.Id.Should().Be(blockId);
    }

    [Fact]
    public void GetBlock_UnknownId_Throws()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);

        // act
        var act = () => site.GetBlock(Guid.NewGuid());

        // assert
        act.Should().Throw<BlockNotFoundInSiteException>();
    }

    [Fact]
    public void Archive_FlagsTheSiteWithATimestamp()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);
        var when = new DateTime(2026, 5, 24, 9, 0, 0, DateTimeKind.Utc);

        // act
        site.Archive(when);

        // assert
        site.IsDeleted.Should().BeTrue();
        site.DeletedOnUtc.Should().Be(when);
    }

    [Fact]
    public void Archive_Twice_KeepsTheFirstTimestamp()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);
        var first = new DateTime(2026, 5, 24, 9, 0, 0, DateTimeKind.Utc);
        site.Archive(first);

        // act
        site.Archive(first.AddDays(1));

        // assert
        site.DeletedOnUtc.Should().Be(first);
    }

    [Fact]
    public void Restore_ClearsTheArchivedFlag()
    {
        // arrange
        var site = Site.Create(SampleSiteName, SampleSiteAddress);
        site.Archive(new DateTime(2026, 5, 24, 9, 0, 0, DateTimeKind.Utc));

        // act
        site.Restore();

        // assert
        site.IsDeleted.Should().BeFalse();
        site.DeletedOnUtc.Should().BeNull();
    }
}
