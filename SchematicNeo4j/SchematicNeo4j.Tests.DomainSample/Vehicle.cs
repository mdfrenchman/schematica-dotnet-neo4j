using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
{
    [Node(Label = "Car")]
    public class Vehicle
    {
        [NodeKey]
        public string Make { get; set; }

        [NodeKey]
        public string Model { get; set; }

        [NodeKey]
        public int ModelYear { get; set; }
    }
}
