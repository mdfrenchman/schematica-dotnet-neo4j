using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;

namespace SchematicNeo4j
{
    public class ConstraintRecord
    {
        public string name;
        public string type;
        public string entityType;
        public string[] labelsOrTypes;
        public string[] properties;
        public string ownedIndex;
        public object options;
        public string createStatement;

        public static ConstraintRecord From(IRecord constraintRecord)
        {


            return new ConstraintRecord()
            {
                name = constraintRecord.GetValue<string>("name"),
                type = constraintRecord.GetValue<string>("type"),
                entityType = constraintRecord.GetValue<string>("entityType"),
                labelsOrTypes = constraintRecord.GetValue<string[]>("labelsOrTypes"),
                properties = constraintRecord.GetValue<string[]>("properties"),
                ownedIndex = constraintRecord.GetValue<string>("ownedIndex"),
                options = constraintRecord.GetValue<object>("options"),
                createStatement = constraintRecord.GetValue<string>("createStatement")

            };
        }
    }
}
