using System.ComponentModel;
using Kaleido;
using Kaleido.Metadata;

public sealed class RecordDescriptorFactory
    : IRecordDescriptorFactory
{
    public RecordDescriptor Create(
        RuntimeRecordMetadata metadata)
    {
        return new RecordDescriptor(
            metadata.Name,
            metadata.Version,
            metadata.Source,

            metadata.Fields
                .Select(CreateField)
                .ToArray(),

            metadata.AllowedQueries
                .Select(x => new AllowedQueryDescriptor(
                    x.Name,
                    x.Description,
                    x.Parameters))
                .ToArray(),

            metadata.Pageable is null
                ? null
                : new PageableDescriptor(
                    metadata.Pageable.DefaultSize,
                    metadata.Pageable.MaxSize));
    }

    private static FieldDescriptor CreateField(
        RuntimeFieldMetadata field)
    {
        return new FieldDescriptor(
            field.Name,

            CreateDataType(field.FieldType),

            field.IsFilterable,

            field.FilterOperators
                .Select(GetDescription)
                .ToArray(),

            field.IsSearchable,

            field.MatchModes
                .Select(GetDescription)
                .ToArray(),

            field.IsSortable
        );
    }

    private static string GetDescription<TEnum>(
    TEnum value)
    where TEnum : Enum
    {
        var field =
            typeof(TEnum)
                .GetField(value.ToString());

        var description =
            field?
                .GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false)
                .Cast<DescriptionAttribute>()
                .FirstOrDefault();

        return description?.Description
            ?? value.ToString();
    }

    private static DataTypeDescriptor CreateDataType(
    Type fieldType)
    {
        var type =
            Nullable.GetUnderlyingType(fieldType)
            ?? fieldType;

        if (type == typeof(string))
            return new("string");

        if (type == typeof(bool))
            return new("boolean");

        if (type == typeof(int) ||
            type == typeof(short))
            return new("integer");

        if (type == typeof(long))
            return new("integer", "int64");

        if (type == typeof(decimal))
            return new("number", "decimal");

        if (type == typeof(double))
            return new("number", "double");

        if (type == typeof(Guid))
            return new("string", "uuid");

        if (type == typeof(DateOnly))
            return new("string", "date");

        if (type == typeof(DateTime))
            return new("string", "date-time");

        if (type == typeof(TimeOnly))
            return new("string", "time");

        if (type.IsEnum)
            return new(
                "string",
                "enum");

        return new("object");
    }
}