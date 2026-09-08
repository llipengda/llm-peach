using System;
using System.Globalization;
using NUnit.Framework;
using Peach.Core;
using Peach.Core.Test;
using Peach.Pro.Core.Storage;
using Peach.Pro.Core.WebServices.Models;

namespace Peach.Pro.Test.Core.Storage
{
	[TestFixture]
	[Quick]
	class JobTests
	{
		TempFile _tmp;

		[SetUp]
		public void SetUp()
		{
			_tmp = new TempFile();
		}

		[TearDown]
		public void TearDown()
		{
			_tmp.Dispose();
		}

		[Test]
		public void StartStopDate()
		{
			// Verify we can properly round trip StartDate and StopDate to the job database
			const string dateFmt = "M/d/yyyy h:mm:ss tt";

			var startDate = DateTime.Parse("5/2/2001 5:38:09 AM", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);

			Assert.AreEqual(DateTimeKind.Local, startDate.Kind);
			Assert.AreEqual("5/2/2001 5:38:09 AM", startDate.ToString(dateFmt, CultureInfo.InvariantCulture));

			var j = new Job
			{
				Guid = Guid.Empty,
				StartDate = startDate,
			};

			Assert.AreEqual(DateTimeKind.Local, j.StartDate.Kind);
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StartDate.ToString(dateFmt, CultureInfo.InvariantCulture));
			Assert.False(j.StopDate.HasValue);

			using (var db = new NodeDatabase())
				db.InsertJob(j);

			Assert.AreEqual(DateTimeKind.Local, j.StartDate.Kind);
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StartDate.ToString(dateFmt, CultureInfo.InvariantCulture));
			Assert.False(j.StopDate.HasValue, "StopDate has a value");

			// Issue update w/o a stop date

			using (var db = new NodeDatabase())
				db.UpdateJob(j);

			Assert.AreEqual(DateTimeKind.Local, j.StartDate.Kind);
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StartDate.ToString(dateFmt, CultureInfo.InvariantCulture));
			Assert.False(j.StopDate.HasValue, "StopDate has a value");

			// Issue update with a stop date

			j.StopDate = startDate;

			Assert.True(j.StopDate.HasValue, "StopDate should be set");
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StopDate.Value.ToString(dateFmt, CultureInfo.InvariantCulture));

			using (var db = new NodeDatabase())
				db.UpdateJob(j);

			Assert.AreEqual(DateTimeKind.Local, j.StartDate.Kind);
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StartDate.ToString(dateFmt, CultureInfo.InvariantCulture));
			Assert.True(j.StopDate.HasValue, "StopDate should be set");
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StopDate.Value.ToString(dateFmt, CultureInfo.InvariantCulture));

			using (var db = new NodeDatabase())
				j = db.GetJob(j.Guid);

			Assert.NotNull(j, "Job is null");
			Assert.AreEqual(DateTimeKind.Local, j.StartDate.Kind);
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StartDate.ToString(dateFmt, CultureInfo.InvariantCulture));
			Assert.True(j.StopDate.HasValue, "StopDate should be set");
			Assert.AreEqual("5/2/2001 5:38:09 AM", j.StopDate.Value.ToString(dateFmt, CultureInfo.InvariantCulture));

			// Ensure they are stored in the database as UTC

			using (var db = new NodeDatabase())
			{
				var dt = db.SelectDateTime("SELECT StartDate from Job where Id='" + Guid.Empty + "'");

				Assert.AreEqual(DateTimeKind.Unspecified, dt.Kind);

				var asUtc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);

				Assert.AreEqual(j.StartDate.ToUniversalTime().ToString("o"), asUtc.ToString("o"));
			}
		}

		[Test]
		public void TestRunTime()
		{
			// Ensure runtime round trips from the job database

			var j = new Job
			{
				Guid = Guid.NewGuid(),
				Runtime = TimeSpan.FromMilliseconds(36111),
			};

			using (var db = new NodeDatabase())
				db.InsertJob(j);

			using (var db = new NodeDatabase())
				j = db.GetJob(j.Guid);

			Assert.AreEqual(TimeSpan.FromSeconds(36), j.Runtime);

			// Ensure TimeSpan is stored as total seconds

			using (var db = new NodeDatabase())
			{
				var val = db.SelectLong("SELECT Runtime from Job where Id='" + j.Id + "'");

				Assert.That(val, Is.TypeOf<long>());
				Assert.AreEqual(36, val);
			}
		}
	}
}
