# Neo4j Schema Manager
Tools to ensure a consistent Neo4j graph schema for a domain layer that is defined in a .NET library.

## For Developers

### Defining your domain schema

#### Identifying Nodes and their Node Key
Annotate the class with a NodeAttribute (or it can also default to the class name)
```csharp
[Node(Label = "Person")]
public class Class1 {}

public class Person {}

[Node]
public class Person {}
```
Annotate the properties that make up the Node Key
```csharp
public class Car {
  [NodeKey]
  public string Make { get; set; }

  [NodeKey]
  public string Model { get; set;}
}
```

## Neo4j.Schema.Extensions
There are a few provided extensions that we can take advantage of when using the CustomAttributes.
Assume the following Vehicle definition:
```csharp
public class Vehicle {
  [NodeKey]
  public string Make { get; set; }

  [NodeKey]
  public string Model { get; set;}

  [NodeKey]
  public string ModelYear { get; set; }

}
```

### We can get the Label by:
```csharp
// for a type
var theLabel = typeof(Vehicle).Label();
```

### We can get the Node Key properties by:
```csharp
// for a type
List<string> vehicleNodeKey = typeof(Vehicle).NodeKey();
```
