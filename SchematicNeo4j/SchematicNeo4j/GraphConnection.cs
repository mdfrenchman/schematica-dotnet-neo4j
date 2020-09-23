using Neo4j.Driver;
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
        public static void SetDriver(string boltConnection, IAuthToken authToken, Action<ConfigBuilder> driverConfig = null)
        {
            if (driverConfig is null) driverConfig = DriverConfig;
            _neo4jDriver = GraphDatabase.Driver(boltConnection, authToken, driverConfig);
        }
        public static void SetDriver(string boltConnection, string username, string password, Action<ConfigBuilder> driverConfig = null)
        {
            if (driverConfig is null) driverConfig = DriverConfig;
            _neo4jDriver = GraphDatabase.Driver(boltConnection, AuthTokens.Basic(username, password), driverConfig);
        }
        
        public static Action<ConfigBuilder> DriverConfig { get; set; } = o => o.WithMaxTransactionRetryTime(TimeSpan.FromSeconds(60)).WithConnectionTimeout(TimeSpan.FromMilliseconds(-1));
    }
}