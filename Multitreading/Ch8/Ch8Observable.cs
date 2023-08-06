using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace dotnet7_research.Multitreading.Ch8;

public static class Ch8Observable
{
    public static void Start()
    {
        var o = Observable.Return(0);
        using (var sub = OutputToConsole(o)){};
        Console.WriteLine("------------------------");

        o = Observable.Empty<int>();
        using (var sub = OutputToConsole(o)) { };
        Console.WriteLine("------------------------");

        o = Observable.Repeat(42).Take(5);
        using (var sub = OutputToConsole(o)) { };
        Console.WriteLine("------------------------");

        o = Observable.Throw<int>(new Exception());
        using (var sub = OutputToConsole(o)) { };
        Console.WriteLine("------------------------");

        o = Observable.Range(0, 10);
        using (var sub = OutputToConsole(o)) { };
        Console.WriteLine("------------------------");

        o = Observable.Create<int>(ob =>
        {
            for (var i = 0; i < 10; i++)
            {
                ob.OnNext(i);
            }
            return Disposable.Empty;
        });
        using (var sub = OutputToConsole(o)) { };
        Console.WriteLine("------------------------");

        o = Observable.Generate(0, i => i < 5, i => ++i, i => i * 2);
        using (var sub = OutputToConsole(o)) { };
        Console.WriteLine("------------------------");

        IObservable<long> ol = Observable.Interval(TimeSpan.FromSeconds(1));
        using (var sub = OutputToConsole(ol))
        {
            Thread.Sleep(TimeSpan.FromSeconds(3));
        };
        Console.WriteLine("------------------------");

        ol = Observable.Timer(DateTimeOffset.Now.AddSeconds(1));
        using (var sub = OutputToConsole(ol))
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
        };
        Console.WriteLine("------------------------");
    }

    private static IDisposable OutputToConsole<T>(IObservable<T> sequence)
    {
        return sequence.Subscribe(
            obj => Console.WriteLine("{0}", obj),
            ex => Console.WriteLine("Error: {0}", ex.Message),
            () => Console.WriteLine("Completed"));
    }
}