using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace DynamicPowerShellApi.Logging
{
	/// <summary>
	/// File implementation of crash logger.
	/// </summary>
	public class CrashLogger
		: ICrashLogger
	{
		/// <summary>
		/// The base path for log entries
		/// </summary>
		private readonly string _basePath;

		/// <summary>
		/// Initializes a new instance of the <see cref="CrashLogger"/> class.
		/// </summary>
		public CrashLogger()
		{
			_basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".", "Logs");

			if (!Directory.Exists(_basePath))
			{
				Directory.CreateDirectory(_basePath);
			}
		}

		/// <summary>
		/// Saves the log entry to a file
		/// </summary>
		/// <param name="entry">The error entry.</param>
		/// <returns>
		/// The file name that the error entry became.
		/// </returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public string SaveLog(CrashLogEntry entry)
		{
			try
			{
				string logFileName = entry.RequestMethod + entry.LogTime.ToFileTime() + ".xml";
				StreamWriter srWriter = new StreamWriter(Path.Combine(_basePath, logFileName));

				XmlSerializer serializer = new XmlSerializer(typeof(CrashLogEntry));

				serializer.Serialize(srWriter, entry);

				srWriter.Close();

				return logFileName;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Failed to write log entry {0}", ex.Message);
				return "Failed to log file.";
			}
		}
	}
}
