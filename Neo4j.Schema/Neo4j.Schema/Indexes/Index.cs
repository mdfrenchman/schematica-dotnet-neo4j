using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Schema.Indexes
{
    public class Index
    {
        public void Create(IDriver driver = null) { }

        public void Create(ISession session) { }

        public void Create(ITransaction tx) { }

        // new Schema.Indexes.Index() => Schema.Indexes.Index { Label, Properties };
        // (Schema.Indexes.Index instance).Create(tx|session|driver);
    }
}
