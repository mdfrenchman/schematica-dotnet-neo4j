using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace SchematicNeo4j.Tests
{
    public class Schema_SubClass_Tests : IDisposable
    {
        private IDriver driver = null;
        private string carConstraint = "CONSTRAINT ON ( car:Car ) ASSERT (car.Make, car.Model, car.ModelYear) IS NODE KEY";
        private string truckConstraint = "CONSTRAINT ON ( truck:Truck ) ASSERT (truck.Make, truck.Model, truck.ModelYear) IS NODE KEY";

        public Schema_SubClass_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
            //GraphConnection.SetDriver(driver);
        }

        // Test that Create keys off of first Label in NodeAttribute.Label
        [Fact]
        public void NodeKey_Create_Will_Use_First_Label_in_NodeAttribute_Label()
        {
            // Expected initial state
            Assert.Empty(GetConstraints("NODE KEY", "Car"));

            //Create for Truck will match exists for Car(from Vehicle) and Create if missing.
            // Truck NodeAttribute == [Node(Label="Car:Truck")]
            SchematicNeo4j.Constraints.NodeKey.Create(type: typeof(DomainSample.Truck), driver: driver);

            // Actual end state : Car node key is created because it doesn't exist.
            Assert.Empty(GetConstraints("NODE KEY", "Truck"));
            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);
        }
        // If NodeAttributeLabel is not split, then proceeds with normal order.
        // Test that Drop won't drop inherited nodekey
        // Test that exists and matchexisting will key off label used for create.
        // Test that initialize won't create node key for subclass if a complex label is specified.

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                if (GetConstraints("NODE KEY", "Car").Count() == 1)
                    session.WriteTransaction(tx => tx.Run($"DROP {carConstraint}"));
            }
        }

        private IResult GetConstraints(string ofType, string forLabel)
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                var result = session.ReadTransaction(tx => tx.Run(
                    "CALL db.constraints() yield description WHERE description contains (':'+$typeLabel+' ') AND description contains $constraintType RETURN description",
                    new { typeLabel = forLabel, constraintType = ofType }
                    ));
                return result;
            }
        }
    }
}
