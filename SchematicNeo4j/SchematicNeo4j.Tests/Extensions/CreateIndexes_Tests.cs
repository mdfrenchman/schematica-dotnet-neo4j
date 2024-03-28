using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using SchematicNeo4j;
using SchematicNeo4j.Tests.DomainSample;

namespace SchematicNeo4j.Tests.Extensions
{
    public class CreateIndexes_Tests :IDisposable
    {
        private IDriver driver = null;
        private Type testType = typeof(TestIndexCreate);
        private List<Index> expected = new List<Index>() { 
            new Index(label:"TestIndexCreate", properties: new string[] {"Prop1"}),
            new Index(label:"TestIndexCreate", properties: new string[] {"Prop1", "Prop2"}),
            new Index(label:"TestIndexCreate", properties: new string[] {"Prop2", "Prop3"})
        };
        
        public CreateIndexes_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "SchematicNeo4j-Test!"));
        }

        
        [Fact]
        public void CreateIndexes_With_Driver_Will_Create_All_Indexes_For_Type()
        {
            // Ensure none of the indexes exist yet.
            Assert.All(expected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));

            // Execute
            testType.CreateIndexes(driver);

            // Test Outcome
            List<Index> actual = new List<Index>();
            for (int i = 0; i < 3; i++)
            {
                actual.Add(GetIndexForTest_OverloadUnnamedIndexAsNull(expected[i]));
            }
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateIndexes_With_GraphConnectionDriver_Will_Create_All_Indexes_For_Type()
        {
            // Ensure none of the indexes exist yet.
            Assert.All(expected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));

            // Execute
            GraphConnection.SetDriver(driver);
            testType.CreateIndexes();

            // Test Outcome
            List<Index> actual = new List<Index>();
            for (int i = 0; i < 3; i++)
            {
                actual.Add(GetIndexForTest_OverloadUnnamedIndexAsNull(expected[i]));
            }
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateIndexes_Without_Driver_Will_Throw_Exception()
        {
            GraphConnection.SetDriver(null);
            Assert.Throws<Neo4jException>(() => testType.CreateIndexes(driver: null));

        }

        [Fact]
        public void CreateIndexes_With_Session_Will_Create_All_Indexes_For_Type()
        {
            // Ensure none of the indexes exist yet.
            Assert.All(expected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));

            // Execute
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                testType.CreateIndexes(session);
            }

            // Test Outcome
            List<Index> actual = new List<Index>();
            for (int i = 0; i < 3; i++)
            {
                actual.Add(GetIndexForTest_OverloadUnnamedIndexAsNull(expected[i]));
            }
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CreateIndexes_With_Transaction_Will_Create_All_Indexes_For_Type()
        {
            // Ensure none of the indexes exist yet.
            Assert.All(expected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));

            // Execute
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => { testType.CreateIndexes(tx); return true; });
            }

            // Test Outcome
            List<Index> actual = new List<Index>();
            for (int i = 0; i < 3; i++)
            {
                actual.Add(GetIndexForTest_OverloadUnnamedIndexAsNull(expected[i]));
            }
            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => {
                    foreach (Index item in expected)
                    {
                        if (item.Exists(tx))
                            item.Drop(tx);
                    }
                    return true;
                });
            }
        }

        private Index GetIndexForTest_OverloadUnnamedIndexAsNull(Index index)
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                var recordList = session.ExecuteRead(tx => tx.Run("CALL db.indexes() yield indexName, tokenNames, properties WITH CASE indexName WHEN 'Unnamed index' THEN null ELSE indexName END as Name, tokenNames[0] as Label, properties as Properties WHERE Label = $Label AND Properties = $Properties RETURN *", index).ToList());
                return recordList.Select(record => new Index(name: record["Name"].As<string>(), label: record["Label"].As<string>(), properties: record["Properties"].As<IList<string>>().ToArray<string>())).FirstOrDefault();
            }
        }
    }
}
