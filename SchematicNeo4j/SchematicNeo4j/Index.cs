using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SchematicNeo4j
{
    public class Index : IEquatable<Index>
    {

        // new Schema.Indexes.Index() => Schema.Indexes.Index { _label, _properties };
        // new Schema.Indexes.Index() => Schema.Indexes.Index { _name, _label, _properties };
        // (Schema.Indexes.Index instance).Create(tx|session|driver);
        // (Schema.Indexes.Index instance).Drop(tx|session|driver);
        // (Schema.Indexes.Index instance).Exists(tx|session|driver);

        /// <summary>
        /// _name is used for neo4j v4.0+ prior to v4 it's not stored in the graph.
        /// </summary>
        public string Name { get; set; }
        public string Label { get; set; }
        public string[] Properties { get; set; }

        public Index(){ }
        public Index(string label, string[] properties)
        {
            Label = label;
            Properties = properties;
        }
        public Index(string name, string label, string[] properties)
        {
            Name = name;
            Label = label;
            Properties = properties;
        }

        public static void Create(IDriver driver = null) { }

        public static void Create(ISession session) { }

        public static void Create(ITransaction tx) { }

        public static void Drop(IDriver driver = null) { }

        public static void Drop(ISession session) { }

        public static void Drop(ITransaction tx) { }

        
        #region Equality
        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return (this == (Index)obj);
            }
        }
        public bool Equals(Index other)
        {
            return other != null &&
                   Name == other.Name &&
                   Label == other.Label &&
                   Properties.SequenceEqual(other.Properties);
        }

        public override int GetHashCode()
        {
            var hashCode = -1233081209;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Label);
            hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(Properties);
            return hashCode;
        }

        public static bool operator ==(Index left, Index right)
        {
            return EqualityComparer<Index>.Default.Equals(left, right);
        }

        public static bool operator !=(Index left, Index right)
        {
            return !(left == right);
        }
        #endregion

        #region Exists

        /// <summary>
        /// Checks if the Index exists in the graph
        /// </summary>
        /// <remarks>
        /// pre neo4j v4 the match is done on the label and properties. Ignoring the name. neo4j v4 allows naming of node_label_property indexes
        /// </remarks>
        /// <param name="driver"></param>
        /// <returns></returns>
        public bool Exists(IDriver driver = null)
        {
            if (driver is null)
                driver = GraphConnection.Driver;
            if (driver is null)
                throw new Neo4jException(code: "GraphConnection.Driver.Missing", message: "Index.Exists() => The driver was not passed in or set for the library. Recommend: GraphConnection.SetDriver(driver);");
            using (var session = driver.Session(AccessMode.Write))
            {
                return this.Exists(session);
            }
        }

        /// <summary>
        /// Checks if the Index exists in the graph
        /// /// </summary>
        /// <remarks>
        /// pre neo4j v4 the match is done on the label and properties. Ignoring the name. neo4j v4 allows naming of node_label_property indexes
        /// </remarks>
        /// <param name="session"></param>
        /// <returns></returns>
        public bool Exists(ISession session)
        {
            return session.ReadTransaction(tx => this.Exists(tx));
        }

        /// <summary>
        /// Checks if the Index exists in the graph by Label and Properties
        /// </summary>
        /// <remarks>
        /// pre neo4j v4 the match is done on the label and properties. Ignoring the name. neo4j v4 allows naming of node_label_property indexes. So we'll upgrade to that for the v4 version.
        /// </remarks>
        /// <param name="tx"></param>
        /// <returns></returns>
        public bool Exists(ITransaction tx)
        {
            var record = tx.Run(
                "CALL db.indexes() yield indexName, tokenNames, properties, type WITH indexName as Name, tokenNames[0] as label, properties, type WHERE type = 'node_label_property' AND label = $label AND properties = $props RETURN (count(*) = 1)",
                new { label = this.Label, props = this.Properties.ToList<string>() }
                ).FirstOrDefault<IRecord>();
            return (record is null) ? false : record[0].As<bool>();
        }

        #endregion
    }

}
