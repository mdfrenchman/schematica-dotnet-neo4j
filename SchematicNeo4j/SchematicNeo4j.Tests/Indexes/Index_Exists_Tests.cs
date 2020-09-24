using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SchematicNeo4j.Tests.Indexes
{
    public class Index_Exists_Tests : IDisposable
    {
        private IDriver driver = null;
        private readonly Index testIndex = new Index(label: "Truck", new string[] { "Make", "TowingCapacity" });

        public Index_Exists_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
        }

        [Fact]
        public void Index_Exists_Will_Return_True_When_Index_Exists_By_Label_And_Properties_With_Driver()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            // Execute
            Assert.True(testIndex.Exists(driver));
        }

        [Fact]
        public void Index_Exists_Will_Return_True_When_Index_Exists_By_Label_And_Properties_WithOut_Driver()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            GraphConnection.SetDriver(driver);
            // Execute
            Assert.True(testIndex.Exists());
        }

        [Fact]
        public void Index_Exists_Will_ThrowException_With_No_Driver()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            // Ensure the GraphConnection.Driver are null.
            GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => testIndex.Exists(driver: null));
        }

        [Fact]
        public void Index_Exists_Will_Return_True_When_Index_Exists_By_Label_And_Properties_With_Session()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            // Execute
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                Assert.True(testIndex.Exists(session));
            }
        }



        [Fact]
        public void Index_Exists_Will_Return_True_When_Index_Exists_By_Label_And_Properties_But_Name_Is_UnnamedIndex()
        {
            // Ensure Index does exist.
            CreateIndexForTest();
            // Ensure Name is a value that won't match
            testIndex.Name = "Bob_is_your_uncle";
            // Execute
            Assert.True(testIndex.Exists(driver));
        }

        [Fact]
        public void Index_Exists_Will_Return_False_When_Index_Does_NOT_Exist_By_Label_And_Properties()
        {
            // Ensure Index doesn't exist on the graph.
            Assert.Null(GetIndexForTest(testIndex));
            // Execute
            Assert.False(testIndex.Exists(driver));

            // Create Index so that Dispose doesn't error that the index doesn't exist.
            CreateIndexForTest();
        }

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.WriteTransaction(tx => {
                    if (testIndex.Exists(tx))
                        tx.Run("DROP INDEX ON :Truck(Make,TowingCapacity)");
                    return true;
                });
            }
        }


        private void CreateIndexForTest()
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.WriteTransaction(tx => { tx.Run("CREATE INDEX ON :Truck(Make,TowingCapacity)"); return true; });
            }
        }

        private Index GetIndexForTest(Index index)
        {
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                var recordList = session.ReadTransaction(tx => tx.Run("CALL db.indexes() yield indexName, tokenNames, properties WITH indexName as Name, tokenNames[0] as Label, properties as Properties WHERE Label = $Label AND Properties = $Properties RETURN *", index).ToList());
                return recordList.Select(record => new Index(label: record["Label"].As<string>(), properties: record["Properties"].As<IList<string>>().ToArray<string>())).FirstOrDefault();
            }
        }
    }
}
