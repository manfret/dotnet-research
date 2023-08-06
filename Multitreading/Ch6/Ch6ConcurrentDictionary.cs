using System.Collections.Concurrent;
using System.Diagnostics;
using Dynamitey.DynamicObjects;

namespace dotnet7_research.Multitreading.Ch6;

public static class Ch6ConcurrentDictionary
{
    private const string Item = "Dictionary item";
    public static string CurrentItem;

    public static void Start()
    {
        var concurrentDictionary = new ConcurrentDictionary<int, string>();
        var dictionary = new Dictionary<int, string>();

        var sw = new Stopwatch();

        sw.Start();
        for (var i = 0; i < 1_000_000; i++)
        {
            lock (dictionary)
            {
                dictionary[i] = Item;
            }
        }
        sw.Stop();
        Console.WriteLine("Writing a dictionary with a lock: {0}", sw.Elapsed);

        sw.Restart();
        for (var i = 0; i < 1_000_000; i++)
        {
            concurrentDictionary[i] = Item;
        }
        sw.Stop();
        Console.WriteLine("Writing a concurrent dictionary with a lock: {0}", sw.Elapsed);

        sw.Restart();
        for (var i = 0; i < 1_000_000; i++)
        {
            lock (dictionary)
            {
                CurrentItem = dictionary[i];
            }
        }
        sw.Stop();
        Console.WriteLine("Reading from a dictionary with a lock: {0}", sw.Elapsed);

        sw.Restart();
        for (var i = 0; i < 1_000_000; i++)
        {
            CurrentItem = concurrentDictionary[i];
        }

        sw.Stop();
        Console.WriteLine("Reading from concurrent a dictionary with a lock: {0}", sw.Elapsed);
    }
}