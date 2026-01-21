namespace SDM.AbstractionsTests.Shared
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.SDM;

	public class TestClass : SdmObject<TestClass>
	{
		public string Name { get; set; }

		public int Age { get; set; }

		public DateTime CreatedAt { get; set; }

		public TimeSpan Range { get; set; }

		public decimal Score { get; set; }

		public float Rating { get; set; }

		public bool IsActive { get; set; }

		public Status Status { get; set; }

		public List<string> Tags { get; set; }

		public ICollection<Status> Statuses { get; set; }

		public SubClass Sub { get; set; }

		public ICollection<SubClass> SubClasses { get; set; }
	}
}
