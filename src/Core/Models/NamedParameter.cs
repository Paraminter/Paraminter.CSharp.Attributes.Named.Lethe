namespace Paraminter.CSharp.Attributes.Named.Lethe.Models;

using Paraminter.Parameters.Named.Models;

internal sealed class NamedParameter
    : INamedParameter
{
    private readonly string Name;

    public NamedParameter(
        string name)
    {
        Name = name;
    }

    string INamedParameter.Name => Name;
}
