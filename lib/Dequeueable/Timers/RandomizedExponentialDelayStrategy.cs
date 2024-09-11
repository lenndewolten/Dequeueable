namespace Dequeueable.Timers
{
    /// <summary>
    /// Implements a delay strategy that uses exponential backoff with randomization for retry attempts.
    /// </summary>
    public sealed class RandomizedExponentialDelayStrategy : IDelayStrategy
    {
        private const int _randomizationFactor = 20;
        private TimeSpan _minimumInterval;
        private readonly TimeSpan _maximumInterval;
        private readonly TimeSpan _deltaBackoff;

        private TimeSpan _currentInterval = TimeSpan.Zero;
        private uint _backoffExponent;
        private Random? _random;

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomizedExponentialDelayStrategy"/> class.
        /// </summary>
        /// <param name="minimumInterval">The minimum delay interval between retries.</param>
        /// <param name="maximumInterval">The maximum delay interval between retries.</param>
        /// <param name="deltaBackoff">
        /// The increment added to the delay on each retry attempt. Defaults to <paramref name="minimumInterval"/>.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="minimumInterval"/> or <paramref name="maximumInterval"/> is negative.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown if <paramref name="minimumInterval"/> is greater than <paramref name="maximumInterval"/>.
        /// </exception>
        public RandomizedExponentialDelayStrategy(TimeSpan minimumInterval, TimeSpan maximumInterval, TimeSpan? deltaBackoff = null)
        {
            if (minimumInterval.Ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimumInterval), $"'{nameof(minimumInterval)}' must not be negative or zero.");
            }

            if (maximumInterval.Ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumInterval), $"'{nameof(maximumInterval)}' must not be negative or zero.");
            }

            if (minimumInterval.Ticks > maximumInterval.Ticks)
            {
                throw new ArgumentException($"The '{nameof(minimumInterval)}' must not be greater than the '{nameof(maximumInterval)}'.",
                    nameof(minimumInterval));
            }

            _minimumInterval = minimumInterval;
            _maximumInterval = maximumInterval;
            _deltaBackoff = deltaBackoff ?? minimumInterval;
        }

        /// <summary>
        /// Gets or sets the minimum delay interval between retries.
        /// </summary>
        public TimeSpan MinimalRenewalDelay { get => _minimumInterval; set { _minimumInterval = value; } }

        /// <summary>
        /// Calculates the next delay interval based on the previous execution result.
        /// </summary>
        /// <param name="nextVisibleOn">Optional parameter indicating when the message will be visible next.</param>
        /// <param name="executionSucceeded">
        /// Indicates whether the last execution was successful. If true, the delay resets to the minimum interval.
        /// </param>
        /// <returns>The next delay interval to be used before retrying.</returns>
        public TimeSpan GetNextDelay(DateTimeOffset? nextVisibleOn = null, bool? executionSucceeded = null)
        {
            if (executionSucceeded == true)
            {
                _currentInterval = _minimumInterval;
                _backoffExponent = 1;
            }
            else if (_currentInterval != _maximumInterval)
            {
                var backoffInterval = _minimumInterval;

                if (_backoffExponent > 0)
                {
                    _random ??= new Random();
#pragma warning disable CA5394 // Do not use insecure randomness
                    var randomIncrementMsec = (double)_random.Next(100 - _randomizationFactor, 100 + _randomizationFactor) / 100;
#pragma warning restore CA5394 // Do not use insecure randomness
                    var incrementMsec = randomIncrementMsec *
                        Math.Pow(2.0, _backoffExponent - 1) *
                        _deltaBackoff.TotalMilliseconds;
                    backoffInterval += TimeSpan.FromMilliseconds(incrementMsec);
                }

                if (backoffInterval < _maximumInterval)
                {
                    _currentInterval = backoffInterval;
                    _backoffExponent++;
                }
                else
                {
                    _currentInterval = _maximumInterval;
                }
            }

            return _currentInterval;
        }
    }
}
