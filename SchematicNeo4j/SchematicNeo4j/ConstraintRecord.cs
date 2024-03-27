using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System;
using System.Linq;

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

        public static ConstraintRecord GetNodeKeyFrom(Type domainModel)
        {
            var cr = new ConstraintRecord()
            {
                name = domainModel.NodeKeyName(),
                type = "NODE_KEY",
                entityType = "NODE",
                // TODO: NODE KEY is for a single label, that's how it's been. Not going to change it for this release.
                labelsOrTypes = new string[] { domainModel.Label().Split(':')[0] },
                properties = domainModel.NodeKey().ToArray(),
                ownedIndex = null,
                options = null
            };
            cr.createStatement = cr.properties.Length == 0 ? String.Empty :
                $"CREATE CONSTRAINT `{cr.name}` FOR (n:`{cr.labelsOrTypes[0]}`) REQUIRE ({String.Join(", ", cr.properties.Select(nk => $"n.`{nk}`"))}) IS NODE KEY";

            return cr;
        }
    }
}
