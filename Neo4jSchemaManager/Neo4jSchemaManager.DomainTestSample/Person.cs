using Neo4j.Schema.Attributes;
using System;

namespace Neo4jSchemaManager.DomainTestSample
{
    public class Person
    {
        [NodeKey]
        public string Name { get; set; }

    }
}
