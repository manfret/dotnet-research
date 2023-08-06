using System.Collections.Concurrent;

namespace dotnet7_research.Multitreading.OR;

public static class OrSemaphore
{
    public static void Start()
    {
        var t = ProdCons();
        t.Wait();
    }

    private static async Task ProdCons()
    {
        for (var i = 0; i < 10; i++)
        {
            _ = Prod(false);
        }

        await Cons();

        await Task.Delay(1000);
    }

    private static readonly ConcurrentQueue<int> _bag = new();
    //сигнал, что 10 можно зайти
    //при release число CurrentCount увеличивается на 1 (можно зайти еще одному)
    //при успешном Wait число уменьшается (еще один успешно зашел)
    //если число = 0 - заходить нельзя
    //если число выше Max - заходить нельзя
    private static readonly SemaphoreSlim _semaphore = new(10, 10);
    private static int _i;

    private static async Task Prod(bool withDelay)
    {
        Interlocked.Increment(ref _i);
        _bag.Enqueue(_i);
        if (withDelay)
        {
            await Task.Delay(new Random().Next(1000, 2000));
            _semaphore.Release();
        }
    }

    private static async Task Cons()
    {
        while (await _semaphore.WaitAsync(-1))
        {
            var res = _bag.TryDequeue(out var r);
            if (!res) continue;

            Console.WriteLine("Value = {0}, capacity = {1}", r, _semaphore.CurrentCount + 1);

            _ = Prod(true);
            await Task.Delay(new Random().Next(50, 200));
        }
    }
}