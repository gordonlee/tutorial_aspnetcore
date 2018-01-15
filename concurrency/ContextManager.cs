using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace concurrency
{
	public class Element
	{
		char[] data = new char[1024];
		public Element()
		{
		}

	}

	interface IManager
	{
		bool CreatePool(int key);
		bool HasPool(int key);
		Element GetContext(int key);
		bool PushContext(int key, Element element);
	}

	public class ElementWrapper : IDisposable
	{
		int Key;
		public ElementWrapper(int key)
		{
			Key = key;
			// 사용 시작 => 꺼내기
			element = ContextManager.GetInstance().GetContext(key);
		}

		public void Dispose()
		{
			// 사용 완료 => 반납
			ContextManager.GetInstance().PushContext(Key, element);
		}

		Element element;
	}

    public class ContextManager 
    {
		static ContextManager instance = new ContextManager();

		static Dictionary<int, Stack<Element>> dict
			= new Dictionary<int, Stack<Element>>();

		static int StackSize = 100;

		static public ContextManager GetInstance()
		{
			return instance;
		}

		public ContextManager()
		{
			
		}

		public Element GetContext(int key)
		{
			// 이렇게 쓰면, pool에서 context를 찾는 함수(기존에 GetContext)와
			// 나중에 다 쓰고 pool에 반납하는 코드를 사용자가 작성해야 한다. 
			Element result = null;

			Monitor.Enter(instance);

			Stack<Element> stack = null;

			stack = dict.GetValueOrDefault(key);
			if (stack == null)
			{
				stack = new Stack<Element>();
				dict.Add(key, stack);
			}

			if (stack.TryPop(out result))
			{
				// pop 성공
			}
			else
			{
				result = new Element();
			}
			
			Monitor.Exit(instance);

			return result;
		}

		public void PushContext(int key, Element elem)
		{
			Monitor.Enter(instance);
			Stack<Element> stack = null;

			stack = dict.GetValueOrDefault(key);
			if (stack == null)
			{
				stack = new Stack<Element>();
				dict.Add(key, stack);
			}

			if (stack.Count < StackSize)
			{
				stack.Push(elem);
			}

			Monitor.Exit(instance);
		}

		public override string ToString()
		{
			int i = 0;
			StringBuilder builder = new StringBuilder();
			builder.AppendLine("Start point of ToString()");
			Monitor.Enter(instance);
			foreach (var stack in dict.Values)
			{
				builder.AppendLine(string.Format("{0} {1}", i, stack.Count));
				++i;
			}
			Monitor.Exit(instance);
			builder.AppendLine("End point of ToString()");
			builder.AppendLine();

			return builder.ToString();
		}
	}
}


namespace concurrency.ConcurrentVersion
{
	public enum OverFlowAction
	{
		Unknown = 0,
		CreateNewOne,
		WaitForOthers,
	}

	public class ContextManagerOptions
	{
		public ContextManagerOptions()
		{
			SpinCountOnDequeue = 1000;
			OverFlowAction = OverFlowAction.CreateNewOne;
			BucketSize = 32;
		}

		public int SpinCountOnDequeue { get; set; }
		public OverFlowAction OverFlowAction { get; set; }

		public int BucketSize { get; set; }
    }


    public class ContextManager : IManager
    {
        ConcurrentDictionary<int, ContextPool> dict
        = new ConcurrentDictionary<int, ContextPool>();

        ContextManagerOptions options;

        public ContextManager(ContextManagerOptions _options)
        {
            options = _options;
		}

        public bool CreatePool(int key)
        {
            var newQueue = new ContextPool(options);
            return dict.TryAdd(key, newQueue);
        }

		public bool HasPool(int key)
		{
			ContextPool queue = null;
			if (!dict.TryGetValue(key, out queue))
			{
				return false;
			}
			return true;
		}

        public Element GetContext(int key)
        {
			ContextPool queue = null;
            if (!dict.TryGetValue(key, out queue))
            {
                // not found in dictionary
                return null;
            }

			return queue.GetContext();
        }

        public bool PushContext(int key, Element element)
        {
			ContextPool queue = null;
			if (!dict.TryGetValue(key, out queue))
			{
                // not found in dictionary
                return false;
			}

			return queue.PushContext(element);
        }
    }

	public class ContextPool
	{
		ContextManagerOptions options;

		int queueSize;

		ConcurrentQueue<Element[]> bucket = new ConcurrentQueue<Element[]>();
		ConcurrentQueue<Element> freeQueue = new ConcurrentQueue<Element>();

		public ContextPool(ContextManagerOptions _options)
		{
			options = _options;

			queueSize = 0;

			PushContext(ExpandQueueSize());
		}

		public Element GetContext()
		{
			Element element = null;
			for (int i = 0; i < options.SpinCountOnDequeue; ++i)
			{
				if (freeQueue.TryDequeue(out element))
				{
					return element;
				}

				//MEMO: 지금 돌아가고 있는 Thread를 잠깐 놔줌. (다른 큐에서 넣어줄 지 모르니)
				Thread.Sleep(0);
			}

			// not found on queue.
			// we have to choose between resizing queue size or just create new one
			return ExpandQueueSize();
		}

		public bool PushContext(Element element)
		{
			freeQueue.Enqueue(element);
			return true;
		}

		private Element ExpandQueueSize()
		{
			//FIXME: expand sync with other threads
			Element[] arr = new Element[options.BucketSize];
			for (int i = 0; i < arr.Length; ++i)
			{
				arr[i] = new Element();
				if (i != 0)
				{
					freeQueue.Enqueue(arr[i]);
				}
			}
			bucket.Enqueue(arr);

			queueSize += options.BucketSize;

			return arr[0];
		}

		public void Clear()
		{
			Element[] result = null;
			while(bucket.TryDequeue(out result))
			{

			}

			Element element = null;
			while(freeQueue.TryDequeue(out element))
			{

			}
		}

	}
}