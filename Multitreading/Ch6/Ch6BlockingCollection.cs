using System.Collections.Concurrent;

namespace dotnet7_research.Multitreading.Ch6;

public static class Ch6BlockingCollection
{
    public static void Start()
    {
        Console.WriteLine("Using a Queue inside of BlockingCollection");
        Console.WriteLine();
        var t = RunProgram();
        t.Wait();

        Console.WriteLine();
        Console.WriteLine("Using a Stack inside of BlockingCollection");
        Console.WriteLine();
        t = RunProgram(new ConcurrentStack<CustomTask>());
        t.Wait();
    }

    private static async Task RunProgram(IProducerConsumerCollection<CustomTask>? collection = null)
    {
        var taskCollection = new BlockingCollection<CustomTask>();
        if (collection != null)
        {
            taskCollection = new BlockingCollection<CustomTask>(collection);
        }

        var taskProducer = TaskProducer(taskCollection);

        var taskConsumers = new Task[4];
        for (var i = 1; i <= 4; i++)
        {
            var consumerId = $"Consumer {i}";
            taskConsumers[i - 1] = TaskConsumer(taskCollection, consumerId);
        }

        await taskProducer;
        await Task.WhenAll(taskConsumers);
    }

    private static async Task TaskProducer(BlockingCollection<CustomTask> collection)
    {
        for (var i = 1; i <= 20; i++)
        {
            await Task.Delay(20);
            var workItem = new CustomTask { Id = i };
            collection.Add(workItem);
            Console.WriteLine("Task {0} have been posted", workItem.Id);
        }
        collection.CompleteAdding();
    }

    private static async Task TaskConsumer(BlockingCollection<CustomTask> collection, string name)
    {
        await GetRandomDelay();
        foreach (var item in collection.GetConsumingEnumerable())
        {
            Console.WriteLine("Task {0} have been processed by {1}", item.Id, name);
            await GetRandomDelay();
        }
    }

    private static Task GetRandomDelay()
    {
        var delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
        return Task.Delay(delay);
    }

    private class CustomTask
    {
        public int Id { get; set; }
    }
}