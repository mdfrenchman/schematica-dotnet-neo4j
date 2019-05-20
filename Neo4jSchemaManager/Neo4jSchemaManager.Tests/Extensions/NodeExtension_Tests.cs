
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
        public void NodeExtension_Label_With_NodeAttribute_Returns_AttributeProperty_Label()
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
        #endregion
    }
}
