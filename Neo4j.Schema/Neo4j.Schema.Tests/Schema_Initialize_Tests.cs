using Neo4j.Driver.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Neo4j.Schema.Tests
{
    public class Schema_Initialize_Tests : IDisposable
    {
        private IDriver driver = null;
        private string carConstraint = "CONSTRAINT ON ( car:Car ) ASSERT (car.Make, car.Model, car.ModelYear) IS NODE KEY";
        private string personConstraint = "CONSTRAINT ON ( person:Person ) ASSERT (person.Name) IS NODE KEY";

        public Schema_Initialize_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
            //GraphConnection.SetDriver(driver);
        }

        #region Empty graph and schema starting point
        [Fact]
        public void Initialize_Will_SetNodeKey_For_Type()
        {
            Assert.Empty(GetConstraints("NODE KEY", "Car"));

            Schema.Initialize(typeof(Tests.DomainSample.Vehicle), driver);

            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);
        }

        [Fact]
        public void Initialize_Will_SetNodeKeys_For_CollectionOf_Types()
        {
            Assert.Empty(GetConstraints("NODE KEY", "Car"));
            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
            Assert.Empty(GetConstraints("NODE KEY", "Person"));

            var domainTypeList = new List<Type>()
            {
                typeof(DomainSample.Keyless),
                typeof(DomainSample.Person),
                typeof(DomainSample.Vehicle)
            };

            Schema.Initialize(domainTypeList, driver);

            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);

            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
        }

        [Fact]
        public void Initialize_Will_SetNodeKeys_For_All_Types_In_Assembly()
        {
            Assert.Empty(GetConstraints("NODE KEY", "Car"));
            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
            Assert.Empty(GetConstraints("NODE KEY", "Person"));

           
            Schema.Initialize(assembly:Assembly.GetAssembly(typeof(DomainSample.Person)), driver);

            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);

            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
        }

        [Fact]
        public void Initialize_Will_SetIndexes_For_Type()
        {

        }

        [Fact]
        public void Initialize_Will_SetIndexes_For_CollectionOf_Types()
        {

        }

        [Fact]
        public void Initialize_Will_SetIndexes_For_All_Types_In_Assembly()
        {

        }
        #endregion

        public void Dispose()
        {
            using (var session = driver.Session(AccessMode.Write))
            {
                if (GetConstraints("NODE KEY","Car").Count() == 1)
                    session.WriteTransaction(tx => tx.Run($"DROP {carConstraint}"));
                if (GetConstraints("NODE KEY", "Person").Count() == 1)
                    session.WriteTransaction(tx => tx.Run($"DROP {personConstraint}"));
            }
        }

        private IStatementResult GetConstraints(string ofType, string forLabel)
        {
            using (var session = driver.Session(AccessMode.Read))
            {
                var result = session.ReadTransaction(tx => tx.Run(
                    "CALL db.constraints() yield description WHERE description contains $typeLabel AND description contains $constraintType RETURN description",
                    new { typeLabel = forLabel, constraintType = ofType }
                    ));
                return result;
            }
        }


        // TODO: We probably should test the individual private methods in NodeKey.cs individually.
        // Schema.Constraint.NodeKey.Create(type, tx|session|driver, forceReplace = false) => If a node key does not already exist, create it.
        // Schema.Constraint.NodeKey.Drop(type, tx|session|driver) => if a node key exists, drop it.
        // Schema.Constraint.NodeKey.Exists(type, tx|session|driver) => bool if any node key exists for the provided type.Label().
        // Schema.Constraint.NodeKey.MatchesExisting(type, tx|session|driver) => bool if the type NodeKey matches the nodekey in graph.

    }
}
