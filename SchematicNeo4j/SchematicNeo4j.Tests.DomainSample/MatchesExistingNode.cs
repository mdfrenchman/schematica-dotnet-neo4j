using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
{
    [Node(Label = "ME")]
    public class MatchesExistingNode
    {
        [NodeKey]
        public string Name { get; set; }
    }
}
