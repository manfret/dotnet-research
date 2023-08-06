using System.Dynamic;
using System.Runtime.CompilerServices;
using ImpromptuInterface;
using ImpromptuInterface.Build;

namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5Dynamic
{
    public static void Start()
    {
        var t = AsyncronousProcessing();
        t.Wait();
    }

    private static async Task AsyncronousProcessing()
    {
        string result = await GetDynamicAwaitableObject(true);
        Console.WriteLine(result);

        result = await GetDynamicAwaitableObject(false);
        Console.WriteLine(result);
    }

    private static dynamic GetDynamicAwaitableObject(bool completeSyncronously)
    {
        dynamic result = new ExpandoObject();
        dynamic awaiter = new ExpandoObject();

        awaiter.Message = "Completed syncronously";
        awaiter.IsCompleted = completeSyncronously;
        awaiter.GetResult = (Func<string>)(() => awaiter.Message);

        awaiter.OnCompleted = (Action<Action?>)(callback =>
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                awaiter.Message = GetInfo();
                callback?.Invoke();
            });
        });

        IAwaiter<string> proxy = Impromptu.ActLike(awaiter);
        result.GetAwaiter = (Func<dynamic>)(() => proxy);
        return result;
    }

    private static string GetInfo()
    {
        return $"Task is running on a thread id {Environment.CurrentManagedThreadId}. " +
               $"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
    }

    public interface IAwaiter<T> : INotifyCompletion
    {
        bool IsCompleted { get; }
        T GetResult();
    }
}