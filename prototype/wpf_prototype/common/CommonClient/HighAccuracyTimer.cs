using System.Diagnostics;

namespace CommonClient;

public class HighAccuracyTimer : IDisposable
{
    private readonly CancellationTokenSource _timerCancellationTokenSource = new();
    private readonly Dictionary<TimerEvent, TimerEventInfo> _timerEvents = [];
    private readonly Lock _lock = new();

    public HighAccuracyTimer()
    {
        Task.Factory.StartNew(TimerLoop,
                _timerCancellationTokenSource.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default)
            .Unwrap();
    }

    private async Task TimerLoop()
    {
        while (!_timerCancellationTokenSource.Token.IsCancellationRequested)
        {
            Dictionary<TimerEvent, TimerEventInfo> timerEvents;
            using (_lock.EnterScope())
            {
                timerEvents = _timerEvents.ToDictionary();
            }

            foreach (var (timerEvent, timerEventInfo) in timerEvents)
            {
                if (_timerCancellationTokenSource.Token.IsCancellationRequested)
                {
                    return;
                }

                if (Stopwatch.GetElapsedTime(timerEventInfo.LastExecuted) < timerEvent.Interval)
                {
                    continue;
                }

                Task.Run(timerEvent.Elapsed, _timerCancellationTokenSource.Token);
                timerEventInfo.LastExecuted += timerEvent.Interval.Ticks;

                if (timerEventInfo.RemainingExecutions == -1)
                {
                    continue;
                }

                timerEventInfo.RemainingExecutions--;

                if (timerEventInfo.RemainingExecutions == 0)
                {
                    Stop(timerEvent);
                }
            }

            await Task.Yield();
        }
    }

    public void Start(TimerEvent timerEvent)
    {
        if (timerEvent.NumberOfActivations == 0)
        {
            throw new Exception("Number of activations must be -1 or > 0");
        }

        using (_lock.EnterScope())
        {
            _timerEvents[timerEvent] = new TimerEventInfo
            {
                RemainingExecutions = timerEvent.NumberOfActivations,
                LastExecuted = Stopwatch.GetTimestamp(),
            };
        }
    }

    public void Stop(TimerEvent timerEvent)
    {
        using (_lock.EnterScope())
        {
            _timerEvents.Remove(timerEvent);
        }
    }

    public void Dispose()
    {
        try
        {
            _timerCancellationTokenSource.Cancel();
        }
        catch (OperationCanceledException) { }

        _timerCancellationTokenSource.Dispose();
    }

    private class TimerEventInfo
    {
        public int RemainingExecutions { get; set; }
        public long LastExecuted { get; set; }
    }
}

public record TimerEvent(Func<Task> Elapsed,
    TimeSpan Interval,
    int NumberOfActivations = -1);
