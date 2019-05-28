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
        private string teConstraint = "CONSTRAINT ON ( te:TE ) ASSERT (te.Name, te.Identity) IS NODE KEY";

        public NodeKey_MatchesExisting_Tests()
        {
            driver = GraphDatabase.Driver("bolt://localhost:7687", AuthTokens.Basic("neo4j", "scratch"));
            //GraphConnection.SetDriver(driver);
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_True_When_NodeKey_MatchesExisting_For_Type_With_Driver()
        {
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_True_When_NodeKey_MatchesExisting_For_Type_WithOut_Driver()
        {
        }

        [Fact]
        public void NodeKey_MatchesExisting_Returns_True_When_NodeKey_MatchesExisting_For_Type_With_Session()
        {
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
