using System.Threading.Tasks.Dataflow;

namespace CommonClient;

public class SimpleProcessingQueue<T>
{
    private readonly ActionBlock<T> _itemQueue;
    private readonly Lock _lock = new();
    private event EventHandler? CollectionEmptyEvent;

    public SimpleProcessingQueue(Func<T, CancellationToken, Task> processItem,
        ExecutionDataflowBlockOptions? executionDataflowBlockOptions)
    {
        executionDataflowBlockOptions ??= new();
        _itemQueue = new(async item => await ProcessItem(processItem, item, executionDataflowBlockOptions.CancellationToken), executionDataflowBlockOptions);
    }

    public SimpleProcessingQueue(Func<T, CancellationToken, Task> processItem,
        CancellationToken cancellationToken = default,
        int maxDegreeOfParallelism = -1) : this(processItem, new ExecutionDataflowBlockOptions
        {
            CancellationToken = cancellationToken,
            MaxDegreeOfParallelism = maxDegreeOfParallelism,
        })
    {

    }

    public int Count { get; private set; }
    public bool AddingCompleted { get; private set; }

    private async Task ProcessItem(Func<T, CancellationToken, Task> processItem,
        T item,
        CancellationToken cancellationToken)
    {
        await processItem(item, cancellationToken);

        using (_lock.EnterScope())
        {
            Count--;

            if (Count == 0)
            {
                CollectionEmptyEvent?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void Enqueue(T item)
    {
        using (_lock.EnterScope())
        {
            if (AddingCompleted)
            {
                throw new Exception($"Can no longer add new items to the {nameof(SimpleProcessingQueue<object>)}");
            }

            Count++;
            _itemQueue.Post(item);
        }
    }

    public async Task WaitForQueueToEmpty(CancellationToken cancellationToken = default)
    {
        using (_lock.EnterScope())
        {
            if (Count == 0)
            {
                return;
            }
        }

        cancellationToken.ThrowIfCancellationRequested();
        var tcs = new TaskCompletionSource();

        CollectionEmptyEvent += FinishWaiting;

        try
        {
            await using (cancellationToken.Register(() => { tcs.TrySetCanceled(); }))
            {
                await tcs.Task;
            }
        }
        finally
        {
            CollectionEmptyEvent -= FinishWaiting;
        }

        return;

        void FinishWaiting(object? sender, EventArgs eventArgs)
        {
            tcs.SetResult();
        }
    }

    public async Task WaitForCompletion()
    {
        await _itemQueue.Completion;
    }

    public void CompleteAdding()
    {
        using (_lock.EnterScope())
        {
            AddingCompleted = true;
            _itemQueue.Complete();
        }
    }
}
