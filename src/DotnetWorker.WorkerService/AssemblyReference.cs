using System.Reflection;

namespace DotnetWorker.WorkerService;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
