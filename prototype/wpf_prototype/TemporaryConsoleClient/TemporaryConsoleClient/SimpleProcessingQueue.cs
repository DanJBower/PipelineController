using System.Collections.Concurrent;

namespace TemporaryConsoleClient;

public class SimpleProcessingQueue<T>
{
    private readonly BlockingCollection<T> _itemProcessor = [];
    private readonly HashSet<Task> _itemsBeingProcessed = [];
    private readonly Lock _lock = new();

    public SimpleProcessingQueue(Func<T, CancellationToken, Task> processItem,
        ParallelOptions? parallelOptions = default)
    {
        parallelOptions ??= new();
        Task.Run(() => ProcessQueue(processItem, parallelOptions));
    }

    public SimpleProcessingQueue(Func<T, CancellationToken, Task> processItem,
        CancellationToken cancellationToken) : this(processItem, new ParallelOptions()
        {
            CancellationToken = cancellationToken
        })
    {

    }

    private async Task ProcessQueue(Func<T, CancellationToken, Task> processItem,
        ParallelOptions parallelOptions)
    {
        await Task.Run(() =>
        {
            Parallel.ForEach(_itemProcessor.GetConsumingEnumerable(parallelOptions.CancellationToken), parallelOptions, Process);
        }, parallelOptions.CancellationToken);

        return;

        void Process(T item)
        {
            // TODO Check this runs synchronously
            Task.Run(async () =>
            {
                await ProcessItem(processItem, item, parallelOptions.CancellationToken);
            }, parallelOptions.CancellationToken);

            // Might need
            // ProcessItem(processItem, item, parallelOptions.CancellationToken).RunSynchronously();
        }
    }

    private async Task ProcessItem(Func<T, CancellationToken, Task> processItem,
        T item,
        CancellationToken cancellationToken)
    {
        var task = processItem(item, cancellationToken);
        using (_lock.EnterScope())
        {
            _itemsBeingProcessed.Add(task);
        }

        await task;

        using (_lock.EnterScope())
        {
            _itemsBeingProcessed.Remove(task);
            Count--;
        }
    }

    public int Count { get; private set; }

    public void Enqueue(T item)
    {
        using (_lock.EnterScope())
        {
            Count++;
            _itemProcessor.Add(item);
        }
    }

    public async Task WaitForQueueToEmpty()
    {
        while (true)
        {
            HashSet<Task> tasksSnapshot;

            using (_lock.EnterScope())
            {
                // if (_itemProcessor.Count == 0 &&
                // _itemsBeingProcessed.Count == 0)
                if (Count == 0)
                {
                    return;
                }

                tasksSnapshot = _itemsBeingProcessed.ToHashSet();
            }

            await Task.WhenAll(tasksSnapshot);
        }
    }

    public void CompleteAdding()
    {
        _itemProcessor.CompleteAdding();
    }
}
