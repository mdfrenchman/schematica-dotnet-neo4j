using SchematicNeo4j.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchematicNeo4j.Tests.DomainSample
{
    /// <summary>
    /// Test class for a sub-type that shares the node key of it's parent.
    /// </summary>
    [Node(Label="Car:Truck")]
    public class Truck : Vehicle
    {

        [Index(Label = "Truck", Name = "Truck_BrandTowCap")]
        public int TowingCapacity { get; set; }
    }
}
