namespace dotnet7_research;

public class TimesDemo
{
    private Timer? _timer;

    public TimesDemo()
    {
        _timer = new Timer(Callback, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    private void Callback(object? state)
    {
        Console.WriteLine("123");
        throw new NotImplementedException();
    }
}