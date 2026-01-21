namespace SDM.AbstractionsTests.Shared
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.Exposers;

	public class TestClassExposers
	{
		public static readonly Exposer<TestClass, string> Identifier = new Exposer<TestClass, string>((obj) => obj.Identifier, "Identifier");
		public static readonly Exposer<TestClass, string> Name = new Exposer<TestClass, string>((obj) => obj.Name, "Name");
		public static readonly Exposer<TestClass, int> Age = new Exposer<TestClass, int>((obj) => obj.Age, "Age");
		public static readonly Exposer<TestClass, DateTime> CreatedAt = new Exposer<TestClass, DateTime>((obj) => obj.CreatedAt, "CreatedAt");
		public static readonly Exposer<TestClass, TimeSpan> Range = new Exposer<TestClass, TimeSpan>((obj) => obj.Range, "Range");
		public static readonly Exposer<TestClass, decimal> Score = new Exposer<TestClass, decimal>((obj) => obj.Score, "Score");
		public static readonly Exposer<TestClass, float> Rating = new Exposer<TestClass, float>((obj) => obj.Rating, "Rating");
		public static readonly Exposer<TestClass, bool> IsActive = new Exposer<TestClass, bool>((obj) => obj.IsActive, "IsActive");
		public static readonly Exposer<TestClass, Status> Status = new Exposer<TestClass, Status>((obj) => obj.Status, "Status");
		public static readonly CollectionExposer<TestClass, string> Tags = new CollectionExposer<TestClass, string>((obj) => obj.Tags, "Tags");
		public static readonly CollectionExposer<TestClass, Status> Statuses = new CollectionExposer<TestClass, Status>((obj) => obj.Statuses, "Statuses");

		public static class Sub
		{
			public static readonly Exposer<TestClass, string> Description = new Exposer<TestClass, string>((obj) => obj.Sub.Description, "Sub.Description");
			public static readonly Exposer<TestClass, double> Size = new Exposer<TestClass, double>((obj) => obj.Sub.Size, "Sub.Size");
			public static readonly Exposer<TestClass, SdmObjectReference<TestClass>> Reference = new Exposer<TestClass, SdmObjectReference<TestClass>>((obj) => obj.Sub.Reference, "Sub.Reference");
			public static readonly Exposer<TestClass, Guid> Guid = new Exposer<TestClass, Guid>((obj) => obj.Sub.Guid, "Sub.Guid");
		}

		public static class SubClasses
		{
			public static readonly CollectionExposer<TestClass, string> Description = new CollectionExposer<TestClass, string>((obj) => obj.SubClasses.Where(x => x != null).Select(x => x.Description), "SubClasses.Description");
			public static readonly CollectionExposer<TestClass, double> Size = new CollectionExposer<TestClass, double>((obj) => obj.SubClasses.Where(x => x != null).Select(x => x.Size), "SubClasses.Size");
			public static readonly CollectionExposer<TestClass, SdmObjectReference<TestClass>> Reference = new CollectionExposer<TestClass, SdmObjectReference<TestClass>>((obj) => obj.SubClasses.Where(x => x != null).Select(x => x.Reference), "SubClasses.Reference");
			public static readonly CollectionExposer<TestClass, Guid> Guid = new CollectionExposer<TestClass, Guid>((obj) => obj.SubClasses.Where(x => x != null).Select(x => x.Guid), "SubClasses.Guid");
		}
	}
}
