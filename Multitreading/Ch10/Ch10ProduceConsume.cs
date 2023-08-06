using System.Collections.Concurrent;

namespace dotnet7_research.Multitreading.Ch10;

public static class Ch10ProduceConsume
{
    private const int COLLECTIONS_NUMBER = 4;
    private const int COUNT = 10;

    public static void Start()
    {
        var cts = new CancellationTokenSource();

        Task.Run(() =>
        {
            if (Console.ReadKey().KeyChar == 'c') cts.Cancel();
        });

        var sourceArrays = new BlockingCollection<int>[COLLECTIONS_NUMBER];
        for (var i = 0; i < sourceArrays.Length; i++)
        {
            sourceArrays[i] = new BlockingCollection<int>(COUNT);
        }

        var filter1 = new PipelineWorker<int, decimal>(
            sourceArrays,
            n => Convert.ToDecimal(n * 0.97),
            cts.Token,
            "filter1");

        var filter2 = new PipelineWorker<decimal, string>(
            filter1.Output,
            s => string.Format("--{0}--", s),
            cts.Token,
            "filter2");

        var filter3 = new PipelineWorker<string, string>(
            filter2.Output,
            s => Console.WriteLine("The final result is {0} on therad id {1}", s, Environment.CurrentManagedThreadId),
            cts.Token,
            "filter3");

        try
        {
            Parallel.Invoke(
                () =>
                {
                    Parallel.For(0, sourceArrays.Length * COUNT,
                        (j, state) =>
                        {
                            if (cts.Token.IsCancellationRequested)
                            {
                                state.Stop();
                            }

                            int k = BlockingCollection<int>.TryAddToAny(sourceArrays, j);
                            if (k >= 0)
                            {
                                Console.WriteLine("Added {0} to source data on thread id {1}", j, Environment.CurrentManagedThreadId);
                                Thread.Sleep(TimeSpan.FromMicroseconds(100));
                            }
                        });
                    foreach (var arr in sourceArrays)
                    {
                        arr.CompleteAdding();
                    }
                },
                () => filter1.Run(),
                () => filter2.Run(),
                () => filter3.Run()
                );
        }
        catch (AggregateException ae)
        {
            foreach (var ex in ae.InnerExceptions)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        if (cts.Token.IsCancellationRequested)
        {
            Console.WriteLine("Operation has been cancelled! Press ENTER to exit.");
        }
        else
        {
            Console.WriteLine("Press ENTER to exit.");
        }

        Console.ReadLine();
    }

    private class PipelineWorker<TInput, TOutput>
    {
        private readonly Func<TInput, TOutput>? _processor = null;
        private readonly Action<TInput>? _outputProcessor = null;
        private readonly BlockingCollection<TInput>[] _input;
        private readonly CancellationToken _token;

        public BlockingCollection<TOutput>[]? Output { get; private set; } 
        public string Name { get; private set; }

        public PipelineWorker(
            BlockingCollection<TInput>[] input,
            Func<TInput, TOutput> processor,
            CancellationToken token,
            string name)
        {
            _input = input;
            Output = new BlockingCollection<TOutput>[_input.Length];
            for (var i = 0; i < Output.Length; i++)
            {
                Output[i] = null == input[i] ? null : new BlockingCollection<TOutput>(COUNT);
            }

            _processor = processor;
            _token = token;
            Name = name;
        }

        public PipelineWorker(
            BlockingCollection<TInput>[] input,
            Action<TInput> renderer,
            CancellationToken token,
            string name)
        {
            _input = input;
            _outputProcessor = renderer;
            _token = token;
            Name = name;
            Output = null;
        }

        public void Run()
        {
            Console.WriteLine($"{Name} is running");
            while (!_input.All(bc => bc.IsCompleted) && !_token.IsCancellationRequested)
            {
                var i = BlockingCollection<TInput>.TryTakeFromAny(_input, out var receivedItem, 50, _token);
                if (i >= 0)
                {
                    if (Output != null)
                    {
                        var outputItem = _processor!(receivedItem!);
                        BlockingCollection<TOutput>.AddToAny(Output, outputItem);
                        Console.WriteLine($"{Name} sent {outputItem} to next, on thread id {Environment.CurrentManagedThreadId}");
                        Thread.Sleep(TimeSpan.FromMicroseconds(100));
                    }
                    else
                    {
                        _outputProcessor!(receivedItem!);
                    }
                }
                else
                {
                    Thread.Sleep(TimeSpan.FromMicroseconds(50));
                }
            }

            if (Output != null)
            {
                foreach (var bc in Output)
                {
                    bc.CompleteAdding();
                }
            }
        }
    }
}