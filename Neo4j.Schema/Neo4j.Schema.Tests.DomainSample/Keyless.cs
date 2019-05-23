using Neo4j.Schema.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Schema.Tests.DomainSample
{
    [Node]
    public class Keyless
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
