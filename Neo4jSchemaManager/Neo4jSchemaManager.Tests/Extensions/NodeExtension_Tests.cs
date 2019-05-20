
using Xunit;
using Neo4jSchemaManager;
using System;
using System.Collections.Generic;
using System.Text;

namespace Neo4jSchemaManager.Tests.Extensions
{
    public class NodeExtension_Tests
    {
        #region Label
        [Fact]
        public void NodeExtension_Label_With_NodeAttribute_Returns_Label_AttributeProperty()
        {
            var testType = typeof(DomainTestSample.Vehicle);
            var actual = testType.Label();
            Xunit.Assert.NotEqual("Vehicle", actual);
            Xunit.Assert.Equal("Car", actual);
        }

        [Fact]
        public void NodeExtension_Label_With_OUT_NodeAttribute_Returns_ClassName()
        {
            var testType = typeof(DomainTestSample.Person);
            Xunit.Assert.Equal("Person", testType.Label());
        }

        [Fact]
        public void NodeExtension_Label_With_NodeAttribute_And_No_Label_AttributeProperty_Returns_ClassName()
        {
            var testType = typeof(DomainTestSample.Keyless);
            Xunit.Assert.Equal("Keyless", testType.Label());
        }
        #endregion

        #region NodeKey
        [Fact]
        public void NodeExtension_NodeKey_With_No_Properties_Returns_Empty_List()
        {
            var testType = typeof(DomainTestSample.Keyless);
            Xunit.Assert.Empty(testType.NodeKey());
        }

        [Fact]
        public void NodeExtension_NodeKey_With_One_Property_Returns_List()
        {
            var testType = typeof(DomainTestSample.Person);
            List<string> expected = new List<string>() { "Name" };
            Xunit.Assert.Equal(expected, testType.NodeKey());
        }

        [Fact]
        public void NodeExtension_NodeKey_With_Multiple_Properties_Returns_List()
        {
            var testType = typeof(DomainTestSample.Vehicle);
            List<string> expected = new List<string>() { "Make", "Model", "ModelYear" };
            Xunit.Assert.Equal(expected, testType.NodeKey());
        }

        #endregion
    }
}
