using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Neo4j.Schema.Tests
{
    public class Schema_Initialize_Tests
    {

        [Fact]
        public void Initialize_Will_SetNodeKey_For_Type()
        {

        }

        [Fact]
        public void Initialize_Will_SetNodeKeys_For_CollectionOf_Types()
        {

        }

        [Fact]
        public void Initialize_Will_SetNodeKeys_For_All_Types_In_Assembly()
        {

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


        // TODO: We probably should test the individual private methods in NodeKey.cs individually.
        // Schema.Constraint.NodeKey.Create(type, tx|session|driver, forceReplace = false) => If a node key does not already exist, create it.
        // Schema.Constraint.NodeKey.Drop(type, tx|session|driver) => if a node key exists, drop it.
        // Schema.Constraint.NodeKey.Exists(type, tx|session|driver) => bool if any node key exists for the provided type.Label().
        // Schema.Constraint.NodeKey.MatchesExisting(type, tx|session|driver) => bool if the type NodeKey matches the nodekey in graph.

    }
}
