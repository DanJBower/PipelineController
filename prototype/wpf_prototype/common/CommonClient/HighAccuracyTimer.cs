using System.Diagnostics;

namespace CommonClient;

public class HighAccuracyTimer
{
    private CancellationTokenSource _cancellationTokenSource;
    private readonly bool _useLongRunningHint;
    private readonly bool _slew;
    private readonly double _interval;
    private readonly Func<Task> _action;

    public HighAccuracyTimer(double intervalMs,
        Func<Task> action,
        bool useLongRunningHint = false,
        bool slew = false)
    {
        _interval = intervalMs;
        _action = action;
        _slew = slew;
        _useLongRunningHint = useLongRunningHint;
    }

    public static HighAccuracyTimer FromHz(double hz,
        Func<Task> action,
        bool slew = false,
        bool useLongRunningHint = false)
    {
        var interval = 1000 / hz;
        return new(intervalMs: interval,
            action: action,
            useLongRunningHint: useLongRunningHint,
            slew: slew);
    }

    public void Start()
    {
        _cancellationTokenSource = new();

        if (_useLongRunningHint)
        {
            Task.Factory.StartNew(RunTimer,
                    _cancellationTokenSource.Token,
                    TaskCreationOptions.LongRunning,
                    TaskScheduler.Default)
                .Unwrap();
        }
        else
        {
            Task.Run(RunTimer);
        }
    }

    private async Task RunTimer()
    {
        var targetDelay = TimeSpan.FromMilliseconds(_interval);
        var lastStart = Stopwatch.GetTimestamp();
        await _action();

        while (!_cancellationTokenSource.Token.IsCancellationRequested)
        {
            var timeElapsed = Stopwatch.GetElapsedTime(lastStart);
            if (timeElapsed < targetDelay)
            {
                await Task.Yield();
                continue;
            }

            lastStart = Stopwatch.GetTimestamp();
            await _action();

            if (_slew)
            {
                lastStart = Stopwatch.GetTimestamp();
            }
        }
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    public void StopAfter(TimeSpan delay)
    {
        _cancellationTokenSource.CancelAfter(delay);
    }
}
