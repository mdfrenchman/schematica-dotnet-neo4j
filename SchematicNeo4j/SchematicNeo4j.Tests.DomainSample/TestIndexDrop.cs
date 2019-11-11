using SchematicNeo4j.Attributes;
using System;

namespace SchematicNeo4j.Tests.DomainSample
{
    public class TestIndexDrop
    {
        [Index(Name = "TID_Prop1")]
        [Index(Name = "TID_Prop12")]
        public string Prop1 { get; set; }

        [Index(Name = "TID_Prop12")]
        [Index(Name = "TID_Prop23")]
        public int Prop2 { get; set; }

        [Index(Name = "TID_Prop23")]
        public DateTimeOffset? Prop3 { get; set; }
    }
}
