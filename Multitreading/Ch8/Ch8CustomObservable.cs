using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace dotnet7_research.Multitreading.Ch8;

//Observable - для обработки операций, которые больше подходят событийному способу
//если нужно особая логика действий на переходах по элементам - можно сделать свой CustomSequence
//если нужна особая логика действий на эелементе и недостаточно просто Subject - можно сделать CustomObserver

public static class Ch8CustomObservable
{
    public static void Start()
    {
        var observer = new CustomObserver();

        var goodObservable = new CustomSequence(new[] { 1, 2, 3, 4, 5 });
        var badObservable = new CustomSequence(null);

        using (IDisposable subscription = goodObservable.Subscribe(observer))
        {

        }

        using (IDisposable subscription = goodObservable
                   .SubscribeOn(TaskPoolScheduler.Default)
                   .Subscribe(observer))
        {
            //Т.к. операция обработки асинхронная - то нужно подождать
            Thread.Sleep(100);
        }

        using (IDisposable subscription = badObservable
                   .SubscribeOn(TaskPoolScheduler.Default)
                   .Subscribe(observer))
        {
            Console.ReadLine();
        }
    }

    private class CustomObserver : IObserver<int>
    {
        public void OnNext(int value)
        {
            Console.WriteLine("Next value: {0}, Thread id: {1}", value, Environment.CurrentManagedThreadId);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Error: {0}", error.Message);
        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
        }
    }

    private class CustomSequence : IObservable<int>
    {
        private readonly IEnumerable<int> _numbers;

        public CustomSequence(IEnumerable<int> numbers)
        {
            _numbers = numbers;
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            foreach (var number in _numbers)
            {
                observer.OnNext(number);
            }
            observer.OnCompleted();
            return Disposable.Empty;
        }
    }
}