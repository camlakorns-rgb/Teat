using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/ItemScripts/ItemObjectHandler.cs")]
public class ItemObjectHandler : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName SetupItem = "SetupItem";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName itemInformation = "itemInformation";

		public static readonly StringName spriteParentController = "spriteParentController";

		public static readonly StringName trueSize = "trueSize";
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public ItemDataRes itemInformation;

	[Export(PropertyHint.None, "")]
	public ItemSpriteController spriteParentController;

	public Vector2I trueSize;

	public void SetupItem()
	{
		spriteParentController.setupItemSprites(this);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(MethodName.SetupItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.SetupItem && args.Count == 0)
		{
			SetupItem();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.SetupItem)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.itemInformation)
		{
			itemInformation = VariantUtils.ConvertTo<ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName.spriteParentController)
		{
			spriteParentController = VariantUtils.ConvertTo<ItemSpriteController>(in value);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			trueSize = VariantUtils.ConvertTo<Vector2I>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.itemInformation)
		{
			value = VariantUtils.CreateFrom(in itemInformation);
			return true;
		}
		if (name == PropertyName.spriteParentController)
		{
			value = VariantUtils.CreateFrom(in spriteParentController);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			value = VariantUtils.CreateFrom(in trueSize);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.itemInformation, PropertyHint.ResourceType, "ItemDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spriteParentController, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.trueSize, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.itemInformation, Variant.From(in itemInformation));
		info.AddProperty(PropertyName.spriteParentController, Variant.From(in spriteParentController));
		info.AddProperty(PropertyName.trueSize, Variant.From(in trueSize));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.itemInformation, out var value))
		{
			itemInformation = value.As<ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName.spriteParentController, out var value2))
		{
			spriteParentController = value2.As<ItemSpriteController>();
		}
		if (info.TryGetProperty(PropertyName.trueSize, out var value3))
		{
			trueSize = value3.As<Vector2I>();
		}
	}
}
