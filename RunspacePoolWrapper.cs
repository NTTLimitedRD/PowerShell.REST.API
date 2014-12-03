namespace DynamicPowerShellApi
{
	using System;
	using System.Management.Automation.Runspaces;

	/// <summary>
	/// The run space pool wrapper.
	/// </summary>
	public class RunspacePoolWrapper : IDisposable
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
		/// Dispose of the run space.
		/// </summary>
		public void Dispose()
		{
			LazyRunspacePool.Value.Close();
		}
	}
}