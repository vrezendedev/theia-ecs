using Theia.ECS.Relations.Attributes;

namespace Theia.Benchmarks.Source.Resources;

public struct Component1
{
    public int Value;
}

public struct FComponent1 : Friflo.Engine.ECS.IComponent
{
    public int Value;
}

public struct Component2
{
    public int Value;
}

public struct FComponent2 : Friflo.Engine.ECS.IComponent
{
    public int Value;
}

public struct Component3
{
    public int Value;
}

public struct FComponent3 : Friflo.Engine.ECS.IComponent
{
    public int Value;
}

public struct Component4
{
    public int Value;
}

public struct FComponent4 : Friflo.Engine.ECS.IComponent
{
    public int Value;
}

public struct Component5
{
    public int Value;
}

public struct FComponent5 : Friflo.Engine.ECS.IComponent
{
    public int Value;
}

public struct Component6
{
    public int Value;
}

public struct FComponent6 : Friflo.Engine.ECS.IComponent
{
    public int Value;
}

#region Serialization
public struct Position
{
    public float X;
    public float Y;
    public float Z;
}

public struct Velocity
{
    public float X;
    public float Y;
    public float Z;
}

public struct Rotation
{
    public float Yaw;
}

public struct Level
{
    public int Value;
}

public struct Health
{
    public int Current;
    public int Max;
}

public struct Inventory
{
    public int Gold;
    public int Items;
}

public struct WorldTick
{
    public ulong Value;
}

[Relationship]
public struct Owns { }

[Relationship]
public struct Follows { }
#endregion
