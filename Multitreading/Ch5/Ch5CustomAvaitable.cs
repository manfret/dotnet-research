using System.Runtime.CompilerServices;

namespace dotnet7_research.Multitreading.Ch5;

public static class Ch5CustomAvaitable
{
    public static void Start()
    {
        var t = AsyncronousProcessing();
        t.Wait();
    }

    private static async Task AsyncronousProcessing()
    {
        var sync = new CustomAvaitable(true);
        string result = await sync;
        Console.WriteLine(result);

        var async = new CustomAvaitable(false);
        result = await async;
        Console.WriteLine(result);
    }

    private class CustomAvaitable
    {
        private readonly bool _completeSyncronously;

        public CustomAvaitable(bool completeSyncronously)
        {
            _completeSyncronously = completeSyncronously;
        }

        public CustomAwaiter GetAwaiter()
        {
            return new CustomAwaiter(_completeSyncronously);
        }
    }

    private class CustomAwaiter : INotifyCompletion
    {
        private string _result = "Completed syncronously";
        public bool IsCompleted { get; }

        public CustomAwaiter(bool completeSyncronously)
        {
            IsCompleted = completeSyncronously;
        }

        public string GetResult()
        {
            return _result;
        }

        public void OnCompleted(Action? continuation)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                _result = GetInfo();
                continuation?.Invoke();
            });
        }

        private string GetInfo()
        {
            return $"Task is running on a thread id {Environment.CurrentManagedThreadId}. " +
                   $"Is thread pool thread: {Thread.CurrentThread.IsThreadPoolThread}";
        }
    }
}