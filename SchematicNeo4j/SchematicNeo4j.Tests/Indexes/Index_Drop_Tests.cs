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
    public class Index_Drop_Tests : IDisposable
    {
        private IDriver driver = null;
        private readonly Index testIndex = new Index(label: "IndexDropTest", new string[] { "Prop1", "Prop2" });
        public Index_Drop_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
        }

        [Fact]
        public void Index_Drop_Will_DropIndex_By_Label_And_Properties_With_Driver()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            Assert.Equal(testIndex, GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Execute
            testIndex.Drop(driver);
            // Test Outcome
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
        }

        [Fact]
        public void Index_Drop_Will_DropIndex_By_Label_And_Properties_With_GraphConnection_SetDriver()
        {
            GraphConnection.SetDriver(driver);
            // Ensure Index does exist.
            CreateIndexForTest();
            Assert.Equal(testIndex, GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Execute
            testIndex.Drop();
            // Test Outcome
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
        }

        [Fact]
        public void Index_Drop_Will_ThrowException_With_No_Driver()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            Assert.Equal(testIndex, GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Ensure the GraphConnection.Driver are null.
            GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => testIndex.Drop(driver: null));
        }

        [Fact]
        public void Index_Drop_Will_DropIndex_By_Label_And_Properties_With_Session()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            Assert.Equal(testIndex, GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
            // Execute
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                testIndex.Drop(session);
            }
            // Test Outcome
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
        }



        [Fact]
        public void Index_Drop_Will_DropIndex_By_Label_And_Properties_But_Name_Is_UnnamedIndex()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            var actual = GetIndexForTest_KeepUnnamedIndex(testIndex);
            Assert.Equal(testIndex.Label, actual.Label);
            Assert.Equal(testIndex.Properties, actual.Properties);
            Assert.NotEqual(testIndex.Name, actual.Name);
            Assert.Equal("Unnamed index", actual.Name); 
            // Execute
            testIndex.Drop(driver);
            // Test Outcome
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(testIndex));
        }

        [Fact]
        public void Index_Drop_Will_Do_Nothing_When_Index_Doesnt_Exist_By_Label_And_Properties()
        {
            var nonExistantIndex = new Index("noExist", "SomethingThatDoesntExistForATest", new string[] { "Sally", "Bob" });
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(nonExistantIndex));
            // Execute
            nonExistantIndex.Drop(driver);

            // Test Outcome
            Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(nonExistantIndex));


        }

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.WriteTransaction(tx => {
                    if (testIndex.Exists(tx))
                        tx.Run("DROP INDEX ON :IndexDropTest(Prop1,Prop2)");
                    return true;
                });
            }
        }


        private void CreateIndexForTest()
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.WriteTransaction(tx => tx.Run("CREATE INDEX ON :IndexDropTest(Prop1,Prop2)"));
            }
        }

        private Index GetIndexForTest_OverloadUnnamedIndexAsNull(Index index)
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                var result = session.ReadTransaction(tx => tx.Run("CALL db.indexes() yield indexName, tokenNames, properties WITH CASE indexName WHEN 'Unnamed index' THEN null ELSE indexName END as Name, tokenNames[0] as Label, properties as Properties WHERE Label = $Label AND Properties = $Properties RETURN *", index));
                return result.Select(record => new Index(name: record["Name"].As<string>(), label: record["Label"].As<string>(), properties: record["Properties"].As<IList<string>>().ToArray<string>())).FirstOrDefault();
            }
        }

        private Index GetIndexForTest_KeepUnnamedIndex(Index index)
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                var result = session.ReadTransaction(tx => tx.Run("CALL db.indexes() yield indexName, tokenNames, properties WITH indexName as Name, tokenNames[0] as Label, properties as Properties WHERE Label = $Label AND Properties = $Properties RETURN *", index));
                return result.Select(record => new Index(name: record["Name"].As<string>(), label: record["Label"].As<string>(), properties: record["Properties"].As<IList<string>>().ToArray<string>())).FirstOrDefault();
            }
        }

    }
}
