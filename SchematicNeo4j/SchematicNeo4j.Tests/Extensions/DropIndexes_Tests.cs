﻿using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SchematicNeo4j.Tests.DomainSample;
using System.Linq.Expressions;

namespace SchematicNeo4j.Tests.Extensions
{
    public class DropIndexes_Tests : IDisposable
    {
        private IDriver driver = null;
        private Type testType = typeof(TestIndexDrop);
        private List<Index> notExpected = new List<Index>() {
            new Index(label:"TestIndexDrop", properties: new string[] {"Prop1"}),
            new Index(label:"TestIndexDrop", properties: new string[] {"Prop1", "Prop2"}),
            new Index(label:"TestIndexDrop", properties: new string[] {"Prop2", "Prop3"})
        };

        public DropIndexes_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "SchematicNeo4j-Test!"));
        }


        [Fact]
        public void DropIndexes_With_Driver_Will_Drop_All_Indexes_For_Type()
        {
            // Ensure all of the indexes exist.
            notExpected.ForEach(idx => idx.Create(driver));
            Assert.All(notExpected, idx => idx.Exists(driver));

            // Execute
            testType.DropIndexes(driver);

            // Test Outcome
            Assert.All(notExpected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));
        }

        [Fact]
        public void DropIndexes_With_GraphConnectionDriver_Will_Drop_All_Indexes_For_Type()
        {
            // Ensure all of the indexes exist.
            notExpected.ForEach(idx => idx.Create(driver));
            Assert.All(notExpected, idx => idx.Exists(driver));

            // Execute
            GraphConnection.SetDriver(driver);
            testType.DropIndexes();

            // Test Outcome
            Assert.All(notExpected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));
        }

        [Fact]
        public void DropIndexes_Without_Driver_Will_Throw_Exception()
        {
            // Ensure all of the indexes exist.
            notExpected.ForEach(idx => idx.Create(driver));
            Assert.All(notExpected, idx => idx.Exists(driver));

            GraphConnection.SetDriver(null);
            Assert.Throws<Neo4jException>(() => testType.DropIndexes(driver: null));

        }

        [Fact]
        public void DropIndexes_With_Session_Will_Drop_All_Indexes_For_Type()
        {
            // Ensure all of the indexes exist.
            notExpected.ForEach(idx => idx.Create(driver));
            Assert.All(notExpected, idx => idx.Exists(driver));
            
            // Execute
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                testType.DropIndexes(session);
            }

            // Test Outcome
            Assert.All(notExpected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));
        }

        [Fact]
        public void DropIndexes_With_Transaction_Will_Drop_All_Indexes_For_Type()
        {
            // Ensure all of the indexes exist.
            notExpected.ForEach(idx => idx.Create(driver));
            Assert.All(notExpected, idx => idx.Exists(driver));

            // Execute
            using (ISession session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => { testType.DropIndexes(tx); return true; });
            }

            // Test Outcome
            Assert.All(notExpected, idx => Assert.Null(GetIndexForTest_OverloadUnnamedIndexAsNull(idx)));
        }

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => {
                    foreach (Index item in notExpected)
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
