using System;
using NLog;
using NLog.Targets;
using Peach.Pro.Core.Storage;
using Peach.Pro.Core.WebServices.Models;

namespace Peach.Pro.Core.Loggers
{
	class DatabaseTarget : TargetWithLayout
	{
		NodeDatabase _db = new NodeDatabase();
		readonly string _jobId;

		public DatabaseTarget(Guid jobId)
		{
			Name = "DatabaseTarget";
			_jobId = jobId.ToString();
		}

		protected override void Write(LogEventInfo logEvent)
		{
			// RandomStrategy emits startup debug messages before Engine_TestStarting
			// has inserted the Job row.  The old provider silently tolerated that
			// ordering; SQLite with foreign keys enabled does not.  Ignore only
			// those orphan startup messages and keep the FK enforced for real logs.
			Guid jobId;
			if (!Guid.TryParse(_jobId, out jobId) || _db.GetJob(jobId) == null)
				return;

			_db.InsertJobLog(new JobLog
			{
				JobId = _jobId,
				Message = Layout.Render(logEvent),
			});
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing && _db != null)
			{
				_db.Dispose();
				_db = null;
			}
		}
	}
}
