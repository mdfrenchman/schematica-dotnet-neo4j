# Neo4j Schema Manager
Tools to ensure a consistent Neo4j graph schema for a domain layer that is defined in a .NET library.

## For Developers

### Defining your domain schema

#### Identifying Nodes and their Node Key
Annotate the class with a NodeAttribute (or it can also default to the class name)
```csharp
[Node(Label = "Person")]
public class Class1 {

}
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
