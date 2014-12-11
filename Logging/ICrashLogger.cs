namespace DynamicPowerShellApi.Logging
{
	/// <summary>
	/// Crash logging.
	/// </summary>
	public interface ICrashLogger
	{
		/// <summary>
		/// Saves the log entry to a file 
		/// </summary>
		/// <param name="entry">The error entry.</param>
		/// <returns>The file name that the error entry became.</returns>
		string SaveLog(CrashLogEntry entry);
	}
}