namespace dotnet7_research.Multitreading.Ch2;

public static class Ch2Mutex
{
    public static void Start()
    {
        const string mutexName = "CSharpThreadingCookbook";
        
        using (var m = new Mutex(false, mutexName))
        {
            if (!m.WaitOne(TimeSpan.FromSeconds(5), false))
            {
                Console.WriteLine("Second instance is rinnung");
            }
            else
            {
                Console.WriteLine("Running!");
                Console.ReadLine();
                m.ReleaseMutex();
            }
        }
    }
}