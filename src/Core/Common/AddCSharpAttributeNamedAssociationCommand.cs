namespace Paraminter.CSharp.Attributes.Named.Lethe.Common;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.CSharp.Attributes.Named.Commands;

internal sealed class AddCSharpAttributeNamedAssociationCommand
    : IAddCSharpAttributeNamedAssociationCommand
{
    private readonly string ParameterName;
    private readonly AttributeArgumentSyntax SyntacticArgument;

    public AddCSharpAttributeNamedAssociationCommand(
        string parameterName,
        AttributeArgumentSyntax syntacticArgument)
    {
        ParameterName = parameterName;
        SyntacticArgument = syntacticArgument;
    }

    string IAddCSharpAttributeNamedAssociationCommand.ParameterName => ParameterName;
    AttributeArgumentSyntax IAddCSharpAttributeNamedAssociationCommand.SyntacticArgument => SyntacticArgument;
}
