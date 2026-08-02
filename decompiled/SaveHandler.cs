using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SaveAndLoad/SaveHandler.cs")]
public class SaveHandler : Node
{
	public enum Kinks
	{
		UNTYPED,
		CUCKING
	}

	public enum SeenObjectTypes
	{
		UNTYPED,
		ITEMS,
		NSFW_SCENES,
		BRAIN_DANCE_SCENES,
		POP_UPS
	}

	public new class MethodName : Node.MethodName
	{
		public static readonly StringName GetSavePath = "GetSavePath";

		public static readonly StringName SaveExists = "SaveExists";

		public static readonly StringName CreateNewSave = "CreateNewSave";

		public static readonly StringName AttemptLoad = "AttemptLoad";

		public static readonly StringName createOrLoadSave = "createOrLoadSave";

		public static readonly StringName KinksToStringArray = "KinksToStringArray";

		public static readonly StringName StringArrayToKinks = "StringArrayToKinks";

		public static readonly StringName WriteDefaults = "WriteDefaults";

		public static readonly StringName WriteInputDefaults = "WriteInputDefaults";

		public static readonly StringName ApplyToParent = "ApplyToParent";

		public static readonly StringName ApplyInputBindings = "ApplyInputBindings";

		public static readonly StringName SaveSettings = "SaveSettings";

		public static readonly StringName SaveInputBindings = "SaveInputBindings";

		public static readonly StringName GetDerivedWhitelist = "GetDerivedWhitelist";

		public static readonly StringName InputEventsToStringArray = "InputEventsToStringArray";

		public static readonly StringName StringToInputEvent = "StringToInputEvent";
	}

	public new class PropertyName : Node.PropertyName
	{
		public static readonly StringName versionNumber = "versionNumber";

		public static readonly StringName config = "config";

		public static readonly StringName parent = "parent";
	}

	public new class SignalName : Node.SignalName
	{
	}

	private const string SAVE_GAME_BASE_PATH = "user://save";

	private const string PET_NAME = "Byte";

	private const string SETTINGS_SECTION = "settings";

	private const string USER_INFO_SECTION = "user info";

	private const string INPUT_SECTION = "input";

	private const string TAGS_SECTION = "saved_tags";

	private const string MINIGAMES_SECTION = "minigames";

	private const string MODS_SECTION = "mods";

	private int versionNumber = 1;

	private ConfigFile config = new ConfigFile();

	[Export(PropertyHint.None, "")]
	public Main parent;

	public static string GetSavePath()
	{
		return "user://save_Byte.cfg";
	}

	private static bool SaveExists()
	{
		return FileAccess.FileExists(GetSavePath());
	}

	public void CreateNewSave()
	{
		if (SaveExists())
		{
			DirAccess.RemoveAbsolute(GetSavePath());
			GD.Print("Old save deleted.");
		}
		config = new ConfigFile();
		WriteDefaults();
		ApplyToParent();
		SaveSettings();
		GD.Print("Successfully Started a New Game");
	}

	public void AttemptLoad()
	{
		createOrLoadSave();
		ApplyToParent();
		GD.Print("Successfully Loaded Game Data");
		SaveSettings();
	}

	private void createOrLoadSave()
	{
		bool flag = SaveExists();
		if (flag)
		{
			GD.Print("Save exists, attempting load...");
			config = new ConfigFile();
			Error error = config.Load(GetSavePath());
			if (error != Error.Ok)
			{
				GD.PrintErr("Failed to load config:" + error);
				flag = false;
			}
			else
			{
				int num = (int)config.GetValue("settings", "version", 0);
				if (num != versionNumber)
				{
					GD.Print("Version mismatch (file:" + num + ", expected: " + versionNumber + "). Resetting.");
					DirAccess.RemoveAbsolute(GetSavePath());
					config = new ConfigFile();
					flag = false;
				}
			}
		}
		if (!flag)
		{
			GD.Print("No valid save found. Loading defaults.");
			config = new ConfigFile();
			WriteDefaults();
		}
	}

	private static Array<string> KinksToStringArray(Array<Kinks> arr)
	{
		Array<string> array = new Array<string>();
		foreach (Kinks item in arr)
		{
			array.Add(item.ToString());
		}
		return array;
	}

	private static Array<Kinks> StringArrayToKinks(Array<string> arr)
	{
		Array<Kinks> array = new Array<Kinks>();
		foreach (string item in arr)
		{
			if (Enum.TryParse<Kinks>(item, out var result))
			{
				array.Add(result);
			}
			else
			{
				GD.PrintErr("Unknown ContentGuidelines value in save: " + item);
			}
		}
		return array;
	}

	private void WriteDefaults()
	{
		config.SetValue("settings", "version", versionNumber);
		config.SetValue("settings", "EULA", false);
		config.SetValue("settings", "SpriteScaling", 1f);
		config.SetValue("settings", "ItemScaling", 1f);
		config.SetValue("settings", "UIScaling", 1f);
		config.SetValue("settings", "spawnItems", true);
		config.SetValue("settings", "spawnActors", true);
		config.SetValue("settings", "audioOn", true);
		config.SetValue("settings", "passivePlayMode", false);
		config.SetValue("settings", "removePopups", false);
		config.SetValue("settings", "removeConvos", false);
		config.SetValue("settings", "blacklistedContent", KinksToStringArray(new Array<Kinks>
		{
			Kinks.UNTYPED,
			Kinks.CUCKING
		}));
		config.SetValue("settings", "whitelistedContent", KinksToStringArray(GetDerivedWhitelist()));
		string text = System.Environment.GetEnvironmentVariable("USERNAME") ?? System.Environment.GetEnvironmentVariable("USER") ?? "USER";
		config.SetValue("user info", "userName", text);
		config.SetValue("user info", "seenItems", new Array<string>());
		config.SetValue("user info", "seenNSFWScenes", new Array<string>());
		config.SetValue("user info", "seenBrainDances", new Array<string>());
		config.SetValue("user info", "seenPopups", new Array<string>());
		WriteInputDefaults();
		config.SetValue("saved_tags", "tags", new Godot.Collections.Dictionary<string, int>());
		config.SetValue("minigames", "tickets", 0);
		config.SetValue("minigames", "minigame_data", new Godot.Collections.Dictionary<string, Variant>());
		config.SetValue("mods", "modsEnabled", false);
		config.SetValue("mods", "enabledMods", new Array<string>());
	}

	private void WriteInputDefaults()
	{
		string[] trackedActions = SettingsMenu.TrackedActions;
		foreach (string text in trackedActions)
		{
			if (InputMap.HasAction(text))
			{
				config.SetValue("input", text, InputEventsToStringArray(InputMap.ActionGetEvents(text)));
			}
		}
	}

	private void ApplyToParent()
	{
		parent.settingSpriteScaler = (float)config.GetValue("settings", "SpriteScaling", 1f);
		parent.settingItemScaler = (float)config.GetValue("settings", "ItemScaling", 1f);
		parent.settingUIScaler = (float)config.GetValue("settings", "UIScaling", 1f);
		parent.settingEULA = (bool)config.GetValue("settings", "EULA", false);
		parent.settingSpawnItems = (bool)config.GetValue("settings", "spawnItems", false);
		parent.settingSpawnActors = (bool)config.GetValue("settings", "spawnActors", false);
		parent.settingAudioOn = (bool)config.GetValue("settings", "audioOn", false);
		parent.settingPassivePlayMode = (bool)config.GetValue("settings", "passivePlayMode", false);
		parent.settingRemovePopups = (bool)config.GetValue("settings", "removePopups", false);
		parent.settingRemovePopups = (bool)config.GetValue("settings", "removeConvos", false);
		Array<string> arr = config.GetValue("settings", "blacklistedContent", new Array<string>()).AsGodotArray<string>();
		parent.settingBlacklistedContent = StringArrayToKinks(arr);
		parent.userInfoName = (string)config.GetValue("user info", "userName", "USER");
		parent.SeenObjects[SeenObjectTypes.ITEMS] = config.GetValue("user info", "seenItems", new Array<string>()).AsGodotArray<string>();
		parent.SeenObjects[SeenObjectTypes.NSFW_SCENES] = config.GetValue("user info", "seenNSFWScenes", new Array<string>()).AsGodotArray<string>();
		parent.SeenObjects[SeenObjectTypes.BRAIN_DANCE_SCENES] = config.GetValue("user info", "seenBrainDances", new Array<string>()).AsGodotArray<string>();
		parent.SeenObjects[SeenObjectTypes.POP_UPS] = config.GetValue("user info", "seenPopups", new Array<string>()).AsGodotArray<string>();
		ApplyInputBindings();
		foreach (KeyValuePair<string, int> item2 in config.GetValue("saved_tags", "tags", new Dictionary()).AsGodotDictionary<string, int>())
		{
			TagDataRes item = new TagDataRes
			{
				tagName = item2.Key,
				tagAmount = item2.Value,
				tagDuration = -1999f,
				tagOriginalDuration = -1999f,
				savedTag = true
			};
			parent.mainCharacter.existingTags.Add(item);
		}
		parent.userTickets = (int)config.GetValue("minigames", "tickets", 0);
		parent.minigameData = config.GetValue("minigames", "minigame_data", new Godot.Collections.Dictionary<string, Variant>()).AsGodotDictionary<string, Variant>();
		parent.settingMods = (bool)config.GetValue("mods", "modsEnabled", false);
		parent.settingEnabledMods = config.GetValue("mods", "enabledMods", new Array<string>()).AsGodotArray<string>();
	}

	private void ApplyInputBindings()
	{
		string[] trackedActions = SettingsMenu.TrackedActions;
		foreach (string text in trackedActions)
		{
			if (!InputMap.HasAction(text) || !config.HasSectionKey("input", text))
			{
				continue;
			}
			Array<string> array = config.GetValue("input", text).AsGodotArray<string>();
			InputMap.ActionEraseEvents(text);
			foreach (string item in array)
			{
				InputEvent inputEvent = StringToInputEvent(item);
				if (inputEvent != null)
				{
					InputMap.ActionAddEvent(text, inputEvent);
				}
			}
		}
	}

	public void SaveSettings()
	{
		config.SetValue("settings", "version", versionNumber);
		config.SetValue("settings", "EULA", parent.settingEULA);
		config.SetValue("settings", "SpriteScaling", parent.settingSpriteScaler);
		config.SetValue("settings", "ItemScaling", parent.settingItemScaler);
		config.SetValue("settings", "UIScaling", parent.settingUIScaler);
		config.SetValue("settings", "spawnItems", parent.settingSpawnItems);
		config.SetValue("settings", "spawnActors", parent.settingSpawnActors);
		config.SetValue("settings", "audioOn", parent.settingAudioOn);
		config.SetValue("settings", "passivePlayMode", parent.settingPassivePlayMode);
		config.SetValue("settings", "removePopups", parent.settingRemovePopups);
		config.SetValue("settings", "removeConvos", parent.settingRemoveConvos);
		config.SetValue("settings", "blacklistedContent", KinksToStringArray(parent.settingBlacklistedContent));
		config.SetValue("settings", "whitelistedContent", KinksToStringArray(GetDerivedWhitelist()));
		config.SetValue("user info", "userName", parent.userInfoName);
		config.SetValue("user info", "seenItems", parent.SeenObjects[SeenObjectTypes.ITEMS]);
		config.SetValue("user info", "seenNSFWScenes", parent.SeenObjects[SeenObjectTypes.NSFW_SCENES]);
		config.SetValue("user info", "seenBrainDances", parent.SeenObjects[SeenObjectTypes.BRAIN_DANCE_SCENES]);
		config.SetValue("user info", "seenPopups", parent.SeenObjects[SeenObjectTypes.POP_UPS]);
		SaveInputBindings();
		Godot.Collections.Dictionary<string, int> dictionary = new Godot.Collections.Dictionary<string, int>();
		foreach (TagDataRes existingTag in parent.mainCharacter.existingTags)
		{
			if (existingTag.savedTag)
			{
				dictionary[existingTag.tagName] = existingTag.tagAmount;
			}
		}
		config.SetValue("saved_tags", "tags", dictionary);
		config.SetValue("minigames", "tickets", parent.userTickets);
		config.SetValue("minigames", "minigame_data", parent.minigameData);
		config.SetValue("mods", "modsEnabled", parent.settingMods);
		config.SetValue("mods", "enabledMods", parent.settingEnabledMods);
		Error error = config.Save(GetSavePath());
		if (error != Error.Ok)
		{
			GD.PrintErr("Failed to save config:" + error);
		}
	}

	private void SaveInputBindings()
	{
		string[] trackedActions = SettingsMenu.TrackedActions;
		foreach (string text in trackedActions)
		{
			if (InputMap.HasAction(text))
			{
				config.SetValue("input", text, InputEventsToStringArray(InputMap.ActionGetEvents(text)));
			}
		}
	}

	private Array<Kinks> GetDerivedWhitelist()
	{
		Array<Kinks> array = new Array<Kinks>((Kinks[])Enum.GetValues(typeof(Kinks)));
		Array<Kinks> array2 = new Array<Kinks>();
		foreach (Kinks item in array)
		{
			if (!parent.settingBlacklistedContent.Contains(item))
			{
				array2.Add(item);
			}
		}
		return array2;
	}

	private static Array<string> InputEventsToStringArray(Array<InputEvent> events)
	{
		Array<string> array = new Array<string>();
		foreach (InputEvent @event in events)
		{
			if (@event is InputEventKey inputEventKey)
			{
				array.Add($"key:{inputEventKey.PhysicalKeycode}");
			}
			else if (@event is InputEventMouseButton inputEventMouseButton)
			{
				array.Add($"mouse:{inputEventMouseButton.ButtonIndex}");
			}
		}
		return array;
	}

	private static InputEvent StringToInputEvent(string eventStr)
	{
		string[] array = eventStr.Split(':');
		if (array.Length != 2)
		{
			return null;
		}
		string text = array[0];
		int result2;
		if (!(text == "key"))
		{
			if (text == "mouse" && int.TryParse(array[1], out var result))
			{
				return new InputEventMouseButton
				{
					ButtonIndex = (MouseButton)result
				};
			}
		}
		else if (int.TryParse(array[1], out result2))
		{
			return new InputEventKey
			{
				PhysicalKeycode = (Key)result2
			};
		}
		return null;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(16)
		{
			new MethodInfo(MethodName.GetSavePath, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, null, null),
			new MethodInfo(MethodName.SaveExists, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, null, null),
			new MethodInfo(MethodName.CreateNewSave, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.AttemptLoad, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.createOrLoadSave, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.KinksToStringArray, new PropertyInfo(Variant.Type.Array, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Array, "arr", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.StringArrayToKinks, new PropertyInfo(Variant.Type.Array, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Array, "arr", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.WriteDefaults, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.WriteInputDefaults, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ApplyToParent, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ApplyInputBindings, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SaveSettings, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SaveInputBindings, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetDerivedWhitelist, new PropertyInfo(Variant.Type.Array, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.InputEventsToStringArray, new PropertyInfo(Variant.Type.Array, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Array, "events", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.StringToInputEvent, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "eventStr", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.GetSavePath && args.Count == 0)
		{
			string from = GetSavePath();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.SaveExists && args.Count == 0)
		{
			bool from2 = SaveExists();
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.CreateNewSave && args.Count == 0)
		{
			CreateNewSave();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AttemptLoad && args.Count == 0)
		{
			AttemptLoad();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.createOrLoadSave && args.Count == 0)
		{
			createOrLoadSave();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.KinksToStringArray && args.Count == 1)
		{
			Array<string> from3 = KinksToStringArray(VariantUtils.ConvertToArray<Kinks>(in args[0]));
			ret = VariantUtils.CreateFromArray(from3);
			return true;
		}
		if (method == MethodName.StringArrayToKinks && args.Count == 1)
		{
			Array<Kinks> from4 = StringArrayToKinks(VariantUtils.ConvertToArray<string>(in args[0]));
			ret = VariantUtils.CreateFromArray(from4);
			return true;
		}
		if (method == MethodName.WriteDefaults && args.Count == 0)
		{
			WriteDefaults();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.WriteInputDefaults && args.Count == 0)
		{
			WriteInputDefaults();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ApplyToParent && args.Count == 0)
		{
			ApplyToParent();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ApplyInputBindings && args.Count == 0)
		{
			ApplyInputBindings();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SaveSettings && args.Count == 0)
		{
			SaveSettings();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SaveInputBindings && args.Count == 0)
		{
			SaveInputBindings();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetDerivedWhitelist && args.Count == 0)
		{
			Array<Kinks> derivedWhitelist = GetDerivedWhitelist();
			ret = VariantUtils.CreateFromArray(derivedWhitelist);
			return true;
		}
		if (method == MethodName.InputEventsToStringArray && args.Count == 1)
		{
			Array<string> from5 = InputEventsToStringArray(VariantUtils.ConvertToArray<InputEvent>(in args[0]));
			ret = VariantUtils.CreateFromArray(from5);
			return true;
		}
		if (method == MethodName.StringToInputEvent && args.Count == 1)
		{
			InputEvent from6 = StringToInputEvent(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from6);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.GetSavePath && args.Count == 0)
		{
			string from = GetSavePath();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.SaveExists && args.Count == 0)
		{
			bool from2 = SaveExists();
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.KinksToStringArray && args.Count == 1)
		{
			Array<string> from3 = KinksToStringArray(VariantUtils.ConvertToArray<Kinks>(in args[0]));
			ret = VariantUtils.CreateFromArray(from3);
			return true;
		}
		if (method == MethodName.StringArrayToKinks && args.Count == 1)
		{
			Array<Kinks> from4 = StringArrayToKinks(VariantUtils.ConvertToArray<string>(in args[0]));
			ret = VariantUtils.CreateFromArray(from4);
			return true;
		}
		if (method == MethodName.InputEventsToStringArray && args.Count == 1)
		{
			Array<string> from5 = InputEventsToStringArray(VariantUtils.ConvertToArray<InputEvent>(in args[0]));
			ret = VariantUtils.CreateFromArray(from5);
			return true;
		}
		if (method == MethodName.StringToInputEvent && args.Count == 1)
		{
			InputEvent from6 = StringToInputEvent(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from6);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.GetSavePath)
		{
			return true;
		}
		if (method == MethodName.SaveExists)
		{
			return true;
		}
		if (method == MethodName.CreateNewSave)
		{
			return true;
		}
		if (method == MethodName.AttemptLoad)
		{
			return true;
		}
		if (method == MethodName.createOrLoadSave)
		{
			return true;
		}
		if (method == MethodName.KinksToStringArray)
		{
			return true;
		}
		if (method == MethodName.StringArrayToKinks)
		{
			return true;
		}
		if (method == MethodName.WriteDefaults)
		{
			return true;
		}
		if (method == MethodName.WriteInputDefaults)
		{
			return true;
		}
		if (method == MethodName.ApplyToParent)
		{
			return true;
		}
		if (method == MethodName.ApplyInputBindings)
		{
			return true;
		}
		if (method == MethodName.SaveSettings)
		{
			return true;
		}
		if (method == MethodName.SaveInputBindings)
		{
			return true;
		}
		if (method == MethodName.GetDerivedWhitelist)
		{
			return true;
		}
		if (method == MethodName.InputEventsToStringArray)
		{
			return true;
		}
		if (method == MethodName.StringToInputEvent)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.versionNumber)
		{
			versionNumber = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.config)
		{
			config = VariantUtils.ConvertTo<ConfigFile>(in value);
			return true;
		}
		if (name == PropertyName.parent)
		{
			parent = VariantUtils.ConvertTo<Main>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.versionNumber)
		{
			value = VariantUtils.CreateFrom(in versionNumber);
			return true;
		}
		if (name == PropertyName.config)
		{
			value = VariantUtils.CreateFrom(in config);
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
			new PropertyInfo(Variant.Type.Int, PropertyName.versionNumber, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.config, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.parent, PropertyHint.NodeType, "Main", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.versionNumber, Variant.From(in versionNumber));
		info.AddProperty(PropertyName.config, Variant.From(in config));
		info.AddProperty(PropertyName.parent, Variant.From(in parent));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.versionNumber, out var value))
		{
			versionNumber = value.As<int>();
		}
		if (info.TryGetProperty(PropertyName.config, out var value2))
		{
			config = value2.As<ConfigFile>();
		}
		if (info.TryGetProperty(PropertyName.parent, out var value3))
		{
			parent = value3.As<Main>();
		}
	}
}
