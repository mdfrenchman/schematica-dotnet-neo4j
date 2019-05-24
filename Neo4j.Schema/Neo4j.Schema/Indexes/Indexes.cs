using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Schema.Indexes
{
    public static class Indexes
    {

        // Schema.Indexes.Get(type) => List<Schema.Indexes.Index>
        // Schema.Indexes.Create(type, tx|session|driver) => Creates missing indexes for the type.
        // Schema.Indexes.Drop(type, tx|session|driver) => drop all indexes defined on the type.
        // Schema.Indexes.Exists(type, tx|session|driver) => bool if any index exists for the provided type. Not sure how useful this is at Indexes level.

        public static List<Index> Get(Type type)
        {
            return new List<Index>();
        }

        public static void Create(Type type, IDriver driver = null) { }
        public static void Create(Type type, ISession session) { }
        public static void Create(Type type, ITransaction tx) { }

        public static void Drop(Type type, IDriver driver = null) { }
        public static void Drop(Type type, ISession session) { }
        public static void Drop(Type type, ITransaction tx) { }

        public static bool Exist(Type type, IDriver driver = null) { return false; }
        public static bool Exist(Type type, ISession session) { return false; }
        public static bool Exist(Type type, ITransaction tx) { return false; }

    }
}
