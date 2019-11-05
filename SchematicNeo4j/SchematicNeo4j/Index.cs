using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j
{
    public class Index
    {

        // new Schema.Indexes.Index() => Schema.Indexes.Index { _label, _properties };
        // new Schema.Indexes.Index() => Schema.Indexes.Index { _name, _label, _properties };
        // (Schema.Indexes.Index instance).Create(tx|session|driver);
        // (Schema.Indexes.Index instance).Drop(tx|session|driver);
        // (Schema.Indexes.Index instance).Exists(tx|session|driver);

        /// <summary>
        /// _name is used for neo4j v4.0+ prior to v4 it's not stored in the graph.
        /// </summary>
        public string Name { get; set; }
        public string Label { get; set; }
        public string[] Properties { get; set; }

        public Index(){ }
        public Index(string label, string[] properties)
        {
            Label = label;
            Properties = properties;
        }
        public Index(string name, string label, string[] properties)
        {
            Name = name;
            Label = label;
            Properties = properties;
        }

        public void Create(IDriver driver = null) { }

        public void Create(ISession session) { }

        public void Create(ITransaction tx) { }

        public void Drop(IDriver driver = null) { }

        public void Drop(ISession session) { }

        public void Drop(ITransaction tx) { }

        public bool Exists(IDriver driver = null) { return false; }

        public bool Exists(ISession session) { return false; }

        public bool Exists(ITransaction tx) { return false; }

    }
}
