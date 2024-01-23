using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SchematicNeo4j.Attributes;

namespace SchematicNeo4j
{
    public static class NodeExtensions
    {
        public static List<string> NodeKey(this Type type)
        {
            var nodeType = type;
            PropertyInfo[] propertyInfo = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = propertyInfo.Where(p => p.GetCustomAttributes(typeof(NodeKeyAttribute), true).Any()).Select(p => p.Name).ToList();
            return result;
        }

        /// <summary>
        /// Gets the Node Attribute Label property from the type;
        /// or the type of generic T as a string
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Label(this Type type)
        {
            // Check Label value in the Node Attribute.
            NodeAttribute attr = (NodeAttribute)type.GetCustomAttribute(typeof(NodeAttribute), false);
            return (!(attr is null) && !String.IsNullOrEmpty(attr.Label)) ?
                attr.Label :
            // Get the ClassName if there is no Node Attribute with a Label defined.
                type.Name;

        }

        public static string NodeKeyName(this Type type)
        {
            // Check NodeKey value in the Node Attribute.
            NodeAttribute attr = (NodeAttribute)type.GetCustomAttribute(typeof(NodeAttribute), false);

            // if the NodeKey is property is set on the attribute, then use it.
            if (!string.IsNullOrWhiteSpace(attr?.NodeKey))
                return attr.NodeKey;

            // else use `nkPrimaryLabel`
            var primaryLabel = (!(attr is null) && !String.IsNullOrEmpty(attr.Label)) ?
                attr.Label.Split(':')[0] :
            // Get the ClassName if there is no Node Attribute with a Label defined.
                type.Name;
            return $"nk{primaryLabel}";

        }

    }
}
