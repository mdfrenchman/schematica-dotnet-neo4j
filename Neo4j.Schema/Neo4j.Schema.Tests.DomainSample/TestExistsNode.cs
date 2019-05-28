using Neo4j.Schema.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Schema.Tests.DomainSample
{
    [Node(Label = "TE")]
    public class TestExistsNode
    {
        [NodeKey]
        public string Name { get; set; }

        [NodeKey]
        public string Identity { get; set; }
    }
}
