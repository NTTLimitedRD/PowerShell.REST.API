using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DynamicPowerShellApi.Exceptions;

namespace DynamicPowerShellApi.Logging
{
	/// <summary>
	/// Crash log entry.
	/// </summary>
	[Serializable]
	public class CrashLogEntry
	{
		/// <summary>
		/// The _activity identifier
		/// </summary>
		private Guid _activityId;

		/// <summary>
		/// The activity identifier string (NOT USED)
		/// </summary>
		private string _activityIdStr;

		/// <summary>
		/// Gets or sets the activity identifier.
		/// </summary>
		/// <value>
		/// The activity identifier.
		/// </value>
		[XmlElement]
		public string ActivityId
		{
			get { return _activityId.ToString(); }
			set { _activityIdStr = value; }
		}

		/// <summary>
		/// Sets the activity identifier.
		/// </summary>
		/// <param name="activityId">The activity identifier.</param>
		public void SetActivityId(Guid activityId)
		{
			_activityId = activityId;
			_activityIdStr = activityId.ToString();
		}

		/// <summary>
		/// Gets or sets the log time.
		/// </summary>
		/// <value>
		/// The log time.
		/// </value>
		[XmlElement]
		public DateTime LogTime { get; set; }

		/// <summary>
		/// Gets or sets the request URL.
		/// </summary>
		/// <value>
		/// The request URL.
		/// </value>
		[XmlElement]
		public string RequestUrl { get; set; }

		/// <summary>
		/// Gets or sets the request address.
		/// </summary>
		/// <value>
		/// The request address.
		/// </value>
		[XmlElement]
		public string RequestAddress { get; set; }

		/// <summary>
		/// Gets or sets the exceptions.
		/// </summary>
		/// <value>
		/// The exceptions.
		/// </value>
		[XmlArray]
		public List<PowerShellException> Exceptions { get; set; }

		/// <summary>
		/// Gets or sets the request method.
		/// </summary>
		/// <value>
		/// The request method.
		/// </value>
		[XmlElement]
		public string RequestMethod { get; set; }
	}
}
