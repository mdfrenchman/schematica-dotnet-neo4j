using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class IndexAttribute : Attribute 
    {
        public string Name;
        public string Label;
    }
}
