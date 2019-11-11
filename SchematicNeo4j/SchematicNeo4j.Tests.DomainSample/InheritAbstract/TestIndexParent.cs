using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample.InheritAbstract
{
    [Node]
    public class TestIndexParent
    {
        /// <summary>
        /// Prop used to test inheriting a property as part of an index.
        /// IsAbstract Prevents the Index from being executed on the Parent, but falls through to the child to be included.
        /// </summary>
        [Index(Name = "TISC_P1S1", Label = "TestIndexSubClass", IsAbstract = true)]
        [Index(Name = "TIP_P12")]
        public int ParentProp1 {get; set;}

        [Index(Name = "TIP_P12")]
        [Index(Name = "TIP_P2")]
        public string ParentProp2 { get; set; }
    }
}
