namespace Theia.ECS.Jobs;

internal interface IJob
{
    internal void Execute(int workerIndex);
}
