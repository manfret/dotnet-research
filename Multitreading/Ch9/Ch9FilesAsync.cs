using System.Collections;
using System.Linq;
using System.Text;

namespace dotnet7_research.Multitreading.Ch9;

public static class Ch9FilesAsync
{
    private const int BUFFER_SIZE = 4096;

    public static void Start()
    {
        var t = ProcessAsynchronousIO();
        t.GetAwaiter().GetResult();
    }

    private static async Task ProcessAsynchronousIO()
    {
        using (var stream = new FileStream("test1.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None, BUFFER_SIZE))
        {
            Console.WriteLine("1. Uses I/O Threads: {0}", stream.IsAsync);

            byte[] buffer = Encoding.UTF8.GetBytes(CreateFileContent());

            var writeTask = Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, buffer, 0, buffer.Length, null);
            await writeTask;
        }

        using (var stream = new FileStream("test2.txt", FileMode.Create, FileAccess.ReadWrite, FileShare.None, BUFFER_SIZE, FileOptions.Asynchronous))
        {
            Console.WriteLine("2. Uses I/O Threads: {0}", stream.IsAsync);

            byte[] buffer = Encoding.UTF8.GetBytes(CreateFileContent());

            var writeTask = Task.Factory.FromAsync(stream.BeginWrite, stream.EndWrite, buffer, 0, buffer.Length, null);
            await writeTask;
        }

        using (var stream = File.Create("test3.txt", BUFFER_SIZE, FileOptions.Asynchronous))
        {
            using (var sw = new StreamWriter(stream))
            {
                Console.WriteLine("3. Uses I/O Threads: {0}", stream.IsAsync);
                await sw.WriteAsync(CreateFileContent());
            }
        }

        using (var sw = new StreamWriter("test4.txt", true))
        {
            Console.WriteLine("4. Uses I/O Threads: {0}", ((FileStream)sw.BaseStream).IsAsync);
            await sw.WriteAsync(CreateFileContent());
        }

        Console.WriteLine("Starting parsing files in parallel");

        //Task<long>[] readTasks = new Task<long>[4];
        //for (var i = 0; i < 4; i++)
        //{
        //    readTasks[i] = SumFileContent($"test{i + 1}.txt");
        //}
        //long[] sums = await Task.WhenAll(readTasks);
        //Console.WriteLine("Sum in all files: {0}", sums.Sum());

        var parallelQuery =
            from i in ParallelEnumerable.Range(1, 4)
            select SumFileContent($"test{i}.txt").Result;
        var sum = parallelQuery.Sum();
        Console.WriteLine("Sum in all files: {0}", sum);

        Console.WriteLine("Deleting files...");

        Task[] deleteTasks = new Task[4];
        for (var i = 0; i < 4; i++)
        {
            var filename = $"test{i + 1}.txt";
            deleteTasks[i] = SimulateAsynchronousDelete(filename);
        }

        await Task.WhenAll(deleteTasks);

        Console.WriteLine("Delete complete.");
    }

    private static string CreateFileContent()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < 1_000_00; i++)
        {
            sb.AppendFormat("{0}", new Random(i).Next(0, 99999));
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static async Task<long> SumFileContent(string filename)
    {
        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None, BUFFER_SIZE, FileOptions.Asynchronous))
        {
            using (var sr = new StreamReader(stream))
            {
                long sum = 0;
                while (sr.Peek() > - 1)
                {
                    string? line = await sr.ReadLineAsync();
                    sum += long.Parse(line!);
                }
                return sum;
            }
        }
    }

    private static Task SimulateAsynchronousDelete(string filename)
    {
        return Task.Run(() => File.Delete(filename));
    }
}