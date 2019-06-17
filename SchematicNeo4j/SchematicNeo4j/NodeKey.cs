using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchematicNeo4j.Constraints
{
    public static class NodeKey
    {

        /// <summary>
        /// Crete type.NodeKey if one doesn't already exist in the graph.
        /// </summary>
        /// <remarks>
        /// Use Set unless the graph schema is definitely empty.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        public static void Create(Type type, IDriver driver = null) {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.Create() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                Create(type, session);
            }
        }

        /// <summary>
        /// Crete type.NodeKey if one doesn't already exist in the graph.
        /// </summary>
        /// <remarks>
        /// Use Set unless the graph schema is definitely empty.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="session"></param>
        public static void Create(Type type, ISession session) {
            session.WriteTransaction(tx => Create(type, tx));
        }

        /// <summary>
        /// Drop the NodeKey in the graph if one exists. Doesn't require that it match the type.NodeKey()
        /// </summary>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        public static void Drop(Type type, IDriver driver = null) {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.Drop() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                Drop(type, session);
            }
        }

        /// <summary>
        /// Drop the NodeKey in the graph if one exists. Doesn't require that it match the type.NodeKey()
        /// </summary>
        /// <param name="type"></param>
        /// <param name="session"></param>
        public static void Drop(Type type, ISession session) {
            session.WriteTransaction(tx => Drop(type, tx));
        }

        /// <summary>
        /// A Node Key exists for the type. Does not necessarily match domain type node key.
        /// </summary>
        /// <remarks>
        /// Use MatchesExisting if you want to check for an exact match of the domain type to the graph.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool Exists(Type type, IDriver driver = null) {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.Exists() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Read))
            {
                return Exists(type, session);
            }
        }
        
        /// <summary>
        /// A Node Key exists for the type. Does not necessarily match domain type node key.
        /// </summary>
        /// <remarks>
        /// Use MatchesExisting if you want to check for an exact match of the domain type to the graph.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool Exists(Type type, ISession session) {
            return session.ReadTransaction(tx => Exists(type, tx));
        }

        /// <summary>
        /// Determins if the domain type Node Key is the same as the Node Key in the graph.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool MatchesExisting(Type type, IDriver driver = null) {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.MatchesExisting() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Read))
            {
                return MatchesExisting(type, session);
            }
        }
        
        /// <summary>
        /// Determins if the domain type Node Key is the same as the Node Key in the graph.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool MatchesExisting(Type type, ISession session) {
            return session.ReadTransaction(tx => MatchesExisting(type, tx));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tx"></param>
        internal static void SetNodeKey(this Type type, ITransaction tx)
        {
            if (!type.MatchesExisting(tx))
            {
                type.Drop(tx);
                type.Create(tx);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tx"></param>
        internal static void RemoveNodeKey(this Type type, ITransaction tx)
        {
            Drop(type, tx);
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
                // Older versions of Neo4j a single parameter key would not have the () around it on return. But they're needed for CREATE/DROP.
                return record[0].As<string>().Replace(") ASSERT ", ") ASSERT ( ").Replace(" IS NODE KEY", " ) IS NODE KEY");
            else
                return record[0].As<string>();
        }

        private static string NodeKeyConstraintString(this Type type)
        {
            var nodeKeyParams = type.NodeKey();
            // Split by : to take the first label if a Node is marked as multiple (in case of inheritence).
            // Example: [Node(Label = "Vehicle:Truck")] public class Truck => NodeKey will be created for Vehicle, useful in cases of shared node keys.
            var label = type.Label().Split(':')[0];
            var nodeVariable = label.ToLower();
            var keyString = String.Join(", ", type.NodeKey().Select(nk => $"{nodeVariable}.{nk}"));
            if (String.IsNullOrEmpty(keyString))
                return String.Empty;
            else
                return $"CONSTRAINT ON ( {nodeVariable}:{label} ) ASSERT ({keyString}) IS NODE KEY";
        }

        // TODO: We probably should test the individual private methods in NodeKey.cs individually.
        // Schema.Constraints.NodeKey.Create(type, tx|session|driver, forceReplace = false) => If a node key does not already exist, create it.
        // Schema.Constraints.NodeKey.Drop(type, tx|session|driver) => if a node key exists, drop it.
        // Schema.Constraints.NodeKey.Exists(type, tx|session|driver) => bool if any node key exists for the provided type.Label().
        // Schema.Constraints.NodeKey.MatchesExisting(type, tx|session|driver) => bool if the type NodeKey matches the nodekey in graph.

    }
}
