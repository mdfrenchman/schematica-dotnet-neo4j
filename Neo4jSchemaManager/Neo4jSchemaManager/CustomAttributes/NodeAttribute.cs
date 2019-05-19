using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4jSchemaManager.CustomAttributes
{
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class NodeAttribute : Attribute
    {
        public string Label;
    }
}
