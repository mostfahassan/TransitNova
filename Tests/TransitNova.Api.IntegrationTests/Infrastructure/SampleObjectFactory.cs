using System.Collections;
using System.Reflection;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class SampleObjectFactory
{
    internal static object? Create(Type type)
    {
        return Create(type, new HashSet<Type>());
    }

    private static object? Create(Type type, HashSet<Type> stack)
    {
        var unwrappedType = Nullable.GetUnderlyingType(type) ?? type;

        if (unwrappedType == typeof(string))
            return "sample";
        if (unwrappedType == typeof(Guid))
            return Guid.Parse("33333333-3333-3333-3333-333333333333");
        if (unwrappedType == typeof(DateTime))
            return new DateTime(2026, 7, 9, 10, 30, 0, DateTimeKind.Utc);
        if (unwrappedType == typeof(DateTimeOffset))
            return new DateTimeOffset(2026, 7, 9, 10, 30, 0, TimeSpan.Zero);
        if (unwrappedType == typeof(DateOnly))
            return new DateOnly(2026, 7, 9);
        if (unwrappedType == typeof(TimeOnly))
            return new TimeOnly(10, 30, 15);
        if (unwrappedType == typeof(decimal))
            return 123.45m;
        if (unwrappedType == typeof(double))
            return 123.45d;
        if (unwrappedType == typeof(float))
            return 123.45f;
        if (unwrappedType == typeof(byte))
            return (byte)7;
        if (unwrappedType == typeof(short))
            return (short)7;
        if (unwrappedType == typeof(int))
            return 7;
        if (unwrappedType == typeof(long))
            return 7L;
        if (unwrappedType == typeof(sbyte))
            return (sbyte)7;
        if (unwrappedType == typeof(ushort))
            return (ushort)7;
        if (unwrappedType == typeof(uint))
            return 7U;
        if (unwrappedType == typeof(ulong))
            return 7UL;
        if (unwrappedType == typeof(bool))
            return true;
        if (unwrappedType.IsEnum)
            return Enum.GetValues(unwrappedType).GetValue(0);

        if (TryCreateCollection(unwrappedType, stack, out var collection))
            return collection;

        if (!stack.Add(unwrappedType))
            return null;

        try
        {
            var instance = CreateObjectInstance(unwrappedType, stack);
            if (instance is null)
                return null;

            foreach (var property in unwrappedType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.SetMethod is null || property.GetIndexParameters().Length > 0)
                    continue;

                var value = Create(property.PropertyType, stack);
                property.SetValue(instance, value);
            }

            return instance;
        }
        finally
        {
            stack.Remove(unwrappedType);
        }
    }

    private static object? CreateObjectInstance(Type type, HashSet<Type> stack)
    {
        var parameterlessConstructor = type.GetConstructor(Type.EmptyTypes);
        if (parameterlessConstructor is not null)
            return Activator.CreateInstance(type);

        var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(candidate => candidate.GetParameters().Length)
            .FirstOrDefault();

        if (constructor is null)
            return null;

        var arguments = constructor.GetParameters()
            .Select(parameter => Create(parameter.ParameterType, stack))
            .ToArray();

        return constructor.Invoke(arguments);
    }

    private static bool TryCreateCollection(Type type, HashSet<Type> stack, out object? collection)
    {
        collection = null;

        if (type == typeof(string))
            return false;

        if (type.IsArray)
        {
            var elementType = type.GetElementType()!;
            var array = Array.CreateInstance(elementType, 1);
            array.SetValue(Create(elementType, stack), 0);
            collection = array;
            return true;
        }

        var dictionaryType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
            ? type
            : type.GetInterfaces().FirstOrDefault(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IDictionary<,>));

        if (dictionaryType is not null)
        {
            var genericArguments = dictionaryType.GetGenericArguments();
            var keyType = genericArguments[0];
            var valueType = genericArguments[1];
            var concreteDictionaryType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            var dictionary = (IDictionary)Activator.CreateInstance(concreteDictionaryType)!;
            dictionary.Add(Create(keyType, stack)!, Create(valueType, stack));
            collection = dictionary;
            return true;
        }

        var enumerableType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            ? type
            : type.GetInterfaces().FirstOrDefault(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableType is null)
            return false;

        var elementTypeArgument = enumerableType.GetGenericArguments()[0];
        var listType = typeof(List<>).MakeGenericType(elementTypeArgument);
        var list = (IList)Activator.CreateInstance(listType)!;
        list.Add(Create(elementTypeArgument, stack));
        collection = list;
        return true;
    }
}
