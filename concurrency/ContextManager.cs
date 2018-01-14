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
		Element GetContext(int key);

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

    public class ContextManager : IManager
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
    public class ContextManagerOptions
    {
        public ContextManagerOptions()
        {
            SpinCountOnDequeue = 1000;
        }

        public int SpinCountOnDequeue { get; set; }
    }


    public class ContextManager : IManager
    {
        ConcurrentDictionary<int, ConcurrentQueue<Element>> dict
        = new ConcurrentDictionary<int, ConcurrentQueue<Element>>();

        ContextManagerOptions options;

        public ContextManager(ContextManagerOptions _options)
        {
            options = _options;
        }

        public bool CreateContextPool(int key)
        {
            var newQueue = new ConcurrentQueue<Element>();
            var queue = dict.GetOrAdd(key, newQueue);

            //FIXME: newQueue와 queue의 비교는 주소값 비교여야 한다.
            if (newQueue == queue)
            {
                return true;
            }
            return false;
        }

        public Element GetContext(int key)
        {
            ConcurrentQueue<Element> queue = null;
            if (!dict.TryGetValue(key, out queue))
            {
                // not found in dictionary
                return null;
            }

            Element element = null;
            for (int i = 0; i < options.SpinCountOnDequeue; ++i)
            {
                if(queue.TryDequeue(out element))
                {
                    return element;
                }    
            }

            // not found on queue.
            // we have to choose between resizing queue size or just create new one
            //FIXME: handling later.
            return new Element();
        }

        public bool PushContext(int key, Element element)
        {
			ConcurrentQueue<Element> queue = null;
			if (!dict.TryGetValue(key, out queue))
			{
                // not found in dictionary
                return false;
			}

            queue.Enqueue(element);
            return true;
        }


    }
}