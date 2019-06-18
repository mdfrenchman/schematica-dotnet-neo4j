using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
{
    [Node]
    public class ThatThing
    {
        [NodeKey]
        public string Name { get; set; }
        [NodeKey]
        public string Identity { get; set; }
    }

    [Node]
    public class That
    {
        [NodeKey]
        public string Name { get; set; }
        [NodeKey]
        public string Identity { get; set; }
    }
}
