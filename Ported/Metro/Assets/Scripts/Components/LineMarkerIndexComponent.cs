using Unity.Entities;
public struct LineMarkerIndexComponent : IComponentData
{
    public int Value;
}

// Instead of having each marker be its own entity, have an entity that represents a line and use...
 
// DynamicBufferComponent - like a component, but internally like a list. Resizeable array. Order does not change by itself.
//    Markers need: Position, Station-or-not info, index is implicit by position in array
//    Read/write capable. 

// - OR  -

// BlobArray - structs and arrays in any combination. 
//    Read only. Could work here, since track layout doesn't change at runtime. Spline sample uses this. 
//    Could be more involved, specific API and conventions. Can only be manipulated via references, constituted of pointers

