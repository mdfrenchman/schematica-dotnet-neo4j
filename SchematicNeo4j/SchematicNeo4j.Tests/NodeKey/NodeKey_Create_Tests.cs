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
    public class NodeKey_Create_Tests : IDisposable
    {
        private IDriver driver = null;
        private string carConstraint = "CREATE CONSTRAINT `nkCar` FOR (n:`Car`) REQUIRE (n.`Make`, n.`Model`, n.`ModelYear`) IS NODE KEY OPTIONS {indexConfig: {}, indexProvider: 'range-1.0'}";
        private ConstraintRecord carConstraintRecord = new() { name = "nkCar" };
        private string personConstraint = "CREATE CONSTRAINT `nkPerson` FOR (person:`Person`) REQUIRE (person.`Name`) IS NODE KEY OPTIONS {indexConfig: {}, indexProvider: 'range-1.0'}";
        private ConstraintRecord personConstraintRecord = new() { name = "nkPerson" };

        public NodeKey_Create_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "SchematicNeo4j-Test!"));
            //GraphConnection.SetDriver(driver);
        }

        [Fact]
        public void NodeKey_Create_Will_CreateNodeKey_For_Type_With_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Car"));

            // Execute
            SchematicNeo4j.Constraints.NodeKey.Create(typeof(Tests.DomainSample.Vehicle), driver);

            // After
            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);


        }

        [Fact]
        public void NodeKey_Create_Will_CreateNodeKey_For_Type_With_GraphConnection_SetDriver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Car"));

            // Set Driver for SchematicNeo4j
            SchematicNeo4j.GraphConnection.SetDriver(driver);
            // Execute
            SchematicNeo4j.Constraints.NodeKey.Create(typeof(Tests.DomainSample.Vehicle));

            // After
            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);

        }

        [Fact]
        public void NodeKey_Create_Will_ThrowException_For_Type_With_No_Driver()
        {

            // Set Driver for SchematicNeo4j to null
            SchematicNeo4j.GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => SchematicNeo4j.Constraints.NodeKey.Create(typeof(Tests.DomainSample.Vehicle), driver: null));

        }

        [Fact]
        public void NodeKey_Create_Will_CreateNodeKey_For_Type_With_Session()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Car"));

            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                // Execute
                SchematicNeo4j.Constraints.NodeKey.Create(typeof(Tests.DomainSample.Vehicle), session);
            }
            // After
            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);
        }

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                if (GetConstraints("NODE KEY", "Car").Count() == 1)
                    session.ExecuteWrite(tx => tx.Run($"DROP CONSTRAINT {carConstraintRecord.name}"));
                if (GetConstraints("NODE KEY", "Person").Count() == 1)
                    session.ExecuteWrite(tx => tx.Run($"DROP CONSTRAINT {personConstraintRecord.name}"));
            }
        }

        private List<IRecord> GetConstraints(string ofType, string forLabel)
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                return session.ExecuteRead(tx =>
                tx.Run(
                    "SHOW CONSTRAINTS YIELD createStatement WHERE createStatement contains (':`'+$typeLabel+'`') AND createStatement contains $constraintType RETURN createStatement",
                    new { typeLabel = forLabel, constraintType = ofType }
                    ).ToList()
                );
            }
        }


    }
}
