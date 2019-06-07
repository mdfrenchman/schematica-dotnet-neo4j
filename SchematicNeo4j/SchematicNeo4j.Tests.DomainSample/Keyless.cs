using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
{
    [Node]
    public class Keyless
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
