using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/RecipeBook/RecipeMenuHandler.cs")]
public class RecipeMenuHandler : Window
{
	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName RefreshSeenItems = "RefreshSeenItems";

		public static readonly StringName BuildCombinationCache = "BuildCombinationCache";

		public static readonly StringName PopulateItemGrid = "PopulateItemGrid";

		public static readonly StringName OnSearchChanged = "OnSearchChanged";

		public static readonly StringName SelectItem = "SelectItem";

		public static readonly StringName ClearDetailPanel = "ClearDetailPanel";

		public static readonly StringName UpdateItemName = "UpdateItemName";

		public static readonly StringName UpdateItemSprite = "UpdateItemSprite";

		public static readonly StringName UpdateItemDescription = "UpdateItemDescription";

		public static readonly StringName UpdateActorWants = "UpdateActorWants";

		public static readonly StringName UpdateCombinations = "UpdateCombinations";

		public static readonly StringName SetupCombinationSlot = "SetupCombinationSlot";

		public static readonly StringName WireCombinationButton = "WireCombinationButton";

		public static readonly StringName IsItemSeen = "IsItemSeen";

		public static readonly StringName IsItemAllowed = "IsItemAllowed";

		public static readonly StringName IsActorAllowed = "IsActorAllowed";

		public static readonly StringName ClearGrid = "ClearGrid";

		public static readonly StringName GrabItemFrame = "GrabItemFrame";

		public static readonly StringName TintButtonIfMatch = "TintButtonIfMatch";

		public static readonly StringName OnCloseRequested = "OnCloseRequested";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName itemGridContainer = "itemGridContainer";

		public static readonly StringName itemButtonScene = "itemButtonScene";

		public static readonly StringName searchBox = "searchBox";

		public static readonly StringName itemNameLabel = "itemNameLabel";

		public static readonly StringName itemHolderTexture = "itemHolderTexture";

		public static readonly StringName itemDescriptionLabel = "itemDescriptionLabel";

		public static readonly StringName actorText = "actorText";

		public static readonly StringName actorGridContainer = "actorGridContainer";

		public static readonly StringName actorIconMap = "actorIconMap";

		public static readonly StringName combinationGridContainer = "combinationGridContainer";

		public static readonly StringName combinationUiScene = "combinationUiScene";

		public static readonly StringName unknownItemTexture = "unknownItemTexture";

		public static readonly StringName _selectedItem = "_selectedItem";

		public static readonly StringName _selectedGridButton = "_selectedGridButton";
	}

	public new class SignalName : Window.SignalName
	{
	}

	[ExportGroup("Item Grid", "")]
	[Export(PropertyHint.None, "")]
	public GridContainer itemGridContainer;

	[Export(PropertyHint.None, "")]
	public PackedScene itemButtonScene;

	[Export(PropertyHint.None, "")]
	public LineEdit searchBox;

	[ExportGroup("Item Detail Panel", "")]
	[Export(PropertyHint.None, "")]
	public RichTextLabel itemNameLabel;

	[Export(PropertyHint.None, "")]
	public TextureRect itemHolderTexture;

	[Export(PropertyHint.None, "")]
	public RichTextLabel itemDescriptionLabel;

	[ExportGroup("Actor Wants Panel", "")]
	[Export(PropertyHint.None, "")]
	public RichTextLabel actorText;

	[Export(PropertyHint.None, "")]
	public GridContainer actorGridContainer;

	[Export(PropertyHint.None, "")]
	public Godot.Collections.Dictionary<string, Texture2D> actorIconMap = new Godot.Collections.Dictionary<string, Texture2D>();

	[ExportGroup("Combination Panel", "")]
	[Export(PropertyHint.None, "")]
	public GridContainer combinationGridContainer;

	[Export(PropertyHint.None, "")]
	public PackedScene combinationUiScene;

	[Export(PropertyHint.None, "")]
	public Texture2D unknownItemTexture;

	private ItemDataRes _selectedItem;

	private Button _selectedGridButton;

	private List<ItemDataRes> _seenItems = new List<ItemDataRes>();

	private List<ItemDataRes> _filteredItems = new List<ItemDataRes>();

	private System.Collections.Generic.Dictionary<string, List<(ItemDataRes source, CombinationDataRes combo)>> _combinationCache = new System.Collections.Generic.Dictionary<string, List<(ItemDataRes, CombinationDataRes)>>();

	public override void _Ready()
	{
		base.CloseRequested += OnCloseRequested;
		searchBox.TextChanged += OnSearchChanged;
		RefreshSeenItems();
		BuildCombinationCache();
		if (_filteredItems.Count > 0)
		{
			_selectedItem = _filteredItems[0];
		}
		PopulateItemGrid();
		if (_selectedItem != null)
		{
			SelectItem(_selectedItem);
		}
		else
		{
			ClearDetailPanel();
		}
	}

	private void RefreshSeenItems()
	{
		_seenItems.Clear();
		if (Main.Instance == null)
		{
			return;
		}
		foreach (string item2 in Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.ITEMS])
		{
			if (ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].TryGetValue(item2, out var value) && value is ItemDataRes item && IsItemAllowed(item))
			{
				_seenItems.Add(item);
			}
		}
		_filteredItems = new List<ItemDataRes>(_seenItems);
	}

	private void BuildCombinationCache()
	{
		_combinationCache.Clear();
		HashSet<(string, string, string)> hashSet = new HashSet<(string, string, string)>();
		foreach (KeyValuePair<string, Resource> item5 in ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM])
		{
			Resource value = item5.Value;
			ItemDataRes itemData = value as ItemDataRes;
			if (itemData == null)
			{
				continue;
			}
			foreach (CombinationDataRes possibleCombination in itemData.possibleCombinations)
			{
				CombinationDataRes combo = possibleCombination;
				if (combo.requiredItem == null || combo.outputItem == null)
				{
					continue;
				}
				string text = itemData.itemID ?? "";
				string text2 = combo.requiredItem?.itemID ?? "";
				string item = combo.outputItem?.itemID ?? "";
				string item2 = ((string.Compare(text, text2, StringComparison.Ordinal) <= 0) ? text : text2);
				string item3 = ((string.Compare(text, text2, StringComparison.Ordinal) <= 0) ? text2 : text);
				(string, string, string) item4 = (item2, item3, item);
				if (hashSet.Add(item4))
				{
					AddToCache(itemData.itemID);
					if (combo.requiredItem != null)
					{
						AddToCache(combo.requiredItem.itemID);
					}
					if (combo.outputItem != null)
					{
						AddToCache(combo.outputItem.itemID);
					}
				}
				void AddToCache(string id)
				{
					if (!_combinationCache.TryGetValue(id, out List<(ItemDataRes, CombinationDataRes)> value2))
					{
						value2 = new List<(ItemDataRes, CombinationDataRes)>();
						_combinationCache[id] = value2;
					}
					value2.Add((itemData, combo));
				}
			}
		}
	}

	private void PopulateItemGrid()
	{
		if (itemGridContainer == null || itemButtonScene == null)
		{
			return;
		}
		foreach (Node child in itemGridContainer.GetChildren())
		{
			child.QueueFree();
		}
		_selectedGridButton = null;
		foreach (ItemDataRes filteredItem in _filteredItems)
		{
			ItemDataRes captured = filteredItem;
			Button itemButton = itemButtonScene.Instantiate<Button>(PackedScene.GenEditState.Disabled);
			itemGridContainer.AddChild(itemButton, forceReadableName: false, InternalMode.Disabled);
			TextureRect nodeOrNull = itemButton.GetNodeOrNull<TextureRect>("TextureRect");
			if (nodeOrNull != null)
			{
				nodeOrNull.Texture = GrabItemFrame(captured);
			}
			itemButton.TooltipText = captured.Name;
			if (itemButton.GetNodeOrNull<Node>("FlagPivot/Flag") is CanvasItem canvasItem)
			{
				canvasItem.Visible = captured.taggedKinks.Count > 0;
			}
			if (_selectedItem != null && captured == _selectedItem)
			{
				_selectedGridButton = itemButton;
				TintButtonIfMatch(itemButton, captured, captured);
			}
			itemButton.Pressed += delegate
			{
				if (_selectedGridButton != null)
				{
					_selectedGridButton.RemoveThemeStyleboxOverride("normal");
				}
				_selectedGridButton = itemButton;
				TintButtonIfMatch(itemButton, captured, captured);
				SelectItem(captured);
			};
		}
	}

	private void OnSearchChanged(string newText)
	{
		string lower = newText.ToLower().Trim();
		_filteredItems = (string.IsNullOrEmpty(lower) ? new List<ItemDataRes>(_seenItems) : _seenItems.Where((ItemDataRes i) => i.Name.ToLower().Contains(lower)).ToList());
		PopulateItemGrid();
	}

	public void SelectItem(ItemDataRes item)
	{
		_selectedItem = item;
		UpdateItemName(item);
		UpdateItemSprite(item);
		UpdateItemDescription(item);
		UpdateActorWants(item);
		UpdateCombinations(item);
	}

	private void ClearDetailPanel()
	{
		if (itemNameLabel != null)
		{
			itemNameLabel.Text = "";
		}
		if (itemDescriptionLabel != null)
		{
			itemDescriptionLabel.Text = "";
		}
		if (itemHolderTexture != null)
		{
			itemHolderTexture.Visible = false;
		}
		ClearGrid(actorGridContainer);
		ClearGrid(combinationGridContainer);
	}

	private void UpdateItemName(ItemDataRes item)
	{
		if (itemNameLabel != null)
		{
			itemNameLabel.Text = Regex.Replace(item.Name, "(?<=[a-z])(?=[A-Z])", " ");
		}
	}

	private void UpdateItemSprite(ItemDataRes item)
	{
		if (itemHolderTexture != null)
		{
			Texture2D texture2D = GrabItemFrame(item);
			if (texture2D != null)
			{
				itemHolderTexture.Texture = texture2D;
				itemHolderTexture.Visible = true;
			}
			else
			{
				itemHolderTexture.Visible = false;
			}
		}
	}

	private void UpdateItemDescription(ItemDataRes item)
	{
		if (itemDescriptionLabel != null)
		{
			itemDescriptionLabel.Text = item.itemDescription ?? "";
		}
	}

	private void UpdateActorWants(ItemDataRes item)
	{
		if (actorGridContainer == null)
		{
			return;
		}
		ClearGrid(actorGridContainer);
		List<AiItemDataRes> list = item.possibleUsableAIs.Where((AiItemDataRes aiEntry) => aiEntry.targetActorsID != "" && IsActorAllowed(aiEntry.targetActorsID)).ToList();
		if (list.Count == 0)
		{
			actorText.Text = "No one else other than Byte wishes for this item";
			return;
		}
		actorText.Text = "Wanted by Others:";
		foreach (AiItemDataRes item2 in list)
		{
			if (actorIconMap.TryGetValue(item2.targetActorsID, out var value) && value != null)
			{
				TextureRect textureRect = new TextureRect();
				textureRect.Texture = value;
				textureRect.CustomMinimumSize = new Vector2(64f, 64f);
				textureRect.StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered;
				textureRect.TooltipText = item2.targetActorsID;
				actorGridContainer.AddChild(textureRect, forceReadableName: false, InternalMode.Disabled);
			}
		}
	}

	private void UpdateCombinations(ItemDataRes item)
	{
		if (combinationGridContainer == null || combinationUiScene == null)
		{
			return;
		}
		ClearGrid(combinationGridContainer);
		if (!_combinationCache.TryGetValue(item.itemID, out List<(ItemDataRes, CombinationDataRes)> value) || value.Count == 0)
		{
			return;
		}
		foreach (var (itemDataRes, combinationDataRes) in value)
		{
			if (IsItemAllowed(itemDataRes) && IsItemAllowed(combinationDataRes.requiredItem) && IsItemAllowed(combinationDataRes.outputItem))
			{
				CombinationUIHandler combinationUIHandler = combinationUiScene.Instantiate<CombinationUIHandler>(PackedScene.GenEditState.Disabled);
				combinationGridContainer.AddChild(combinationUIHandler, forceReadableName: false, InternalMode.Disabled);
				bool alwaysSeen = IsItemSeen(itemDataRes);
				bool alwaysSeen2 = IsItemSeen(combinationDataRes.requiredItem);
				bool alwaysSeen3 = IsItemSeen(combinationDataRes.outputItem);
				TintButtonIfMatch(combinationUIHandler.item1Button, itemDataRes, item);
				TintButtonIfMatch(combinationUIHandler.item2Button, combinationDataRes.requiredItem, item);
				TintButtonIfMatch(combinationUIHandler.item3Button, combinationDataRes.outputItem, item);
				SetupCombinationSlot(combinationUIHandler.item1Button, combinationUIHandler.item1Texture, itemDataRes, alwaysSeen);
				SetupCombinationSlot(combinationUIHandler.item2Button, combinationUIHandler.item2Texture, combinationDataRes.requiredItem, alwaysSeen2);
				SetupCombinationSlot(combinationUIHandler.item3Button, combinationUIHandler.item3Texture, combinationDataRes.outputItem, alwaysSeen3);
				WireCombinationButton(combinationUIHandler.item1Button, itemDataRes, alwaysSeen);
				WireCombinationButton(combinationUIHandler.item2Button, combinationDataRes.requiredItem, alwaysSeen2);
				WireCombinationButton(combinationUIHandler.item3Button, combinationDataRes.outputItem, alwaysSeen3);
			}
		}
	}

	private void SetupCombinationSlot(Button button, TextureRect iconRect, ItemDataRes itemData, bool alwaysSeen)
	{
		if (button == null)
		{
			return;
		}
		if (!alwaysSeen || itemData == null)
		{
			button.Disabled = true;
			if (iconRect != null)
			{
				iconRect.Texture = unknownItemTexture;
			}
		}
		else
		{
			button.Disabled = false;
			if (iconRect != null)
			{
				iconRect.Texture = GrabItemFrame(itemData);
			}
		}
	}

	private void WireCombinationButton(Button button, ItemDataRes itemData, bool alwaysSeen)
	{
		if (button != null && itemData != null && alwaysSeen)
		{
			ItemDataRes captured = itemData;
			button.Pressed += delegate
			{
				SelectItem(captured);
			};
		}
	}

	private bool IsItemSeen(ItemDataRes item)
	{
		if (item == null || Main.Instance == null)
		{
			return false;
		}
		if (!IsItemAllowed(item))
		{
			return false;
		}
		return Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.ITEMS].Contains(item.itemID);
	}

	private bool IsItemAllowed(ItemDataRes item)
	{
		if (item == null || Main.Instance == null)
		{
			return false;
		}
		foreach (SaveHandler.Kinks taggedKink in item.taggedKinks)
		{
			if (Main.Instance.settingBlacklistedContent.Contains(taggedKink))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsActorAllowed(CharacterInfoDataRes actor)
	{
		if (actor == null || Main.Instance == null)
		{
			return false;
		}
		foreach (SaveHandler.Kinks taggedKink in actor.taggedKinks)
		{
			if (Main.Instance.settingBlacklistedContent.Contains(taggedKink))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsActorAllowed(string actorID)
	{
		if (string.IsNullOrEmpty(actorID) || Main.Instance == null)
		{
			return false;
		}
		if (!ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER].TryGetValue(actorID, out var value) || !(value is CharacterInfoDataRes actor))
		{
			return false;
		}
		return IsActorAllowed(actor);
	}

	private static void ClearGrid(GridContainer grid)
	{
		if (grid == null)
		{
			return;
		}
		foreach (Node child in grid.GetChildren())
		{
			child.QueueFree();
		}
	}

	private Texture2D GrabItemFrame(ItemDataRes item)
	{
		if (item?.ItemAnimations == null || item.ItemAnimations.Count == 0)
		{
			return null;
		}
		SpriteFrames spriteFrames = item.ItemAnimations[0];
		if (spriteFrames == null)
		{
			return null;
		}
		string[] animationNames = spriteFrames.GetAnimationNames();
		if (animationNames.Length == 0)
		{
			return null;
		}
		string text = animationNames[0];
		int frameCount = spriteFrames.GetFrameCount(text);
		if (frameCount == 0)
		{
			return null;
		}
		return spriteFrames.GetFrameTexture(text, frameCount / 2);
	}

	private void TintButtonIfMatch(Button itemButton, ItemDataRes btnItem, ItemDataRes target)
	{
		if (itemButton != null && btnItem == target)
		{
			StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
			styleBoxFlat.BgColor = new Color(1f, 1f, 1f, 0.15f);
			itemButton.AddThemeStyleboxOverride("normal", styleBoxFlat);
		}
	}

	private void OnCloseRequested()
	{
		QueueFree();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(21)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RefreshSeenItems, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BuildCombinationCache, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.PopulateItemGrid, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnSearchChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "newText", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SelectItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.ClearDetailPanel, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateItemName, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateItemSprite, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateItemDescription, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateActorWants, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateCombinations, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.SetupCombinationSlot, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "button", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Button"), exported: false),
				new PropertyInfo(Variant.Type.Object, "iconRect", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("TextureRect"), exported: false),
				new PropertyInfo(Variant.Type.Object, "itemData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Bool, "alwaysSeen", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.WireCombinationButton, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "button", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Button"), exported: false),
				new PropertyInfo(Variant.Type.Object, "itemData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Bool, "alwaysSeen", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.IsItemSeen, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.IsItemAllowed, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.IsActorAllowed, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "actor", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.ClearGrid, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "grid", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("GridContainer"), exported: false)
			}, null),
			new MethodInfo(MethodName.GrabItemFrame, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Texture2D"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.TintButtonIfMatch, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "itemButton", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Button"), exported: false),
				new PropertyInfo(Variant.Type.Object, "btnItem", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Object, "target", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.OnCloseRequested, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.RefreshSeenItems && args.Count == 0)
		{
			RefreshSeenItems();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BuildCombinationCache && args.Count == 0)
		{
			BuildCombinationCache();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PopulateItemGrid && args.Count == 0)
		{
			PopulateItemGrid();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnSearchChanged && args.Count == 1)
		{
			OnSearchChanged(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SelectItem && args.Count == 1)
		{
			SelectItem(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ClearDetailPanel && args.Count == 0)
		{
			ClearDetailPanel();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateItemName && args.Count == 1)
		{
			UpdateItemName(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateItemSprite && args.Count == 1)
		{
			UpdateItemSprite(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateItemDescription && args.Count == 1)
		{
			UpdateItemDescription(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateActorWants && args.Count == 1)
		{
			UpdateActorWants(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateCombinations && args.Count == 1)
		{
			UpdateCombinations(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupCombinationSlot && args.Count == 4)
		{
			SetupCombinationSlot(VariantUtils.ConvertTo<Button>(in args[0]), VariantUtils.ConvertTo<TextureRect>(in args[1]), VariantUtils.ConvertTo<ItemDataRes>(in args[2]), VariantUtils.ConvertTo<bool>(in args[3]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.WireCombinationButton && args.Count == 3)
		{
			WireCombinationButton(VariantUtils.ConvertTo<Button>(in args[0]), VariantUtils.ConvertTo<ItemDataRes>(in args[1]), VariantUtils.ConvertTo<bool>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.IsItemSeen && args.Count == 1)
		{
			bool from = IsItemSeen(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.IsItemAllowed && args.Count == 1)
		{
			bool from2 = IsItemAllowed(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.IsActorAllowed && args.Count == 1)
		{
			bool from3 = IsActorAllowed(VariantUtils.ConvertTo<CharacterInfoDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.ClearGrid && args.Count == 1)
		{
			ClearGrid(VariantUtils.ConvertTo<GridContainer>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GrabItemFrame && args.Count == 1)
		{
			Texture2D from4 = GrabItemFrame(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.TintButtonIfMatch && args.Count == 3)
		{
			TintButtonIfMatch(VariantUtils.ConvertTo<Button>(in args[0]), VariantUtils.ConvertTo<ItemDataRes>(in args[1]), VariantUtils.ConvertTo<ItemDataRes>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnCloseRequested && args.Count == 0)
		{
			OnCloseRequested();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.ClearGrid && args.Count == 1)
		{
			ClearGrid(VariantUtils.ConvertTo<GridContainer>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName._Ready)
		{
			return true;
		}
		if (method == MethodName.RefreshSeenItems)
		{
			return true;
		}
		if (method == MethodName.BuildCombinationCache)
		{
			return true;
		}
		if (method == MethodName.PopulateItemGrid)
		{
			return true;
		}
		if (method == MethodName.OnSearchChanged)
		{
			return true;
		}
		if (method == MethodName.SelectItem)
		{
			return true;
		}
		if (method == MethodName.ClearDetailPanel)
		{
			return true;
		}
		if (method == MethodName.UpdateItemName)
		{
			return true;
		}
		if (method == MethodName.UpdateItemSprite)
		{
			return true;
		}
		if (method == MethodName.UpdateItemDescription)
		{
			return true;
		}
		if (method == MethodName.UpdateActorWants)
		{
			return true;
		}
		if (method == MethodName.UpdateCombinations)
		{
			return true;
		}
		if (method == MethodName.SetupCombinationSlot)
		{
			return true;
		}
		if (method == MethodName.WireCombinationButton)
		{
			return true;
		}
		if (method == MethodName.IsItemSeen)
		{
			return true;
		}
		if (method == MethodName.IsItemAllowed)
		{
			return true;
		}
		if (method == MethodName.IsActorAllowed)
		{
			return true;
		}
		if (method == MethodName.ClearGrid)
		{
			return true;
		}
		if (method == MethodName.GrabItemFrame)
		{
			return true;
		}
		if (method == MethodName.TintButtonIfMatch)
		{
			return true;
		}
		if (method == MethodName.OnCloseRequested)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.itemGridContainer)
		{
			itemGridContainer = VariantUtils.ConvertTo<GridContainer>(in value);
			return true;
		}
		if (name == PropertyName.itemButtonScene)
		{
			itemButtonScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.searchBox)
		{
			searchBox = VariantUtils.ConvertTo<LineEdit>(in value);
			return true;
		}
		if (name == PropertyName.itemNameLabel)
		{
			itemNameLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.itemHolderTexture)
		{
			itemHolderTexture = VariantUtils.ConvertTo<TextureRect>(in value);
			return true;
		}
		if (name == PropertyName.itemDescriptionLabel)
		{
			itemDescriptionLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.actorText)
		{
			actorText = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.actorGridContainer)
		{
			actorGridContainer = VariantUtils.ConvertTo<GridContainer>(in value);
			return true;
		}
		if (name == PropertyName.actorIconMap)
		{
			actorIconMap = VariantUtils.ConvertToDictionary<string, Texture2D>(in value);
			return true;
		}
		if (name == PropertyName.combinationGridContainer)
		{
			combinationGridContainer = VariantUtils.ConvertTo<GridContainer>(in value);
			return true;
		}
		if (name == PropertyName.combinationUiScene)
		{
			combinationUiScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.unknownItemTexture)
		{
			unknownItemTexture = VariantUtils.ConvertTo<Texture2D>(in value);
			return true;
		}
		if (name == PropertyName._selectedItem)
		{
			_selectedItem = VariantUtils.ConvertTo<ItemDataRes>(in value);
			return true;
		}
		if (name == PropertyName._selectedGridButton)
		{
			_selectedGridButton = VariantUtils.ConvertTo<Button>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.itemGridContainer)
		{
			value = VariantUtils.CreateFrom(in itemGridContainer);
			return true;
		}
		if (name == PropertyName.itemButtonScene)
		{
			value = VariantUtils.CreateFrom(in itemButtonScene);
			return true;
		}
		if (name == PropertyName.searchBox)
		{
			value = VariantUtils.CreateFrom(in searchBox);
			return true;
		}
		if (name == PropertyName.itemNameLabel)
		{
			value = VariantUtils.CreateFrom(in itemNameLabel);
			return true;
		}
		if (name == PropertyName.itemHolderTexture)
		{
			value = VariantUtils.CreateFrom(in itemHolderTexture);
			return true;
		}
		if (name == PropertyName.itemDescriptionLabel)
		{
			value = VariantUtils.CreateFrom(in itemDescriptionLabel);
			return true;
		}
		if (name == PropertyName.actorText)
		{
			value = VariantUtils.CreateFrom(in actorText);
			return true;
		}
		if (name == PropertyName.actorGridContainer)
		{
			value = VariantUtils.CreateFrom(in actorGridContainer);
			return true;
		}
		if (name == PropertyName.actorIconMap)
		{
			value = VariantUtils.CreateFromDictionary(actorIconMap);
			return true;
		}
		if (name == PropertyName.combinationGridContainer)
		{
			value = VariantUtils.CreateFrom(in combinationGridContainer);
			return true;
		}
		if (name == PropertyName.combinationUiScene)
		{
			value = VariantUtils.CreateFrom(in combinationUiScene);
			return true;
		}
		if (name == PropertyName.unknownItemTexture)
		{
			value = VariantUtils.CreateFrom(in unknownItemTexture);
			return true;
		}
		if (name == PropertyName._selectedItem)
		{
			value = VariantUtils.CreateFrom(in _selectedItem);
			return true;
		}
		if (name == PropertyName._selectedGridButton)
		{
			value = VariantUtils.CreateFrom(in _selectedGridButton);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Nil, "Item Grid", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.itemGridContainer, PropertyHint.NodeType, "GridContainer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.itemButtonScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.searchBox, PropertyHint.NodeType, "LineEdit", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Item Detail Panel", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.itemNameLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.itemHolderTexture, PropertyHint.NodeType, "TextureRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.itemDescriptionLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Actor Wants Panel", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.actorText, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.actorGridContainer, PropertyHint.NodeType, "GridContainer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.actorIconMap, PropertyHint.TypeString, "4/0:;24/17:Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Combination Panel", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.combinationGridContainer, PropertyHint.NodeType, "GridContainer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.combinationUiScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.unknownItemTexture, PropertyHint.ResourceType, "Texture2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName._selectedItem, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._selectedGridButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.itemGridContainer, Variant.From(in itemGridContainer));
		info.AddProperty(PropertyName.itemButtonScene, Variant.From(in itemButtonScene));
		info.AddProperty(PropertyName.searchBox, Variant.From(in searchBox));
		info.AddProperty(PropertyName.itemNameLabel, Variant.From(in itemNameLabel));
		info.AddProperty(PropertyName.itemHolderTexture, Variant.From(in itemHolderTexture));
		info.AddProperty(PropertyName.itemDescriptionLabel, Variant.From(in itemDescriptionLabel));
		info.AddProperty(PropertyName.actorText, Variant.From(in actorText));
		info.AddProperty(PropertyName.actorGridContainer, Variant.From(in actorGridContainer));
		info.AddProperty(PropertyName.actorIconMap, Variant.CreateFrom(actorIconMap));
		info.AddProperty(PropertyName.combinationGridContainer, Variant.From(in combinationGridContainer));
		info.AddProperty(PropertyName.combinationUiScene, Variant.From(in combinationUiScene));
		info.AddProperty(PropertyName.unknownItemTexture, Variant.From(in unknownItemTexture));
		info.AddProperty(PropertyName._selectedItem, Variant.From(in _selectedItem));
		info.AddProperty(PropertyName._selectedGridButton, Variant.From(in _selectedGridButton));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.itemGridContainer, out var value))
		{
			itemGridContainer = value.As<GridContainer>();
		}
		if (info.TryGetProperty(PropertyName.itemButtonScene, out var value2))
		{
			itemButtonScene = value2.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.searchBox, out var value3))
		{
			searchBox = value3.As<LineEdit>();
		}
		if (info.TryGetProperty(PropertyName.itemNameLabel, out var value4))
		{
			itemNameLabel = value4.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.itemHolderTexture, out var value5))
		{
			itemHolderTexture = value5.As<TextureRect>();
		}
		if (info.TryGetProperty(PropertyName.itemDescriptionLabel, out var value6))
		{
			itemDescriptionLabel = value6.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.actorText, out var value7))
		{
			actorText = value7.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.actorGridContainer, out var value8))
		{
			actorGridContainer = value8.As<GridContainer>();
		}
		if (info.TryGetProperty(PropertyName.actorIconMap, out var value9))
		{
			actorIconMap = value9.AsGodotDictionary<string, Texture2D>();
		}
		if (info.TryGetProperty(PropertyName.combinationGridContainer, out var value10))
		{
			combinationGridContainer = value10.As<GridContainer>();
		}
		if (info.TryGetProperty(PropertyName.combinationUiScene, out var value11))
		{
			combinationUiScene = value11.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.unknownItemTexture, out var value12))
		{
			unknownItemTexture = value12.As<Texture2D>();
		}
		if (info.TryGetProperty(PropertyName._selectedItem, out var value13))
		{
			_selectedItem = value13.As<ItemDataRes>();
		}
		if (info.TryGetProperty(PropertyName._selectedGridButton, out var value14))
		{
			_selectedGridButton = value14.As<Button>();
		}
	}
}
