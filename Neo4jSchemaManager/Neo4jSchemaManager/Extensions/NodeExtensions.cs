using Neo4j.Schema.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Neo4jSchemaManager
{
    public static class NodeExtensions
    {
        public static string Label(this Type type)
        {
            // Check Label value in the Node Attribute.
            NodeAttribute attr = (NodeAttribute)type.GetCustomAttribute(typeof(NodeAttribute), false);
            if (!(attr is null) && !String.IsNullOrEmpty(attr.Label))
                return attr.Label;
            // Get the ClassName if there is no Node Attribute with a Label defined.
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int iBacktick = friendlyName.IndexOf('`');
                if (iBacktick > 0)
                {
                    friendlyName = friendlyName.Remove(iBacktick);
                }
                friendlyName += $"<{string.Join(",", type.GetGenericArguments().Select(p => type.Label()))}>";
            }

            return friendlyName;
        }
    }
}
