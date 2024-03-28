using Neo4j.Driver;
using SchematicNeo4j.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SchematicNeo4j
{
    public static class Schema
    {
        public static bool Initialize(Assembly assembly, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            return Initialize(assembly.ExportedTypes.Where(t => !t.IsAbstract), driver, sessionConfigOptions);
        }


        public static bool Initialize(IEnumerable<Type> domainTypes, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Initialize() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");


            if (sessionConfigOptions is null)
                sessionConfigOptions = o => o.WithDefaultAccessMode(AccessMode.Write);

            using (var session = driver.Session(sessionConfigOptions))
            {
                return session.ExecuteWrite(tx =>
                {
                    foreach (Type type in domainTypes)
                    {
                        type.SetNodeKey(tx);
                        type.CreateIndexes(tx);
                    }
                    return true;
                });
            }
        }

        public static bool Initialize(Type type, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Initialize() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");

            if (sessionConfigOptions is null)
                sessionConfigOptions = o => o.WithDefaultAccessMode(AccessMode.Write);

            using (var session = driver.Session(sessionConfigOptions))
            {
                return session.ExecuteWrite(tx => {
                    type.SetNodeKey(tx);
                    type.CreateIndexes(tx);
                    return true;
                });
            }
        }

        #region Clear
        public static bool Clear(Assembly assembly, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            return Clear(assembly.ExportedTypes, driver, sessionConfigOptions);
        }


        public static bool Clear(IEnumerable<Type> domainTypes, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Clear() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");

            if (sessionConfigOptions is null)
                sessionConfigOptions = o => o.WithDefaultAccessMode(AccessMode.Write);

            using (var session = driver.Session(sessionConfigOptions))
            {
                return session.ExecuteWrite(tx => {
                    foreach (Type type in domainTypes)
                    {
                        type.RemoveNodeKey(tx);
                        type.DropIndexes(tx);
                    }
                    return true;
                });
            }
        }

        public static bool Clear(Type type, IDriver driver = null, Action<SessionConfigBuilder> sessionConfigOptions = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Clear() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");

            if (sessionConfigOptions is null)
                sessionConfigOptions = o => o.WithDefaultAccessMode(AccessMode.Write);

            using (var session = driver.Session(sessionConfigOptions))
            {
                return session.ExecuteWrite(tx => {
                    type.RemoveNodeKey(tx);
                    type.DropIndexes(tx);
                    return true;
                });
            }
        }
        #endregion
    }
}
