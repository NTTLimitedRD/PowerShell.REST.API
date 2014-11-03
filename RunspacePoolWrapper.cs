namespace DynamicPowerShellApi
{
	using System;
	using System.Management.Automation.Runspaces;

	using DD.Cloud.Aperture.Platform.Core;

	/// <summary>
	/// The run space pool wrapper.
	/// </summary>
	public class RunspacePoolWrapper : DisposableObject
	{
		/// <summary>
		/// The run space pool.
		/// </summary>
		private static readonly Lazy<RunspacePool> LazyRunspacePool = new Lazy<RunspacePool>(
			() =>
				{
					var rs = RunspaceFactory.CreateRunspacePool(1, 2);
					rs.Open();
					return rs;
				});

		/// <summary>
		/// Prevents a default instance of the <see cref="RunspacePoolWrapper"/> class from being created.
		/// </summary>
		private RunspacePoolWrapper()
		{
		}

		/// <summary>
		/// Gets the run space pool.
		/// </summary>
		public static RunspacePool Pool
		{
			get
			{
				return LazyRunspacePool.Value;
			}
		}

		/// <summary>
		/// The dispose.
		/// </summary>
		/// <param name="disposing">
		/// The disposing.
		/// </param>
		protected override void Dispose(bool disposing)
		{
			if (!disposing)
				LazyRunspacePool.Value.Close();
			base.Dispose(disposing);
		}
	}
}
