
using Xunit;
using Schematica.Neo4j;
using Neo4j.Schema.Tests.DomainSample;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4j.Schema.Tests.Extensions
{
    public class NodeExtension_Tests
    {
        #region Label
        [Fact]
        public void NodeExtension_Label_With_NodeAttribute_Returns_Label_AttributeProperty()
        {
            var testType = typeof(Neo4j.Schema.Tests.DomainSample.Vehicle);
            var actual = testType.Label();
            Xunit.Assert.NotEqual("Vehicle", actual);
            Xunit.Assert.Equal("Car", actual);
        }

        [Fact]
        public void NodeExtension_Label_With_OUT_NodeAttribute_Returns_ClassName()
        {
            var testType = typeof(Person);
            Xunit.Assert.Equal("Person", testType.Label());
        }

        [Fact]
        public void NodeExtension_Label_With_NodeAttribute_And_No_Label_AttributeProperty_Returns_ClassName()
        {
            var testType = typeof(Keyless);
            Xunit.Assert.Equal("Keyless", testType.Label());
        }
        #endregion

        #region NodeKey
        [Fact]
        public void NodeExtension_NodeKey_With_No_Properties_Returns_Empty_List()
        {
            var testType = typeof(Keyless);
            Xunit.Assert.Empty(testType.NodeKey());
        }

        [Fact]
        public void NodeExtension_NodeKey_With_One_Property_Returns_List()
        {
            var testType = typeof(Person);
            List<string> expected = new List<string>() { "Name" };
            Xunit.Assert.Equal(expected, testType.NodeKey());
        }

        [Fact]
        public void NodeExtension_NodeKey_With_Multiple_Properties_Returns_List()
        {
            var testType = typeof(Vehicle);
            List<string> expected = new List<string>() { "Make", "Model", "ModelYear" };
            Xunit.Assert.Equal(expected, testType.NodeKey());
        }

        #endregion
    }
}
