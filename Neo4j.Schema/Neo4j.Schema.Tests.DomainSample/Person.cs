using Neo4j.Schema.Attributes;
using System;

namespace Neo4j.Schema.Tests.DomainSample
{
    public class Person
    {
        [NodeKey]
        public string Name { get; set; }

    }
}
