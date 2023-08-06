using System.Collections.Concurrent;

namespace dotnet7_research.Multitreading.Ch6;

//конкурентный доступ - имеется в виду удобство доступа к разным эелемнтам, а не к одному
//если к одному - то блокировка
public static class Ch6ConcurrentQueue
{
    public static void Start()
    {
        var t = RunProgram();
        t.Wait();
    }

    private static async Task RunProgram()
    {
        var taskQueue = new ConcurrentQueue<CustomTask>();
        var cts = new CancellationTokenSource();

        var producer = TaskProducer(taskQueue);

        var consumers = new Task[4];
        for (var i = 1; i <= 4; i++)
        {
            consumers[i - 1] = TaskConsumer(taskQueue, $"Consumer {i-1}", cts.Token);
        }

        await producer;
        cts.CancelAfter(TimeSpan.FromSeconds(2));

        await Task.WhenAll(consumers);
    }

    private static async Task TaskProducer(ConcurrentQueue<CustomTask> queue)
    {
        for (var i = 0; i <= 20; i++)
        {
            await Task.Delay(50);
            var workItem = new CustomTask { Id = i };
            queue.Enqueue(workItem);
            Console.WriteLine("Task {0} has been posted", workItem.Id);
        }
    }

    private static async Task TaskConsumer(
        ConcurrentQueue<CustomTask> queue, 
        string name, CancellationToken token)
    {
        await GetRandomDelay();
        do
        {
            var dequeueSuccessful = queue.TryDequeue(out var workItem);
            if (dequeueSuccessful)
            {
                Console.WriteLine("Task {0} has been consumed by {1}", workItem!.Id, name);
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