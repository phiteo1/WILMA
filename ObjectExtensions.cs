// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Text.Json;


internal static class ObjectExtensions
{
    public static TObject DumpToConsole<TObject>(this TObject @object)
    {
        var output = "NULL";
        if (@object != null)
        {
            output = JsonSerializer.Serialize(@object, new JsonSerializerOptions
            {
                WriteIndented = true
            });
        }
        
        Debug.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");
        return @object;
    }
}