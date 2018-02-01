using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

namespace PerformanceChecker
{
	public class ScopeTimerCollector
	{
		List<ScopeTimerTrait> scopeTimers = new List<ScopeTimerTrait>();

		public double Sum { get; set; }
		public double Min { get; set; }
		public double Max { get; set; }
		public double Avg { get; set; }
		public long Count { get; set; }

		public ScopeTimerCollector()
		{
			Min = double.MaxValue;
			Max = double.MinValue;
		}

		public void Add(ScopeTimerTrait timer)
		{
			scopeTimers.Add(timer);
		}

		public void Synthesize()
		{
			foreach (var elem in scopeTimers)
			{
				double elementMillis = elem.GetMillis();
				Min = Math.Min(Min, elementMillis);
				Max = Math.Max(Max, elementMillis);
				Sum += elementMillis;
			}

			Avg = Sum / scopeTimers.Count;
			Count = scopeTimers.Count;
		}

		public override string ToString()
		{
			return string.Format("Sum: {0}, Min: {1}, Max: {2}, Avg: {3}, Count: {4}"
								 , Sum, Min, Max, Avg, Count);
		}

		public static ScopeTimerCollector operator +(ScopeTimerCollector lhs, ScopeTimerCollector rhs)
		{
			return new ScopeTimerCollector()
			{
				Sum = lhs.Sum + rhs.Sum,
				Min = Math.Min(lhs.Min, rhs.Min),
				Max = Math.Max(lhs.Max, rhs.Max),
				Avg = (lhs.Sum + rhs.Sum) / (lhs.Count + rhs.Count),
				Count = lhs.Count + rhs.Count,
			};
		}

	}

	// expand multiple threads
	public class ScopeTimerCollector2
	{
		private List<ScopeTimerCollector> collectors = new List<ScopeTimerCollector>();
		private ScopeTimerCollector aggregationCollector;
		object lockObject = new object();

		public void Push(ScopeTimerCollector collector)
		{
			lock (lockObject)
			{
				collectors.Add(collector);
			}
		}

		public void Synthesize()
		{
			lock (lockObject)
			{
				aggregationCollector = new ScopeTimerCollector();
				foreach (var collector in collectors)
				{
					aggregationCollector += collector;
				}
			}
		}


		public override string ToString()
		{
			if (aggregationCollector == null)
			{
				Synthesize();
			}
			return aggregationCollector.ToString();
		}

	}

}

