using Neo4j.Schema.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Schema.Tests.DomainSample
{
    [Node(Label = "ME")]
    public class MatchesExistingNode
    {
        [NodeKey]
        public string Name { get; set; }
    }
}
