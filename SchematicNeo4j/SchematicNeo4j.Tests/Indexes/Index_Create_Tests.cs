using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using SchematicNeo4j;

namespace SchematicNeo4j.Tests.Indexes
{
    public class Index_Create_Tests : IDisposable
    {
        private IDriver driver = null;
        private readonly Index testIndex = new Index(label: "IndexCreateTest", new string[] { "Prop1", "Prop2" });
        public Index_Create_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "SchematicNeo4j-Test!"));
        }

        [Fact]
        public void Index_Create_Will_CreateIndex_By_Label_And_Properties_With_Driver()
        {
            // Ensure Index does not exist.
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Execute
            testIndex.Create(driver);
            // Test Outcome
            Assert.Equal(testIndex, GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
        }

        [Fact]
        public void Index_Create_Will_CreateIndex_By_Label_And_Properties_With_GraphConnection_SetDriver()
        {
            GraphConnection.SetDriver(driver);
            // Ensure Index does not exist.
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Execute
            testIndex.Create();
            // Test Outcome
            Assert.Equal(testIndex, GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
        }

        [Fact]
        public void Index_Create_Will_ThrowException_With_No_Driver()
        {
            // Ensure Index does not exist.
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Ensure the GraphConnection.Driver are null.
            GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => testIndex.Create(driver: null));
        }

        [Fact]
        public void Index_Create_Will_CreateIndex_By_Label_And_Properties_With_Session()
        {
            // Ensure Index does not exist.
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Execute
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                testIndex.Create(session);
            }
            // Test Outcome
            Assert.Equal(testIndex, GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
        }



        [Fact]
        public void Index_Create_Will_CreateIndex_By_Label_And_Properties_But_Name_Is_UnnamedIndex()
        {
            // Ensure Index does not exist.
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Execute
            testIndex.Create(driver);
            // Test Outcome
            var actual = GetIndexForTest_KeepUnnamedIndex(testIndex);
            Assert.Equal(testIndex.Label, actual.Label);
            Assert.Equal(testIndex.Properties, actual.Properties);
            Assert.NotEqual(testIndex.Name, actual.Name);
            Assert.Equal("Unnamed index", actual.Name);
        }

        [Fact]
        public void Index_Create_Will_Succeeed_When_Index_Already_Exists_By_Label_And_Properties()
        {
            // Ensure Index exists on the graph.
            CreateIndexForTest();
            var preExisting = GetIndexForTest_KeepUnnamedIndex(testIndex);
            Assert.Equal(testIndex.Label, preExisting.Label);
            Assert.Equal(testIndex.Properties, preExisting.Properties);
            Assert.NotEqual(testIndex.Name, preExisting.Name);
            Assert.Equal("Unnamed index", preExisting.Name);

            // Execute
            testIndex.Create(driver);

            // Test Outcome
            var actual = GetIndexForTest_KeepUnnamedIndex(testIndex);
            Assert.Equal(testIndex.Label, actual.Label);
            Assert.Equal(testIndex.Properties, actual.Properties);
            Assert.NotEqual(testIndex.Name, actual.Name);
            Assert.Equal("Unnamed index", actual.Name);

        }

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.WriteTransaction(tx => {
                    if (testIndex.Exists(tx))
                        tx.Run("DROP INDEX ON :IndexCreateTest(Prop1,Prop2)");
                    return true;
                });
            }
        }


        private void CreateIndexForTest()
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.WriteTransaction(tx => { tx.Run("CREATE INDEX ON :IndexCreateTest(Prop1,Prop2)"); return true; });
            }
        }

        private Index GetIndexForTest_OverloadUnnamedIndexAsNull(Index index)
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                var recordList = session.ReadTransaction(tx => tx.Run("CALL db.indexes() yield indexName, tokenNames, properties WITH CASE indexName WHEN 'Unnamed index' THEN null ELSE indexName END as Name, tokenNames[0] as Label, properties as Properties WHERE Label = $Label AND Properties = $Properties RETURN *", index).ToList());
                return recordList.Select(record => new Index(name: record["Name"].As<string>(), label: record["Label"].As<string>(), properties: record["Properties"].As<IList<string>>().ToArray<string>())).FirstOrDefault();
            }
        }

        private Index GetIndexForTest_KeepUnnamedIndex(Index index)
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                var recordList = session.ReadTransaction(tx => tx.Run("CALL db.indexes() yield indexName, tokenNames, properties WITH indexName as Name, tokenNames[0] as Label, properties as Properties WHERE Label = $Label AND Properties = $Properties RETURN *", index).ToList());
                return recordList.Select(record => new Index(name: record["Name"].As<string>(), label: record["Label"].As<string>(), properties: record["Properties"].As<IList<string>>().ToArray<string>())).FirstOrDefault();
            }
        }

    }
}
