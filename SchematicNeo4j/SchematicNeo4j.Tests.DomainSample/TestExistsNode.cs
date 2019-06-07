using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
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
