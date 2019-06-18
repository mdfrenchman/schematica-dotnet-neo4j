using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using SchematicNeo4j;

namespace SchematicNeo4j.Tests.NodeKey
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
        public void NodeKey_MatchesExisting_Returns_When_NodeKey_MatchesExisting_For_Type_With_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));

            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {meConstraint}"));
            }

            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "ME"));
            Assert.Equal(meConstraint, GetConstraints("NODE KEY", "ME").First()[0]);

            // Test True
            var actualTrue = SchematicNeo4j.Constraints.NodeKey.MatchesExisting(typeof(Tests.DomainSample.MatchesExistingNode), driver);
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_When_NodeKey_MatchesExisting_For_Type_WithOut_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));
           
            
            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {meConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "ME"));
            Assert.Equal(meConstraint, GetConstraints("NODE KEY", "ME").First()[0]);

            // Test True
             GraphConnection.SetDriver(driver);
            var actualTrue = SchematicNeo4j.Constraints.NodeKey.MatchesExisting(typeof(Tests.DomainSample.MatchesExistingNode));
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_When_NodeKey_MatchesExisting_For_Type_With_Session()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));
            
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
                var actualTrue = SchematicNeo4j.Constraints.NodeKey.MatchesExisting(typeof(Tests.DomainSample.MatchesExistingNode), session);
                Assert.True(actualTrue);
            }
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_False_When_None_Exists() {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));
            // Returns False
            using (var session = driver.Session(AccessMode.Read))
            {
                var actual = SchematicNeo4j.Constraints.NodeKey.MatchesExisting(typeof(Tests.DomainSample.MatchesExistingNode), session);
                Assert.False(actual);
            }

        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_True_When_Exact_Match_Exists() {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));

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
                var actualTrue = SchematicNeo4j.Constraints.NodeKey.MatchesExisting(typeof(Tests.DomainSample.MatchesExistingNode), session);
                Assert.True(actualTrue);
            }
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_False_When_Different_Exists() {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "ME"));

            // Setup to Wrong Key
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE CONSTRAINT ON ( me:ME ) ASSERT (me.WrongOne) IS NODE KEY"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "ME"));
            Assert.NotEqual(meConstraint, GetConstraints("NODE KEY", "ME").First()[0]);
            // Return False
            using (var session = driver.Session(AccessMode.Read))
            {
                var actual = SchematicNeo4j.Constraints.NodeKey.MatchesExisting(typeof(Tests.DomainSample.MatchesExistingNode), session);
                Assert.False(actual);
            }

            // Setup to Wrong Key
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"DROP CONSTRAINT ON ( me:ME ) ASSERT (me.WrongOne) IS NODE KEY"));
            }
        }

        [Fact]
        public void NodeKey_MatchesExisting_Will_ThrowException_For_Type_With_No_Driver()
        {

            // Set Driver for SchematicNeo4j to null
            SchematicNeo4j.GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => SchematicNeo4j.Constraints.NodeKey.MatchesExisting(typeof(Tests.DomainSample.Vehicle), driver: null));

        }

        public void Dispose()
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx =>
                {
                    if (GetConstraints("NODE KEY", "ME", tx).Count() == 1)
                        try
                        {
                            tx.Run($"DROP {meConstraint}");
                        }
                        catch (Exception)
                        {
                            
                        }
                        
                });
            }
        }

        private IStatementResult GetConstraints(string ofType, string forLabel, ITransaction tx)
        {
            return tx.Run(
                    "CALL db.constraints() yield description WHERE description contains (':'+$typeLabel+' ') AND description contains $constraintType RETURN description",
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
