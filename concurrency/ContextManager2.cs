using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using concurrency.ConcurrentVersion;

namespace concurrency
{
	public class ContextPool2
	{
		ContextManagerOptions options;
		ConcurrentQueue<Element> freeQueue = new ConcurrentQueue<Element>();
		object lockObject = new object();
		int queueSize;

		public ContextPool2(ContextManagerOptions _options)
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

			// not found in queue.
			// we have to choose between resizing queue size or just create new one.
			return ExpandQueueSize();
		}

		public bool PushContext(Element element)
		{
			freeQueue.Enqueue(element);
			return true;
		}

		private Element ExpandQueueSize()
		{
			//MEMO: thread sync
			lock(lockObject)
			{
				Element element = null;
				if (freeQueue.TryDequeue(out element))
				{
					return element;
				}
				else
				{
					for (int i = 1; i < options.BucketSize; ++i)
					{
						freeQueue.Enqueue(new Element());
					}

					queueSize += options.BucketSize;
				}
			}

			return new Element();
		}

		public void Clear()
		{
			Element element = null;
			while (freeQueue.TryDequeue(out element))
			{
			}
		}

	}
}
