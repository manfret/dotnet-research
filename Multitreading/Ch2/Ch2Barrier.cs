namespace dotnet7_research.Multitreading.Ch2;

public class Ch2Barrier
{
    private static readonly Barrier _barrier = new(2, barrier => Console.WriteLine("End of phase {0}", barrier.CurrentPhaseNumber + 1));

    public static void Start()
    {
        var t1 = new Thread(() => PlayMusic("the guitarist", "play an amazing solo", 5));
        var t2 = new Thread(() => PlayMusic("the singer", "sings his song", 2));

        t1.Start();
        t2.Start();
    }

    private static void PlayMusic(string name, string message, int seconds)
    {
        for (var i = 0; i < 3; i++)
        {
            Console.WriteLine("-----------------------------------------");
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("{0} starts to {1}", name, message);
            Thread.Sleep(TimeSpan.FromSeconds(seconds));
            Console.WriteLine("{0} finishes to {1}", name, message);
            _barrier.SignalAndWait();
        }
    }
}