using SchematicNeo4j.Attributes;
using System;

namespace SchematicNeo4j.Tests.DomainSample
{
    public class Person
    {
        [NodeKey]
        public string Name { get; set; }

        [Index(Name = "PersonIndex")]
        public int Age { get; set; }

    }
}
