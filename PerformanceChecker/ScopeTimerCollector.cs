﻿using System;
using System.Collections.Generic;
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
			foreach ( var elem in scopeTimers)
			{
				double elementMillis = elem.GetMillis();
				Min = Math.Min(Min, elementMillis);
				Max = Math.Max(Max, elementMillis);
				Sum += elementMillis;
			}

			Avg = Sum / scopeTimers.Count;
		}

		public override string ToString()
		{
			return string.Format("Sum: {0}, Min: {1}, Max: {2}, Avg: {3}, Count: {4}"
				, Sum, Min, Max, Avg, scopeTimers.Count);
		}

	}
}
