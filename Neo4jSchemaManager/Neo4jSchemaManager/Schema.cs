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
        }


        public static void Initialize(IEnumerable<Type> domainTypes, IDriver driver = null)
        {
        }

        public static void Initialize(Type type, IDriver driver = null)
        {
        }
    }
}
