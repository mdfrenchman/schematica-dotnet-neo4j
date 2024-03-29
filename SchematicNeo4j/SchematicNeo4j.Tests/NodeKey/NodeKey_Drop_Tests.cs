﻿using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;
using SchematicNeo4j;

namespace SchematicNeo4j.Tests.NodeKey
{
    public class NodeKey_Drop_Tests : IDisposable
    {
        private IDriver driver = null;

        private string personConstraint = "CREATE CONSTRAINT `nkPerson` FOR (n:`Person`) REQUIRE (n.`Name`) IS NODE KEY OPTIONS {indexConfig: {}, indexProvider: 'range-1.0'}";
        private ConstraintRecord personConstraintRecord = new() { name = "nkPerson" };


        public NodeKey_Drop_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "SchematicNeo4j-Test!"));
            //GraphConnection.SetDriver(driver);
        }

        [Fact]
        public void NodeKey_Drop_Will_DropNodeKey_For_Type_With_Driver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
            // Setup
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"{personConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            // Execute
            SchematicNeo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Person), driver);

            // After
            Assert.Empty(GetConstraints("NODE KEY", "Person"));


        }

        [Fact]
        public void NodeKey_Drop_Will_DropNodeKey_For_Type_With_GraphConnection_SetDriver()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
            // Setup
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"{personConstraint}"));
            }

            // TODO: This is occassionally failing with the Create_Tests using the same constraint and tests running in parallel.
            // Running the Drop tests separate from create is successful 100% of the time.

            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            // Set Driver
            SchematicNeo4j.GraphConnection.SetDriver(driver);
            // Execute
            SchematicNeo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Person));

            // After
            Assert.Empty(GetConstraints("NODE KEY", "Person"));

        }

        [Fact]
        public void NodeKey_Drop_Will_DropNodeKey_For_Type_With_Session()
        {
            // Before
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
            // Setup
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                session.ExecuteWrite(tx => tx.Run($"{personConstraint}"));
            }
            //Confirm Setup
            Assert.Single(GetConstraints("NODE KEY", "Person"));
            Assert.Equal(personConstraint, GetConstraints("NODE KEY", "Person").First()[0]);

            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                // Execute
                SchematicNeo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Person), session);
            }
            // After
            Assert.Empty(GetConstraints("NODE KEY", "Person"));
        }

        [Fact]
        public void NodeKey_Drop_Will_ThrowException_For_Type_With_No_Driver()
        {

            // Set Driver for SchematicNeo4j to null
            SchematicNeo4j.GraphConnection.SetDriver(null);
            // Execute
            Assert.Throws<Neo4jException>(() => SchematicNeo4j.Constraints.NodeKey.Drop(typeof(Tests.DomainSample.Vehicle), driver: null));

        }
        public void Dispose()
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Write)))
            {
                if (GetConstraints("NODE KEY", "Person").Count() == 1)
                    session.ExecuteWrite(tx => tx.Run($"DROP CONSTRAINT {personConstraintRecord.name}"));
            }
        }

        private List<IRecord> GetConstraints(string ofType, string forLabel)
        {
            using (var session = driver.Session(o => o.WithDefaultAccessMode(AccessMode.Read)))
            {
                return session.ExecuteRead(tx => tx.Run(
                    "SHOW CONSTRAINTS YIELD createStatement WHERE createStatement contains (':`'+$typeLabel+'`') AND createStatement contains $constraintType RETURN createStatement",
                    new { typeLabel = forLabel, constraintType = ofType }
                    ).ToList());
            }
        }
    }
}
