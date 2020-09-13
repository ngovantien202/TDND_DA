using System;

namespace TheoDoiNguoiDung
{
	using System;
	using System.Diagnostics;
	using System.Runtime.InteropServices;
	using System.Text;
	using System.Threading;

	public class Program
	{
		private class NativeMethods
		{
			[DllImport("user32.dll")]
			public static extern IntPtr GetForegroundWindow();

			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			public static extern int GetWindowThreadProcessId(IntPtr windowHandle, out int processId);
			[DllImport("user32.dll")]
			public static extern int GetWindowText(int hWnd, StringBuilder text, int count);
		}

		private string lastWindowTitle;

		private void WriteCurrentWindowInformation()
		{
			var activeWindowId = NativeMethods.GetForegroundWindow();
			if (activeWindowId.Equals(0))
			{
				return;
			}

			int processId;
			NativeMethods.GetWindowThreadProcessId(activeWindowId, out processId);

			if (processId == 0)
			{
				return;
			}

			Process foregroundProcess = Process.GetProcessById(processId);
			var windowTitle = string.Empty;

			try
			{
				if (!string.IsNullOrEmpty(foregroundProcess.MainWindowTitle))
				{
					windowTitle = foregroundProcess.MainWindowTitle;
				}
			}
			catch (Exception)
			{
			}

			try
			{
				if (string.IsNullOrEmpty(windowTitle))
				{
					const int Count = 1024;
					var sb = new StringBuilder(Count);
					NativeMethods.GetWindowText((int)activeWindowId, sb, Count);

					windowTitle = sb.ToString();
				}
			}
			catch (Exception)
			{
			}

			if (lastWindowTitle != windowTitle)
			{

				Console.WriteLine("Time: {0}\nWindowTitle: {1}\n",
					DateTime.Now.ToString("dd/MM/yyyy  HH:mm:ss"),
					windowTitle);

				lastWindowTitle = windowTitle;
			}
		}
		static void Main(string[] args)
		{
			Program program = new Program();
			while (true)
			{
				program.WriteCurrentWindowInformation();
				Thread.Sleep(1000);
			}
		}
	}
}
