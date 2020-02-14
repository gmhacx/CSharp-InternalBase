﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static TestInject.Memory;

namespace TestInject
{
	public class MyLibrary
    {
	    [DllExport("DllMain", CallingConvention.Cdecl)]
		public static void EntryPoint()
		{
			UpdateProcessInformation();
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			
			if (!DebugConsole.InitiateDebugConsole())
				MessageBox.Show("Failed initiating the debugging console, please restart the program as admin!",
					"Debugging Console Exception",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error);

			// Probably dont do any hacking here, start a thread to your real main entry point

			var result = Pattern.FindPattern(
				"ac_client.exe",
				"C3 E8 E8 D9 00 00 E9 78 FE FF FF");

			Console.WriteLine($"Scan from all modules: 0x{result:X8}");

			Console.ReadLine();
		}

		static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			HelperMethods.PrintExceptionData(e?.ExceptionObject, true);
			if (!Debugger.IsAttached) return;

			Exception obj = (Exception)e?.ExceptionObject;
			if (obj != null)
				PInvoke.SetLastError((uint)obj.HResult);
			Debugger.Break();
		}
    }
}
