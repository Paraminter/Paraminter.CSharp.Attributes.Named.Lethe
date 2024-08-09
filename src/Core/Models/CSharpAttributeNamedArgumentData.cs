namespace Paraminter.CSharp.Attributes.Named.Lethe.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Arguments.CSharp.Attributes.Named.Models;

internal sealed class CSharpAttributeNamedArgumentData
    : ICSharpAttributeNamedArgumentData
{
    private readonly AttributeArgumentSyntax SyntacticArgument;

    public CSharpAttributeNamedArgumentData(
        AttributeArgumentSyntax syntacticArgument)
    {
        SyntacticArgument = syntacticArgument;
    }

    AttributeArgumentSyntax ICSharpAttributeNamedArgumentData.SyntacticArgument => SyntacticArgument;
}
