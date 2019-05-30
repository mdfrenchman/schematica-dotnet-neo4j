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
    public class NodeKey_Drop_Tests : IDisposable
    {
        private IDriver driver = null;
        
        private string personConstraint = "CONSTRAINT ON ( person:Person ) ASSERT (person.Name) IS NODE KEY";

        public NodeKey_Drop_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
            //GraphConnection.SetDriver(driver);
        }

        [Fact]
        public void NodeKey_Drop_Will_DropNodeKey_For_Type_With_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {personConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            // Execute
            Schematica.Neo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Person), driver);

            // After
            Assert.Empty(GetConstraints("NODE KEY", "Person"));


        }

        [Fact]
        public void NodeKey_Drop_Will_DropNodeKey_For_Type_With_GraphConnection_SetDriver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {personConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            // Set Driver
            Schematica.Neo4j.GraphConnection.SetDriver(driver);
            // Execute
            Schematica.Neo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Person));

            // After
            Assert.Empty(GetConstraints("NODE KEY", "Person"));

        }

        [Fact]
        public void NodeKey_Drop_Will_DropNodeKey_For_Type_With_Session()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
            // Setup
            using (var session = driver.Session(AccessMode.Write))
            {
                session.WriteTransaction(tx => tx.Run($"CREATE {personConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            using (var session = driver.Session(AccessMode.Write))
            {
                // Execute
                Schematica.Neo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Person), session);
            }
            // After
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
        }

        [Fact]
        public void NodeKey_Drop_Will_ThrowException_For_Type_With_No_Driver()
        {

            // Set Driver for Schematica.Neo4j to null
            Schematica.Neo4j.GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => Schematica.Neo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Vehicle), driver: null));

        }
        public void Dispose()
        {
            using (var session = driver.Session(AccessMode.Write))
            {
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


    }
}
