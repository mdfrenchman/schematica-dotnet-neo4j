using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
{
    [Node]
    public class TestIndexCreate
    {
        [Index(Name="TIC_Prop12")]
        public string Prop1 { get; set; }

        [Index(Name ="TIC_Prop12")]
        public int Prop2 { get; set; }
    }
}
