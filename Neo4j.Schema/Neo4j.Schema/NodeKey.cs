using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Neo4j.Schema
{
    internal static class NodeKey
    {
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
