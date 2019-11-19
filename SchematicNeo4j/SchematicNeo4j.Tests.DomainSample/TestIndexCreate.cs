using SchematicNeo4j.Attributes;
using System;

namespace SchematicNeo4j.Tests.DomainSample
{
    [Node]
    public class TestIndexCreate
    {
        [Index(Name = "TIC_Prop1")]
        [Index(Name = "TIC_Prop12")]
        public string Prop1 { get; set; }

        [Index(Name = "TIC_Prop12")]
        [Index(Name = "TIC_Prop23")]
        public int Prop2 { get; set; }
        
        [Index(Name = "TIC_Prop23")]
        public DateTimeOffset? Prop3 { get; set; }
    }
}
