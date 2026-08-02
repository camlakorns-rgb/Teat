using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/DataResources/ScreenDataHandler.cs")]
public class ScreenDataHandler : Resource
{
	public new class MethodName : Resource.MethodName
	{
		public static readonly StringName UpdateScreenInfo = "UpdateScreenInfo";

		public static readonly StringName UpdateCurrentScreen = "UpdateCurrentScreen";

		public static readonly StringName RecalculateHorizontalBounds = "RecalculateHorizontalBounds";

		public static readonly StringName AreHorizontallyAdjacent = "AreHorizontallyAdjacent";

		public static readonly StringName LockToCurrentScreen = "LockToCurrentScreen";

		public static readonly StringName UnlockScreen = "UnlockScreen";

		public static readonly StringName ClampAcrossAllScreensX = "ClampAcrossAllScreensX";

		public static readonly StringName ClampAcrossAllScreensThinnerX = "ClampAcrossAllScreensThinnerX";

		public static readonly StringName ClampAcrossAllScreensY = "ClampAcrossAllScreensY";

		public static readonly StringName ClampAttachmentY = "ClampAttachmentY";

		public static readonly StringName GetVerticalAdjacentTaskbarY = "GetVerticalAdjacentTaskbarY";
	}

	public new class PropertyName : Resource.PropertyName
	{
		public static readonly StringName IsScreenLocked = "IsScreenLocked";

		public static readonly StringName LockedScreenIndex = "LockedScreenIndex";

		public static readonly StringName EffectiveLeftX = "EffectiveLeftX";

		public static readonly StringName EffectiveRightX = "EffectiveRightX";

		public static readonly StringName taskbarPos = "taskbarPos";

		public static readonly StringName currentScreenTop = "currentScreenTop";

		public static readonly StringName screenWidth = "screenWidth";

		public static readonly StringName screenIndex = "screenIndex";

		public static readonly StringName totalScreenWidth = "totalScreenWidth";

		public static readonly StringName leftmostScreenX = "leftmostScreenX";

		public static readonly StringName _isScreenLocked = "_isScreenLocked";

		public static readonly StringName _lockedScreenLeft = "_lockedScreenLeft";

		public static readonly StringName _lockedScreenTop = "_lockedScreenTop";

		public static readonly StringName _lockedScreenWidth = "_lockedScreenWidth";

		public static readonly StringName _lockedScreenTaskbarPos = "_lockedScreenTaskbarPos";
	}

	public new class SignalName : Resource.SignalName
	{
	}

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
			refWindow.Position = new Vector2I(refWindow.Position.X, refWindow.Position.Y - (taskbarPos - num));
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

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(11)
		{
			new MethodInfo(MethodName.UpdateScreenInfo, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Vector2I, "trueSize", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateCurrentScreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "refWindow", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false),
				new PropertyInfo(Variant.Type.Vector2I, "trueSize", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.RecalculateHorizontalBounds, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Vector2I, "trueSize", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.AreHorizontallyAdjacent, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Rect2I, "a", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Rect2I, "b", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.LockToCurrentScreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Vector2I, "trueSize", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UnlockScreen, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ClampAcrossAllScreensX, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "pos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Int, "trueSizeX", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ClampAcrossAllScreensThinnerX, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "pos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Int, "trueSizeX", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "widthFraction", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ClampAcrossAllScreensY, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "pos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ClampAttachmentY, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "pos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Int, "attachSizeY", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetVerticalAdjacentTaskbarY, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Vector2I, "mousePos", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Vector2I, "trueSize", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.UpdateScreenInfo && args.Count == 1)
		{
			UpdateScreenInfo(VariantUtils.ConvertTo<Vector2I>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateCurrentScreen && args.Count == 2)
		{
			UpdateCurrentScreen(VariantUtils.ConvertTo<Window>(in args[0]), VariantUtils.ConvertTo<Vector2I>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RecalculateHorizontalBounds && args.Count == 1)
		{
			RecalculateHorizontalBounds(VariantUtils.ConvertTo<Vector2I>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AreHorizontallyAdjacent && args.Count == 2)
		{
			bool from = AreHorizontallyAdjacent(VariantUtils.ConvertTo<Rect2I>(in args[0]), VariantUtils.ConvertTo<Rect2I>(in args[1]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.LockToCurrentScreen && args.Count == 1)
		{
			LockToCurrentScreen(VariantUtils.ConvertTo<Vector2I>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UnlockScreen && args.Count == 0)
		{
			UnlockScreen();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ClampAcrossAllScreensX && args.Count == 2)
		{
			int from2 = ClampAcrossAllScreensX(VariantUtils.ConvertTo<int>(in args[0]), VariantUtils.ConvertTo<int>(in args[1]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.ClampAcrossAllScreensThinnerX && args.Count == 3)
		{
			int from3 = ClampAcrossAllScreensThinnerX(VariantUtils.ConvertTo<int>(in args[0]), VariantUtils.ConvertTo<int>(in args[1]), VariantUtils.ConvertTo<float>(in args[2]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.ClampAcrossAllScreensY && args.Count == 1)
		{
			int from4 = ClampAcrossAllScreensY(VariantUtils.ConvertTo<int>(in args[0]));
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.ClampAttachmentY && args.Count == 2)
		{
			int from5 = ClampAttachmentY(VariantUtils.ConvertTo<int>(in args[0]), VariantUtils.ConvertTo<int>(in args[1]));
			ret = VariantUtils.CreateFrom(in from5);
			return true;
		}
		if (method == MethodName.GetVerticalAdjacentTaskbarY && args.Count == 2)
		{
			int from6 = GetVerticalAdjacentTaskbarY(VariantUtils.ConvertTo<Vector2I>(in args[0]), VariantUtils.ConvertTo<Vector2I>(in args[1]));
			ret = VariantUtils.CreateFrom(in from6);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.AreHorizontallyAdjacent && args.Count == 2)
		{
			bool from = AreHorizontallyAdjacent(VariantUtils.ConvertTo<Rect2I>(in args[0]), VariantUtils.ConvertTo<Rect2I>(in args[1]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.UpdateScreenInfo)
		{
			return true;
		}
		if (method == MethodName.UpdateCurrentScreen)
		{
			return true;
		}
		if (method == MethodName.RecalculateHorizontalBounds)
		{
			return true;
		}
		if (method == MethodName.AreHorizontallyAdjacent)
		{
			return true;
		}
		if (method == MethodName.LockToCurrentScreen)
		{
			return true;
		}
		if (method == MethodName.UnlockScreen)
		{
			return true;
		}
		if (method == MethodName.ClampAcrossAllScreensX)
		{
			return true;
		}
		if (method == MethodName.ClampAcrossAllScreensThinnerX)
		{
			return true;
		}
		if (method == MethodName.ClampAcrossAllScreensY)
		{
			return true;
		}
		if (method == MethodName.ClampAttachmentY)
		{
			return true;
		}
		if (method == MethodName.GetVerticalAdjacentTaskbarY)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.taskbarPos)
		{
			taskbarPos = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.currentScreenTop)
		{
			currentScreenTop = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.screenWidth)
		{
			screenWidth = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.screenIndex)
		{
			screenIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.totalScreenWidth)
		{
			totalScreenWidth = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.leftmostScreenX)
		{
			leftmostScreenX = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._isScreenLocked)
		{
			_isScreenLocked = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._lockedScreenLeft)
		{
			_lockedScreenLeft = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._lockedScreenTop)
		{
			_lockedScreenTop = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._lockedScreenWidth)
		{
			_lockedScreenWidth = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._lockedScreenTaskbarPos)
		{
			_lockedScreenTaskbarPos = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.IsScreenLocked)
		{
			bool from = IsScreenLocked;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		int from2;
		if (name == PropertyName.LockedScreenIndex)
		{
			from2 = LockedScreenIndex;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.EffectiveLeftX)
		{
			from2 = EffectiveLeftX;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.EffectiveRightX)
		{
			from2 = EffectiveRightX;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.taskbarPos)
		{
			value = VariantUtils.CreateFrom(in taskbarPos);
			return true;
		}
		if (name == PropertyName.currentScreenTop)
		{
			value = VariantUtils.CreateFrom(in currentScreenTop);
			return true;
		}
		if (name == PropertyName.screenWidth)
		{
			value = VariantUtils.CreateFrom(in screenWidth);
			return true;
		}
		if (name == PropertyName.screenIndex)
		{
			value = VariantUtils.CreateFrom(in screenIndex);
			return true;
		}
		if (name == PropertyName.totalScreenWidth)
		{
			value = VariantUtils.CreateFrom(in totalScreenWidth);
			return true;
		}
		if (name == PropertyName.leftmostScreenX)
		{
			value = VariantUtils.CreateFrom(in leftmostScreenX);
			return true;
		}
		if (name == PropertyName._isScreenLocked)
		{
			value = VariantUtils.CreateFrom(in _isScreenLocked);
			return true;
		}
		if (name == PropertyName._lockedScreenLeft)
		{
			value = VariantUtils.CreateFrom(in _lockedScreenLeft);
			return true;
		}
		if (name == PropertyName._lockedScreenTop)
		{
			value = VariantUtils.CreateFrom(in _lockedScreenTop);
			return true;
		}
		if (name == PropertyName._lockedScreenWidth)
		{
			value = VariantUtils.CreateFrom(in _lockedScreenWidth);
			return true;
		}
		if (name == PropertyName._lockedScreenTaskbarPos)
		{
			value = VariantUtils.CreateFrom(in _lockedScreenTaskbarPos);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Int, PropertyName.taskbarPos, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.currentScreenTop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.screenWidth, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.screenIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.totalScreenWidth, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.leftmostScreenX, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._isScreenLocked, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._lockedScreenLeft, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._lockedScreenTop, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._lockedScreenWidth, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._lockedScreenTaskbarPos, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.IsScreenLocked, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.LockedScreenIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.EffectiveLeftX, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.EffectiveRightX, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.taskbarPos, Variant.From(in taskbarPos));
		info.AddProperty(PropertyName.currentScreenTop, Variant.From(in currentScreenTop));
		info.AddProperty(PropertyName.screenWidth, Variant.From(in screenWidth));
		info.AddProperty(PropertyName.screenIndex, Variant.From(in screenIndex));
		info.AddProperty(PropertyName.totalScreenWidth, Variant.From(in totalScreenWidth));
		info.AddProperty(PropertyName.leftmostScreenX, Variant.From(in leftmostScreenX));
		info.AddProperty(PropertyName._isScreenLocked, Variant.From(in _isScreenLocked));
		info.AddProperty(PropertyName._lockedScreenLeft, Variant.From(in _lockedScreenLeft));
		info.AddProperty(PropertyName._lockedScreenTop, Variant.From(in _lockedScreenTop));
		info.AddProperty(PropertyName._lockedScreenWidth, Variant.From(in _lockedScreenWidth));
		info.AddProperty(PropertyName._lockedScreenTaskbarPos, Variant.From(in _lockedScreenTaskbarPos));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.taskbarPos, out var value))
		{
			taskbarPos = value.As<int>();
		}
		if (info.TryGetProperty(PropertyName.currentScreenTop, out var value2))
		{
			currentScreenTop = value2.As<int>();
		}
		if (info.TryGetProperty(PropertyName.screenWidth, out var value3))
		{
			screenWidth = value3.As<int>();
		}
		if (info.TryGetProperty(PropertyName.screenIndex, out var value4))
		{
			screenIndex = value4.As<int>();
		}
		if (info.TryGetProperty(PropertyName.totalScreenWidth, out var value5))
		{
			totalScreenWidth = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName.leftmostScreenX, out var value6))
		{
			leftmostScreenX = value6.As<int>();
		}
		if (info.TryGetProperty(PropertyName._isScreenLocked, out var value7))
		{
			_isScreenLocked = value7.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._lockedScreenLeft, out var value8))
		{
			_lockedScreenLeft = value8.As<int>();
		}
		if (info.TryGetProperty(PropertyName._lockedScreenTop, out var value9))
		{
			_lockedScreenTop = value9.As<int>();
		}
		if (info.TryGetProperty(PropertyName._lockedScreenWidth, out var value10))
		{
			_lockedScreenWidth = value10.As<int>();
		}
		if (info.TryGetProperty(PropertyName._lockedScreenTaskbarPos, out var value11))
		{
			_lockedScreenTaskbarPos = value11.As<int>();
		}
	}
}
