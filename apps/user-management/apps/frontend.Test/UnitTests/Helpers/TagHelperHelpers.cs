using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Dfe.Sww.Ecf.Frontend.Test.UnitTests.Helpers;

public static class TagHelperHelpers
{
    public static Tuple<TagHelperContext, TagHelperOutput> CreateContextAndOutput(string tagName)
    {
        var context = new TagHelperContext(tagName, [], new Dictionary<object, object>(), "test");

        var output = new TagHelperOutput(
            tagName,
            [],
            (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent())
        );

        return Tuple.Create(context, output);
    }
}
