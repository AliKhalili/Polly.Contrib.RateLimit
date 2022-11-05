using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimit;

namespace DotNet.Polly.Contrib.RateLimiting;

internal static class RateLimitEngine
{
    internal static TResult Implementation<TResult>(
        RateLimiter rateLimiter,
        Func<RateLimitLease, Context, TResult> retryAfterFactory,
        Func<Context, CancellationToken, TResult> action,
        Context context,
        CancellationToken cancellationToken
    )
    {
        var lease = rateLimiter.AttemptAcquire(1);

        if (lease.IsAcquired)
        {
            return action(context, cancellationToken);
        }


        if (retryAfterFactory != null)
        {
            return retryAfterFactory(lease, context);
        }

        lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter);
        throw new RateLimitRejectedException(retryAfter);
    }
}
