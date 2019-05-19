using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4jSchemaManager.CustomAttributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NodeKeyAttribute : Attribute { }

}
