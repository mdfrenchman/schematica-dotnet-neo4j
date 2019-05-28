﻿using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Schematica.Neo4j;

namespace Neo4j.Schema.Tests.NodeKey
{
    public class NodeKey_Exists_Tests :IDisposable
    {
        private IDriver driver = null;
        private string teConstraint = "CONSTRAINT ON ( te:TE ) ASSERT (te.Name, te.Identity) IS NODE KEY";

        public NodeKey_Exists_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
            //GraphConnection.SetDriver(driver);
        }

        [Fact]
        public void NodeKey_Exists_Will_ExistsNodeKey_For_Type_With_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "TE"));

            // Test False
            var actualFalse = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
            Assert.False(actualFalse);

            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {teConstraint}"));
            }

            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "TE"));
            Assert.Equal(teConstraint, GetConstraints("NODE KEY", "TE").First()[0]);

            // Test True
            var actualTrue = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_Exists_Will_ExistsNodeKey_For_Type_WithOut_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "TE"));
            GraphConnection.SetDriver(driver);
            // Test False
            var actualFalse = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode));
            Assert.False(actualFalse);

            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {teConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "TE"));
            Assert.Equal(teConstraint, GetConstraints("NODE KEY", "TE").First()[0]);

            // Test True
            var actualTrue = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode));
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_Exists_Will_ExistsNodeKey_For_Type_With_Session()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "TE"));
            using (var session = driver.Session(AccessMode.Read))
            {
                // Test False
                var actualFalse = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
                Assert.False(actualFalse);
            }
            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {teConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "TE"));
            Assert.Equal(teConstraint, GetConstraints("NODE KEY", "TE").First()[0]);

            using (var session = driver.Session(AccessMode.Read))
            {
                // Test True
                var actualTrue = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
                Assert.True(actualTrue);
            }
        }

        public void Dispose()
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                    session.WriteTransaction(tx =>
                    {
                        if (GetConstraints("NODE KEY", "TE", tx).Count() == 1)
                            tx.Run($"DROP {teConstraint}");
                    });
            }
        }

        private IStatementResult GetConstraints(string ofType, string forLabel, ITransaction tx)
        {
            return tx.Run(
                    "CALL db.constraints() yield description WHERE description contains $typeLabel AND description contains $constraintType RETURN description",
                    new { typeLabel = forLabel, constraintType = ofType }
                    );
        }

        private IStatementResult GetConstraints(string ofType, string forLabel)
        {
            using (var session = driver.Session(AccessMode.Read))
            {
                return session.ReadTransaction(tx => GetConstraints(ofType, forLabel, tx));
            }
        }


    }
}
