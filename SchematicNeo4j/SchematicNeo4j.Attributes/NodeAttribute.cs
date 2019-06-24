using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Attributes
{
    /// <summary>
    /// Indicates a Class is a Graph Node.
    /// </summary>
    /// <remarks>
    /// The class name will be used for the Label unless a Label property is set.
    /// </remarks>
    /// <example>
    /// [Node(Label="Vehicle:Car")] | [Node(Label="Person")] | [Node]
    /// </example>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class NodeAttribute : Attribute
    {
        public string Label;
    }
}
