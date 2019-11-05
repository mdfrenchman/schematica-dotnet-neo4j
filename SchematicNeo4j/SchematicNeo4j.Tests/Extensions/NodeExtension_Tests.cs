
using Xunit;
using SchematicNeo4j;
using SchematicNeo4j.Tests.DomainSample;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SchematicNeo4j.Tests.Extensions
{
    public class NodeExtension_Tests
    {
        #region Label
        [Fact]
        public void NodeExtension_Label_With_NodeAttribute_Returns_Label_AttributeProperty()
        {
            var testType = typeof(SchematicNeo4j.Tests.DomainSample.Vehicle);
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

        #region Indexes
        [Fact]
        public void NodeExtension_Indexes_With_No_Properties_Returns_Empty_List()
        {
            var testType = typeof(Keyless);
            Assert.Empty(testType.Indexes());

        }

        [Fact]
        public void NodeExtension_Indexes_With_One_Index_Returns_List()
        {
            var testType = typeof(Person);
            List<Index> expected = new List<Index>() { 
                new Index(name: "PersonIndex", label: "Person", properties: new string[] { "Age" })};
            var actual = testType.Indexes();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void NodeExtension_Indexes_With_Multiple_Indexes_Returns_List()
        {
            var testType = typeof(Vehicle);
            List<Index> expected = new List<Index>() {
                new Index(name: "CarMake", label: "Car", properties: new string[] { "Make" }),
                new Index(name: "CarMakeModel", label: "Car", properties: new string[] { "Make", "Model" }),
                new Index(name: "CarModelYear", label: "Car", properties: new string[] { "ModelYear" })
            };
            var actual = testType.Indexes();
            Assert.Equal(expected, actual);
        }

        #endregion
    }
}
