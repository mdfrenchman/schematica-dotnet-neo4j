﻿using Neo4j.Driver.V1;
using Schematica.Neo4j.Constraints;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Schematica.Neo4j
{
    public static class Schema
    {
        public static void Initialize(Assembly assembly, IDriver driver = null)
        {
            Initialize(assembly.ExportedTypes, driver);
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
                session.WriteTransaction(tx => type.SetNodeKey(tx));
            }
        }
    }
}
