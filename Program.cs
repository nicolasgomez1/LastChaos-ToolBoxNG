namespace LastChaos_ToolBoxNG
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
			Application.ThreadException += (_, e) => LogUnhandledException(e.Exception);
			AppDomain.CurrentDomain.UnhandledException += (_, e) =>
			{
				if (e.ExceptionObject is Exception ex)
					LogUnhandledException(ex);
				else
					LogUnhandledException(new Exception(e.ExceptionObject?.ToString() ?? "Unknown unhandled exception."));
			};

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			Application.SetColorMode(SystemColorMode.System);
			ApplicationConfiguration.Initialize();
			Application.Run(new Main());
		}

		private static void LogUnhandledException(Exception ex)
		{
			string strCrashLogPath = Path.Combine(AppContext.BaseDirectory, "Crash.log");
			string strCrashText = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\r\n{ex}\r\n\r\n";

			try
			{
				File.AppendAllText(strCrashLogPath, strCrashText);
			}
			catch
			{
				// The process is already failing; avoid throwing from the crash logger.
			}

			MessageBox.Show(
				$"ToolBoxNG hit an unexpected error. Details were written to:\n{strCrashLogPath}",
				"LastChaos ToolBoxNG",
				MessageBoxButtons.OK,
				MessageBoxIcon.Error);
		}
	}
}
