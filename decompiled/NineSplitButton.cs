using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/PauseMenu/NineSplitButton.cs")]
public class NineSplitButton : NinePatchRect
{
	public new class MethodName : NinePatchRect.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName MouseEnteredOn = "MouseEnteredOn";

		public static readonly StringName MouseExitedOff = "MouseExitedOff";
	}

	public new class PropertyName : NinePatchRect.PropertyName
	{
		public static readonly StringName offTexture = "offTexture";

		public static readonly StringName onTexture = "onTexture";

		public static readonly StringName frameCount = "frameCount";

		public static readonly StringName framesPerSecond = "framesPerSecond";

		public static readonly StringName currentFrame = "currentFrame";

		public static readonly StringName timeAccumulator = "timeAccumulator";

		public static readonly StringName frameWidth = "frameWidth";

		public static readonly StringName isPlaying = "isPlaying";
	}

	public new class SignalName : NinePatchRect.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private Texture2D offTexture;

	[Export(PropertyHint.None, "")]
	private AtlasTexture onTexture;

	[Export(PropertyHint.None, "")]
	private int frameCount = 18;

	[Export(PropertyHint.None, "")]
	private float framesPerSecond = 5f;

	private int currentFrame;

	private double timeAccumulator;

	private float frameWidth;

	private bool isPlaying;

	public override void _Ready()
	{
		if (onTexture != null)
		{
			frameWidth = onTexture.Region.Size.X;
		}
	}

	public override void _Process(double delta)
	{
		if (isPlaying && onTexture != null)
		{
			timeAccumulator += delta;
			double num = 1.0 / (double)framesPerSecond;
			if (timeAccumulator >= num)
			{
				timeAccumulator -= num;
				currentFrame = (currentFrame + 1) % frameCount;
				Rect2 region = onTexture.Region;
				region.Position = new Vector2(frameWidth * (float)currentFrame, region.Position.Y);
				onTexture.Region = region;
			}
		}
	}

	public void MouseEnteredOn()
	{
		base.Texture = onTexture;
		isPlaying = true;
		currentFrame = 0;
		timeAccumulator = 0.0;
	}

	public void MouseExitedOff()
	{
		base.Texture = offTexture;
		isPlaying = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(4)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.MouseEnteredOn, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.MouseExitedOff, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName._Ready && args.Count == 0)
		{
			_Ready();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MouseEnteredOn && args.Count == 0)
		{
			MouseEnteredOn();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MouseExitedOff && args.Count == 0)
		{
			MouseExitedOff();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.MouseEnteredOn)
		{
			return true;
		}
		if (method == MethodName.MouseExitedOff)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.offTexture)
		{
			offTexture = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.onTexture)
		{
			onTexture = VariantUtils.ConvertTo<AtlasTexture>(in value);
			return true;
		}
		if (name == PropertyName.frameCount)
		{
			frameCount = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.framesPerSecond)
		{
			framesPerSecond = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.currentFrame)
		{
			currentFrame = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.timeAccumulator)
		{
			timeAccumulator = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.frameWidth)
		{
			frameWidth = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.isPlaying)
		{
			isPlaying = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.offTexture)
		{
			value = VariantUtils.CreateFrom(in offTexture);
			return true;
		}
		if (name == PropertyName.onTexture)
		{
			value = VariantUtils.CreateFrom(in onTexture);
			return true;
		}
		if (name == PropertyName.frameCount)
		{
			value = VariantUtils.CreateFrom(in frameCount);
			return true;
		}
		if (name == PropertyName.framesPerSecond)
		{
			value = VariantUtils.CreateFrom(in framesPerSecond);
			return true;
		}
		if (name == PropertyName.currentFrame)
		{
			value = VariantUtils.CreateFrom(in currentFrame);
			return true;
		}
		if (name == PropertyName.timeAccumulator)
		{
			value = VariantUtils.CreateFrom(in timeAccumulator);
			return true;
		}
		if (name == PropertyName.frameWidth)
		{
			value = VariantUtils.CreateFrom(in frameWidth);
			return true;
		}
		if (name == PropertyName.isPlaying)
		{
			value = VariantUtils.CreateFrom(in isPlaying);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.offTexture, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.onTexture, PropertyHint.ResourceType, "AtlasTexture", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.frameCount, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.framesPerSecond, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.currentFrame, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.timeAccumulator, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.frameWidth, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isPlaying, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.offTexture, Variant.From(in offTexture));
		info.AddProperty(PropertyName.onTexture, Variant.From(in onTexture));
		info.AddProperty(PropertyName.frameCount, Variant.From(in frameCount));
		info.AddProperty(PropertyName.framesPerSecond, Variant.From(in framesPerSecond));
		info.AddProperty(PropertyName.currentFrame, Variant.From(in currentFrame));
		info.AddProperty(PropertyName.timeAccumulator, Variant.From(in timeAccumulator));
		info.AddProperty(PropertyName.frameWidth, Variant.From(in frameWidth));
		info.AddProperty(PropertyName.isPlaying, Variant.From(in isPlaying));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.offTexture, out var value))
		{
			offTexture = value.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.onTexture, out var value2))
		{
			onTexture = value2.As<AtlasTexture>();
		}
		if (info.TryGetProperty(PropertyName.frameCount, out var value3))
		{
			frameCount = value3.As<int>();
		}
		if (info.TryGetProperty(PropertyName.framesPerSecond, out var value4))
		{
			framesPerSecond = value4.As<float>();
		}
		if (info.TryGetProperty(PropertyName.currentFrame, out var value5))
		{
			currentFrame = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName.timeAccumulator, out var value6))
		{
			timeAccumulator = value6.As<double>();
		}
		if (info.TryGetProperty(PropertyName.frameWidth, out var value7))
		{
			frameWidth = value7.As<float>();
		}
		if (info.TryGetProperty(PropertyName.isPlaying, out var value8))
		{
			isPlaying = value8.As<bool>();
		}
	}
}
