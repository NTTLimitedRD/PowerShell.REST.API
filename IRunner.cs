namespace DynamicPowerShellApi
{
	using System.Collections.Generic;
	using System.Threading.Tasks;

	/// <summary>
	/// The Runner interface for executing methods with parameters.
	/// </summary>
	public interface IRunner
	{
		/// <summary>
		/// Asynchronous execution of a method with parameters.
		/// </summary>
		/// <param name="filename">
		/// The filename.
		/// </param>
		/// <param name="snapin">
		/// The snap in.
		/// </param>
		/// <param name="parametersList">
		/// The parameters List.
		/// </param>
		/// <returns>
		/// The <see cref="Task"/>.
		/// </returns>
		Task<string> ExecuteAsync(
			string filename,
			string snapin,
			IList<KeyValuePair<string, string>> parametersList);
	}
}
