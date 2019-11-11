using Xunit;
using SchematicNeo4j.Tests.DomainSample;
using System.Collections.Generic;
using SchematicNeo4j.Tests.DomainSample.InheritAbstract;

namespace SchematicNeo4j.Tests.Extensions
{
    public class IndexExtension_Tests
    {
        [Fact]
        public void IndexExtension_Indexes_Type_With_No_Properties_Returns_Empty_List()
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
        public void IndexExtension_Indexes_Type_With_One_Index_Returns_List()
        {
            var testType = typeof(Person);
            List<Index> expected = new List<Index>() {
                new Index(name: "PersonIndex", label: "Person", properties: new string[] { "Age" })};
            var actual = testType.Indexes();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IndexExtension_Indexes_Type_With_Multiple_Indexes_Returns_List()
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

        [Fact]
        public void IndexExtension_Indexes_Does_Not_Return_Indexes_From_Parent_Class_When_IsAbstract_Is_True()
        {
            var parentType = typeof(TestIndexParent);
            var subType = typeof(TestIndexSubClass);

            List<Index> expectedParentIndexes = new List<Index>() {
                new Index(name: "TIP_P12", label: "TestIndexParent", properties: new string[] { "ParentProp1", "ParentProp2" }),
                new Index(name: "TIP_P2", label: "TestIndexParent", properties: new string[] { "ParentProp2" })
            };
            List<Index> expectedSubClassIndexes = new List<Index>() {
                new Index(name: "TISC_P1S1", label: "TestIndexSubClass", properties: new string[] { "SubProp1", "ParentProp1" }),
                new Index(name: "TISC_Sub12", label: "TestIndexSubClass", properties: new string[] { "SubProp1", "SubProp2" }),
                new Index(name: "TISC_Sub2", label: "TestIndexSubClass", properties: new string[] { "SubProp2" })
            };
            
            Assert.Equal(expectedParentIndexes, parentType.Indexes());
            Assert.Equal(expectedSubClassIndexes, subType.Indexes());
            
        }
    }
}
