using Xunit;
using SchematicNeo4j.Tests.DomainSample;
using System;
using System.Collections.Generic;

namespace SchematicNeo4j.Tests.Extensions
{
    public class NodeExtension_Tests
    {
        #region Label
        [Fact]
        public void NodeExtension_Label_With_NodeAttribute_Returns_Label_AttributeProperty()
        {
            var testType = typeof(Vehicle);
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

        #region Setup for Dynamic Type test
        internal interface ITestNode
        {
        }
        public class Generic : ITestNode
        {
            internal static string TestLabelForGeneric<T>(){
                Type type = typeof(T);
                return type.Label();
            }
        }
        #endregion
        [Fact]
        public void NodeExtension_Label_Returns_ClassName_From_DynamicType()
        {
            var anonymousTypeLabel = Generic.TestLabelForGeneric<Keyless>();
            Xunit.Assert.Equal("Keyless",anonymousTypeLabel);
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
