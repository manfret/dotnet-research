using System.Collections.Concurrent;
using Console = System.Console;

namespace dotnet7_research.Multitreading.Ch6;

public static class Ch6ConcurrentStack
{
    public static void Start()
    {
        var t = RunProgram();
        t.Wait();
    }

    private static async Task RunProgram()
    {
        var taskStack = new ConcurrentStack<CustomTask>();
        var cts = new CancellationTokenSource();

        var producer = TaskProducer(taskStack);

        var consumers = new Task[4];
        for (var i = 1; i <= 4; i++)
        {
            consumers[i - 1] = TaskConsumer(taskStack, $"Consumer {i}", cts.Token);
        }

        await producer;
        cts.CancelAfter(TimeSpan.FromSeconds(2));
        await Task.WhenAll(consumers);
    }

    private static async Task TaskProducer(ConcurrentStack<CustomTask> stack)
    {
        for (var i = 1; i <= 20; i++)
        {
            await Task.Delay(50);
            var workItem = new CustomTask { Id = i };
            stack.Push(workItem);
            Console.WriteLine("Task {0} has been produced", workItem.Id);
        }
    }

    private static async Task TaskConsumer(
        ConcurrentStack<CustomTask> stack, string name, CancellationToken token)
    {
        await GetRandomDelay();
        do
        {
            var popSuccessful = stack.TryPop(out var workItem);
            if (popSuccessful)
            {
                Console.WriteLine("Task {0} has been processed by {1}", workItem!.Id, name);
            }
            await GetRandomDelay();

        } while (!token.IsCancellationRequested);
    }

    private static Task GetRandomDelay()
    {
        var delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
        return Task.Delay(delay);
    }

    private class CustomTask
    {
        public int Id { get; init; }
    }
}