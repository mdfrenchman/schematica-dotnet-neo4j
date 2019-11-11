using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SchematicNeo4j.Attributes;
using Neo4j.Driver.V1;

namespace SchematicNeo4j
{
    public static class IndexExtensions
    {
        public static List<Index> Indexes(this Type type)
        {
            var nodeType = type;
            PropertyInfo[] propertyInfo = nodeType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var result = propertyInfo.SelectMany(pi =>
                ((IndexAttribute[])pi.GetCustomAttributes(typeof(IndexAttribute), true)).Where(ai => (pi.DeclaringType == nodeType && !ai.IsAbstract) || (pi.DeclaringType != nodeType && ai.IsAbstract))
                    .Select(ia => (name: ((IndexAttribute)ia).Name, label: ((IndexAttribute)ia).Label, prop: pi.Name))
                ).GroupBy(t => (t.name, t.label))
                .Select(grouping => new Index(
                        name: grouping.Key.name,
                        label: String.IsNullOrWhiteSpace(grouping.Key.label) ? type.Label() : grouping.Key.label,
                        properties: grouping.Select(t => t.prop).ToArray()
                        )
                ).ToList();
            return result;
        }

        #region CreateIndexes
        /// <summary>
        /// Gets all indexes from the Type and Creates them in the graph.
        /// </summary>
        /// <remarks>
        ///  pre neo4j version 4.0, the Index.Name is not able to be stored.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="driver"></param>
        public static void CreateIndexes(this Type type, IDriver driver = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Index.CreateIndexes() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                type.Indexes().ForEach(idx => idx.Create(session: session));
            }
        }

        /// <summary>
        /// Gets all indexes from the Type and Creates them in the graph.
        /// </summary>
        /// <remarks>
        ///  pre neo4j version 4.0, the Index.Name is not able to be stored.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="session"></param>
        public static void CreateIndexes(this Type type, ISession session)
        {
            type.Indexes().ForEach(idx => idx.Create(session: session));
        }

        /// <summary>
        /// Gets all indexes from the Type and Creates them in the graph.
        /// </summary>
        /// <remarks>
        ///  pre neo4j version 4.0, the Index.Name is not able to be stored.
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="tx"></param>
        public static void CreateIndexes(this Type type, ITransaction tx)
        {
            type.Indexes().ForEach(idx => idx.Create(tx: tx));
        }

        #endregion
    }
}
