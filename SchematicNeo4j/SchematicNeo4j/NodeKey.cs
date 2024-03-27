using Neo4j.Driver;
using System;
using System.Linq;
using System.Text;

namespace SchematicNeo4j.Constraints
{
    // TODO: Rename static class to <verb>ConstraintExtensions
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
        public static void Create(Type type, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.Create() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(sessionConfigOptions))
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
        public static void Create(Type type, ISession session)
        {
            session.ExecuteWrite(tx => { Create(type, tx); return true; });
        }
        // TODO: move to Async
        //public static async Task CreateAsync(Type type, IAsyncSession session)
        //{
        //    return await session.ExecuteWriteAsync(async tx =>
        //    {
        //        Create(type, await tx); return true;
        //    })
        //}

        /// <summary>
        /// Drop the NodeKey in the graph if one exists. Doesn't require that it match the type.NodeKey()
        /// </summary>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        public static void Drop(Type type, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.Drop() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(sessionConfigOptions))
            {
                Drop(type, session);
            }
        }

        /// <summary>
        /// Drop the NodeKey in the graph if one exists. Doesn't require that it match the type.NodeKey()
        /// </summary>
        /// <param name="type"></param>
        /// <param name="session"></param>
        public static void Drop(Type type, ISession session)
        {
            session.ExecuteWrite(tx => { Drop(type, tx); return true; });
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
        public static bool Exists(Type type, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.Exists() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(sessionConfigOptions))
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
        public static bool Exists(Type type, ISession session)
        {
            return session.ExecuteWrite(tx => Exists(type, tx));
        }

        /// <summary>
        /// Determines if the domain type Node Key is the same as the Node Key in the graph.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool MatchesExisting(Type type, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "NodeKey.MatchesExisting() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(sessionConfigOptions))
            {
                return MatchesExisting(type, session);
            }
        }

        /// <summary>
        /// Determines if the domain type Node Key is the same as the Node Key in the graph.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool MatchesExisting(Type type, ISession session)
        {
            return session.ExecuteRead(tx => MatchesExisting(type, tx));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="tx"></param>
        internal static void SetNodeKey(this Type type, IQueryRunner tx)
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
        internal static void RemoveNodeKey(this Type type, IQueryRunner tx)
        {
            Drop(type, tx);
        }

        private static bool MatchesExisting(this Type type, IQueryRunner tx)
        {
            var existing = type.Get(tx);
            if (String.IsNullOrEmpty(existing))
                return false;
            else
                return (existing == type.NodeKeyConstraintString());
        }

        private static bool Exists(this Type type, IQueryRunner tx)
        {
            return !String.IsNullOrEmpty(type.Get(tx));
        }

        private static void Create(this Type type, IQueryRunner tx)
        {
            if (!type.Exists(tx))
            {
                var constraint = type.NodeKeyConstraintString();
                if (!String.IsNullOrEmpty(constraint))
                    tx.Run($"CREATE {constraint}");
            }
        }

        private static void Drop(this Type type, IQueryRunner tx)
        {
            tx.Run($"DROP CONSTRAINT {type.NodeKeyName()} IF EXISTS");
        }

        private static string Get(this Type type, IQueryRunner tx)
        {
            var record = tx.Run("SHOW CONSTRAINTS YIELD createStatement WHERE createStatement contains (':'+$typeName+' ') AND createStatement contains $constraintType RETURN createStatement", new { typeName = type.Label(), constraintType = "NODE KEY" }).FirstOrDefault();
            if (record is null)
                return String.Empty;
            else if (!record[0].As<string>().Contains(") ASSERT ("))
                // Older versions of Neo4j a single parameter key would not have the () around it on return. But they're needed for CREATE/DROP.
                return record[0].As<string>().Replace(") ASSERT ", ") ASSERT ( ").Replace(" IS NODE KEY", " ) IS NODE KEY");
            else
                return record[0].As<string>();
        }

        // TODO: update to v4.4->v5 syntax for GET
        //private static ConstraintResponse  Get(this Type type, IQueryRunner tx, decimal neo4jVersion = 5)
        //{
        //    var record = tx.Run("SHOW CONSTRAINTS YIELD * WHERE name = $nodeKeyName", new { nodeKeyName = type.NodeKeyName() }).FirstOrDefault();
        //    if (record is null)
        //        return null;
        //    return ConstraintResponse.From(record);
        //}

        private static string NodeKeyConstraintString(this Type type)
        {
            var nodeKeyParams = type.NodeKey();
            // Split by : to take the first label if a Node is marked as multiple (in case of inheritence).
            // Example: [Node(Label = "Vehicle:Truck")] public class Truck => NodeKey will be created for Vehicle, useful in cases of shared node keys.
            var label = type.Label().Split(':')[0];
            var nodeVariable = label.ToLower();
            var keyString = String.Join(", ", type.NodeKey().Select(nk => $"n.{nk}"));
            var keyName = type.NodeKeyName();

            if (String.IsNullOrEmpty(keyString))
                return String.Empty;
            else
                return $"CONSTRAINT { keyName } FOR (n:{label}) REQUIRE ({keyString}) IS NODE KEY";
        }

        // TODO: update to v4.4->v5 syntax for create etc
        //private static ConstraintResponse NodeKeyConstraint(this Type type)
        //{
        //    var nodeKeyParams = type.NodeKey();
        //    var label = type.Label().Split(":")[0];
        //    var nodeVariable = label.ToLower();

        //    return new ConstraintResponse() { name = $"nk{label}", entityType= "NODE", labelsOrTypes = new[] { label }, properties = type.NodeKey().ToArray(), type="NODE_KEY", options   };

        //}

        // TODO: We probably should test the individual private methods in NodeKey.cs individually.
        // Schema.Constraints.NodeKey.Create(type, tx|session|driver, forceReplace = false) => If a node key does not already exist, create it.
        // Schema.Constraints.NodeKey.Drop(type, tx|session|driver) => if a node key exists, drop it.
        // Schema.Constraints.NodeKey.Exists(type, tx|session|driver) => bool if any node key exists for the provided type.Label().
        // Schema.Constraints.NodeKey.MatchesExisting(type, tx|session|driver) => bool if the type NodeKey matches the nodekey in graph.

    }
}
