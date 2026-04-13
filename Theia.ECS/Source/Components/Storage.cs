using System.Buffers;

namespace Theia.ECS.Components;

internal abstract class Storage
{
    internal abstract void Move(int from, int to);

    internal abstract void Transfer(int oldIndex, int newIndex, Storage to);

    internal abstract void Write(ArrayBufferWriter<byte> arrayBufferWriter, int length);
}
