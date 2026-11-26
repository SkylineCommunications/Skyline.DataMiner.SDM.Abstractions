namespace SDM.AbstractionsTests
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.Exposers;

	[TestClass]
	public class PartialObjectTests
	{
		private static readonly Exposer<Example, int> Number = new Exposer<Example, int>(x => x.Number, nameof(Example.Number));
		private static readonly Exposer<Example, int?> OptionalNumber = new Exposer<Example, int?>(x => x.OptionalNumber, nameof(Example.OptionalNumber));
		private static readonly Exposer<Example, string?> Text = new Exposer<Example, string?>(x => x.Text, nameof(Example.Text));
		private static readonly Exposer<Example, ExampleStatus?> Status = new Exposer<Example, ExampleStatus?>(x => x.Status, nameof(Example.Status));
		private static readonly Exposer<Example, SdmObjectReference<Example>> Reference = new Exposer<Example, SdmObjectReference<Example>>(x => x.Reference, nameof(Example.Reference));
		private static readonly CollectionExposer<Example, string> Items = new CollectionExposer<Example, string>(x => x.Items, nameof(Example.Items));
		private static readonly Exposer<Example, Unsupported> UnsupportedField = new Exposer<Example, Unsupported>(x => x.Unsupported, nameof(Example.Unsupported));

		[TestMethod]
		public void SelectedFieldsReturnValuesIncludingNullAndDefault()
		{
			var instance = new Example { Number = 0, OptionalNumber = null, Text = null, Status = ExampleStatus.Active, Reference = new SdmObjectReference<Example>("other"), Items = new[] { "one", "two" } };
			var partial = PartialObject<Example>.CreateBuilder()
				.Include(Number, instance.Number)
				.Include(OptionalNumber, instance.OptionalNumber)
				.Include(Text, instance.Text)
				.Include(Status, instance.Status)
				.Include(Reference, instance.Reference)
				.Include(Items, instance.Items)
				.Build();

			Assert.IsTrue(partial.HasField(Number));
			Assert.IsTrue(partial.HasField((FieldExposer)Number));
			Assert.IsTrue(partial.TryGetValue(Number, out var number));
			Assert.AreEqual(0, number);
			Assert.AreEqual(0, partial.GetValue(Number));
			Assert.IsTrue(partial.TryGetValue(OptionalNumber, out var optional));
			Assert.IsNull(optional);
			Assert.IsTrue(partial.TryGetValue(Text, out var text));
			Assert.IsNull(text);
			Assert.IsTrue(partial.TryGetValue(Status, out var status));
			Assert.AreEqual(ExampleStatus.Active, status);
			Assert.IsTrue(partial.TryGetValue(Reference, out var reference));
			Assert.AreEqual("other", reference.Identifier);
			Assert.ThrowsExactly<InvalidOperationException>(() => partial.TryGetValue(Items, out _));
			Assert.IsTrue(partial.TryGetValues(Items, out var items));
			CollectionAssert.AreEqual(new[] { "one", "two" }, items.ToArray());
			CollectionAssert.AreEqual(new[] { "one", "two" }, partial.GetValues(Items).ToArray());
		}

		[TestMethod]
		public void BuilderPreservesNullAndEmptyCollectionValues()
		{
			IEnumerable<string> nullItems = null;
			var nullCollection = PartialObject<Example>.CreateBuilder()
				.Include(Items, nullItems)
				.Build();
			var emptyCollection = PartialObject<Example>.CreateBuilder()
				.Include(Items, Array.Empty<string>())
				.Build();

			Assert.IsTrue(nullCollection.TryGetValues(Items, out var retrievedNullItems));
			Assert.IsNull(retrievedNullItems);
			Assert.IsTrue(emptyCollection.TryGetValues(Items, out var retrievedEmptyItems));
			Assert.IsNotNull(retrievedEmptyItems);
			Assert.IsFalse(retrievedEmptyItems.Any());
		}

		[TestMethod]
		public void IncludingTheSameExposerUsesTheLastValue()
		{
			var partial = PartialObject<Example>.CreateBuilder()
				.Include(Number, 1)
				.Include(Number, 2)
				.Build();

			Assert.AreEqual(2, partial.GetValue(Number));
		}

		[TestMethod]
		public void BuildCopiesTheSelectedValues()
		{
			var builder = PartialObject<Example>.CreateBuilder()
				.Include(Number, 1);
			var partial = builder.Build();

			builder.Include(Text, "added later");

			Assert.IsTrue(partial.HasField(Number));
			Assert.IsFalse(partial.HasField(Text));
		}

		[TestMethod]
		public void DerivedCollectionExposerIsHandledAsACollection()
		{
			var exposer = new DerivedItemsExposer();
			var partial = PartialObject<Example>.CreateBuilder()
				.Include(exposer, new List<string> { "one" })
				.Build();

			Assert.ThrowsExactly<InvalidOperationException>(() => partial.TryGetValue(exposer, out _));
			Assert.IsTrue(partial.TryGetValues(exposer, out var values));
			CollectionAssert.AreEqual(new[] { "one" }, values.ToArray());
		}

		[TestMethod]
		public void NullableValueCanBePresentOrAbsentIndependentlyOfItsValue()
		{
			var instance = new Example { OptionalNumber = 42 };
			var selected = PartialObject<Example>.CreateBuilder()
				.Include(OptionalNumber, instance.OptionalNumber)
				.Build();
			Assert.IsTrue(selected.TryGetValue(OptionalNumber, out var value));
			Assert.AreEqual(42, value);
			Assert.IsFalse(new PartialObject<Example>().TryGetValue(OptionalNumber, out var missing));
			Assert.IsNull(missing);
		}

		[TestMethod]
		public void UnselectedFieldReturnsFalseAndDefault()
		{
			var partial = PartialObject<Example>.CreateBuilder()
				.Include(Text, "present")
				.Build();
			Assert.IsFalse(partial.HasField(Number));
			Assert.IsFalse(partial.TryGetValue(Number, out var number));
			Assert.AreEqual(0, number);
			Assert.ThrowsExactly<InvalidOperationException>(() => partial.GetValue(Number));
			Assert.ThrowsExactly<InvalidOperationException>(() => partial.GetValues(Items));
			Assert.IsTrue(partial.TryGetValue(Text, out var text));
			Assert.AreEqual("present", text);
			Assert.IsFalse(new PartialObject<Example>().TryGetValue(Text, out _));
		}

		[TestMethod]
		public void ExposerCollectionDeduplicatesTypedExposers()
		{
			var fields = new ExposerCollection<Example>();

			Assert.IsTrue(fields.Add(Number));
			Assert.IsFalse(fields.Add(Number));
			Assert.IsTrue(fields.Contains(Number));
			Assert.AreEqual(1, fields.Count);
			Assert.AreSame(Number, fields.First());
		}

		[TestMethod]
		public void ExposerCollectionRejectsNullExposer()
		{
			var fields = new ExposerCollection<Example>();

			Assert.ThrowsExactly<ArgumentNullException>(() => fields.Add<int>(null!));
			Assert.ThrowsExactly<ArgumentNullException>(() => fields.Contains<int>(null!));
		}

		[TestMethod]
		public void SelectionUsesExposerEquality()
		{
			var partial = PartialObject<Example>.CreateBuilder()
				.Include(Number, 7)
				.Build();
			var anotherExposer = new Exposer<Example, int>(x => x.Number, nameof(Example.Number));
			Assert.IsTrue(partial.HasField(Number));
			Assert.IsFalse(partial.TryGetValue(anotherExposer, out _));
		}

		[TestMethod]
		public void RejectsUnsupportedFieldsAndInvalidArguments()
		{
			var instance = new Example();
			var empty = new PartialObject<Example>();
			Assert.IsFalse(empty.HasField(Number));
			Assert.ThrowsExactly<ArgumentNullException>(() => PartialObject<Example>.CreateBuilder().Include<int>(null!, 0));
			var withUnsupported = PartialObject<Example>.CreateBuilder().Include(UnsupportedField, instance.Unsupported).Build();
			Assert.IsTrue(withUnsupported.TryGetValue(UnsupportedField, out var retrievedUnsupported));
			Assert.IsNotNull(retrievedUnsupported);
			var partial = new PartialObject<Example>();
			Assert.ThrowsExactly<ArgumentNullException>(() => partial.HasField((FieldExposer)null!));
			Assert.ThrowsExactly<ArgumentNullException>(() => partial.TryGetValue<int>(null!, out _));
			Assert.IsFalse(partial.TryGetValue(UnsupportedField, out var missingUnsupported));
			Assert.IsNull(missingUnsupported);
		}

		public enum ExampleStatus { Active }

		public sealed class Unsupported { }

		private sealed class DerivedItemsExposer : CollectionExposer<Example, string>
		{
			public DerivedItemsExposer()
				: base(x => x.Items, nameof(Example.Items))
			{
			}
		}

		public sealed class Example : SdmObject<Example>
		{
			public int Number { get; set; }
			public int? OptionalNumber { get; set; }
			public string? Text { get; set; }
			public ExampleStatus? Status { get; set; }
			public SdmObjectReference<Example> Reference { get; set; }
			public IEnumerable<string> Items { get; set; } = Array.Empty<string>();
			public Unsupported Unsupported { get; set; } = new Unsupported();
		}

		public sealed class OtherExample : SdmObject<OtherExample>
		{
			public int Number { get; set; }
		}
	}
}
