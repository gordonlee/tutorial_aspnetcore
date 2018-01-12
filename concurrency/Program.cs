using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace concurrency
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

			ReadMapMain();			
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
}
