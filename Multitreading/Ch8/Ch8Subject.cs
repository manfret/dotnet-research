using System.Reactive.Subjects;

namespace dotnet7_research.Multitreading.Ch8;

public static class Ch8Subject
{
    public static void Start()
    {
        Console.WriteLine("Subject");
        var subject = new Subject<string>();

        //не будет показано
        subject.OnNext("A");
        using (var subscription = OutputToConsole(subject))
        {
            subject.OnNext("B");
            subject.OnNext("C");
            subject.OnNext("D");
            subject.OnCompleted();
            subject.OnNext("Will not printer out");
        }

        Console.WriteLine("---");
        Console.WriteLine("ReplaySubject");
        var replaySubject = new ReplaySubject<string>();
        //будет показано т.к. replaySubject умеет кэшировать элементы и до начала обработки
        replaySubject.OnNext("A");
        using (var subscription = OutputToConsole(replaySubject))
        {
            replaySubject.OnNext("B");
            replaySubject.OnNext("C");
            replaySubject.OnNext("D");
            replaySubject.OnCompleted();
        }

        Console.WriteLine("---");
        Console.WriteLine("Buffered ReplaySubject");
        var bufferedSubject = new ReplaySubject<string>(2);
        bufferedSubject.OnNext("Will not printer out A");
        //будет показано
        bufferedSubject.OnNext("B");
        //будет показано
        bufferedSubject.OnNext("C");
        using (var subscription = OutputToConsole(bufferedSubject))
        {
            bufferedSubject.OnNext("D");
            bufferedSubject.OnCompleted();
        }

        Console.WriteLine("---");
        Console.WriteLine("Time window ReplaySubject");
        var timeSubject = new ReplaySubject<string>(TimeSpan.FromMicroseconds(700));
        timeSubject.OnNext("A");
        Thread.Sleep(TimeSpan.FromMicroseconds(200));
        timeSubject.OnNext("B");
        Thread.Sleep(TimeSpan.FromMicroseconds(200));
        timeSubject.OnNext("C");
        Thread.Sleep(TimeSpan.FromMicroseconds(200));
        using (var subscription = OutputToConsole(timeSubject))
        {
            Thread.Sleep(TimeSpan.FromMicroseconds(200));
            timeSubject.OnNext("D");
            timeSubject.OnCompleted();
        }

        Console.WriteLine("---");
        Console.WriteLine("AsyncSubject");
        var asyncSubject = new AsyncSubject<string>();
        asyncSubject.OnNext("Will not printer out A");
        using (var subscription = OutputToConsole(asyncSubject))
        {
            asyncSubject.OnNext("Will not printer out B");
            asyncSubject.OnNext("Will not printer out C");
            asyncSubject.OnNext("D");
            asyncSubject.OnCompleted();
        }

        Console.WriteLine("---");
        Console.WriteLine("BehaviorSubject");
        var behaviorSubject = new BehaviorSubject<string>("");
        behaviorSubject.OnNext("A");
        using (var subscription = OutputToConsole(behaviorSubject))
        {
            behaviorSubject.OnNext("B");
            behaviorSubject.OnNext("C");
            behaviorSubject.OnNext("D");
            behaviorSubject.OnCompleted();
        }
    }

    private static IDisposable OutputToConsole<T>(IObservable<T> sequence)
    {
        return sequence.Subscribe(
            obj => Console.WriteLine("{0}", obj),
            ex => Console.WriteLine("Error: {0}", ex.Message),
            () => Console.WriteLine("Completed"));
    }
}