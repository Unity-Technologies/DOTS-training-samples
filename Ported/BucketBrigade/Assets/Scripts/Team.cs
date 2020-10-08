

using Unity.Entities;

public struct Team : IComponentData
{
	public int Id;
	public int Length;
	
	public Entity LineFullHead;
	public Entity LineFullTail;
	
	public Entity LineEmptyHead;
	public Entity LineEmptyTail;
}
