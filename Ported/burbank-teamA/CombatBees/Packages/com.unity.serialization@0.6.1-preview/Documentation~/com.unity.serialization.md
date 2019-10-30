# Introduction to Unity Serialization

Unity Serialization is a high performance general purpose serialization library written entirely in C#. It currently only supports the JSON format but can be extended to support others.

# Design

This library and was designed with the following performance considerations

* *Streamed* - We should work on very large input streams. We should be able to reason about the input without 
having to read and store the entire stream in memory.
* *I/O Bound* - We should be able to serialize/deserialize as fast as the underlying storage allows (basically run as fast as we can)
* *Non Allocating* - We should be able to read or write data without allocations. Or at least with only predictable up front allocations.

JSON was chosen as the format for usability considerations

* *Human Readable* - A text editor is the only thing you need to read and understand it.
* *Compatibility* - It's understood almost everywhere on every platform.
* *Simple Spec* - The JSON spec is fairly simple and can be parsed extremly fast.

# Deserialization

When it comes to deserializing JSON there are two main approaches.

#### DOM (Document Object Model)

This gives a very user friendly and easy to use API over deserialized data. It allows users to freely walk the data tree.
On the other hand it requires that the entire deserialized object must live in memory which means we can't stream.

#### SAX (Simple API over XML) / Forward-Only Reader

This is a very performant way of deserializing. It gives us very low allocations, only the currently depth must remain in memory and on the stack.
On the other hand this is a much harder to use API since it pushes more work on the user.

This library supports both approaches and allows mixing the two together.

#### API Usage

Forward-Only usage example:
```csharp
using (var reader = new SerializedObjectReader(stream))
{
    NodeType node = reader.Step(out SerializedValueView current);

    switch (node) 
    {
        case NodeType.BeginObject:
        case NodeType.EndObject:
            break;
        case NodeType.Primitive:
            var value = current.AsInt64();
            break;
    }
}
```

DOM usage example:
```csharp
/*
{
    "a": 10,
    "b": "hello",
    "c": { "x": 0, "y": 0 }
}
*/
using (var reader = new SerializedObjectReader(stream))
{
    SerializedObjectView obj = reader.ReadObject();

    var a = obj["a"].AsInt64();
    var b = obj["b"].AsStringView();
        
    var position = obj["c"].AsObjectView();
    var x = position["x"].AsInt64();
    var y = position["y"].AsInt64();
}
```

Mixed usage example:
```csharp
/*
[{
    "Id": "{GUID}",
    // ...
},
{
    "Id": "{GUID}",
    // ...
}]
*/
using (var reader = new SerializedObjectReader(stream))
{
    if (reader.Step() != NodeType.BeginArray) {
        // error
    }

    while (reader.ReadArrayElement(out var element)) 
    {
        var entity = element.AsObject();
        SerializedStringView id = entity["Id"].AsStringView();
        // ...
        reader.DiscardCompleted();
    }

    if (reader.Step() != NodeType.EndArray) {
        // error
    }
}

```
