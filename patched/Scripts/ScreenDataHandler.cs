using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/DataResources/ScreenDataHandler.cs")]
public partial class ScreenDataHandler : Resource
{

	public int taskbarPos;

	public int currentScreenTop;

	public int screenWidth;

	public int screenIndex;

	public int totalScreenWidth;

	public int leftmostScreenX;

	private bool _isScreenLocked;

	private int _lockedScreenLeft;

	private int _lockedScreenTop;

	private int _lockedScreenWidth;

	private int _lockedScreenTaskbarPos;

	public bool IsScreenLocked => _isScreenLocked;

	public int LockedScreenIndex
	{
		get
		{
			if (!_isScreenLocked)
			{
				return -1;
			}
			return screenIndex;
		}
	}

	public int EffectiveLeftX
	{
		get
		{
			if (!_isScreenLocked)
			{
				return leftmostScreenX;
			}
			return _lockedScreenLeft;
		}
	}

	public int EffectiveRightX
	{
		get
		{
			if (!_isScreenLocked)
			{
				return leftmostScreenX + totalScreenWidth;
			}
			return _lockedScreenLeft + _lockedScreenWidth;
		}
	}

	public void UpdateScreenInfo(Vector2I trueSize)
	{
		screenIndex = 0;
		RecalculateHorizontalBounds(trueSize);
	}

	public void UpdateCurrentScreen(Window refWindow, Vector2I trueSize)
	{
		int currentScreen = refWindow.CurrentScreen;
		if (currentScreen != screenIndex)
		{
			int num = taskbarPos;
			screenIndex = currentScreen;
			Rect2I rect2I = DisplayServer.ScreenGetUsableRect(screenIndex);
			taskbarPos = rect2I.End.Y - trueSize.Y;
			currentScreenTop = rect2I.Position.Y;
			screenWidth = rect2I.Size.X;
			if (!Main._isMobile) refWindow.Position = new Vector2I(refWindow.Position.X, refWindow.Position.Y - (taskbarPos - num));
			RecalculateHorizontalBounds(trueSize);
		}
	}

	private void RecalculateHorizontalBounds(Vector2I trueSize)
	{
		int screenCount = DisplayServer.GetScreenCount();
		Rect2I rect2I = DisplayServer.ScreenGetUsableRect(screenIndex);
		int num = rect2I.Position.X;
		int num2 = rect2I.Position.X + rect2I.Size.X;
		for (int i = 0; i < screenCount; i++)
		{
			if (i != screenIndex)
			{
				Rect2I rect2I2 = DisplayServer.ScreenGetUsableRect(i);
				int num3 = Mathf.Max(rect2I.Position.Y, rect2I2.Position.Y);
				if (Mathf.Min(rect2I.Position.Y + rect2I.Size.Y, rect2I2.Position.Y + rect2I2.Size.Y) > num3)
				{
					num = Mathf.Min(num, rect2I2.Position.X);
					num2 = Mathf.Max(num2, rect2I2.Position.X + rect2I2.Size.X);
				}
			}
		}
		leftmostScreenX = num;
		totalScreenWidth = num2 - num;
		Rect2I rect2I3 = rect2I;
		taskbarPos = rect2I3.End.Y - trueSize.Y;
		currentScreenTop = rect2I3.Position.Y;
		screenWidth = rect2I3.Size.X;
	}

	private static bool AreHorizontallyAdjacent(Rect2I a, Rect2I b)
	{
		int y = a.Position.Y;
		int num = a.Position.Y + a.Size.Y;
		int y2 = b.Position.Y;
		int num2 = b.Position.Y + b.Size.Y;
		if (y >= num2 || y2 >= num)
		{
			return false;
		}
		int x = a.Position.X;
		int num3 = a.Position.X + a.Size.X;
		int x2 = b.Position.X;
		int num4 = b.Position.X + b.Size.X;
		bool num5 = Mathf.Abs(num3 - x2) <= 2 || Mathf.Abs(num4 - x) <= 2;
		bool flag = x < num4 && x2 < num3;
		return num5 || flag;
	}

	public void LockToCurrentScreen(Vector2I trueSize)
	{
		_isScreenLocked = true;
		Rect2I rect2I = DisplayServer.ScreenGetUsableRect(screenIndex);
		_lockedScreenLeft = rect2I.Position.X;
		_lockedScreenTop = rect2I.Position.Y;
		_lockedScreenWidth = rect2I.Size.X;
		_lockedScreenTaskbarPos = rect2I.End.Y - trueSize.Y;
	}

	public void UnlockScreen()
	{
		_isScreenLocked = false;
	}

	public int ClampAcrossAllScreensX(int pos, int trueSizeX)
	{
		if (_isScreenLocked)
		{
			return Mathf.Clamp(pos, _lockedScreenLeft, _lockedScreenLeft + _lockedScreenWidth - trueSizeX);
		}
		return Mathf.Clamp(pos, leftmostScreenX, leftmostScreenX + totalScreenWidth - trueSizeX);
	}

	public int ClampAcrossAllScreensThinnerX(int pos, int trueSizeX, float widthFraction)
	{
		if (_isScreenLocked)
		{
			return Mathf.Clamp(pos, _lockedScreenLeft, _lockedScreenLeft + _lockedScreenWidth - trueSizeX);
		}
		return Mathf.Clamp(pos, leftmostScreenX, leftmostScreenX + totalScreenWidth - trueSizeX);
	}

	public int ClampAcrossAllScreensY(int pos)
	{
		if (_isScreenLocked)
		{
			return Mathf.Clamp(pos, _lockedScreenTop, _lockedScreenTaskbarPos);
		}
		return Mathf.Clamp(pos, currentScreenTop, taskbarPos);
	}

	public int ClampAttachmentY(int pos, int attachSizeY)
	{
		if (_isScreenLocked)
		{
			return Mathf.Clamp(pos, _lockedScreenTop, _lockedScreenTaskbarPos - attachSizeY);
		}
		int max = DisplayServer.ScreenGetUsableRect(screenIndex).End.Y - attachSizeY;
		return Mathf.Clamp(pos, currentScreenTop, max);
	}

	public int GetVerticalAdjacentTaskbarY(Vector2I mousePos, Vector2I trueSize)
	{
		int screenCount = DisplayServer.GetScreenCount();
		Rect2I rect2I = DisplayServer.ScreenGetUsableRect(screenIndex);
		for (int i = 0; i < screenCount; i++)
		{
			if (i == screenIndex)
			{
				continue;
			}
			Rect2I rect2I2 = DisplayServer.ScreenGetUsableRect(i);
			if (rect2I2.HasPoint(mousePos))
			{
				int num = Mathf.Max(rect2I.Position.Y, rect2I2.Position.Y);
				if (Mathf.Min(rect2I.Position.Y + rect2I.Size.Y, rect2I2.Position.Y + rect2I2.Size.Y) <= num)
				{
					return rect2I2.End.Y - trueSize.Y;
				}
			}
		}
		return -1;
	}

}
