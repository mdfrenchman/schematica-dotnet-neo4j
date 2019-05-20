using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Neo4jSchemaManager
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
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => {
                    foreach (Type type in domainTypes)
                    {
                        type.SetNodeKeyConstraint(tx);
                    }
                });
            }
        }

        public static void Initialize(Type type, IDriver driver = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => type.SetNodeKeyConstraint(tx));
            }
        }
    }
}
