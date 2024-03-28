using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using SchematicNeo4j;

namespace SchematicNeo4j.Tests.NodeKey
{
    public class NodeKey_Exists_Tests :IDisposable
    {
        private IDriver driver = null;
        private string teConstraint = "CREATE CONSTRAINT `nkTE` FOR (n:`TE`) REQUIRE (n.`Name`, n.`Identity`) IS NODE KEY OPTIONS {indexConfig: {}, indexProvider: 'range-1.0'}";
        private ConstraintRecord teConstraintRecord = new() { name = "nkTE" };


        public NodeKey_Exists_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "SchematicNeo4j-Test!"));
            //GraphConnection.SetDriver(driver);
        }

        [Fact]
        public void NodeKey_Exists_Will_Return_TrueFalse_When_NodeKey_Exists_For_Type_With_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "TE"));

            // Test False
            var actualFalse = SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
            Assert.False(actualFalse);

            // Setup
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"{teConstraint}"));
            }

            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "TE"));
            Assert.Equal(teConstraint, GetConstraints("NODE KEY", "TE").First()[0]);

            // Test True
            var actualTrue = SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_Exists_Will_Return_TrueFalse_When_NodeKey_Exists_For_Type_WithOut_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "TE"));
            GraphConnection.SetDriver(driver);
            // Test False
            var actualFalse = SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode));
            Assert.False(actualFalse);

            // Setup
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"{teConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "TE"));
            Assert.Equal(teConstraint, GetConstraints("NODE KEY", "TE").First()[0]);

            // Test True
            var actualTrue = SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode));
            Assert.True(actualTrue);
        }

        [Fact]
        public void NodeKey_Exists_Will_Return_TrueFalse_When_NodeKey_Exists_For_Type_With_Session()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "TE"));
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                // Test False
                var actualFalse = SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
                Assert.False(actualFalse);
            }
            // Setup
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"{teConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "TE"));
            Assert.Equal(teConstraint, GetConstraints("NODE KEY", "TE").First()[0]);

            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                // Test True
                var actualTrue = SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.TestExistsNode), driver);
                Assert.True(actualTrue);
            }
        }

        [Fact]
        public void NodeKey_Exists_Will_ThrowException_For_Type_With_No_Driver()
        {

            // Set Driver for SchematicNeo4j to null
            SchematicNeo4j.GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.Vehicle), driver: null));

        }

        [Fact]
        public void NodeKey_Exists_Will_Not_Match_For_A_Partial_TypeName()
        {
            // Testing Get where a node key constraint for 'ThatThing' would be returned if the Label in question was 'That'
            var listConstraints = new List<string>() {
                "CONSTRAINT nkThatThing FOR (tt:ThatThing) REQUIRE (tt.Name, tt.Identity) IS NODE KEY",
                "CONSTRAINT nkThat FOR (tt:That) REQUIRE (tt.Name, tt.Identity) IS NODE KEY"
            };

            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"CREATE {listConstraints[0]}"));
            }
            Assert.True(SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.ThatThing), driver));
            // Test Get via Exists.
            Assert.False(SchematicNeo4j.Constraints.NodeKey.Exists(typeof(Tests.DomainSample.That), driver));
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"DROP CONSTRAINT nkThatThing"));
            }
        }
        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                    session.ExecuteWrite(tx =>
                    {
                        if (GetConstraints("NODE KEY", "TE", tx).Count() == 1)
                            tx.Run($"DROP CONSTRAINT {teConstraintRecord.name}");
                        return true;
                    });
            }
        }

        private IResult GetConstraints(string ofType, string forLabel, IQueryRunner tx)
        {
            return tx.Run(
                    "SHOW CONSTRAINTS YIELD createStatement WHERE createStatement contains (':`'+$typeLabel+'`') AND createStatement contains $constraintType RETURN createStatement",
                    new { typeLabel = forLabel, constraintType = ofType }
                    );
        }

        private List<IRecord> GetConstraints(string ofType, string forLabel)
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                return session.ExecuteRead(tx => GetConstraints(ofType, forLabel, tx).ToList());
            }
        }


    }
}
