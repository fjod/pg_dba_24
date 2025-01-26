using System;
using System.Text.Json.Serialization;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class JsonPointConverterAttribute : JsonConverterAttribute
{
    public JsonPointConverterAttribute() : base(typeof(PointJsonConverter))
    {
    }
}