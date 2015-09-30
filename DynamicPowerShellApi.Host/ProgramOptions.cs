namespace DynamicPowerShellApi.Host
{
	using System;

	using CommandLine;
	using CommandLine.Text;

	/// <summary>
	///		Options for the Host service program.
	/// </summary>
	public class ProgramOptions
	{
		/// <summary>
		///		Run the Dynamic PowerShell API as a console application?
		/// </summary>
		[Option("console", HelpText = "Run the Dynamic PowerShell API as a console application")]
		public bool RunConsole
		{
			get;
			set;
		}

		/// <summary>
		///		Run the Dynamic PowerShell API as a windows service?
		/// </summary>
		[Option("service", HelpText = "Run the Dynamic PowerShell API as a windows service")]
		public bool RunService
		{
			get;
			set;
		}

		/// <summary>
		///		Install the Host Windows service?
		/// </summary>
		[Option('i', "install-service", DefaultValue = false, MutuallyExclusiveSet = "install", HelpText = "Install the Windows service")]
		public bool InstallService
		{
			get;
			set;
		}

		/// <summary>
		///		Install the Host Windows service?
		/// </summary>
		[Option('r', "uninstall-service", DefaultValue = false, MutuallyExclusiveSet = "uninstall", HelpText = "Uninstall the Windows service")]
		public bool UninstallService
		{
			get;
			set;
		}

		/// <summary>
		///		The service user name.
		/// </summary>
		[Option('u', "service-user", HelpText = "Windows service user name (required if installing the service)")]
		public string ServiceUserName
		{
			get;
			set;
		}

		/// <summary>
		///		The service user name.
		/// </summary>
		[Option('p', "service-password", HelpText = "Windows service password (required if installing the service)")]
		public string ServicePassword
		{
			get;
			set;
		}

		/// <summary>
		///		The last parser state.
		/// </summary>
		[ParserState]
		public IParserState ParserState
		{
			get;
			set;
		}

		/// <summary>
		///		Get a message representing information on how to call the program.
		/// </summary>
		/// <returns>
		///		The help message.
		/// </returns>
		[HelpOption]
		public string Help()
		{
			HelpText helpText = new HelpText
			{
				Heading = "Dynamic Powershell API Host",
				Copyright = "Copyright (c) 2014 Dimension Data",
				MaximumDisplayWidth = 80
			};

			string errors = helpText.RenderParsingErrorsText(this, 4);
			if (!String.IsNullOrWhiteSpace(errors))
			{
				// Trim off trailing newline.
				if (errors.EndsWith(Environment.NewLine))
					errors = errors.Substring(0, errors.Length - Environment.NewLine.Length);

				helpText.AddPreOptionsLine(errors);
			}

			helpText.AddOptions(this);

			return helpText;
		}

		/// <summary>
		///		Parses command line
		/// </summary>
		/// <param name="commandLineArguments">
		///		The command-line arguments to parse.
		/// </param>
		/// <param name="options">
		///		Receives the program options.
		/// </param>
		/// <returns>
		///		<c>true</c>, if the command-line arguments were successfully parsed; otherwise, <c>false</c>.
		/// </returns>
		public static bool ParseCommandLine(string[] commandLineArguments, out ProgramOptions options)
		{
			if (commandLineArguments == null)
				throw new ArgumentNullException("commandLineArguments");

			options = new ProgramOptions();

			Parser parser = new Parser(
				with =>
					with.MutuallyExclusive = true
			);

			return
				parser.ParseArgumentsStrict(
					args: commandLineArguments,
					options: options,
					onFail:
						() =>
						{
							// Let the caller decide what to do.
						}
				);
		}
	}
}