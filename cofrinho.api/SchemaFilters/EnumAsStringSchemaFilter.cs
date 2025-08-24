using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace cofrinho_api.SchemaFilters;

public class EnumAsStringSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        var type = Nullable.GetUnderlyingType(context.Type) ?? context.Type;
        if (!type.IsEnum) return;

        schema.Type = "string";
        schema.Format = null; // remove "int32" quando vem enum numérico

        schema.Enum = Enum.GetNames(type)
            .Select(n => (IOpenApiAny)new OpenApiString(n))
            .ToList();

        // Opcional: descrição com pares NAME = valor numérico
        var pairs = Enum.GetValues(type)
            .Cast<Enum>()
            .Select(e => $"{e} = {Convert.ToInt64(e)}");
        var legend = string.Join(", ", pairs);
        schema.Description = string.IsNullOrWhiteSpace(schema.Description)
            ? legend
            : $"{schema.Description} ({legend})";
    }
}