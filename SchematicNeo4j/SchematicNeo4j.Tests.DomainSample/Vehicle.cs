using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
{
    [Node(Label = "Car")]
    public class Vehicle
    {
        [NodeKey]
        [Index(Name = "CarMake")]
        [Index(Name = "CarMakeModel")]
        [Index(Label = "Truck", Name ="Truck_BrandTowCap")]
        public string Make { get; set; }

        [NodeKey]
        [Index(Name = "CarMakeModel")]
        public string Model { get; set; }

        [NodeKey]
        [Index(Name = "CarModelYear")]
        public int ModelYear { get; set; }
    }
}
