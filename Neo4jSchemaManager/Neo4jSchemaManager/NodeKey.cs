using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4jSchemaManager
{
    internal static class NodeKey
    {
        internal static void SetNodeKeyConstraint(this Type type, ITransaction tx)
        {
        }

        internal static void RemoveNodeKeyConstraint(this Type type, ITransaction tx)
        {

        }

        private static bool MatchesExisting(this Type type, ITransaction tx)
        {
            return false;
        }

        private static bool Exists(this Type type, ITransaction tx)
        {
            return String.IsNullOrEmpty(type.Get(tx));
        }

        private static void Create(this Type type, ITransaction tx)
        {

        }

        private static void Drop(this Type type, ITransaction tx)
        {

        }

        private static string Get(this Type type, ITransaction tx)
        {
            return String.Empty;
        }
    }
}
