using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Attributes
{
    /// <summary>
    /// Assign a property to a Nodes NodeKey Constraint.
    /// </summary>
    /// <remarks>
    /// Need to do some further testing around using Inherited to true. 
    /// Makes sense with a lot of cases, but not sure we've thought of all of them.
    /// </remarks>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class NodeKeyAttribute : Attribute { }

}
