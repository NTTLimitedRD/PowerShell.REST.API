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
