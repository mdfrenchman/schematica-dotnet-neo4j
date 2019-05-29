using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using Schematica.Neo4j;

namespace Neo4j.Schema.Tests.NodeKey
{
    public class NodeKey_MatchesExisting_Tests :IDisposable
    {
        private IDriver driver = null;
        private string meConstraint = "CONSTRAINT ON ( me:ME ) ASSERT (me.Name) IS NODE KEY";

        public NodeKey_MatchesExisting_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
            //GraphConnection.SetDriver(driver);
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_TrueFalse_When_NodeKey_MatchesExisting_For_Type_With_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));

            // Test False
            var actualFalse = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.MatchesExistingNode), driver);
            Assert.False(actualFalse);

            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {meConstraint}"));
            }

            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "ME"));
            Assert.Equal(meConstraint, GetConstraints("NODE KEY", "ME").First()[0]);

            // Test True
            var actualTrue = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.MatchesExistingNode), driver);
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_TrueFalse_When_NodeKey_MatchesExisting_For_Type_WithOut_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));
            GraphConnection.SetDriver(driver);
            // Test False
            var actualFalse = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.MatchesExistingNode));
            Assert.False(actualFalse);

            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {meConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "ME"));
            Assert.Equal(meConstraint, GetConstraints("NODE KEY", "ME").First()[0]);

            // Test True
            var actualTrue = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.MatchesExistingNode));
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_TrueFalse_When_NodeKey_MatchesExisting_For_Type_With_Session()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));
            using (var session = driver.Session(AccessMode.Read))
            {
                // Test False
                var actualFalse = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.MatchesExistingNode), driver);
                Assert.False(actualFalse);
            }
            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {meConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "ME"));
            Assert.Equal(meConstraint, GetConstraints("NODE KEY", "ME").First()[0]);

            using (var session = driver.Session(AccessMode.Read))
            {
                // Test True
                var actualTrue = Schematica.Neo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.MatchesExistingNode), driver);
                Assert.True(actualTrue);
            }
        }

        public void Dispose()
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx =>
                {
                    if (GetConstraints("NODE KEY", "ME", tx).Count() == 1)
                        tx.Run($"DROP {meConstraint}");
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
