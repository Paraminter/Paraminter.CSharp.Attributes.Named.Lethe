namespace Paraminter.CSharp.Attributes.Named.Lethe.Queries;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;

/// <summary>Represents data used to associate syntactic C# named attribute arguments.</summary>
public interface IAssociateSyntacticCSharpAttributeNamedData
{
    /// <summary>The syntactic C# named attribute arguments, possibly also containing syntactic C# constructor attribute arguments.</summary>
    public abstract IReadOnlyList<AttributeArgumentSyntax> SyntacticArguments { get; }
}
