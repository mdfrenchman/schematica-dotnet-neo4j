using Neo4j.Schema.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Schema.Tests.DomainSample
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
