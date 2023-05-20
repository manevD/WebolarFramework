using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Webolar.Framework;

public class EntityCloner
{
    private static readonly List<DictionaryEntity> propertyDictionary = new();

    public static PropertyInfo[] GetProperties(Type entityType)
    {
        PropertyInfo[] result = null;
        foreach (var DE in propertyDictionary.Where(p => p.entityType == entityType))
            result = DE.entityProperties;
        if (result == null)
        {
            result = entityType.GetProperties();
            propertyDictionary.Add(
                new DictionaryEntity
                {
                    entityType = entityType,
                    entityProperties = result
                });
        }

        return result;
    }

    public static void CloneProperties<Entity>(Entity from, Entity to)
    {
        var typeofentity = typeof(Entity);
        var properties = GetProperties(typeof(Entity));
        foreach (var property in properties) property.SetValue(to, property.GetValue(from));
    }

    private struct DictionaryEntity
    {
        public Type entityType;
        public PropertyInfo[] entityProperties;
    }
}