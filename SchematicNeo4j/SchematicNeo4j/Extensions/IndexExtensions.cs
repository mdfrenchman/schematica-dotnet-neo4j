using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SchematicNeo4j.Attributes;
using Neo4j.Driver.V1;

namespace SchematicNeo4j
{
    public static class IndexExtensions
    {
        public static List<Index> Indexes(this Type type)
        {
            var nodeType = type;
            PropertyInfo[] propertyInfo = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = propertyInfo.SelectMany(pi =>
                ((IndexAttribute[])pi.GetCustomAttributes(typeof(IndexAttribute), true)).Where(ai => !ai.IsAbstract)
                    .Select(ia => (name: ((IndexAttribute)ia).Name, label: ((IndexAttribute)ia).Label, prop: pi.Name))
                ).GroupBy(t => (t.name, t.label))
                .Select(grouping => new Index(
                        name: grouping.Key.name,
                        label: String.IsNullOrWhiteSpace(grouping.Key.label) ? type.Label() : grouping.Key.label,
                        properties: grouping.Select(t => t.prop).ToArray()
                        )
                ).ToList();
            return result;
        }
    }
}
