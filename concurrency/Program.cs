using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

using concurrency.ConcurrentVersion;
using PerformanceChecker;

namespace concurrency
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			for ( int i = 0; i < 5; ++i)
			{
				var module = new ConcurrentVersionTest();

				module.AddMap();
				module.ReadMap();
				module.GetElement();
				module.TestFunction();
				module.PushElement();
			}
		}
	}


	public class ConcurrentVersionTest
	{
		IManager manager = null;

		const int KeyLoopCount = 10000;
		const int ValueLoopCount = 300;

		public ConcurrentVersionTest()
		{
			ContextManagerOptions options = new ContextManagerOptions();
			manager = new concurrency.ConcurrentVersion.ContextManager(options);
			// manager = concurrency.ContextManager.GetInstance();
		}

		public void AddMap()
		{
			ScopeTimerCollector collector = new ScopeTimerCollector();


			for (int i = 0; i < KeyLoopCount; ++i)
			{
				var trait = new ScopeTimerTrait();
				collector.Add(trait);
				using (var scopeTimer = new ScopeTimer(trait))
				{
					manager.CreatePool(i);
				}
			}

			collector.Synthesize();
			Console.WriteLine("AddMap: {0}", collector.ToString());
		}

		public void ReadMap()
		{
			ScopeTimerCollector collector = new ScopeTimerCollector();

			for (int i = 0; i < KeyLoopCount; ++i)
			{
				var trait = new ScopeTimerTrait();
				collector.Add(trait);
				using (var scopeTimer = new ScopeTimer(trait))
				{
					manager.HasPool(i);
				}
			}

			collector.Synthesize();
			Console.WriteLine("ReadMap: {0}", collector.ToString());
		}

		public void GetElement()
		{
			ScopeTimerCollector collector = new ScopeTimerCollector();
			List<Element> list = new List<Element>();
			for (int i = 0; i < KeyLoopCount; ++i)
			{
				Element elem = null;
				for (int j = 0; j < ValueLoopCount; ++j)
				{
					var trait = new ScopeTimerTrait();
					collector.Add(trait);

					using (var scopeTimer = new ScopeTimer(trait))
					{
						elem = manager.GetContext(i);
						if (elem == null)
						{
							throw new Exception("Element is null on GetElement()");
						}
					}
				}
				list.Add(elem);
			}

			collector.Synthesize();
			Console.WriteLine("GetContext: {0}", collector.ToString());
		}

		public void PushElement()
		{
			ScopeTimerCollector collector = new ScopeTimerCollector();

			for (int i = 0; i < KeyLoopCount; ++i)
			{
				var trait = new ScopeTimerTrait();
				collector.Add(trait);
				var element = new Element();
				using (var scopeTimer = new ScopeTimer(trait))
				{
					manager.PushContext(i, element);
				}
			}

			collector.Synthesize();
			Console.WriteLine("PushContext: {0}", collector.ToString());
		}

		const int ThreadCount = 10;
		public class TestFunctionCallbackParameter
		{
			public int Index { get; set; }
			public ScopeTimerCollector2 Collectors { get; set; }
			public int LoopSpinCount { get; set; }
		}

		public void TestFunction()
		{
			ScopeTimerCollector2 collectors = new ScopeTimerCollector2();

			var threads = new List<Thread>();
			for (int i = 0; i < ThreadCount; ++i)
			{
				var thread = new Thread(TestFunctionCallback);
				threads.Add(thread);
				thread.Start(new TestFunctionCallbackParameter()
				{
					Collectors = collectors,
					Index = i,
					LoopSpinCount = 1000,
				});
			}

			foreach (var thread in threads)
			{
				thread.Join();
			}

			Console.WriteLine("GetContext on multi-threads: {0}", collectors.ToString());

		}

		public void TestFunctionCallback(Object obj)
		{
			TestFunctionCallbackParameter parameters = (TestFunctionCallbackParameter)obj;
			Random rnd1 = new Random();

			ScopeTimerCollector collector = new ScopeTimerCollector();
			List<Element> list = new List<Element>();

			for ( int i = 0; i < parameters.LoopSpinCount; ++i)
			{
				Element elem = null;
				for (int j = 0; j < ValueLoopCount; ++j)
				{
					var trait = new ScopeTimerTrait();
					collector.Add(trait);

					using (var scopeTimer = new ScopeTimer(trait))
					{
						elem = manager.GetContext(rnd1.Next() % parameters.LoopSpinCount);
						if (elem == null)
						{
							throw new Exception("Element is null on GetElement()");
						}
					}

					list.Add(elem);
				}
				
			}
			collector.Synthesize();
			// Console.WriteLine("GetContext: {0}", collector.ToString());

			parameters.Collectors.Push(collector);
		}

	}

}
