namespace dotnet7_research;

public class AsyncEnumDemo
{
    public IEnumerable<int> Print()
    {
        Thread.Sleep(1000);
        yield return 1;
        Thread.Sleep(1000);
        yield return 2;
    }

    public async IAsyncEnumerable<int> PrintAsync()
    {
        await Task.Delay(1000);
        yield return 1;
        await Task.Delay(1000);
        yield return 2;
    }
}