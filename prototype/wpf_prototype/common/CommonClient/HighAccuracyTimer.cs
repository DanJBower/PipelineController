using System.Diagnostics;

namespace CommonClient;

public class HighAccuracyTimer
{
    private CancellationTokenSource _cancellationTokenSource;
    private readonly bool _useLongRunningHint;
    private readonly double _interval;
    private readonly Func<Task> _action;

    public HighAccuracyTimer(double intervalMs,
        Func<Task> action,
        bool useLongRunningHint = false)
    {
        _interval = intervalMs;
        _action = action;
        _useLongRunningHint = useLongRunningHint;
    }

    public HighAccuracyTimer FromHz(double hz,
        Func<Task> action,
        bool useLongRunningHint = false)
    {
        var interval = 1000 / hz;
        return new(interval, action, useLongRunningHint);
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
