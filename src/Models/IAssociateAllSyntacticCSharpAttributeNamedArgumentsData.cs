namespace Paraminter.CSharp.Attributes.Named.Lethe.Models;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Paraminter.Models;

using System.Collections.Generic;

/// <summary>Represents data used to associate all syntactic C# named attribute arguments with parameters.</summary>
public interface IAssociateAllSyntacticCSharpAttributeNamedArgumentsData
    : IAssociateAllArgumentsData
{
    /// <summary>The syntactic C# named attribute arguments, possibly also containing syntactic C# constructor attribute arguments.</summary>
    public abstract IReadOnlyList<AttributeArgumentSyntax> SyntacticArguments { get; }
}
