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
            IEnumerable<int> numbers = new[] { 1, 2, 3, 4, 5 };

            IObservable<int> numberObservable = numbers.ToObservable();

            IEnumerable<int> collectThatCrap = numberObservable.ToEnumerable();

            int sum = 0;
            void incrementSum(int x) => sum += x;

            // https://swimlanes.io/u/nGvokYYT3

            // disposing subscription means cancelling the subscription
            using IDisposable subscription = numberObservable.Subscribe(
                onNext: incrementSum,
                onError: ex => Console.Error.WriteLine($"Exception: {ex.Message}"),
                onCompleted: () => Console.Out.WriteLine("Completed"));
            Assert.Equal(15, sum);
            subscription.Dispose();


            // cancelling the subscription via a CancellationToken
            var cts = new CancellationTokenSource();
            numberObservable.Subscribe(
                onNext: incrementSum,
                onError: ex => Console.Error.WriteLine($"Exception: {ex.Message}"),
                onCompleted: () => Console.Out.WriteLine("Completed"),
                token: cts.Token);
            Assert.Equal(30, sum);
            cts.Cancel();
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

            object _lock = new object();
            int sum = 0;

            bool gotException = false;
            bool completed = false;

            _ = nums.Subscribe(
                onNext: i =>
                {
                    lock (_lock)
                    {
                        sum += i;
                    }
                },
                onError: ex => gotException = true,
                onCompleted: () => completed = true);

            nums.Subscribe(
                onNext: i =>
                {
                    lock (_lock)
                    {
                        sum += i;
                    }
                },
                onError: ex => gotException = true,
                onCompleted: () => completed = true);

            Assert.Equal(30, sum);
            Assert.True(gotException);
            Assert.False(completed);
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

            subscription.Dispose();    

            Assert.Null(ex);
            Assert.True(800 < millis && millis < 1300);
            Assert.Equal(new[] { 9, 4, 1 }.ToArray(), results.ToArray());
        }


        [Fact]
        public async Task TestFunctionTimeBasedEmit1()
        {
            // demo multiple values firing at different times
            IObservable<IEnumerable<int>> nums =
                new[] { 1, 2, 3 }.EmitIn(TimeSpan.FromSeconds(2)).And(
                    new[] { 4, 5 }).In(TimeSpan.FromSeconds(1)).And(
                    new[] { 6, 7, 8 }).In(TimeSpan.FromSeconds(0.5)).And(
                    new[] { 1001 }).In(TimeSpan.FromDays(2));

            IObservable<int> flattenedNumbers = nums
                .SelectMany(i => i)
                .TakeUntil(endTime: DateTimeOffset.Now.AddSeconds(1.5));

            var results = new List<int>();

            var tcs = new TaskCompletionSource<Exception>();

            var sw = new Stopwatch();
            sw.Start();

            using var subscription = flattenedNumbers.Subscribe(
                onNext: results.Add,
                onCompleted: () => tcs.SetResult(null),
                onError: ex => tcs.SetResult(ex));

            Exception ex = await tcs.Task; // This one blocks

            sw.Stop();
            var millis = sw.ElapsedMilliseconds;

            Assert.Null(ex);
            Assert.True(1300 < millis && millis < 1700);
            Assert.Equal(new[] { 6, 7, 8, 4, 5 }.ToArray(), results.ToArray());
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
        {
            IObservable<T> sequence1 = tup.Item1;
            IObservable<T> sequence2 = tup.Item2.EmitIn(dueTime);

            return Observable.Merge(sequence1, sequence2);
        }

        private static IObservable<int> Demo() =>
           1.EmitIn(TimeSpan.FromSeconds(1))
           .And(2).In(TimeSpan.FromSeconds(2));
    }
}
