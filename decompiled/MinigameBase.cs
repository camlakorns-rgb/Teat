using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[GlobalClass]
[ScriptPath("res://Scripts/Minigames/MinigameBase.cs")]
public class MinigameBase : Window
{
	[Signal]
	public delegate void MinigameReadyEventHandler();

	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName PauseGame = "PauseGame";

		public static readonly StringName GetAllDescendants = "GetAllDescendants";

		public static readonly StringName MatchUISizing = "MatchUISizing";

		public static readonly StringName SpawnByteItem = "SpawnByteItem";

		public static readonly StringName SpawnActor = "SpawnActor";

		public static readonly StringName SpawnDialogue = "SpawnDialogue";

		public static readonly StringName SpawnPopup = "SpawnPopup";

		public static readonly StringName SpawnRandomPopup = "SpawnRandomPopup";

		public static readonly StringName ApplyTag = "ApplyTag";

		public static readonly StringName SaveMinigame = "SaveMinigame";

		public static readonly StringName LoadMinigame = "LoadMinigame";

		public static readonly StringName CloseMinigame = "CloseMinigame";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName OverridePause = "OverridePause";

		public static readonly StringName storedModes = "storedModes";

		public static readonly StringName isDisabled = "isDisabled";
	}

	public new class SignalName : Window.SignalName
	{
		public static readonly StringName MinigameReady = "MinigameReady";
	}

	[Export(PropertyHint.None, "")]
	public bool OverridePause;

	private Godot.Collections.Dictionary<Node, ProcessModeEnum> storedModes = new Godot.Collections.Dictionary<Node, ProcessModeEnum>();

	private bool isDisabled;

	private MinigameReadyEventHandler backing_MinigameReady;

	public event MinigameReadyEventHandler MinigameReady
	{
		add
		{
			backing_MinigameReady = (MinigameReadyEventHandler)Delegate.Combine(backing_MinigameReady, value);
		}
		remove
		{
			backing_MinigameReady = (MinigameReadyEventHandler)Delegate.Remove(backing_MinigameReady, value);
		}
	}

	public override void _Ready()
	{
		MatchUISizing();
		EmitSignal(SignalName.MinigameReady);
	}

	public void PauseGame(bool Pause)
	{
		Array<Node> allDescendants = GetAllDescendants(this);
		if (Pause)
		{
			storedModes.Clear();
			{
				foreach (Node item in allDescendants)
				{
					if (item.ProcessMode != ProcessModeEnum.Inherit)
					{
						storedModes[item] = item.ProcessMode;
					}
					item.ProcessMode = ProcessModeEnum.Disabled;
				}
				return;
			}
		}
		foreach (Node item2 in allDescendants)
		{
			if (storedModes.TryGetValue(item2, out var value))
			{
				item2.ProcessMode = value;
			}
			else
			{
				item2.ProcessMode = ProcessModeEnum.Inherit;
			}
		}
		storedModes.Clear();
	}

	private Array<Node> GetAllDescendants(Node node)
	{
		Array<Node> array = new Array<Node>();
		foreach (Node child in node.GetChildren())
		{
			array.Add(child);
			array.AddRange(GetAllDescendants(child));
		}
		return array;
	}

	private void MatchUISizing()
	{
		if (Main.Instance != null)
		{
			base.ContentScaleSize = base.Size;
			base.Size = new Vector2I(Mathf.RoundToInt((float)base.Size.X * Main.Instance.settingUIScaler), Mathf.RoundToInt((float)base.Size.Y * Main.Instance.settingUIScaler));
		}
	}

	public void SpawnByteItem(string ItemID)
	{
		if (Main.Instance == null)
		{
			return;
		}
		if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || !ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].ContainsKey(ItemID))
		{
			GD.PrintErr("SpawnByteItem: Item ID '" + ItemID + "' not found.");
			return;
		}
		ItemDataRes itemDataRes = (ItemDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][ItemID];
		ScreenDataHandler screenDataHandler = Main.Instance.screenDataHandler;
		Vector2I vector2I = (Vector2I)(itemDataRes.itemSize * itemDataRes.itemScale * Main.Instance.settingSpriteScaler);
		int num = GD.RandRange(screenDataHandler.EffectiveLeftX, screenDataHandler.EffectiveRightX - vector2I.X);
		int y = DisplayServer.ScreenGetUsableRect(screenDataHandler.screenIndex).Position.Y;
		for (int i = 0; i < DisplayServer.GetScreenCount(); i++)
		{
			Rect2I rect2I = DisplayServer.ScreenGetUsableRect(i);
			if (num >= rect2I.Position.X && num < rect2I.Position.X + rect2I.Size.X)
			{
				y = rect2I.Position.Y;
				break;
			}
		}
		Main.Instance.CallItemSpawn(itemDataRes, new Vector2I(num, y - vector2I.Y));
	}

	public void SpawnActor(string ActorID)
	{
		if (Main.Instance != null)
		{
			if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.CHARACTER) || !ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER].ContainsKey(ActorID))
			{
				GD.PrintErr("SpawnActor: Actor ID '" + ActorID + "' not found.");
				return;
			}
			CharacterInfoDataRes spawningActor = (CharacterInfoDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][ActorID];
			Main.Instance.CallActorSpawn(spawningActor);
		}
	}

	public void SpawnDialogue(DialogueDataRes dialogue)
	{
		if (Main.Instance != null)
		{
			Main.Instance.ClearAllAttachments();
			Main.Instance.dialogueStack.Add(dialogue);
			Main.Instance.PopDialogueInStack(skipTimer: true);
		}
	}

	public void SpawnPopup(string PopupID)
	{
		if (Main.Instance != null)
		{
			Godot.Collections.Dictionary<string, Resource> dictionary = (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.SPAM) ? ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.SPAM] : new Godot.Collections.Dictionary<string, Resource>());
			if (!dictionary.ContainsKey(PopupID))
			{
				GD.PrintErr("SpawnPopup: Popup ID '" + PopupID + "' not found.");
			}
			else
			{
				Main.Instance.CallCharacterAttachmentSpawn((AttachDataRes)dictionary[PopupID], unclearableAttachment: true);
			}
		}
	}

	public void SpawnRandomPopup()
	{
		if (Main.Instance == null)
		{
			return;
		}
		Godot.Collections.Dictionary<string, Resource> obj = (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.SPAM) ? ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.SPAM] : new Godot.Collections.Dictionary<string, Resource>());
		Array<AttachDataRes> array = new Array<AttachDataRes>();
		foreach (KeyValuePair<string, Resource> item in obj)
		{
			if (item.Value is AttachDataRes attachDataRes && !Main.Instance.IsBlacklisted(attachDataRes.taggedKinks) && !attachDataRes.excludePopup)
			{
				array.Add(attachDataRes);
			}
		}
		if (array.Count == 0)
		{
			GD.PrintErr("SpawnRandomPopup: No valid popups found in cache.");
			return;
		}
		AttachDataRes objData = array[GD.RandRange(0, array.Count - 1)];
		Main.Instance.CallCharacterAttachmentSpawn(objData, unclearableAttachment: true);
	}

	public void ApplyTag(TagDataRes passedTag)
	{
		if (Main.Instance != null)
		{
			Main.Instance.mainCharacter.AddTag(passedTag);
		}
	}

	public void SaveMinigame(Variant savedData)
	{
		if (Main.Instance != null)
		{
			string title = base.Title;
			Main.Instance.minigameData[title] = savedData;
			Main.Instance.saveHandler.SaveSettings();
		}
	}

	public Variant LoadMinigame()
	{
		if (Main.Instance == null)
		{
			return -1;
		}
		string title = base.Title;
		if (Main.Instance.minigameData.ContainsKey(title))
		{
			return Main.Instance.minigameData[title];
		}
		return -1;
	}

	public void CloseMinigame()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		QueueFree();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(13)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.PauseGame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "Pause", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetAllDescendants, new PropertyInfo(Variant.Type.Array, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "node", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false)
			}, null),
			new MethodInfo(MethodName.MatchUISizing, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SpawnByteItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "ItemID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SpawnActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "ActorID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SpawnDialogue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "dialogue", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.SpawnPopup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "PopupID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SpawnRandomPopup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ApplyTag, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "passedTag", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.SaveMinigame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Nil, "savedData", PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.NilIsVariant, exported: false)
			}, null),
			new MethodInfo(MethodName.LoadMinigame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.NilIsVariant, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CloseMinigame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.PauseGame && args.Count == 1)
		{
			PauseGame(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetAllDescendants && args.Count == 1)
		{
			Array<Node> allDescendants = GetAllDescendants(VariantUtils.ConvertTo<Node>(in args[0]));
			ret = VariantUtils.CreateFromArray(allDescendants);
			return true;
		}
		if (method == MethodName.MatchUISizing && args.Count == 0)
		{
			MatchUISizing();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnByteItem && args.Count == 1)
		{
			SpawnByteItem(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnActor && args.Count == 1)
		{
			SpawnActor(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnDialogue && args.Count == 1)
		{
			SpawnDialogue(VariantUtils.ConvertTo<DialogueDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnPopup && args.Count == 1)
		{
			SpawnPopup(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnRandomPopup && args.Count == 0)
		{
			SpawnRandomPopup();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ApplyTag && args.Count == 1)
		{
			ApplyTag(VariantUtils.ConvertTo<TagDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SaveMinigame && args.Count == 1)
		{
			SaveMinigame(VariantUtils.ConvertTo<Variant>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LoadMinigame && args.Count == 0)
		{
			Variant from = LoadMinigame();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.CloseMinigame && args.Count == 0)
		{
			CloseMinigame();
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
		if (method == MethodName.PauseGame)
		{
			return true;
		}
		if (method == MethodName.GetAllDescendants)
		{
			return true;
		}
		if (method == MethodName.MatchUISizing)
		{
			return true;
		}
		if (method == MethodName.SpawnByteItem)
		{
			return true;
		}
		if (method == MethodName.SpawnActor)
		{
			return true;
		}
		if (method == MethodName.SpawnDialogue)
		{
			return true;
		}
		if (method == MethodName.SpawnPopup)
		{
			return true;
		}
		if (method == MethodName.SpawnRandomPopup)
		{
			return true;
		}
		if (method == MethodName.ApplyTag)
		{
			return true;
		}
		if (method == MethodName.SaveMinigame)
		{
			return true;
		}
		if (method == MethodName.LoadMinigame)
		{
			return true;
		}
		if (method == MethodName.CloseMinigame)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.OverridePause)
		{
			OverridePause = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.storedModes)
		{
			storedModes = VariantUtils.ConvertToDictionary<Node, ProcessModeEnum>(in value);
			return true;
		}
		if (name == PropertyName.isDisabled)
		{
			isDisabled = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.OverridePause)
		{
			value = VariantUtils.CreateFrom(in OverridePause);
			return true;
		}
		if (name == PropertyName.storedModes)
		{
			value = VariantUtils.CreateFromDictionary(storedModes);
			return true;
		}
		if (name == PropertyName.isDisabled)
		{
			value = VariantUtils.CreateFrom(in isDisabled);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, PropertyName.OverridePause, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.storedModes, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isDisabled, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.OverridePause, Variant.From(in OverridePause));
		info.AddProperty(PropertyName.storedModes, Variant.CreateFrom(storedModes));
		info.AddProperty(PropertyName.isDisabled, Variant.From(in isDisabled));
		info.AddSignalEventDelegate(SignalName.MinigameReady, backing_MinigameReady);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.OverridePause, out var value))
		{
			OverridePause = value.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.storedModes, out var value2))
		{
			storedModes = value2.AsGodotDictionary<Node, ProcessModeEnum>();
		}
		if (info.TryGetProperty(PropertyName.isDisabled, out var value3))
		{
			isDisabled = value3.As<bool>();
		}
		if (info.TryGetSignalEventDelegate<MinigameReadyEventHandler>(SignalName.MinigameReady, out var value4))
		{
			backing_MinigameReady = value4;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(SignalName.MinigameReady, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	protected void EmitSignalMinigameReady()
	{
		EmitSignal(SignalName.MinigameReady);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.MinigameReady && args.Count == 0)
		{
			backing_MinigameReady?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.MinigameReady)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
