# SchematicNeo4j
A code-first approach to manage a consistent Neo4j graph schema for a domain layer that is defined in a .NET library.

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

#### Using Shared (or inherited) Node Keys
If you want to differentiate between 2 subclasses of an object but they are going to share the same node key, defined by the super class.
We can do something like this.
```csharp
public class Person {
  [NodeKey]
  public string FirstName { get; set; }

  [NodeKey]
  public string LastName { get; set;}
}

[Node(Label="Person:Coach")]
public class Coach : Person {
  public int YearStarted {get; set;}
}

[Node(Label="Person:Player")]
public class Player : Person {
  public string Postion {get; set;}
}
```
Both players and coaches have a name; players can be coaches, and vice versa. Either way, this keeps our data clean preventing a person from having 2 records in the system.

## SchematicNeo4j.Extensions
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

## Using SchematicNeo4j.Schema.Initialize
Once we have our domain models identified we can use the `Schema` methods to put it into the graph.
```cs
