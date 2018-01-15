using System;


namespace PerformanceChecker
{
	interface IScopeTimer
	{
	}

    public class ScopeTimer : IScopeTimer, IDisposable
	{
		ScopeTimerTrait _trait = null;

		public ScopeTimer(ScopeTimerTrait trait)
		{
			_trait = trait;
			
			_trait.Start();
		}

		public void Dispose()
		{
			_trait.End();
		}
	}

	public class ScopeTimerTrait
	{
		public DateTime? StartTime { get; set; }

		public DateTime? EndTime { get; set; }

		public ScopeTimerTrait()
		{
			StartTime = null;
			EndTime = null;
		}

		public bool Start()
		{
			if (StartTime.HasValue)
			{
				return false;
			}

			StartTime = DateTime.UtcNow;
			return true;
		}

		public bool End()
		{
			if (EndTime.HasValue)
			{
				return false;
			}

			EndTime = DateTime.UtcNow;
			return true;
		}

		public double GetMillis()
		{
			if (StartTime.HasValue && EndTime.HasValue && 
				StartTime <= EndTime)
			{
				var diff = EndTime - StartTime;
				return diff.Value.TotalMilliseconds;
			}

			return -1;
		}

	}

}
