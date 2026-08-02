using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[Tool]
[ScriptPath("res://Shaders/AnimatedSpriteShaderSync.cs")]
public class AnimatedSpriteShaderSync : AnimatedSprite2D
{
	public new class MethodName : AnimatedSprite2D.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public new static readonly StringName _ValidateProperty = "_ValidateProperty";

		public static readonly StringName UpdateFrameUV = "UpdateFrameUV";

		public static readonly StringName UpdateMaxBiasOffset = "UpdateMaxBiasOffset";
	}

	public new class PropertyName : AnimatedSprite2D.PropertyName
	{
		public static readonly StringName Progress = "Progress";

		public static readonly StringName _progress = "_progress";
	}

	public new class SignalName : AnimatedSprite2D.SignalName
	{
	}

	private float _progress;

	[Export(PropertyHint.Range, "0,1,0.001")]
	public float Progress
	{
		get
		{
			return _progress;
		}
		set
		{
			_progress = value;
			if (base.Material is ShaderMaterial shaderMaterial)
			{
				shaderMaterial.SetShaderParameter("progress", _progress);
			}
		}
	}

	public override void _Ready()
	{
		UpdateMaxBiasOffset();
	}

	public override void _Process(double delta)
	{
		UpdateFrameUV();
	}

	public override void _ValidateProperty(Dictionary property)
	{
		if (Engine.IsEditorHint())
		{
			UpdateFrameUV();
			UpdateMaxBiasOffset();
		}
	}

	private void UpdateFrameUV()
	{
		if (base.SpriteFrames == null || base.Material == null)
		{
			return;
		}
		StringName animation = base.Animation;
		if (!base.SpriteFrames.HasAnimation(animation))
		{
			return;
		}
		Texture2D frameTexture = base.SpriteFrames.GetFrameTexture(animation, base.Frame);
		if (base.Material is ShaderMaterial shaderMaterial)
		{
			if (!(frameTexture is AtlasTexture atlasTexture))
			{
				shaderMaterial.SetShaderParameter("frame_uv_origin", Vector2.Zero);
				shaderMaterial.SetShaderParameter("frame_uv_size", Vector2.One);
				return;
			}
			Vector2 vector = new Vector2(atlasTexture.Atlas.GetWidth(), atlasTexture.Atlas.GetHeight());
			Rect2 region = atlasTexture.Region;
			shaderMaterial.SetShaderParameter("frame_uv_origin", region.Position / vector);
			shaderMaterial.SetShaderParameter("frame_uv_size", region.Size / vector);
		}
	}

	private void UpdateMaxBiasOffset()
	{
		if (base.Material is ShaderMaterial shaderMaterial)
		{
			Vector2 vector = (Vector2)shaderMaterial.GetShaderParameter("grid_size");
			Vector2 with = (Vector2)shaderMaterial.GetShaderParameter("progress_bias") / 10f;
			float num = new Vector2(vector.X - 1f, vector.Y - 1f).Dot(with);
			shaderMaterial.SetShaderParameter("max_bias_offset", num);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(5)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName._ValidateProperty, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Dictionary, "property", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateFrameUV, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateMaxBiasOffset, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName._ValidateProperty && args.Count == 1)
		{
			_ValidateProperty(VariantUtils.ConvertTo<Dictionary>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateFrameUV && args.Count == 0)
		{
			UpdateFrameUV();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateMaxBiasOffset && args.Count == 0)
		{
			UpdateMaxBiasOffset();
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
		if (method == MethodName._ValidateProperty)
		{
			return true;
		}
		if (method == MethodName.UpdateFrameUV)
		{
			return true;
		}
		if (method == MethodName.UpdateMaxBiasOffset)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.Progress)
		{
			Progress = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._progress)
		{
			_progress = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Progress)
		{
			float from = Progress;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._progress)
		{
			value = VariantUtils.CreateFrom(in _progress);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, PropertyName.Progress, PropertyHint.Range, "0,1,0.001", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName._progress, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName progress = PropertyName.Progress;
		float from = Progress;
		info.AddProperty(progress, Variant.From(in from));
		info.AddProperty(PropertyName._progress, Variant.From(in _progress));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.Progress, out var value))
		{
			Progress = value.As<float>();
		}
		if (info.TryGetProperty(PropertyName._progress, out var value2))
		{
			_progress = value2.As<float>();
		}
	}
}
