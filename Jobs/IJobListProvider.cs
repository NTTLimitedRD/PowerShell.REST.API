using System;
using System.Collections.Generic;

namespace DynamicPowerShellApi.Jobs
{
	/// <summary>	Interface for job list provider. </summary>
	/// <remarks>	Anthony, 6/1/2015. </remarks>
	public interface IJobListProvider
	{
		/// <summary>	Adds a requested job to 'requestedHost'. </summary>
		/// <param name="jobId">			Identifier for the job. </param>
		/// <param name="requestedHost">	The requested host. </param>
		void AddRequestedJob(Guid jobId, string requestedHost);

		/// <summary>	Sets job outcome. </summary>
		/// <param name="jobId">  	Identifier for the job. </param>
		/// <param name="outcome">	true to outcome. </param>
		/// <param name="message">	The message. </param>
		void CompleteJob(Guid jobId, bool outcome, string message);

		/// <summary>	Did job succeed. </summary>
		/// <param name="jobId">	Identifier for the job. </param>
		/// <returns>	true if it succeeds, false if it fails. </returns>
		bool DidJobSucceed(Guid jobId);

		/// <summary>	Gets running jobs. </summary>
		/// <returns>	An array of key value pair&lt; guid,string&gt; </returns>
		KeyValuePair<Guid, string>[] GetRunningJobs();

		/// <summary>	Gets completed jobs. </summary>
		/// <returns>	An array of key value pair&lt; guid,bool&gt; </returns>
		KeyValuePair<Guid, bool>[] GetCompletedJobs();
	}
}
