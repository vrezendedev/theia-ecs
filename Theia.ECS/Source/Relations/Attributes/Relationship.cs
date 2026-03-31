using System;

namespace Theia.ECS.Relations.Attributes;

[AttributeUsage(AttributeTargets.Struct, AllowMultiple = false)]
public sealed class Relationship : Attribute { }
