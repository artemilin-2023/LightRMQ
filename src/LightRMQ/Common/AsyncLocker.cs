namespace LightRMQ.Common;

internal class AsyncLocker : IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public AsyncLocker(int initialCount, int maxCount)
    {
        _semaphore = new SemaphoreSlim(initialCount, maxCount);
    }

    public AsyncLocker(int initialCount)
    {
        _semaphore = new SemaphoreSlim(initialCount);
    }

    public async Task<IDisposable> LockAsync()
    {
        await _semaphore.WaitAsync();
        return this;
    }

    public void Dispose()
        => _semaphore.Release();
  
}
