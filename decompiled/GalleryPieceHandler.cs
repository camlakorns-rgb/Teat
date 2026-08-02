using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/GalleryMenu/GalleryPieceHandler.cs")]
public class GalleryPieceHandler : Button
{
	public new class MethodName : Button.MethodName
	{
		public static readonly StringName AssemblePiece = "AssemblePiece";

		public static readonly StringName RebuildIcon = "RebuildIcon";

		public static readonly StringName OnFramePostDraw = "OnFramePostDraw";

		public static readonly StringName ButtonHit = "ButtonHit";
	}

	public new class PropertyName : Button.PropertyName
	{
		public static readonly StringName IconImage = "IconImage";

		public static readonly StringName LockImage = "LockImage";

		public static readonly StringName parent = "parent";

		public static readonly StringName pieceData = "pieceData";
	}

	public new class SignalName : Button.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private TextureRect IconImage;

	[Export(PropertyHint.None, "")]
	private TextureRect LockImage;

	public GalleryHandler parent;

	public GalleryPieceDataRes pieceData;

	public void AssemblePiece(bool Locked = false)
	{
		if (pieceData.icon == null)
		{
			if (pieceData.pieceFrames != null)
			{
				IconImage.Texture = pieceData.pieceFrames.GetFrameTexture(pieceData.pieceFrames.GetAnimationNames()[0], pieceData.pieceFrames.GetFrameCount(pieceData.pieceFrames.GetAnimationNames()[0]) / 2);
			}
			else
			{
				IconImage.Texture = pieceData.pieceTexture;
			}
		}
		else
		{
			IconImage.Texture = pieceData.icon;
		}
		LockImage.Visible = Locked;
		base.Disabled = Locked;
		RebuildIcon();
	}

	private void RebuildIcon()
	{
		RenderingServer.FramePostDraw += OnFramePostDraw;
	}

	private void OnFramePostDraw()
	{
		RenderingServer.FramePostDraw -= OnFramePostDraw;
		IconImage.PivotOffset = IconImage.Size / 2f;
		IconImage.Scale *= 0.9f;
	}

	public void ButtonHit()
	{
		parent.ShowGalleryPiece(pieceData);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(4)
		{
			new MethodInfo(MethodName.AssemblePiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "Locked", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.RebuildIcon, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnFramePostDraw, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ButtonHit, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.AssemblePiece && args.Count == 1)
		{
			AssemblePiece(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RebuildIcon && args.Count == 0)
		{
			RebuildIcon();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnFramePostDraw && args.Count == 0)
		{
			OnFramePostDraw();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ButtonHit && args.Count == 0)
		{
			ButtonHit();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.AssemblePiece)
		{
			return true;
		}
		if (method == MethodName.RebuildIcon)
		{
			return true;
		}
		if (method == MethodName.OnFramePostDraw)
		{
			return true;
		}
		if (method == MethodName.ButtonHit)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.IconImage)
		{
			IconImage = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName.LockImage)
		{
			LockImage = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName.parent)
		{
			parent = VariantUtils.ConvertTo<GalleryHandler>(in value);
			return true;
		}
		if (name == PropertyName.pieceData)
		{
			pieceData = VariantUtils.ConvertTo<GalleryPieceDataRes>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.IconImage)
		{
			value = VariantUtils.CreateFrom(in IconImage);
			return true;
		}
		if (name == PropertyName.LockImage)
		{
			value = VariantUtils.CreateFrom(in LockImage);
			return true;
		}
		if (name == PropertyName.parent)
		{
			value = VariantUtils.CreateFrom(in parent);
			return true;
		}
		if (name == PropertyName.pieceData)
		{
			value = VariantUtils.CreateFrom(in pieceData);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.IconImage, PropertyHint.NodeType, "TextureRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.LockImage, PropertyHint.NodeType, "TextureRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.parent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.pieceData, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.IconImage, Variant.From(in IconImage));
		info.AddProperty(PropertyName.LockImage, Variant.From(in LockImage));
		info.AddProperty(PropertyName.parent, Variant.From(in parent));
		info.AddProperty(PropertyName.pieceData, Variant.From(in pieceData));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.IconImage, out var value))
		{
			IconImage = value.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName.LockImage, out var value2))
		{
			LockImage = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName.parent, out var value3))
		{
			parent = value3.As<GalleryHandler>();
		}
		if (info.TryGetProperty(PropertyName.pieceData, out var value4))
		{
			pieceData = value4.As<GalleryPieceDataRes>();
		}
	}
}
