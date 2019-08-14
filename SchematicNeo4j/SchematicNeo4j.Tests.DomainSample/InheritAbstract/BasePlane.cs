using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample.InheritAbstract
{
    /// <summary>
    /// Class to test that abstract classes don't get constraints created when calling Schema.Initialize(Assembly).
    /// But the subclasses will create their own constraints and can incorporate abstract super class properties tagged as node keys.
    /// </summary>
    public abstract class BasePlane
    {
        [NodeKey]
        public string Name { get; set; }
        [NodeKey]
        public string TailNumber { get; set; }

    }
}
