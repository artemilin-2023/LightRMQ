namespace LightRMQ.Common;

public sealed class AsyncLocker : IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    public AsyncLocker(int initialCount = 1, int maxCount = 1)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(initialCount);

        if (maxCount < 1 || initialCount > maxCount)
            throw new ArgumentOutOfRangeException(nameof(maxCount));

        _semaphore = new SemaphoreSlim(initialCount, maxCount);
    }

    /// <summary>
    /// Блокирует секцию, можно отменять через токен или задать таймаут.
    /// </summary>
    public async Task<Releaser> LockAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (timeout.HasValue)
            await _semaphore.WaitAsync(timeout.Value, cancellationToken).ConfigureAwait(false);
        
        await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        
        return new Releaser(this);
    }

    private void Release()
    {
        if (_semaphore.CurrentCount == _semaphore.Release())
            throw new SemaphoreFullException("Семафор уже свободен.");
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(AsyncLocker));
    }

    public ValueTask DisposeAsync()
    {
        if (_disposed) return ValueTask.CompletedTask;

        _disposed = true;
        _semaphore.Dispose();

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Вспомогательный struct-«замок», который отпустит семафор при Dispose.
    /// </summary>
    public record struct Releaser : IDisposable
    {
        private readonly AsyncLocker _toRelease { get; init; }
        private bool _taken { get; set; }

        internal Releaser(AsyncLocker toRelease)
        {
            _toRelease = toRelease;
            _taken = true;
        }

        public void Dispose()
        {
            if (_taken)
            {
                _taken = false;
                _toRelease.Release();
            }
        }
    }
}

