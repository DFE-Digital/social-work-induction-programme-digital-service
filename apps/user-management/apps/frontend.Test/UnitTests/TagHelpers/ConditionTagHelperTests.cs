using Bogus;
using Dfe.Sww.Ecf.Frontend.TagHelpers;
using Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;
using FluentAssertions;
using GovUk.Frontend.AspNetCore;
using Xunit;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.TagHelpers;

public class ConditionTagHelperTests
{
    [Fact]
    public async Task ProcessAsync_WithTrue_GeneratesExpectedOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("element-tag");
        var faker = new Faker();
        var content = faker.Lorem.Sentence();
        output.Content.SetContent(content);
        var sut = new ConditionTagHelper() { Condition = true };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        var expectedHtml = $"<element-tag>{content}</element-tag>";
        output.ToHtmlString().Should().Be(expectedHtml);
    }

    [Fact]
    public async Task ProcessAsync_WithFalse_SupressesOutput()
    {
        // Arrange
        var (context, output) = TagHelperHelpers.CreateContextAndOutput("element-tag");
        var faker = new Faker();
        var content = faker.Lorem.Sentence();
        output.Content.SetContent(content);
        var sut = new ConditionTagHelper() { Condition = false };

        // Act
        await sut.ProcessAsync(context, output);

        // Assert
        output.ToHtmlString().Should().BeEmpty();
    }
}
