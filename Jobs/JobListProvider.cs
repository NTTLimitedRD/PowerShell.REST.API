using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DynamicPowerShellApi.Jobs
{
	/// <summary>	A job list provider. </summary>
	/// <remarks>	Anthony, 6/1/2015. </remarks>
	/// <seealso cref="T:DynamicPowerShellApi.Jobs.IJobListProvider"/>
	public class JobListProvider 
		: IJobListProvider
	{
		/// <summary>	The running jobs. </summary>
		private ConcurrentDictionary<Guid, string> _runningJobs;
 
		/// <summary>	The completed jobs. </summary>
		private ConcurrentDictionary<Guid, bool> _completedJobs;

		/// <summary> Initializes a new instance of the DynamicPowerShellApi.Jobs.JobListProvider
		/// 	class. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		public JobListProvider()
		{
			_runningJobs = new ConcurrentDictionary<Guid, string>();
			_completedJobs = new ConcurrentDictionary<Guid, bool>();
		}

		/// <summary>	Adds a requested job to the list of running jobs. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		/// <param name="jobId">			Identifier for the job. </param>
		/// <param name="requestedHost">	The requested host (IP). </param>
		/// <seealso cref="M:DynamicPowerShellApi.Jobs.IJobListProvider.AddRequestedJob(Guid,string)"/>
		public void AddRequestedJob(Guid jobId, string requestedHost)
		{
			if (!_runningJobs.TryAdd(jobId, requestedHost))
			{
				// erm. do something bad.
			}
		}

		/// <summary>	Sets job outcome. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		/// <param name="jobId">  	Identifier for the job. </param>
		/// <param name="outcome">	true to outcome. </param>
		/// <param name="message">	The message. </param>
		/// <seealso cref="M:DynamicPowerShellApi.Jobs.IJobListProvider.CompleteJob(Guid,bool,string)"/>
		public void CompleteJob(Guid jobId, bool outcome, string message)
		{
			// Remove from running jobs
			string requestedHost;

			if (_runningJobs.TryRemove(jobId, out requestedHost))
			{
				// Add to completed jobs.
				_completedJobs.TryAdd(jobId, outcome);
			}
		}

		/// <summary>	Did job succeed. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		/// <param name="jobId">	Identifier for the job. </param>
		/// <returns>	true if it succeeds, false if it fails. </returns>
		/// <seealso cref="M:DynamicPowerShellApi.Jobs.IJobListProvider.DidJobSucceed(Guid)"/>
		public bool DidJobSucceed(Guid jobId)
		{
			bool result;
			if (_completedJobs.TryGetValue(jobId, out result))
			{
				return result;
			}
			return false;
		}

		/// <summary>	Gets running jobs. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		/// <returns>	An array of key value pair&lt; guid,string&gt; </returns>
		/// <seealso cref="M:DynamicPowerShellApi.Jobs.IJobListProvider.GetRunningJobs()"/>
		public KeyValuePair<Guid, string>[] GetRunningJobs()
		{
			return _runningJobs.ToArray();
		}

		/// <summary>	Gets completed jobs. </summary>
		/// <remarks>	Anthony, 6/1/2015. </remarks>
		/// <returns>	An array of key value pair&lt; guid,bool&gt; </returns>
		public KeyValuePair<Guid, bool>[] GetCompletedJobs()
		{
			return _completedJobs.ToArray();
		}
	}
}
