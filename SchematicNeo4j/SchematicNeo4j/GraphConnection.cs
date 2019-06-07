using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j
{
    public static class GraphConnection
    {
        private static IDriver _neo4jDriver;

        internal static IDriver Driver
        {
            get { return _neo4jDriver; }
        }
        public static void SetDriver(IDriver driver)
        {
            _neo4jDriver = driver;
        }
        public static void SetDriver(string boltConnection, IAuthToken authToken)
        {

            _neo4jDriver = GraphDatabase.Driver(boltConnection, authToken, _driverConfig);
        }
        public static void SetDriver(string boltConnection, string username, string password)
        {
            _neo4jDriver = GraphDatabase.Driver(boltConnection, AuthTokens.Basic(username, password), _driverConfig);
        }

        private static Config _driverConfig = new Config { ConnectionTimeout = TimeSpan.FromMilliseconds(-1), MaxTransactionRetryTime = TimeSpan.FromSeconds(60) };
    }
}
