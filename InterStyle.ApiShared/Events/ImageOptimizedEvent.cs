using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InterStyle.ApiShared.Events;

public sealed record ImageOptimizedEvent(
    Guid ImageId,
    ImageOptimizingStatus OptimizingStatus);


[JsonConverter(typeof(CamelCaseEnumConverter))]
public enum ImageOptimizingStatus
{
    Ready,
    Processing,
}

[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
public sealed class CamelCaseEnumConverter : JsonConverter<ImageOptimizingStatus> 
{
    public override ImageOptimizingStatus Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => Enum.Parse<ImageOptimizingStatus>(reader.GetString()!, true);

    public override void Write(Utf8JsonWriter writer, ImageOptimizingStatus value, JsonSerializerOptions options)
        => writer.WriteStringValue(value.ToString().ToLowerInvariant());
}