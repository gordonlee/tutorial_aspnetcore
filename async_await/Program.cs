using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace async_await
{
    class Program
    {
		// static Interlocked interlockedObj = new Interlocked();
		static long interlockedNumber = 0;
		static CancellationTokenSource tokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
			int workers = 0, cpthreads = 0;
			ThreadPool.GetMaxThreads(out workers, out cpthreads);

            Console.WriteLine("Hello World!");
			string command = null;
			// while (command != "exit")
			for (int i = 0; i < 1; ++i)
			{
				LongTask();

				// command = Console.ReadLine();
				Console.WriteLine("Main loop continue: {0} LockCount: {1}"
					, Process.GetCurrentProcess().Threads.Count, interlockedNumber);
				// Task.Delay(1);
				Thread.Sleep(1);
			}
			command = Console.ReadLine();

		}

		static async void LongTask()
		{
			// Interlocked.Add(ref interlockedNumber, 1);
			
			try
			{
				// await Task.Delay(1000, tokenSource.Token);
				await Task.Factory.StartNew(async () => 
				{
					Interlocked.Increment(ref interlockedNumber);

					// Task.Delay(30000);
					Thread.Sleep(30000);
					//var task = Task.Delay(30000);
					//task.Wait();
					//await task.ContinueWith();
					Console.WriteLine("task done. current thread: {0}", Process.GetCurrentProcess().Threads.Count);
					Interlocked.Decrement(ref interlockedNumber);
				});
			}
			catch(TaskCanceledException ex)
			{
				// .. do nothing
				Console.WriteLine("TaskCanceledException {0}", ex.ToString());
			}
			catch(Exception ex)
			{
				Console.WriteLine("Exception {0}", ex.ToString());
			}
			Console.WriteLine("LongTask done. current thread: {0}", Process.GetCurrentProcess().Threads.Count);
		}

		static IEnumerable<int> YieldReturnTest()
		{
			yield return 1;
			yield return 2;
			yield return 3;
			yield return 4;
		}
    }
}
