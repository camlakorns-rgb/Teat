using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/GalleryMenu/PieceDisplayHandler.cs")]
public class PieceDisplayHandler : Control
{
	public new class MethodName : Control.MethodName
	{
		public static readonly StringName OpenGalleryPiece = "OpenGalleryPiece";

		public static readonly StringName PlayAndClamp = "PlayAndClamp";

		public static readonly StringName OnAnimatedSpriteFrameChanged = "OnAnimatedSpriteFrameChanged";

		public static readonly StringName ClampSpriteScale = "ClampSpriteScale";

		public new static readonly StringName _ExitTree = "_ExitTree";

		public static readonly StringName FullScreenGalleryPiece = "FullScreenGalleryPiece";
	}

	public new class PropertyName : Control.PropertyName
	{
		public static readonly StringName parentWindow = "parentWindow";

		public static readonly StringName targetDisplay = "targetDisplay";

		public static readonly StringName targetDiscription = "targetDiscription";

		public static readonly StringName displayAnimationPlayer = "displayAnimationPlayer";

		public static readonly StringName animatedSprite = "animatedSprite";

		public static readonly StringName fullscreen = "fullscreen";

		public static readonly StringName maxScreenPercent = "maxScreenPercent";

		public static readonly StringName baseScale = "baseScale";

		public static readonly StringName _frameChangedConnected = "_frameChangedConnected";
	}

	public new class SignalName : Control.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private Window parentWindow;

	[Export(PropertyHint.None, "")]
	private TextureButton targetDisplay;

	[Export(PropertyHint.None, "")]
	private RichTextLabel targetDiscription;

	[Export(PropertyHint.None, "")]
	private AnimationPlayer displayAnimationPlayer;

	[Export(PropertyHint.None, "")]
	private AnimatedSprite2D animatedSprite;

	private bool fullscreen;

	[Export(PropertyHint.None, "")]
	private float maxScreenPercent = 0.9f;

	[Export(PropertyHint.None, "")]
	private float baseScale = 4f;

	private bool _frameChangedConnected;

	public void OpenGalleryPiece(GalleryPieceDataRes pieceData)
	{
		targetDisplay.TextureNormal = pieceData.pieceTexture;
		targetDiscription.Text = pieceData.pieceDescriptor;
	}

	public void OpenGalleryPiece(GalleryPieceDataRes pieceData, bool BlacklistedContent = false)
	{
		if (BlacklistedContent)
		{
			targetDisplay.TextureNormal = pieceData.altPieceTexture;
			if (pieceData.altpieceFrames != null)
			{
				animatedSprite.Visible = true;
				animatedSprite.SpriteFrames = pieceData.altpieceFrames;
				PlayAndClamp(animatedSprite);
				targetDisplay.TextureNormal = null;
			}
			targetDiscription.Text = pieceData.altPieceDescriptor;
		}
		else
		{
			targetDisplay.TextureNormal = pieceData.pieceTexture;
			if (pieceData.pieceFrames != null)
			{
				animatedSprite.Visible = true;
				animatedSprite.SpriteFrames = pieceData.pieceFrames;
				PlayAndClamp(animatedSprite);
				targetDisplay.TextureNormal = null;
			}
			targetDiscription.Text = pieceData.pieceDescriptor;
		}
		if (!(targetDiscription.Text == "EMPTY DESCRIPTOR") && !(targetDiscription.Text == ""))
		{
			return;
		}
		foreach (Node child in targetDiscription.GetParent().GetChildren())
		{
			child.QueueFree();
		}
	}

	private void PlayAndClamp(AnimatedSprite2D sprite)
	{
		string text = sprite.SpriteFrames.GetAnimationNames()[0];
		sprite.Play(text);
		ClampSpriteScale(sprite);
		if (!_frameChangedConnected)
		{
			sprite.FrameChanged += OnAnimatedSpriteFrameChanged;
			_frameChangedConnected = true;
		}
	}

	private void OnAnimatedSpriteFrameChanged()
	{
		ClampSpriteScale(animatedSprite);
	}

	private void ClampSpriteScale(AnimatedSprite2D sprite)
	{
		if (sprite.SpriteFrames == null || string.IsNullOrEmpty(sprite.Animation))
		{
			return;
		}
		Texture2D frameTexture = sprite.SpriteFrames.GetFrameTexture(sprite.Animation, sprite.Frame);
		if (frameTexture != null)
		{
			Vector2 size = frameTexture.GetSize();
			if (!(size.X <= 0f) && !(size.Y <= 0f))
			{
				Vector2 vector = base.Size * maxScreenPercent;
				float a = vector.X / size.X;
				float b = vector.Y / size.Y;
				float b2 = Mathf.Min(a, b);
				float num = Mathf.Min(baseScale, b2);
				sprite.Scale = new Vector2(num, num);
			}
		}
	}

	public override void _ExitTree()
	{
		if (_frameChangedConnected && GodotObject.IsInstanceValid(animatedSprite))
		{
			animatedSprite.FrameChanged -= OnAnimatedSpriteFrameChanged;
			_frameChangedConnected = false;
		}
	}

	public void FullScreenGalleryPiece()
	{
		if (!displayAnimationPlayer.IsPlaying())
		{
			if (fullscreen)
			{
				displayAnimationPlayer.PlayBackwards("Fullscreen");
				fullscreen = false;
			}
			else
			{
				displayAnimationPlayer.Play("Fullscreen");
				fullscreen = true;
			}
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(7)
		{
			new MethodInfo(MethodName.OpenGalleryPiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "pieceData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.OpenGalleryPiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "pieceData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Bool, "BlacklistedContent", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PlayAndClamp, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "sprite", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("AnimatedSprite2D"), exported: false)
			}, null),
			new MethodInfo(MethodName.OnAnimatedSpriteFrameChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ClampSpriteScale, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "sprite", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("AnimatedSprite2D"), exported: false)
			}, null),
			new MethodInfo(MethodName._ExitTree, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.FullScreenGalleryPiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.OpenGalleryPiece && args.Count == 1)
		{
			OpenGalleryPiece(VariantUtils.ConvertTo<GalleryPieceDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OpenGalleryPiece && args.Count == 2)
		{
			OpenGalleryPiece(VariantUtils.ConvertTo<GalleryPieceDataRes>(in args[0]), VariantUtils.ConvertTo<bool>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlayAndClamp && args.Count == 1)
		{
			PlayAndClamp(VariantUtils.ConvertTo<AnimatedSprite2D>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnAnimatedSpriteFrameChanged && args.Count == 0)
		{
			OnAnimatedSpriteFrameChanged();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ClampSpriteScale && args.Count == 1)
		{
			ClampSpriteScale(VariantUtils.ConvertTo<AnimatedSprite2D>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._ExitTree && args.Count == 0)
		{
			_ExitTree();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FullScreenGalleryPiece && args.Count == 0)
		{
			FullScreenGalleryPiece();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.OpenGalleryPiece)
		{
			return true;
		}
		if (method == MethodName.PlayAndClamp)
		{
			return true;
		}
		if (method == MethodName.OnAnimatedSpriteFrameChanged)
		{
			return true;
		}
		if (method == MethodName.ClampSpriteScale)
		{
			return true;
		}
		if (method == MethodName._ExitTree)
		{
			return true;
		}
		if (method == MethodName.FullScreenGalleryPiece)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.parentWindow)
		{
			parentWindow = VariantUtils.ConvertTo<Window>(in value);
			return true;
		}
		if (name == PropertyName.targetDisplay)
		{
			targetDisplay = VariantUtils.ConvertTo<TextureButton>(in value);
			return true;
		}
		if (name == PropertyName.targetDiscription)
		{
			targetDiscription = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.displayAnimationPlayer)
		{
			displayAnimationPlayer = VariantUtils.ConvertTo<AnimationPlayer>(in value);
			return true;
		}
		if (name == PropertyName.animatedSprite)
		{
			animatedSprite = VariantUtils.ConvertTo<AnimatedSprite2D>(in value);
			return true;
		}
		if (name == PropertyName.fullscreen)
		{
			fullscreen = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.maxScreenPercent)
		{
			maxScreenPercent = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.baseScale)
		{
			baseScale = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName._frameChangedConnected)
		{
			_frameChangedConnected = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.parentWindow)
		{
			value = VariantUtils.CreateFrom(in parentWindow);
			return true;
		}
		if (name == PropertyName.targetDisplay)
		{
			value = VariantUtils.CreateFrom(in targetDisplay);
			return true;
		}
		if (name == PropertyName.targetDiscription)
		{
			value = VariantUtils.CreateFrom(in targetDiscription);
			return true;
		}
		if (name == PropertyName.displayAnimationPlayer)
		{
			value = VariantUtils.CreateFrom(in displayAnimationPlayer);
			return true;
		}
		if (name == PropertyName.animatedSprite)
		{
			value = VariantUtils.CreateFrom(in animatedSprite);
			return true;
		}
		if (name == PropertyName.fullscreen)
		{
			value = VariantUtils.CreateFrom(in fullscreen);
			return true;
		}
		if (name == PropertyName.maxScreenPercent)
		{
			value = VariantUtils.CreateFrom(in maxScreenPercent);
			return true;
		}
		if (name == PropertyName.baseScale)
		{
			value = VariantUtils.CreateFrom(in baseScale);
			return true;
		}
		if (name == PropertyName._frameChangedConnected)
		{
			value = VariantUtils.CreateFrom(in _frameChangedConnected);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.parentWindow, PropertyHint.NodeType, "Window", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.targetDisplay, PropertyHint.NodeType, "TextureButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.targetDiscription, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.displayAnimationPlayer, PropertyHint.NodeType, "AnimationPlayer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.animatedSprite, PropertyHint.NodeType, "AnimatedSprite2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.fullscreen, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.maxScreenPercent, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.baseScale, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName._frameChangedConnected, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.parentWindow, Variant.From(in parentWindow));
		info.AddProperty(PropertyName.targetDisplay, Variant.From(in targetDisplay));
		info.AddProperty(PropertyName.targetDiscription, Variant.From(in targetDiscription));
		info.AddProperty(PropertyName.displayAnimationPlayer, Variant.From(in displayAnimationPlayer));
		info.AddProperty(PropertyName.animatedSprite, Variant.From(in animatedSprite));
		info.AddProperty(PropertyName.fullscreen, Variant.From(in fullscreen));
		info.AddProperty(PropertyName.maxScreenPercent, Variant.From(in maxScreenPercent));
		info.AddProperty(PropertyName.baseScale, Variant.From(in baseScale));
		info.AddProperty(PropertyName._frameChangedConnected, Variant.From(in _frameChangedConnected));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.parentWindow, out var value))
		{
			parentWindow = value.As<Window>();
		}
		if (info.TryGetProperty(PropertyName.targetDisplay, out var value2))
		{
			targetDisplay = value2.As<TextureButton>();
		}
		if (info.TryGetProperty(PropertyName.targetDiscription, out var value3))
		{
			targetDiscription = value3.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.displayAnimationPlayer, out var value4))
		{
			displayAnimationPlayer = value4.As<AnimationPlayer>();
		}
		if (info.TryGetProperty(PropertyName.animatedSprite, out var value5))
		{
			animatedSprite = value5.As<AnimatedSprite2D>();
		}
		if (info.TryGetProperty(PropertyName.fullscreen, out var value6))
		{
			fullscreen = value6.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.maxScreenPercent, out var value7))
		{
			maxScreenPercent = value7.As<float>();
		}
		if (info.TryGetProperty(PropertyName.baseScale, out var value8))
		{
			baseScale = value8.As<float>();
		}
		if (info.TryGetProperty(PropertyName._frameChangedConnected, out var value9))
		{
			_frameChangedConnected = value9.As<bool>();
		}
	}
}
