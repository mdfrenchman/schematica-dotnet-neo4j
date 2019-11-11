using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample.InheritAbstract
{
    [Node]
    public class TestIndexSubClass : TestIndexParent
    {
        /// <summary>
        /// Prop used to test inheriting a property as part of an index.
        /// </summary>
        [Index(Name = "TISC_P1S1", Label ="TestIndexSubClass")]
        [Index(Name = "TISC_Sub12")]
        public int SubProp1 { get; set; }

        [Index(Name = "TISC_Sub12")]
        [Index(Name = "TISC_Sub2")]
        public string SubProp2 { get; set; }

    }
}
