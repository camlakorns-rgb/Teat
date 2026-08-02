using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/GalleryMenu/GalleryItem.cs")]
public class GalleryItem : TextureButton
{
	public new class MethodName : TextureButton.MethodName
	{
		public static readonly StringName AssemblePiece = "AssemblePiece";

		public static readonly StringName itemButtonPressed = "itemButtonPressed";
	}

	public new class PropertyName : TextureButton.PropertyName
	{
		public static readonly StringName galleryItemData = "galleryItemData";

		public static readonly StringName parent = "parent";
	}

	public new class SignalName : TextureButton.SignalName
	{
	}

	public GalleryDataRes galleryItemData;

	public GalleryHandler parent;

	public void AssemblePiece()
	{
		base.TextureNormal = galleryItemData.icon;
		base.Size = new Vector2(64f, 64f);
		base.CustomMinimumSize = new Vector2(64f, 64f);
	}

	public void itemButtonPressed()
	{
		parent.GallerySelected(galleryItemData);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(2)
		{
			new MethodInfo(MethodName.AssemblePiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.itemButtonPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.AssemblePiece && args.Count == 0)
		{
			AssemblePiece();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.itemButtonPressed && args.Count == 0)
		{
			itemButtonPressed();
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
		if (method == MethodName.itemButtonPressed)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.galleryItemData)
		{
			galleryItemData = VariantUtils.ConvertTo<GalleryDataRes>(in value);
			return true;
		}
		if (name == PropertyName.parent)
		{
			parent = VariantUtils.ConvertTo<GalleryHandler>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.galleryItemData)
		{
			value = VariantUtils.CreateFrom(in galleryItemData);
			return true;
		}
		if (name == PropertyName.parent)
		{
			value = VariantUtils.CreateFrom(in parent);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.galleryItemData, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.parent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.galleryItemData, Variant.From(in galleryItemData));
		info.AddProperty(PropertyName.parent, Variant.From(in parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.galleryItemData, out var value))
		{
			galleryItemData = value.As<GalleryDataRes>();
		}
		if (info.TryGetProperty(PropertyName.parent, out var value2))
		{
			parent = value2.As<GalleryHandler>();
		}
	}
}
