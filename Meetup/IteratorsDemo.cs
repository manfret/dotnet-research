using System.Collections;
using Exception = System.Exception;

namespace dotnet7_research.Meetup;

public static class IteratorsDemo
{
    //private static bool _toNext = true;

    private static IEnumerable<int> CreateSimpleIterator()
    {
        yield return 10;
        //pause
        for (var i = 0; i < 3; i++)
        {
            yield return i;
        }
        yield return 20;
    }
    public static void M1()
    {
        //5
        //var count = CreateSimpleIterator().Count();
        //_toNext = false;
        //2
        //count = CreateSimpleIterator().Count();
        foreach (var i in CreateSimpleIterator())
        {
            Console.WriteLine(i);
        }
    }

    //----------

    public static void M2()
    {
        IEnumerable<int> enumerable = CreateSimpleIterator();
        using (IEnumerator<int> enumerator = enumerable.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                int value = enumerator.Current;
                Console.WriteLine(value);
            }
        }
    }

    //----------

    public static void M3()
    {
        var enumerable = CreateSimpleIterator();
    }

    //----------

    public static void M4()
    {
        var enumerable = new MyEnumerable();
        foreach (var i in enumerable)
        {
            Console.WriteLine(i);
        }
    }

    private class MyEnumerable
    {
        private readonly List<int> _enumerable = new(){1, 2, 3};

        public IEnumerator<int> GetEnumerator()
        {
            return _enumerable.GetEnumerator();
        }
    }

    //----------

    private static IEnumerable<int> Fibonacci()
    {
        var current = 0;
        var next = 1;
        while (true)
        {
            yield return current;
            var oldCurrent = current;
            current = next;
            next += oldCurrent;
        }
    }

    public static void M5()
    {
        foreach (var value in Fibonacci())
        {
            Console.WriteLine(value);
            if (value > 1000)
            {
                break;
            }
        }
    }

    //----------

    //Когда будет выполняться finally? В конце (запоминание состояния, нахождение внутри try)?
    //После каждого yeild(выход из метода, переход к следующему MoveNext)
    private static IEnumerable<string> Iterator()
    {
        try
        {
            Console.WriteLine("Before");
            yield return "First";
            Console.WriteLine("Between");
            yield return "Second";
            Console.WriteLine("After");
        }
        finally
        {
            Console.WriteLine("Finally here");
        }
    }

    public static void M6()
    {
        foreach (var i in Iterator())
        {
            Console.WriteLine($"Captured: {i}");
        }
    }

    public static void M7()
    {
        var enumerable = Iterator();
        var enumerator = enumerable.GetEnumerator();
        while (enumerator.MoveNext())
        {
            Console.WriteLine($"Captured: {enumerator.Current}");
            //if (enumerator.Current != null)
            //{
            //    break;
            //}
        }
    }

    //----------

    //count = 5
    //0
    //0
    //1
    //2
    //2
    //4
    //3
    //6
    //4
    //8
    //Finally Here

    private static IEnumerable<int> GenerateIntegers(int count)
    {
        try
        {
            for (var i = 0; i < count; i++)
            {
                Console.WriteLine("Yiled {0}", i);
                yield return i;
                int doubled = i * 2;
                Console.WriteLine("Yiled {0}", doubled);
                yield return doubled;
            }
        }
        finally
        {
            Console.WriteLine("Finally here!");
        }
    }

    //stub method
    private static IEnumerable<int> GenerateIntegers1(int count)
    {
        //вот почему Lazy работает
        var ret = new GeneratedClass(-2);
        ret.count = count;
        return ret;
    }

    //state machine
    private class GeneratedClass : IEnumerable<int>, IEnumerator<int>
    {
        //method parameter
        public int count;
        //fields
        //state == -3 - MoveNext execution
        //state == -2 - GetEnumerator() not called
        //state == -1 - Completed
        //state ==  0 - GetEnumerator() called, MoveNext() not called
        //state ==  1 - for first yield
        //state ==  2 - for second yield
        private int state;
        private int current;
        private int initialThreadId;
        private int i;

        public int Current => current;
        object IEnumerator.Current => current;

        //called by stub + GetEnumerator
        public GeneratedClass(int state)
        {
            this.state = state;
            initialThreadId = Environment.CurrentManagedThreadId;
        }

        public IEnumerator<int> GetEnumerator()
        {
            //returns this if in the same thread and in original state
            //если будет вызываться несколько раз, то будут возвращаться новые StateMachine с копиями начальных параметров
            GeneratedClass enumerator;
            if (state == -2 && initialThreadId == Environment.CurrentManagedThreadId)
            {
                state = 0;
                enumerator = this;
            }
            else
            {
                enumerator = new GeneratedClass(0);
                enumerator.count = count;
            }
            return enumerator;
        }

        public bool MoveNext()
        {
            bool tryBlockCompletedNormally = false;
            try
            {
                switch (state)
                {
                    // Start of method
                    case 0:
                        state = -3;
                        goto methodStart;
                    // After first yield return
                    case 1:
                        state = -3;
                        goto afterFirstYieldReturn;
                    // After second yield return
                    case 2:
                        state = -3;
                        goto afterSecondYieldReturn;
                    // Either a misuse of the state machine, or it's
                    // already completed.
                    default:
                        tryBlockCompletedNormally = true;
                        return false;
                }
            methodStart:
                i = 0;
            loopCondition:
                if (i < count)
                {
                    goto loopBodyStart;
                }
                Finally1();
                tryBlockCompletedNormally = true;
                return false;
            loopBodyStart:
                Console.WriteLine("Yielding {0}", i);
                current = i;
                state = 1;
                tryBlockCompletedNormally = true;
                return true;
            afterFirstYieldReturn:
                int num2 = i * 2;
                Console.WriteLine("Yielding {0}", num2);
                current = num2;
                state = 2;
                tryBlockCompletedNormally = true;
                return true;
            afterSecondYieldReturn:
                i++;
                goto loopCondition;
            }
            // In the IL, this is actually a "fault" block - it's like finally, but
            // is only executed if there's an exception. I'm using
            // tryBlockCompletedNormally to get roughly the same behavior.
            finally
            {
                if (!tryBlockCompletedNormally)
                {
                    Dispose();
                }
            }
        }

        //Generated iterators not supports reset
        public void Reset()
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            int num = state;
            if (num == -3 || ((uint)(num - 1)) <= 1)
            {
                try
                {
                }
                finally
                {
                    Finally1();
                }
            }
        }

        private void Finally1()
        {
            state = -1;
            Console.WriteLine("In finally block");
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static void M8()
    {
        foreach (var i in GenerateIntegers1(5))
        {
            Console.WriteLine($"Captured: {i}");
        }
    }

    //-------------

    private static IEnumerable<int> GenerateIterations1(int from, int to)
    {
        for (var i = from; i < to; i++)
        {
            yield return i;
        }
    }

    public static void M9()
    {
        var from = 0;
        var to = 10;

        var enumerable = GenerateIterations1(from, to);

        from = 5;
        to = 6;

        foreach (var i in enumerable)
        {
            Console.Write(i);
        }
    }

    private class MyC
    {
        public int Value { get; set; }
    }

    private static IEnumerable<int> GenerateIterations1(MyC from, MyC to)
    {
        for (var i = from.Value; i < to.Value; i++)
        {
            yield return i;
        }
    }

    private static IEnumerable<int> GenerateIterations2(MyC from, MyC to)
    {
        for (var i = from.Value; i < to.Value; i++)
        {
            for (var j = from.Value; j < to.Value; j++)
            {
                if (i == 5) yield return i;
            }
        }
    }

    private static async IAsyncEnumerable<int> GenerateIterations4(int from, int to)
    {
        for (var i = from; i < to; i++)
        {
            if (i == (from + to) / 2) throw new Exception("Boom");
            yield return i;
        }
    }

    public static void M10()
    {
        var from = new MyC { Value = 0};
        var to = new MyC { Value = 10 };

        var enumerable = GenerateIterations1(from, to);

        from = new MyC { Value = 5 };
        to = new MyC { Value = 6 };

        foreach (var i in enumerable)
        {
            Console.Write(i);
        }
    }

    public static void M11()
    {
        var from = new MyC { Value = 0 };
        var to = new MyC { Value = 10 };

        var enumerable = GenerateIterations2(from, to);

        from.Value = 5;
        to.Value = 7;

        foreach (var i in enumerable)
        {
            Console.Write(i);
        }
    }

    public static async Task M12()
    {
        var from = 0;
        var to = 10;

        try
        {
            await foreach (var i in GenerateIterations4(from, to))
            {
                Console.Write(i);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    //private static IEnumerable<int> GenerateIterations2(ref MyC from, ref MyC to)
    //{
    //    for (var i = from.Value; i < to.Value; i++)
    //    {
    //        yield return i;
    //    }
    //}

    //public static void M12()
    //{
    //    var from = new MyC { Value = 0 };
    //    var to = new MyC { Value = 10 };

    //    var enumerable = GenerateIterations1(from, to);

    //    from = new MyC { Value = 5 };
    //    to = new MyC { Value = 6 };

    //    foreach (var i in enumerable)
    //    {
    //        Console.Write(i);
    //    }
    //}
}