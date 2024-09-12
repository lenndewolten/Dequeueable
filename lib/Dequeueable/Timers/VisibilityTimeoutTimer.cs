using Dequeueable.Models;
using Dequeueable.Queues;

namespace Dequeueable.Timers
{
    /// <summary>
    /// A timer that periodically extends the visibility timeout of a message in a queue.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the queue message. This type must implement the <see cref="IQueueMessage"/> interface.
    /// </typeparam>
    public sealed class VisibilityTimeoutTimer<TMessage>(IQueueMessageManager<TMessage> queueMessagesManager, TimeProvider timeProvider, IDelayStrategy delayStrategy) : IAsyncDisposable
        where TMessage : class, IQueueMessage
    {
        private readonly CancellationTokenSource _cts = new();
        private Task? _backgroundThread;
        private bool _disposed;

        /// <summary>
        /// Starts the timer to periodically extend the visibility timeout of the specified message.
        /// </summary>
        /// <param name="message">The message for which to extend the visibility timeout.</param>
        /// <param name="onFaultedAction">An optional action to execute if an error occurs during the timer loop.</param>
        public void Start(TMessage message, Action? onFaultedAction = null)
        {
            ArgumentNullException.ThrowIfNull(message);

            _backgroundThread = TimerLoop(message, onFaultedAction);
        }

        private async Task TimerLoop(TMessage message, Action? onFaultedAction)
        {
            using var timer = new PeriodicTimer(delayStrategy.GetNextDelay(message.NextVisibleOn), timeProvider);
            while (await timer.WaitForNextTickAsync(_cts.Token))
            {
                try
                {
                    var nextVisibleOn = await queueMessagesManager.UpdateVisibilityTimeOutAsync(message, _cts.Token);
                    timer.Period = delayStrategy.GetNextDelay(nextVisibleOn);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex) when (ex.InnerException is OperationCanceledException)
                {
                }
                catch
                {
                    onFaultedAction?.Invoke();
                    break;
                }
            }
        }

        /// <summary>
        /// Disposes the timer asynchronously, stopping the timer loop and releasing resources.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous dispose operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            await _cts.CancelAsync();

            try
            {
                if (_backgroundThread is not null)
                {
                    await _backgroundThread;
                }
            }
            catch
            {
                // At this point we really just want to stop extending the visibility timeout
            }

            _cts.Dispose();
        }
    }
}
