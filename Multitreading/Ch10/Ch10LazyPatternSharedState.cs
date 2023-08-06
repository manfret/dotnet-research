namespace dotnet7_research.Multitreading.Ch10;

public static class Ch10LazyPatternSharedState
{
    public static void Start()
    {
        var t = ProcessAsynchronously();
        t.GetAwaiter().GetResult();

        Console.WriteLine("Press ENTER to exit");
        Console.ReadLine();
    }

    private static async Task ProcessAsynchronously()
    {
        var unsafeState = new UnsafeState();
        Task[] tasks = new Task[4];
        for (var i = 0; i < 4; i++)
        {
            tasks[i] = Task.Run(() => Worker(unsafeState));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("-------------------------------");

        var firstState = new DoubleCheckedLocking();
        for (var i = 0; i < 4; i++)
        {
            tasks[i] = Task.Run(() => Worker(firstState));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("-------------------------------");

        var secondState = new BCLDoubleChecked();
        for (var i = 0; i < 4; i++)
        {
            tasks[i] = Task.Run(() => Worker(secondState));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("-------------------------------");

        var thirdState = new Lazy<ValueToAccess>(Compute);
        for (var i = 0; i < 4; i++)
        {
            tasks[i] = Task.Run(() => Worker(thirdState));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("-------------------------------");

        var fourthState = new BCLThreadSafeFactory();
        for (var i = 0; i < 4; i++)
        {
            tasks[i] = Task.Run(() => Worker(fourthState));
        }
        await Task.WhenAll(tasks);
        Console.WriteLine("-------------------------------");
    }

    private static void Worker(IHasValue state)
    {
        Console.WriteLine($"Worker runs on a thread id: {Environment.CurrentManagedThreadId}");
        Console.WriteLine($"State value: {state.Value.Text}");
    }

    private static void Worker(Lazy<ValueToAccess> state)
    {
        Console.WriteLine($"Worker runs on a thread id {Environment.CurrentManagedThreadId}");
        Console.WriteLine($"State value: {state.Value.Text}");
    }

    private static ValueToAccess Compute()
    {
        Console.WriteLine("The value is beign constructed on a thread id {0}", Environment.CurrentManagedThreadId);
        Thread.Sleep(TimeSpan.FromSeconds(1));
        return new ValueToAccess($"Constructed on a thread id {Environment.CurrentManagedThreadId}");
    }

    class ValueToAccess
    {
        public ValueToAccess(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }

    private class UnsafeState : IHasValue
    {
        private ValueToAccess? _value;

        public ValueToAccess Value
        {
            get
            {
                if (_value == null)
                {
                    _value = Compute();
                }
                return _value;
            }
        }
    }

    private class DoubleCheckedLocking : IHasValue
    {
        private object _syncRoot = new object();
        private volatile ValueToAccess? _value;

        public ValueToAccess Value
        {
            get
            {
                if (_value == null)
                {
                    lock (_syncRoot)
                    { 
                        if (_value == null) _value = Compute();
                    }
                }
                return _value;
            }
        }
    }

    private class BCLDoubleChecked : IHasValue
    {
        private object _syncRoot = new object();
        private ValueToAccess _value;
        private bool _initialized = false;

        public ValueToAccess Value
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _value, ref _initialized, ref _syncRoot, Compute);
            }
        }
    }

    private class BCLThreadSafeFactory : IHasValue
    {
        private ValueToAccess _value;

        public ValueToAccess Value
        {
            get
            {
                return LazyInitializer.EnsureInitialized(ref _value, Compute);
            }
        }
    }

    private interface IHasValue
    {
        ValueToAccess Value { get; }
    }
}