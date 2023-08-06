using BenchmarkDotNet.Attributes;
using dotnet7_research.Meetup;
using dotnet7_research.Multitreading.Ch10;
using dotnet7_research.Multitreading.Ch2;
using dotnet7_research.Multitreading.Ch3;
using dotnet7_research.Multitreading.Ch4;
using dotnet7_research.Multitreading.Ch5;
using dotnet7_research.Multitreading.Ch6;
using dotnet7_research.Multitreading.Ch7;
using dotnet7_research.Multitreading.Ch8;
using dotnet7_research.Multitreading.Ch9;
using dotnet7_research.Multitreading.OR;

//[MemoryDiagnoser(displayGenColumns: false)]
//[DisassemblyDiagnoser]
//[HideColumns("Error", "StdDev", "Median", "RatioSD")]
public partial class Program
{
    private static async Task Main(string[] args)
    {
        //var demo = new CronDemo();
        //await Task.Delay(TimeSpan.FromMinutes(500));
        LambdaDemo.M8();
        //Ch9FilesAsync.Start();
    }
}


public class LoopBenchmark
{
    [Benchmark]
    public void M1()
    {
        var sw = new System.Diagnostics.Stopwatch();
        for (var j = 0 ; j < 2; j++)
        {
            sw.Restart();
            for (int trial = 0; trial < 2; trial++)
            {
                int count = 0;
                for (int i = 0; i < char.MaxValue; i++)
                    if (IsAsciiDigit((char)i))
                        count++;
            }
            sw.Stop();
            //Console.WriteLine(sw.Elapsed);
        }

        static bool IsAsciiDigit(char c) => (uint)(c - '0') <= 9;
    }
}