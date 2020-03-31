namespace Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class UnitTestsReactiveExtensions
    {
        [Fact]
        public void TestIEnumerableAsObservable()
        {
            int[] numbers = new[] { 1, 2, 3, 4, 5 };
            IObservable<int> nums = numbers.ToObservable();

            int sum = 0;
            Action<int> incrementSum = x => sum += x;

            // https://swimlanes.io/u/nGvokYYT3

            // disposing subscription means cancelling the subscription
            using IDisposable subscription = nums.Subscribe(
                onNext: incrementSum,
                onError: ex => Console.Error.WriteLine($"Exception: {ex.Message}"),
                onCompleted: () => Console.Out.WriteLine("Completed"));
            Assert.Equal(15, sum);

            // cancelling the subscription via a CancellationToken
            var cts = new CancellationTokenSource();
            nums.Subscribe(
                onNext: incrementSum,
                onError: ex => Console.Error.WriteLine($"Exception: {ex.Message}"),
                onCompleted: () => Console.Out.WriteLine("Completed"),
                token: cts.Token);
            Assert.Equal(30, sum);
        }

        [Fact]
        public void TestFunctionAsObservable()
        {
            static IEnumerable<int> Do()
            {
                yield return 0;
                Thread.Sleep(300);
                yield return 1;
                yield return 2;
                Thread.Sleep(300);
                yield return 3;
                Thread.Sleep(300);
                yield return 4;
                Thread.Sleep(300);
                yield return 5;
                throw new Exception("Oh, shoot");
            }

            IObservable<int> nums = Do().ToObservable();

            int sum = 0;
            bool gotException = false;
            bool completed = false;

            _ = nums.Subscribe(
                onNext: i => sum += i,
                onError: ex => gotException = true,
                onCompleted: () => completed = true);

            Assert.Equal(15, sum);
            Assert.False(completed);
            Assert.True(gotException);
        }

        [Fact]
        public async Task TestFunctionTimeBasedEmit()
        {
            // demo multiple values firing at different times
            IObservable<int> nums =
                1.EmitIn(TimeSpan.FromSeconds(1.2)).And(
                    2).In(TimeSpan.FromSeconds(1)).And(
                    3).In(TimeSpan.FromSeconds(0.8));

            IObservable<int> incrementedNums = nums.Select(i => i + 1);

            IObservable<int> squaredNums = nums.Select(i => i * i);

            var results = new List<int>();

            var tcs = new TaskCompletionSource<Exception>();

            var sw = new Stopwatch();
            sw.Start();
            
            using var subscription = squaredNums.Subscribe(
                onNext: results.Add,
                onCompleted: () => tcs.SetResult(null),
                onError: ex => tcs.SetResult(ex));
            Exception ex = await tcs.Task;

            sw.Stop();
            var millis = sw.ElapsedMilliseconds;

            Assert.Null(ex);
            Assert.True(800 < millis && millis < 1300);
            Assert.Equal(new[] { 9, 4, 1 }.ToArray(), results.ToArray());
        }
    }

    public static class ReactiveExtensionExtensions
    {
        public static IObservable<T> EmitIn<T>(this T t, TimeSpan dueTime)
            => Observable.Timer(dueTime: dueTime).Select(_ => t);

        public static (IObservable<T>, T) And<T>(this IObservable<T> sequence, T t)
            => (sequence, t);

        // https://rxmarbles.com/#merge
        public static IObservable<T> In<T>(this ValueTuple<IObservable<T>, T> tup, TimeSpan dueTime)
            => tup.Item1.Merge(tup.Item2.EmitIn(dueTime));

        private static IObservable<int> Demo() =>
           1.EmitIn(TimeSpan.FromSeconds(1))
           .And(2).In(TimeSpan.FromSeconds(2));
    }
}
