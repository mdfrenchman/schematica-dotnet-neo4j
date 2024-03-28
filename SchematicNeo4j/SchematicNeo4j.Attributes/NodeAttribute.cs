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
    /// The node key will be `nk`+class name or the first label in the Label property.
    /// </remarks>
    /// <example>
    /// [Node(Label="Vehicle:Car")] | [Node(Label="Person")] | [Node]
    /// </example>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class NodeAttribute : Attribute
    {
        /// <summary>
        /// The LabelName of the node. Supports multilabel
        /// </summary>
        /// <remarks>
        /// Defaults to the class name
        /// </remarks>
        /// <example>
        /// Label="Vehicle:Car" or Label="Person"
        /// </example>
        public string Label;

        /// <summary>
        /// The Name of the NodeKey Constraint if any, defaults to `nk`+firstLabel.
        /// </summary>
        public string NodeKey;

        public override string ToString()
        {
            return Label;
        }
    }
}
