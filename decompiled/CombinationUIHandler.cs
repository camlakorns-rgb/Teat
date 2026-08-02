using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/RecipeBook/CombinationUIHandler.cs")]
public class CombinationUIHandler : NinePatchRect
{
	public new class MethodName : NinePatchRect.MethodName
	{
	}

	public new class PropertyName : NinePatchRect.PropertyName
	{
		public static readonly StringName item1Button = "item1Button";

		public static readonly StringName item1Texture = "item1Texture";

		public static readonly StringName item2Button = "item2Button";

		public static readonly StringName item2Texture = "item2Texture";

		public static readonly StringName item3Button = "item3Button";

		public static readonly StringName item3Texture = "item3Texture";
	}

	public new class SignalName : NinePatchRect.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public Button item1Button;

	[Export(PropertyHint.None, "")]
	public TextureRect item1Texture;

	[Export(PropertyHint.None, "")]
	public Button item2Button;

	[Export(PropertyHint.None, "")]
	public TextureRect item2Texture;

	[Export(PropertyHint.None, "")]
	public Button item3Button;

	[Export(PropertyHint.None, "")]
	public TextureRect item3Texture;

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.item1Button)
		{
			item1Button = VariantUtils.ConvertTo<Button>(in value);
			return true;
		}
		if (name == PropertyName.item1Texture)
		{
			item1Texture = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName.item2Button)
		{
			item2Button = VariantUtils.ConvertTo<Button>(in value);
			return true;
		}
		if (name == PropertyName.item2Texture)
		{
			item2Texture = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName.item3Button)
		{
			item3Button = VariantUtils.ConvertTo<Button>(in value);
			return true;
		}
		if (name == PropertyName.item3Texture)
		{
			item3Texture = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.item1Button)
		{
			value = VariantUtils.CreateFrom(in item1Button);
			return true;
		}
		if (name == PropertyName.item1Texture)
		{
			value = VariantUtils.CreateFrom(in item1Texture);
			return true;
		}
		if (name == PropertyName.item2Button)
		{
			value = VariantUtils.CreateFrom(in item2Button);
			return true;
		}
		if (name == PropertyName.item2Texture)
		{
			value = VariantUtils.CreateFrom(in item2Texture);
			return true;
		}
		if (name == PropertyName.item3Button)
		{
			value = VariantUtils.CreateFrom(in item3Button);
			return true;
		}
		if (name == PropertyName.item3Texture)
		{
			value = VariantUtils.CreateFrom(in item3Texture);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.item1Button, PropertyHint.NodeType, "Button", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.item1Texture, PropertyHint.NodeType, "TextureRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.item2Button, PropertyHint.NodeType, "Button", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.item2Texture, PropertyHint.NodeType, "TextureRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.item3Button, PropertyHint.NodeType, "Button", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.item3Texture, PropertyHint.NodeType, "TextureRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.item1Button, Variant.From(in item1Button));
		info.AddProperty(PropertyName.item1Texture, Variant.From(in item1Texture));
		info.AddProperty(PropertyName.item2Button, Variant.From(in item2Button));
		info.AddProperty(PropertyName.item2Texture, Variant.From(in item2Texture));
		info.AddProperty(PropertyName.item3Button, Variant.From(in item3Button));
		info.AddProperty(PropertyName.item3Texture, Variant.From(in item3Texture));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.item1Button, out var value))
		{
			item1Button = value.As<Button>();
		}
		if (info.TryGetProperty(PropertyName.item1Texture, out var value2))
		{
			item1Texture = value2.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName.item2Button, out var value3))
		{
			item2Button = value3.As<Button>();
		}
		if (info.TryGetProperty(PropertyName.item2Texture, out var value4))
		{
			item2Texture = value4.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName.item3Button, out var value5))
		{
			item3Button = value5.As<Button>();
		}
		if (info.TryGetProperty(PropertyName.item3Texture, out var value6))
		{
			item3Texture = value6.As<TextureRect>();
		}
	}
}
