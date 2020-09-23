using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace SchematicNeo4j.Tests
{
    public class Schema_Clear_Tests : IDisposable
    {
        private IDriver driver = null;
        private string carConstraint = "CONSTRAINT ON ( car:Car ) ASSERT (car.Make, car.Model, car.ModelYear) IS NODE KEY";
        private string personConstraint = "CONSTRAINT ON ( person:Person ) ASSERT (person.Name) IS NODE KEY";

        public Schema_Clear_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
            
            //GraphConnection.SetDriver(driver);
        }

        #region Missing Driver Throws an Exception
        [Fact]
        public void Clear_Will_Throw_Exception_For_Type_Collection_With_No_Driver()
        {
            // Set Driver for SchematicNeo4j to null
            SchematicNeo4j.GraphConnection.SetDriver(null);
            var listOfTypes = new List<Type>() { typeof(Tests.DomainSample.Person), typeof(Tests.DomainSample.Vehicle) };
            // Execute
            Assert.Throws<Neo4jException>(() => SchematicNeo4j.Schema.Clear(domainTypes: listOfTypes, driver: null));
        }
        [Fact]
        public void Clear_Will_Throw_Exception_For_Type_With_No_Driver()
        {
            // Set Driver for SchematicNeo4j to null
            SchematicNeo4j.GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => SchematicNeo4j.Schema.Clear(typeof(Tests.DomainSample.Vehicle), driver: null));
        }
        #endregion
        
        #region Empty graph and schema starting point
        [Fact]
        public void Clear_With_EmptySchema_Will_Do_Nothing_For_Type()
        {
            Assert.Empty(GetConstraints("NODE KEY", "Car"));

            SchematicNeo4j.Schema.Clear(typeof(Tests.DomainSample.Vehicle), driver);

            Assert.Empty(GetConstraints("NODE KEY", "Car"));
        }

        [Fact]
        public void Clear_With_EmptySchema_Will_Do_Nothing_For_CollectionOf_Types()
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

            SchematicNeo4j.Schema.Clear(domainTypeList, driver);

            Assert.Empty(GetConstraints("NODE KEY", "Car"));
            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
            Assert.Empty(GetConstraints("NODE KEY", "Person"));

        }

        [Fact]
        public void Clear_With_EmptySchema_Will_Do_Nothing_For_All_Types_In_Assembly()
        {
            Assert.Empty(GetConstraints("NODE KEY", "Car"));
            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
            Assert.Empty(GetConstraints("NODE KEY", "Person"));

            SchematicNeo4j.Schema.Clear(assembly: Assembly.GetAssembly(typeof(DomainSample.Person)), driver);

            Assert.Empty(GetConstraints("NODE KEY", "Car"));
            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
        }
        #endregion

        #region With Existing NodeKey
        [Fact]
        public void Clear_With_Existing_NodeKey_Will_RemoveNodeKey_For_Type()
        {
            // Setup Test
            SchematicNeo4j.Constraints.NodeKey.Create(typeof(Tests.DomainSample.Vehicle), driver);
            // Verify Setup
            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);
            // Execute
            SchematicNeo4j.Schema.Clear(typeof(Tests.DomainSample.Vehicle), driver);
            // Confirm Execution
            Assert.Empty(GetConstraints("NODE KEY", "Car"));
        }

        [Fact]
        public void Clear_With_Existing_NodeKey_Will_RemoveNodeKey_For_CollectionOf_Types()
        {

            var domainTypeList = new List<Type>()
            {
                typeof(DomainSample.Keyless),
                typeof(DomainSample.Person),
                typeof(DomainSample.Vehicle)
            };

            // Setup Test
            SchematicNeo4j.Schema.Initialize(domainTypeList, driver);
            // Verify Setup
            Assert.Single(GetConstraints("NODE KEY", "Car"));
            Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            // Execute
            SchematicNeo4j.Schema.Clear(domainTypes: domainTypeList, driver: driver);
            // Confirm Execution
            Assert.Empty(GetConstraints("NODE KEY", "Car"));
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
            Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
        }
 
        //[Fact]
        //public void Clear_With_EmptySchema_Will_Do_Nothing_For_All_Types_In_Assembly()
        //{
        //    Assert.Empty(GetConstraints("NODE KEY", "Car"));
        //    Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
        //    Assert.Empty(GetConstraints("NODE KEY", "Person"));


        //    Schematica.SchematicNeo4j.Clear(assembly: Assembly.GetAssembly(typeof(DomainSample.Person)), driver);

        //    Assert.Single(GetConstraints("NODE KEY", "Car"));
        //    Assert.Equal(carConstraint, GetConstraints("NODE KEY", "Car").First()[0]);

        //    Assert.Single(GetConstraints("NODE KEY", "Person"));
        //    Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

        //    Assert.Empty(GetConstraints("NODE KEY", "Keyless"));
        //}

        //[Fact(Skip = "Not Implimented Yet")]
        //public void Clear_Will_RemoveIndexes_For_Type()
        //{

        //}

        //[Fact(Skip = "Not Implimented Yet")]
        //public void Clear_Will_RemoveIndexes_For_CollectionOf_Types()
        //{

        //}

        //[Fact(Skip = "Not Implimented Yet")]
        //public void Clear_Will_RemoveIndexes_For_All_Types_In_Assembly()
        //{

        //}

        #endregion

        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                if (GetConstraints("NODE KEY", "Car").Count() == 1)
                    session.WriteTransaction(tx => tx.Run($"DROP {carConstraint}"));
                if (GetConstraints("NODE KEY", "Person").Count() == 1)
                    session.WriteTransaction(tx => tx.Run($"DROP {personConstraint}"));
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


        // TODO: We probably should test the individual private methods in NodeKey.cs individually.
        // Schema.Constraint.NodeKey.Create(type, tx|session|driver, forceReplace = false) => If a node key does not already exist, create it.
        // Schema.Constraint.NodeKey.Drop(type, tx|session|driver) => if a node key exists, drop it.
        // Schema.Constraint.NodeKey.Exists(type, tx|session|driver) => bool if any node key exists for the provided type.Label().
        // Schema.Constraint.NodeKey.MatchesExisting(type, tx|session|driver) => bool if the type NodeKey matches the nodekey in graph.

    }
}
