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

			// ReadMapMain();			
			var module = new ConcurrentVersionTest();

			module.AddMap();
			module.ReadMap();
			module.GetElement();
			module.PushElement();
		}

		static void MonitorMain()
		{
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < 500; ++i)
			{
				var thread = new Thread(ThreadProc);
				threads.Add(thread);
				thread.Start(i);
			}

			while (true)
			{
				Console.WriteLine(ContextManager.GetInstance().ToString());
				Thread.Sleep(1000);
			}
		}

		static readonly int ReadMapSize = 100;
		static int[] ReadMapReadability = new int[ReadMapSize];

		static ConcurrentDictionary<int, int> concurrentDict 
			= new ConcurrentDictionary<int, int>();

		static void ReadMapMain()
		{
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < ReadMapSize; ++i)
			{
				var thread = new Thread(ReadMapProc);
				threads.Add(thread);
				thread.Start(i);
			}

			string input = "";
			while (input != "exit")
			{
				input = Console.ReadLine();
				int index = Convert.ToInt32(input);
				if (concurrentDict.TryAdd(index, index))
				{
					Console.WriteLine("{0} TryAdd() Success!!", index);
				}
				else
				{
					throw new Exception(string.Format("{0} TryAdd() failed.", index));
				}
				
			}
		}

		static void ReadMapProc(object index)
		{
			int key = (int)index;
			while (true)
			{
				int dictValue = -1;
				if (concurrentDict.TryGetValue(key, out dictValue))
				{
					ReadMapReadability[key] = 1;
					//Console.WriteLine("{0} found key!!", key);
				}
				else
				{
					ReadMapReadability[key] = 0;
					// throw new Exception(string.Format("{0} not found key.", key));
					//Console.WriteLine("{0} not found key.", key);
				}

				string line = ""; 
				foreach(var elem in ReadMapReadability)
				{
					line += elem.ToString() + ", ";
				}
				Console.WriteLine(line);

				Thread.Sleep(1);
			}
		}

		static void ThreadProc(object index)
		{
			Random rnd1 = new Random();
			
			while(true)
			{
				int key = (int)index % 5;
				var context = ContextManager.GetInstance().GetContext(key);

				int rand = rnd1.Next() % 200;

				Thread.Sleep(rand);
				// Thread.Sleep(10);

				ContextManager.GetInstance().PushContext(key, context);
				Thread.Sleep(rand);
			}
		}
	}

	

	public class ConcurrentVersionTest
	{
		IManager manager = null;

		const int LoopCount = 100000;

		public ConcurrentVersionTest()
		{
			ContextManagerOptions options = new ContextManagerOptions();
			manager = new concurrency.ConcurrentVersion.ContextManager(options);
		}

		public void AddMap()
		{
			ScopeTimerCollector collector = new ScopeTimerCollector();

			
			for (int i = 0; i < LoopCount; ++i )
			{
				var trait = new ScopeTimerTrait();
				collector.Add(trait);
				using(var scopeTimer = new ScopeTimer(trait))
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

			for (int i = 0; i < LoopCount; ++i)
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

			for (int i = 0; i < LoopCount; ++i)
			{
				var trait = new ScopeTimerTrait();
				collector.Add(trait);
				using (var scopeTimer = new ScopeTimer(trait))
				{
					var elem = manager.GetContext(i);
					if (elem == null)
					{
						throw new Exception("Element is null on GetElement()");
					}
				}
			}

			collector.Synthesize();
			Console.WriteLine("GetContext: {0}", collector.ToString());
		}

		public void PushElement()
		{
			ScopeTimerCollector collector = new ScopeTimerCollector();

			for (int i = 0; i < LoopCount; ++i)
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
                    LoopSpinCount = 10000,
                });
            }

            foreach(var thread in threads)
            {
                thread.Join();
            }

        }

        public void TestFunctionCallback(Object obj)
        {
            TestFunctionCallbackParameter parameters = (TestFunctionCallbackParameter)obj;

        }
	}

}
