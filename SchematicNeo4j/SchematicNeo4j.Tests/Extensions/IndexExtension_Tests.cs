using Xunit;
using SchematicNeo4j.Tests.DomainSample;
using System.Collections.Generic;

namespace SchematicNeo4j.Tests.Extensions
{
    public class IndexExtension_Tests
    {
        [Fact]
        public void IndexExtension_Indexes_With_No_Properties_Returns_Empty_List()
        {
            var testType = typeof(Keyless);
            Assert.Empty(testType.Indexes());

        }

        [Fact]
        public void IndexExtension_Indexes_Will_Not_Return_NodeKey()
        {
            var testType = typeof(Person);
            Index personNodeKey = new Index(label: "Person", properties: new string[] { "Name" });
            Assert.DoesNotContain(personNodeKey, testType.Indexes());
        }

        [Fact]
        public void IndexExtension_Indexes_With_One_Index_Returns_List()
        {
            var testType = typeof(Person);
            List<Index> expected = new List<Index>() {
                new Index(name: "PersonIndex", label: "Person", properties: new string[] { "Age" })};
            var actual = testType.Indexes();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IndexExtension_Indexes_With_Multiple_Indexes_Returns_List()
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
    }
}
