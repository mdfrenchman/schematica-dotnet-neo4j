using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Indexes
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
        private string _name { get; set; }
        private string _label;
        private string[] _properties { get; set; }

        public Index(){ }
        public Index(string label, string[] properties)
        {
            _label = label;
            _properties = properties;
        }
        public Index(string name, string label, string[] properties)
        {
            _name = name;
            _label = label;
            _properties = properties;
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
