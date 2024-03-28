using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Attributes
{
    /// <summary>
    /// Indicates a Class is a Graph Relationship.
    /// </summary>
    /// <remarks>
    /// The class name, properly formatted for convention, will be used for the Relationship TYPE_NAME unless a TypeName is set.
    /// </remarks>
    /// <example>
    /// `[Relationship(TypeName="ACTED_IN")]` | `[Relationship] public class ActedIn`
    /// </example>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class RelationshipAttribute : Attribute
    {
        public string TypeName;

        public override string ToString()
        {
            return TypeName;
        }
    }
}
