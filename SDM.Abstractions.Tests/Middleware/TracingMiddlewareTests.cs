namespace SDM.AbstractionsTests
{
	using System;
	using System.Diagnostics;
	using System.Linq;

	using FluentAssertions;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using SDM.AbstractionsTests.Middleware;

	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.Middleware;
	using Skyline.DataMiner.SDM.Telemetry;

	[TestClass]
	public class TracingMiddlewareTests
	{
		[TestMethod]
		public void Middleware_TracingMiddleware()
		{
			// Arrange
			var repository = Mocked.CreateExampleProvider(new TracingMiddleware<ExampleObject>());

			var item = default(ExampleObject);
			var tracingListener = new TracingListener();
			ActivitySource.AddActivityListener(tracingListener.Listener);

			{
				// Act Read
				var result = repository.Read(new TRUEFilterElement<ExampleObject>()).ToList();
				item = result.FirstOrDefault();

				// Assert Read
				item.Should().NotBeNull();
				Assert.IsTrue(tracingListener.ActivityHasStarted);
				Assert.IsTrue(tracingListener.ActivityHasStopped);
				Assert.IsNotNull(result);
				tracingListener.Reset();
			}

			{
				// Act Update
				item.Name = "Updated Name";
				repository.Update(item);

				// Assert Update
				tracingListener.ActivityHasStarted.Should().BeTrue();
				tracingListener.ActivityHasStopped.Should().BeTrue();
				tracingListener.Reset();
			}

			{
				// Act Create
				item = new ExampleObject
				{
					Name = "Created",
					Info = new Info
					{
						IntProperty = 5,
					},
				};
				repository.Create(item);

				// Assert Create
				tracingListener.ActivityHasStarted.Should().BeTrue();
				tracingListener.ActivityHasStopped.Should().BeTrue();
				tracingListener.Reset();
			}

			{
				// Act Delete
				repository.Delete(item);

				// Assert Delete
				tracingListener.ActivityHasStarted.Should().BeTrue();
				tracingListener.ActivityHasStopped.Should().BeTrue();
				tracingListener.Reset();
			}

			{
				// Act Count
				var result = repository.Count(ExampleObjectExposers.Name.Contains("Update"));

				// Assert Count
				result.Should().Be(1);
				tracingListener.ActivityHasStarted.Should().BeTrue();
				tracingListener.ActivityHasStopped.Should().BeTrue();
				tracingListener.Reset();
			}

			{
				// Act ReadPaged
				var result = repository.ReadPaged(new TRUEFilterElement<ExampleObject>(), 1);

				// Assert ReadPaged
				foreach (var page in result)
				{
					tracingListener.ActivityHasStarted.Should().BeTrue();
					tracingListener.Reset();
				}
			}
		}
	}

	internal class TracingListener
	{
		public TracingListener()
		{
			Listener = new ActivityListener
			{
				ShouldListenTo = source => source.Name == SdmActivitySource.SourceName,
				Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
				ActivityStarted = activity => ActivityHasStarted = true,
				ActivityStopped = activity => ActivityHasStopped = true,
			};
		}

		public ActivityListener Listener { get; }

		public bool ActivityHasStarted { get; private set; }

		public bool ActivityHasStopped { get; private set; }

		public void Reset()
		{
			ActivityHasStarted = false;
			ActivityHasStopped = false;
		}
	}
}