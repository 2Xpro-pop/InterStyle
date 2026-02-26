using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;

namespace InterStyle.Shared;

public static class ImmutableArrayExtension
{
    public static ImmutableArray<T> ToImmutableArrayUnsafe<T>(this T[] array)
    {
        return Unsafe.As<T[], ImmutableArray<T>>(ref array);
    }
}
