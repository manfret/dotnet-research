using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace dotnet7_research.Multitreading.Ch8;

//Observable
//в Subscribe можно указать что делать с каждым итемом в перечислении
//внутри using { } указывается то, что будет после перебора всех элементов
//удобно, если имеется событийная модель, когда действия происходят по добавлению в перечисление

public static class Ch8ConvertCollectionToObservable
{
    public static void Start()
    {
        foreach (var i in EnumerableEventSequence())
        {
            Console.Write(i);
        }
        Console.WriteLine();
        Console.WriteLine("IEnumerable");

        IObservable<int> o = EnumerableEventSequence().ToObservable();
        using (IDisposable subscription = o.Subscribe(Console.Write))
        {
            Console.WriteLine();
            Console.WriteLine("IObservable");
        }

        o = EnumerableEventSequence().ToObservable()
            .SubscribeOn(TaskPoolScheduler.Default);
        using (IDisposable subscription = o.Subscribe(Console.Write))
        {
            Console.WriteLine();
            Console.WriteLine("IObservable async");
            Console.ReadLine();
        }
    }

    private static IEnumerable<int> EnumerableEventSequence()
    {
        for (var i = 0; i < 10; i++)
        {
            Thread.Sleep(TimeSpan.FromSeconds(0.5));
            yield return i;
        }
    }
}