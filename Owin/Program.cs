namespace DynamicPowerShellApi.Owin
{
	using System;

	using DynamicPowerShellApi.Configuration;

	using Microsoft.Owin.Hosting;

	/// <summary>
	/// The main program.
	/// </summary>
	class Program
	{
		/// <summary>
		/// The main.
		/// </summary>
		static void Main()
		{
			Uri baseAddress = WebApiConfiguration.Instance.HostAddress;

			try
			{
				// Start OWIN host 
				using (WebApp.Start<Startup>(url: baseAddress.ToString()))
				{
					Console.WriteLine("Listening on {0}. Press any key to exit.", baseAddress);

					Console.ReadLine();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Could not start service {0}", ex.Message);
				Console.ReadLine();
			}
		}
	}
}