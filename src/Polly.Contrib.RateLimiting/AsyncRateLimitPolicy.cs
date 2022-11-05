using System.Diagnostics;
using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimit;

namespace Polly.Contrib.RateLimiting;

/// <summary>
/// A rate-limit policy that can be applied to asynchronous delegates.
/// </summary>
public class AsyncRateLimitPolicy : AsyncPolicy, IRateLimitPolicy
{
    private readonly RateLimiter _rateLimiter;

    internal AsyncRateLimitPolicy(RateLimiter rateLimiter)
    {
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    protected override Task<TResult> ImplementationAsync<TResult>(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken,
        bool continueOnCapturedContext)
        => AsyncRateLimitEngine.ImplementationAsync(_rateLimiter, null!, action, context, cancellationToken, continueOnCapturedContext);
}

/// <summary>
/// A rate-limit policy that can be applied to asynchronous delegates returning a value of type <typeparamref name="TResult"/>.
/// </summary>
public class AsyncRateLimitPolicy<TResult> : AsyncPolicy<TResult>, IRateLimitPolicy<TResult>
{
    private readonly RateLimiter _rateLimiter;
    private readonly Func<RateLimitLease, Context, TResult> _retryAfterFactory;

    internal AsyncRateLimitPolicy(
        RateLimiter rateLimiter,
        Func<RateLimitLease, Context, TResult> retryAfterFactory)
    {
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _retryAfterFactory = retryAfterFactory;
    }

    /// <inheritdoc/>
    [DebuggerStepThrough]
    protected override Task<TResult> ImplementationAsync(Func<Context, CancellationToken, Task<TResult>> action, Context context, CancellationToken cancellationToken,
        bool continueOnCapturedContext)
        => AsyncRateLimitEngine.ImplementationAsync(_rateLimiter, _retryAfterFactory, action, context, cancellationToken, continueOnCapturedContext);
}
