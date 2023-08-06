using NCrontab;

namespace dotnet7_research.Cron;

public class CronDemo
{
    private Timer _timer;
    private readonly CronieChecker _checker;

    public CronDemo()
    {
        var cronie = "*/1 * * * *";
        _checker = new CronieChecker(cronie);

        _timer = new Timer(Callback, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

    }

    private void Callback(object? state)
    {
        _checker.Check(DateTime.UtcNow);
    }


    //private Timer _timer;
    //private readonly List<DateTime> _timeQueue = new(10);
    //private int _i;
    //private bool _isRunning;

    //public CronDemo()
    //{
    //    _timer = new Timer(Callback, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
    //}

    //private async void Callback(object? state)
    //{
    //    if (_isRunning) return;
    //    _isRunning = true;
    //    var i = _i;
    //    await Task.Run(() => Dequeue(i));
    //    await Task.Run(() => Enqueue(i));
    //    _i++;
    //    _isRunning = false;
    //}

    //private void Dequeue(int i)
    //{
    //    if (_timeQueue.Count <= 0) return;

    //    Console.WriteLine("Check {0} on {1}", i, DateTime.UtcNow);
    //    var times = _timeQueue.Where(a => DateTime.UtcNow > a);
    //    if (times.Any())
    //    {
    //        var latest = times.Max();
    //        Console.WriteLine("Start {0} on {1}", i, latest);
    //        _timeQueue.RemoveAll(a => times.Contains(a));

    //    }
    //}

    //private void Enqueue(int i)
    //{
    //    if (_timeQueue.Count >= 10) return;

    //    var now = DateTime.UtcNow;

    //    var cron = "*/1 * * * *";
    //    var expression = CrontabSchedule.Parse(cron);
    //    var nextUtcs = expression.GetNextOccurrences(now, now.AddHours(5));

    //    if (_timeQueue.Count > 0)
    //    {
    //        var biggest = _timeQueue.Last();
    //        nextUtcs = nextUtcs.Where(a => a > biggest);
    //    }
    //    var toAdd = nextUtcs.Order().Take(_timeQueue.Capacity - _timeQueue.Count).ToList();
    //    foreach (var dateTime in toAdd)
    //    {
    //        _timeQueue.Add(dateTime);
    //        Console.WriteLine("Enqueue {0} with {1}", i, dateTime);
    //    }
    //}
}

public record CronieCheckResult;
public record CronieCheckResultOk(DateTime Dt) : CronieCheckResult;
public record CronieCheckResultFalse : CronieCheckResult;

public class CronieChecker
{
    private const int QUEUE_CAPACITY = 10;
    private const int HOURS_CAPACITY = 5;

    private readonly List<DateTime> _times = new(QUEUE_CAPACITY);
    private bool _occupied;

    //cron = "*1 * * * *"
    //cronie = "*/1 * * * *" <- часть 
    private readonly string _cronie;
    public string Cronie => _cronie;

    public CronieChecker(string cronie)
    {
        _cronie = cronie;
        Enqueue(DateTime.UtcNow);
    }

    public CronieCheckResult Check(DateTime time)
    {
        if (_occupied) return new CronieCheckResultFalse();
        _occupied = true;
        var res = Dequeue(time);
        Enqueue(time);
        _occupied = false;
        return res;
    }

    private CronieCheckResult Dequeue(DateTime time)
    {
        if (_times.Count <= 0) return new CronieCheckResultFalse();

        var times = _times.Where(a => time > a);

        Console.WriteLine("Check on {0}", DateTime.UtcNow);
        if (times.Any())
        {
            var latest = times.Max();
            Console.WriteLine("Start on {0}", latest);
            _times.RemoveAll(a => times.Contains(a));
            return new CronieCheckResultOk(latest);
        }

        return new CronieCheckResultFalse();
    }

    private void Enqueue(DateTime time)
    {
        if (_times.Count >= 10) return;

        var expression = CrontabSchedule.Parse(_cronie);
        var nextUtcs = expression.GetNextOccurrences(time, time.AddHours(HOURS_CAPACITY));

        if (_times.Count > 0)
        {
            var latest = _times.Last();
            nextUtcs = nextUtcs.Where(a => a > latest);
        }
        var toAdd = nextUtcs.Order().Take(_times.Capacity - _times.Count).ToList();
        _times.AddRange(toAdd);
        foreach (var dateTime in toAdd)
        {
            Console.WriteLine("Enqueue with {0}", dateTime);
        }
    }
}