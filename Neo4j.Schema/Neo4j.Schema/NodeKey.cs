using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4j.Schema
{
    public static class NodeKey
    {
        public static void Create(Type type, IDriver driver = null) { }
        public static void Create(Type type, ISession session) { }
        public static void Drop(Type type, IDriver driver = null) { }
        public static void Drop(Type type, ISession session) { }

        /// <summary>
        /// A Node Key exists for the type. Does not necessarily match domain type node key.
        /// </summary>
        /// <remarks>
        /// Use MatchesExisting if you want to check for an exact match of the domain type to the graph.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool Exists(Type type, IDriver driver = null) { return false; }
        /// <summary>
        /// A Node Key exists for the type. Does not necessarily match domain type node key.
        /// </summary>
        /// <remarks>
        /// Use MatchesExisting if you want to check for an exact match of the domain type to the graph.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool Exists(Type type, ISession session) { return false; }

        /// <summary>
        /// Determins if the domain type Node Key is the same as the Node Key in the graph.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool MatchesExisting(Type type, IDriver driver = null) { return false; }
        /// <summary>
        /// Determins if the domain type Node Key is the same as the Node Key in the graph.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool MatchesExisting(Type type, ISession session) { return false; }

        internal static void SetNodeKeyConstraint(this Type type, ITransaction tx)
        {
            if (!type.MatchesExisting(tx))
            {
                type.Drop(tx);
                type.Create(tx);
            }
        }

        internal static void RemoveNodeKeyConstraint(this Type type, ITransaction tx)
        {

        }

        private static bool MatchesExisting(this Type type, ITransaction tx)
        {
            var existing = type.Get(tx);
            if (String.IsNullOrEmpty(existing))
                return false;
            else 
                return (existing == type.NodeKeyConstraintString());
        }

        private static bool Exists(this Type type, ITransaction tx)
        {
            return !String.IsNullOrEmpty(type.Get(tx));
        }

        private static void Create(this Type type, ITransaction tx)
        {
            if (!type.Exists(tx))
            {
                var constraint = type.NodeKeyConstraintString();
                if (!String.IsNullOrEmpty(constraint))
                    tx.Run($"CREATE {constraint}");
            }
        }

        private static void Drop(this Type type, ITransaction tx)
        {
            // TODO: For calls from SetConstraint, we're calling Exists twice. Need to refactor.
            if (type.Exists(tx))
            {
                var constraint = type.NodeKeyConstraintString();
                if (!String.IsNullOrEmpty(constraint))
                    tx.Run($"DROP {constraint}");
            }

        }

        private static string Get(this Type type, ITransaction tx)
        {
            var record = tx.Run("call db.constraints() yield description WHERE description contains $typeName AND description contains 'NODE KEY' RETURN description", new { typeName = type.Label() }).FirstOrDefault();
            if (record is null)
                return String.Empty;
            else if (!record[0].As<string>().Contains(") ASSERT ("))
                return record[0].As<string>().Replace(") ASSERT ", ") ASSERT ( ").Replace(" IS NODE KEY", " ) IS NODE KEY");
            else
                return record[0].As<string>();
        }

        private static string NodeKeyConstraintString(this Type type)
        {
            var nodeKeyParams = type.NodeKey();
            var label = type.Label();
            var nodeVariable = label.ToLower();
            var keyString = String.Join(", ", type.NodeKey().Select(nk => $"{nodeVariable}.{nk}"));
            if (String.IsNullOrEmpty(keyString))
                return String.Empty;
            else
                return $"CONSTRAINT ON ( {nodeVariable}:{label} ) ASSERT ({keyString}) IS NODE KEY";
        }
    }
}
