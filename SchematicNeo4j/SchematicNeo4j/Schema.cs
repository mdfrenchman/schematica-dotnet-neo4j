﻿using Neo4j.Driver.V1;
using SchematicNeo4j.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SchematicNeo4j
{
    public static class Schema
    {
        public static void Initialize(Assembly assembly, IDriver driver = null)
        {
            Initialize(assembly.ExportedTypes.Where(t => !t.IsAbstract), driver);
        }


        public static void Initialize(IEnumerable<Type> domainTypes, IDriver driver = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Initialize() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => {
                    foreach (Type type in domainTypes)
                    {
                        type.SetNodeKey(tx);
                        type.CreateIndexes(tx);
                    }
                });
            }
        }

        public static void Initialize(Type type, IDriver driver = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Initialize() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => {
                    type.SetNodeKey(tx);
                    type.CreateIndexes(tx);
                });
            }
        }

        #region Clear
        public static void Clear(Assembly assembly, IDriver driver = null)
        {
            Clear(assembly.ExportedTypes, driver);
        }


        public static void Clear(IEnumerable<Type> domainTypes, IDriver driver = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Clear() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => {
                    foreach (Type type in domainTypes)
                    {
                        type.RemoveNodeKey(tx);
                        type.DropIndexes(tx);
                    }
                });
            }
        }

        public static void Clear(Type type, IDriver driver = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Schema.Clear() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => {
                    type.RemoveNodeKey(tx);
                    type.DropIndexes(tx);
                });
            }
        }
            #endregion
    }
}
