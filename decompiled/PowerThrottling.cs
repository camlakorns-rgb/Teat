using System;
using System.Runtime.InteropServices;
using Godot;

public static class PowerThrottling
{
	private struct PROCESS_POWER_THROTTLING_STATE
	{
		public uint Version;

		public uint ControlMask;

		public uint StateMask;
	}

	private const uint PROCESS_POWER_THROTTLING_CURRENT_VERSION = 1u;

	private const uint PROCESS_POWER_THROTTLING_EXECUTION_SPEED = 1u;

	private const int ProcessPowerThrottling = 4;

	private const uint ABOVE_NORMAL_PRIORITY_CLASS = 32768u;

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern nint GetCurrentProcess();

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetProcessInformation(nint hProcess, int processInformationClass, ref PROCESS_POWER_THROTTLING_STATE processInformation, uint processInformationSize);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetPriorityClass(nint hProcess, uint priorityClass);

	public static void DisableThrottling()
	{
		if (OS.GetName() != "Windows")
		{
			return;
		}
		try
		{
			PROCESS_POWER_THROTTLING_STATE pROCESS_POWER_THROTTLING_STATE = default(PROCESS_POWER_THROTTLING_STATE);
			pROCESS_POWER_THROTTLING_STATE.Version = 1u;
			pROCESS_POWER_THROTTLING_STATE.ControlMask = 1u;
			pROCESS_POWER_THROTTLING_STATE.StateMask = 0u;
			PROCESS_POWER_THROTTLING_STATE processInformation = pROCESS_POWER_THROTTLING_STATE;
			nint currentProcess = GetCurrentProcess();
			if (!SetProcessInformation(currentProcess, 4, ref processInformation, (uint)Marshal.SizeOf(processInformation)))
			{
				GD.PrintErr("SetProcessInformation failed: " + Marshal.GetLastWin32Error());
			}
			SetPriorityClass(currentProcess, 32768u);
		}
		catch (Exception ex)
		{
			GD.PrintErr("PowerThrottling.DisableThrottling exception: " + ex.Message);
		}
	}
}
