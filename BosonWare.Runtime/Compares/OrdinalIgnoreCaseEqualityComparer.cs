using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace BosonWare.Compares;

public sealed class OrdinalIgnoreCaseEqualityComparer : IEqualityComparer<string>
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(string? x, string? y)
    {
        if (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y))
            return true;

        return x is not null && x.Equals(y, StringComparison.OrdinalIgnoreCase);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHashCode([DisallowNull] string obj)
        => obj.GetHashCode(StringComparison.OrdinalIgnoreCase);
}
