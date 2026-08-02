using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/GalleryMenu/GalleryHandler.cs")]
public class GalleryHandler : Control
{
	public new class MethodName : Control.MethodName
	{
		public static readonly StringName SetupGallery = "SetupGallery";

		public static readonly StringName GallerySelected = "GallerySelected";

		public static readonly StringName ShowGalleryPiece = "ShowGalleryPiece";

		public static readonly StringName CloseGallery = "CloseGallery";
	}

	public new class PropertyName : Control.PropertyName
	{
		public static readonly StringName parent = "parent";

		public static readonly StringName galleryItemContainer = "galleryItemContainer";

		public static readonly StringName galleryPieceContainer = "galleryPieceContainer";

		public static readonly StringName displayPieceWindow = "displayPieceWindow";

		public static readonly StringName windowHolder = "windowHolder";

		public static readonly StringName LockedButton = "LockedButton";

		public static readonly StringName spawnedGallery = "spawnedGallery";

		public static readonly StringName itemScene = "itemScene";

		public static readonly StringName pieceScene = "pieceScene";
	}

	public new class SignalName : Control.SignalName
	{
	}

	[ExportGroup("Dependencies", "")]
	[Export(PropertyHint.None, "")]
	private GalleryWindow parent;

	[Export(PropertyHint.None, "")]
	private GridContainer galleryItemContainer;

	[Export(PropertyHint.None, "")]
	private GridContainer galleryPieceContainer;

	[Export(PropertyHint.None, "")]
	private PackedScene displayPieceWindow;

	[Export(PropertyHint.None, "")]
	private Node windowHolder;

	[Export(PropertyHint.None, "")]
	private Texture2D LockedButton;

	public GalleryDisplay spawnedGallery;

	private string itemScene = "uid://c6sylfhejll7f";

	private string pieceScene = "uid://dn4yndej5dgoe";

	public void SetupGallery()
	{
		foreach (Control child in galleryItemContainer.GetChildren())
		{
			child.QueueFree();
		}
		foreach (GalleryDataRes item in from GalleryDataRes x in ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.GALLERY].Values
			orderby x.galleryOrder
			select x)
		{
			GalleryItem galleryItem = GD.Load<PackedScene>(itemScene).Instantiate<GalleryItem>(PackedScene.GenEditState.Disabled);
			galleryItem.galleryItemData = item;
			galleryItem.parent = this;
			galleryItem.AssemblePiece();
			galleryItemContainer.AddChild(galleryItem, forceReadableName: false, InternalMode.Disabled);
		}
		foreach (Control child2 in galleryPieceContainer.GetChildren())
		{
			child2.QueueFree();
		}
		base.Visible = true;
	}

	public void GallerySelected(GalleryDataRes galleryData)
	{
		foreach (Control child in galleryPieceContainer.GetChildren())
		{
			child.QueueFree();
		}
		if (galleryData.itemCGs.Count() == 0)
		{
			GD.PrintErr("Item [" + galleryData.Name + "] has no CGs!!! This is a massive error as only CG Items should be displayed here");
			return;
		}
		Array<GalleryPieceDataRes> array = galleryData.itemCGs.Duplicate();
		foreach (GalleryPieceDataRes itemCG in galleryData.itemCGs)
		{
			if (Main.Instance.IsBlacklisted(itemCG.taggedKinks) && itemCG.altPieceTexture == null)
			{
				array.Remove(itemCG);
			}
		}
		Vector2 vector = Vector2.Zero;
		int num = array.Count();
		switch (num)
		{
		case 1:
			galleryPieceContainer.Columns = 1;
			vector = new Vector2(428f, 428f);
			break;
		case 2:
			galleryPieceContainer.Columns = 2;
			vector = new Vector2(256f, 256f);
			break;
		case 3:
		case 4:
		case 5:
		case 6:
		case 7:
		case 8:
			galleryPieceContainer.Columns = 4;
			vector = new Vector2(160f, 160f);
			break;
		default:
			if (num > 8)
			{
				galleryPieceContainer.Columns = 5;
				vector = new Vector2(128f, 128f);
			}
			else
			{
				GD.PrintErr("Size Set Failed");
			}
			break;
		}
		foreach (GalleryPieceDataRes item in array)
		{
			GalleryPieceHandler galleryPieceHandler = GD.Load<PackedScene>(pieceScene).Instantiate<GalleryPieceHandler>(PackedScene.GenEditState.Disabled);
			galleryPieceHandler.Size = vector;
			galleryPieceHandler.CustomMinimumSize = vector;
			galleryPieceHandler.pieceData = item;
			galleryPieceHandler.parent = this;
			if (item.requiredKey != "NO KEY" && item.requiredKey != "")
			{
				bool flag = false;
				foreach (KeyValuePair<SaveHandler.SeenObjectTypes, Array<string>> seenObject in Main.Instance.SeenObjects)
				{
					foreach (string item2 in seenObject.Value)
					{
						if (item2 == item.requiredKey)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					galleryPieceHandler.AssemblePiece(Locked: true);
				}
				else
				{
					galleryPieceHandler.AssemblePiece();
				}
			}
			else
			{
				galleryPieceHandler.AssemblePiece();
			}
			galleryPieceContainer.AddChild(galleryPieceHandler, forceReadableName: false, InternalMode.Disabled);
		}
	}

	public void ShowGalleryPiece(GalleryPieceDataRes pieceData)
	{
		GalleryDisplay galleryDisplay = displayPieceWindow.Instantiate<GalleryDisplay>(PackedScene.GenEditState.Disabled);
		windowHolder.AddChild(galleryDisplay, forceReadableName: false, InternalMode.Disabled);
		spawnedGallery = galleryDisplay;
		galleryDisplay.OpenGalleryPiece(pieceData);
		galleryDisplay.parent = this;
	}

	public void CloseGallery()
	{
		base.Visible = false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(4)
		{
			new MethodInfo(MethodName.SetupGallery, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GallerySelected, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "galleryData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.ShowGalleryPiece, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "pieceData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.CloseGallery, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.SetupGallery && args.Count == 0)
		{
			SetupGallery();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GallerySelected && args.Count == 1)
		{
			GallerySelected(VariantUtils.ConvertTo<GalleryDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ShowGalleryPiece && args.Count == 1)
		{
			ShowGalleryPiece(VariantUtils.ConvertTo<GalleryPieceDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CloseGallery && args.Count == 0)
		{
			CloseGallery();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.SetupGallery)
		{
			return true;
		}
		if (method == MethodName.GallerySelected)
		{
			return true;
		}
		if (method == MethodName.ShowGalleryPiece)
		{
			return true;
		}
		if (method == MethodName.CloseGallery)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.parent)
		{
			parent = VariantUtils.ConvertTo<GalleryWindow>(in value);
			return true;
		}
		if (name == PropertyName.galleryItemContainer)
		{
			galleryItemContainer = VariantUtils.ConvertTo<GridContainer>(in value);
			return true;
		}
		if (name == PropertyName.galleryPieceContainer)
		{
			galleryPieceContainer = VariantUtils.ConvertTo<GridContainer>(in value);
			return true;
		}
		if (name == PropertyName.displayPieceWindow)
		{
			displayPieceWindow = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.windowHolder)
		{
			windowHolder = VariantUtils.ConvertTo<Node>(in value);
			return true;
		}
		if (name == PropertyName.LockedButton)
		{
			LockedButton = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.spawnedGallery)
		{
			spawnedGallery = VariantUtils.ConvertTo<GalleryDisplay>(in value);
			return true;
		}
		if (name == PropertyName.itemScene)
		{
			itemScene = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.pieceScene)
		{
			pieceScene = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.parent)
		{
			value = VariantUtils.CreateFrom(in parent);
			return true;
		}
		if (name == PropertyName.galleryItemContainer)
		{
			value = VariantUtils.CreateFrom(in galleryItemContainer);
			return true;
		}
		if (name == PropertyName.galleryPieceContainer)
		{
			value = VariantUtils.CreateFrom(in galleryPieceContainer);
			return true;
		}
		if (name == PropertyName.displayPieceWindow)
		{
			value = VariantUtils.CreateFrom(in displayPieceWindow);
			return true;
		}
		if (name == PropertyName.windowHolder)
		{
			value = VariantUtils.CreateFrom(in windowHolder);
			return true;
		}
		if (name == PropertyName.LockedButton)
		{
			value = VariantUtils.CreateFrom(in LockedButton);
			return true;
		}
		if (name == PropertyName.spawnedGallery)
		{
			value = VariantUtils.CreateFrom(in spawnedGallery);
			return true;
		}
		if (name == PropertyName.itemScene)
		{
			value = VariantUtils.CreateFrom(in itemScene);
			return true;
		}
		if (name == PropertyName.pieceScene)
		{
			value = VariantUtils.CreateFrom(in pieceScene);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Nil, "Dependencies", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.parent, PropertyHint.NodeType, "Window", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.galleryItemContainer, PropertyHint.NodeType, "GridContainer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.galleryPieceContainer, PropertyHint.NodeType, "GridContainer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.displayPieceWindow, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.windowHolder, PropertyHint.NodeType, "Node", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.LockedButton, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawnedGallery, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.itemScene, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.pieceScene, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.parent, Variant.From(in parent));
		info.AddProperty(PropertyName.galleryItemContainer, Variant.From(in galleryItemContainer));
		info.AddProperty(PropertyName.galleryPieceContainer, Variant.From(in galleryPieceContainer));
		info.AddProperty(PropertyName.displayPieceWindow, Variant.From(in displayPieceWindow));
		info.AddProperty(PropertyName.windowHolder, Variant.From(in windowHolder));
		info.AddProperty(PropertyName.LockedButton, Variant.From(in LockedButton));
		info.AddProperty(PropertyName.spawnedGallery, Variant.From(in spawnedGallery));
		info.AddProperty(PropertyName.itemScene, Variant.From(in itemScene));
		info.AddProperty(PropertyName.pieceScene, Variant.From(in pieceScene));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.parent, out var value))
		{
			parent = value.As<GalleryWindow>();
		}
		if (info.TryGetProperty(PropertyName.galleryItemContainer, out var value2))
		{
			galleryItemContainer = value2.As<GridContainer>();
		}
		if (info.TryGetProperty(PropertyName.galleryPieceContainer, out var value3))
		{
			galleryPieceContainer = value3.As<GridContainer>();
		}
		if (info.TryGetProperty(PropertyName.displayPieceWindow, out var value4))
		{
			displayPieceWindow = value4.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.windowHolder, out var value5))
		{
			windowHolder = value5.As<Node>();
		}
		if (info.TryGetProperty(PropertyName.LockedButton, out var value6))
		{
			LockedButton = value6.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.spawnedGallery, out var value7))
		{
			spawnedGallery = value7.As<GalleryDisplay>();
		}
		if (info.TryGetProperty(PropertyName.itemScene, out var value8))
		{
			itemScene = value8.As<string>();
		}
		if (info.TryGetProperty(PropertyName.pieceScene, out var value9))
		{
			pieceScene = value9.As<string>();
		}
	}
}
