using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Reflection;

namespace SolucionDA.Helpers;

public static class EntityMetadataHelper
{
    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _keyCache = new();
    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _insertableCache = new();
    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _updatableCache = new();
    private static readonly ConcurrentDictionary<string, Delegate> _compiledSetters = new();

    public static List<PropertyInfo> GetKeyProperties<T>() =>
        _keyCache.GetOrAdd(typeof(T), t => t.GetProperties()
            .Where(p => p.GetCustomAttribute<KeyAttribute>() != null)
            .ToList());

    public static List<PropertyInfo> GetInsertableProperties<T>(T entity) =>
        _insertableCache.GetOrAdd(typeof(T), t => t.GetProperties()
            .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
            .Where(p =>
            {
                var isKey = p.GetCustomAttribute<KeyAttribute>() != null;
                if (isKey && p.PropertyType == typeof(int))
                    return (int?)p.GetValue(entity) != 0;
                return !isKey;
            }).ToList());

    public static List<PropertyInfo> GetUpdatableProperties<T>() =>
        _updatableCache.GetOrAdd(typeof(T), t => t.GetProperties()
            .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null &&
                        p.GetCustomAttribute<KeyAttribute>() == null)
            .ToList());

    public static void SetKeyProperty<T>(T entity, string propertyName, object value)
    {
        var key = $"{typeof(T).FullName}_{propertyName}";
        if (!_compiledSetters.TryGetValue(key, out var setter))
        {
            var paramEntity = Expression.Parameter(typeof(T), "entity");
            var paramValue = Expression.Parameter(typeof(object), "value");
            var property = Expression.Property(paramEntity, propertyName);
            var convert = Expression.Convert(paramValue, property.Type);
            var assign = Expression.Assign(property, convert);
            var lambda = Expression.Lambda<Action<T, object>>(assign, paramEntity, paramValue);
            setter = lambda.Compile();
            _compiledSetters[key] = setter;
        }

        ((Action<T, object>)setter)(entity, value);
    }
}
