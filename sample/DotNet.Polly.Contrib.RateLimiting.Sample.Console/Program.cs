using Polly;
using Polly.RateLimit;
using System.Diagnostics;

namespace DotNet.Polly.Contrib.RateLimiting.Sample.Console
{
    internal static class Program
    {
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }
        static async Task MainAsync()
        {
            Polly_RateLimiting();
            Sample1_TokenBucket();
            System.Console.ReadKey();
        }

        static void Polly_RateLimiting()
        {
            System.Console.WriteLine("Polly Rate limiting.");
            var policy = Policy.RateLimit(1, TimeSpan.FromSeconds(5));
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                try
                {
                    policy.Execute(async () =>
                    {
                        System.Console.WriteLine("{0:s\\s} permit was acquired.", stopwatch.ElapsedTicks.ToTimeSpan());
                    });
                }
                catch (RateLimitRejectedException exception)
                {
                    System.Console.WriteLine("{0:s\\s} try after {1:s\\s}. permit was not acquired.", stopwatch.ElapsedTicks.ToTimeSpan(), exception.RetryAfter);

                }
                Thread.Sleep(1000);
                if (stopwatch.ElapsedTicks.ToTimeSpan() > TimeSpan.FromSeconds(10))
                {
                    break;
                }
            }
        }
        static void Sample1_TokenBucket()
        {
            System.Console.WriteLine("Polly .NET 7 Token Bucket");
            var policy = RateLimit.TokenBucketRateLimit(option =>
            {
                option.TokenLimit = 1;
                option.TokensPerPeriod = 1;
                option.QueueLimit = 0;
                option.AutoReplenishment = true;
                option.ReplenishmentPeriod = TimeSpan.FromSeconds(5);
            });

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                try
                {
                    policy.Execute(async () =>
                    {
                        System.Console.WriteLine("{0:s\\s} permit was acquired.", stopwatch.ElapsedTicks.ToTimeSpan());
                    });
                }
                catch (RateLimitRejectedException exception)
                {
                    System.Console.WriteLine("{0:s\\s} try after {1:s\\s}. permit was not acquired.", stopwatch.ElapsedTicks.ToTimeSpan(), exception.RetryAfter);

                }
                Thread.Sleep(1000);
                if (stopwatch.ElapsedTicks.ToTimeSpan() > TimeSpan.FromSeconds(10))
                {
                    break;
                }
            }
        }

        static TimeSpan ToTimeSpan(this long ticks)
        {
            return TimeSpan.FromTicks(ticks);
        }
    }
}
