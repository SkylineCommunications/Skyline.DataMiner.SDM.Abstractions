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

		public bool? OptionalIsActive { get; set; }

		public sbyte SignedByte { get; set; }

		public byte UnsignedByte { get; set; }

		public short ShortNumber { get; set; }

		public ushort UnsignedShort { get; set; }

		public int? OptionalAge { get; set; }

		public uint UnsignedInt { get; set; }

		public long LongNumber { get; set; }

		public ulong UnsignedLong { get; set; }

		public double DoubleValue { get; set; }

		public string NickName { get; set; }

		public Status Status { get; set; }

		public List<string> Tags { get; set; }

		public ICollection<Status> Statuses { get; set; }

		public SubClass Sub { get; set; }

		public ICollection<SubClass> SubClasses { get; set; }
	}
}
