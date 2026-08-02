using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Cache/ResourceCache.cs")]
public class ResourceCache : Node
{
	[Signal]
	public delegate void ResourceCacheLoadedEventHandler();

	[Signal]
	public delegate void ResourceCacheProgressChangedEventHandler(float progress);

	public enum ResourceTyping
	{
		UNTYPED,
		CHARACTER,
		ITEM,
		GALLERY,
		SPAM,
		BRAINDACE_WORLDS,
		ASK_CHARACTERS,
		H_SCENES
	}

	public enum PrefabTyping
	{
		UNTYPED,
		MINIGAME,
		TERMINAL_COMMANDS
	}

	public new class MethodName : Node.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName LoadData = "LoadData";

		public static readonly StringName LoadItemData = "LoadItemData";

		public static readonly StringName LoadItemDataInternal = "LoadItemDataInternal";

		public static readonly StringName LoadPrefabData = "LoadPrefabData";

		public static readonly StringName LoadPrefabDataInternal = "LoadPrefabDataInternal";

		public static readonly StringName LoadMods = "LoadMods";

		public static readonly StringName DetectResourceType = "DetectResourceType";

		public static readonly StringName DetectPrefabTypeFromPath = "DetectPrefabTypeFromPath";

		public static readonly StringName GetResourceID = "GetResourceID";

		public static readonly StringName ReadModSaveData = "ReadModSaveData";
	}

	public new class PropertyName : Node.PropertyName
	{
		public static readonly StringName IsLoaded = "IsLoaded";

		public static readonly StringName _saveReadDone = "_saveReadDone";

		public static readonly StringName _modsEnabled = "_modsEnabled";

		public static readonly StringName _loadStarted = "_loadStarted";

		public static readonly StringName _totalLoadSteps = "_totalLoadSteps";

		public static readonly StringName _completedLoadSteps = "_completedLoadSteps";
	}

	public new class SignalName : Node.SignalName
	{
		public static readonly StringName ResourceCacheLoaded = "ResourceCacheLoaded";

		public static readonly StringName ResourceCacheProgressChanged = "ResourceCacheProgressChanged";
	}

	public static Godot.Collections.Dictionary<ResourceTyping, Godot.Collections.Dictionary<string, Resource>> resourcesLoaded = new Godot.Collections.Dictionary<ResourceTyping, Godot.Collections.Dictionary<string, Resource>>();

	public static Godot.Collections.Dictionary<PrefabTyping, Godot.Collections.Dictionary<string, PackedScene>> prefabsLoaded = new Godot.Collections.Dictionary<PrefabTyping, Godot.Collections.Dictionary<string, PackedScene>>();

	public static System.Collections.Generic.Dictionary<string, ModManifest> modManifests = new System.Collections.Generic.Dictionary<string, ModManifest>();

	private bool _saveReadDone;

	private bool _modsEnabled;

	private HashSet<string> _enabledModIDs = new HashSet<string>();

	private static readonly System.Collections.Generic.Dictionary<string, PrefabTyping> _prefabSubfolderMap = new System.Collections.Generic.Dictionary<string, PrefabTyping>(StringComparer.OrdinalIgnoreCase)
	{
		{
			"Generic",
			PrefabTyping.UNTYPED
		},
		{
			"Minigames",
			PrefabTyping.MINIGAME
		},
		{
			"Terminal_Commands",
			PrefabTyping.TERMINAL_COMMANDS
		}
	};

	private static readonly HashSet<string> _ignoredPrefabFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "SubScenes" };

	private bool _loadStarted;

	private int _totalLoadSteps;

	private int _completedLoadSteps;

	private ResourceCacheLoadedEventHandler backing_ResourceCacheLoaded;

	private ResourceCacheProgressChangedEventHandler backing_ResourceCacheProgressChanged;

	public static ResourceCache Instance { get; private set; }

	public bool IsLoaded { get; private set; }

	public event ResourceCacheLoadedEventHandler ResourceCacheLoaded
	{
		add
		{
			backing_ResourceCacheLoaded = (ResourceCacheLoadedEventHandler)Delegate.Combine(backing_ResourceCacheLoaded, value);
		}
		remove
		{
			backing_ResourceCacheLoaded = (ResourceCacheLoadedEventHandler)Delegate.Remove(backing_ResourceCacheLoaded, value);
		}
	}

	public event ResourceCacheProgressChangedEventHandler ResourceCacheProgressChanged
	{
		add
		{
			backing_ResourceCacheProgressChanged = (ResourceCacheProgressChangedEventHandler)Delegate.Combine(backing_ResourceCacheProgressChanged, value);
		}
		remove
		{
			backing_ResourceCacheProgressChanged = (ResourceCacheProgressChangedEventHandler)Delegate.Remove(backing_ResourceCacheProgressChanged, value);
		}
	}

	public override void _Ready()
	{
		if (Instance != null && Instance != this)
		{
			GD.PrintErr("[ResourceCache] Duplicate ResourceCache instance detected — freeing extra copy.");
			QueueFree();
		}
		else
		{
			Instance = this;
		}
	}

	public async void LoadData()
	{
		if (_loadStarted)
		{
			if (OS.HasFeature("editor"))
			{
				GD.Print("[ResourceCache] LoadData already ran — ignoring duplicate call.");
			}
			return;
		}
		_loadStarted = true;
		List<Action> list = new List<Action>
		{
			delegate
			{
				LoadItemData(ResourceTyping.ITEM, "res://Resources/ItemData/", deepSearch: true);
			},
			delegate
			{
				LoadItemData(ResourceTyping.CHARACTER, "res://Resources/CharacterData/PossibleRandom/");
			},
			delegate
			{
				LoadItemData(ResourceTyping.GALLERY, "res://Resources/GalleryPiece/");
			},
			delegate
			{
				LoadItemData(ResourceTyping.SPAM, "res://Resources/AttachmentObject/Spam/");
			},
			delegate
			{
				LoadItemData(ResourceTyping.BRAINDACE_WORLDS, "res://Resources/AdventureMode/_WORLDS/Live/");
			},
			delegate
			{
				LoadItemData(ResourceTyping.ASK_CHARACTERS, "res://Resources/AskMode/");
			},
			delegate
			{
				LoadItemData(ResourceTyping.H_SCENES, "res://Resources/Override Attach Objects/", deepSearch: true);
			},
			delegate
			{
				LoadItemData(ResourceTyping.SPAM, "res://Resources/AdventureMode/KnotCity/DancePops/");
			},
			delegate
			{
				LoadItemData(ResourceTyping.SPAM, "res://Resources/AdventureMode/EverlustGlade/DancePops/");
			},
			delegate
			{
				LoadPrefabData(PrefabTyping.MINIGAME, "res://Scenes/Minigames/Completed/", deepSearch: true);
			}
		};
		if (OS.HasFeature("substar") || OS.HasFeature("editor"))
		{
			list.Add(delegate
			{
				LoadItemData(ResourceTyping.ITEM, "res://Resources/SubStar/ItemData/", deepSearch: true);
			});
			list.Add(delegate
			{
				LoadItemData(ResourceTyping.GALLERY, "res://Resources/SubStar/GalleryPiece/");
			});
			list.Add(delegate
			{
				LoadItemData(ResourceTyping.SPAM, "res://Resources/SubStar/Spam/");
			});
			list.Add(delegate
			{
				LoadItemData(ResourceTyping.BRAINDACE_WORLDS, "res://Resources/AdventureMode/_WORLDS/Substar/");
			});
			list.Add(delegate
			{
				LoadItemData(ResourceTyping.H_SCENES, "res://Resources/SubStar/Override Attach Objects/", deepSearch: true);
			});
			list.Add(delegate
			{
				LoadPrefabData(PrefabTyping.MINIGAME, "res://Scenes/Minigames/CompletedSubStar/", deepSearch: true);
			});
		}
		if (OS.HasFeature("editor"))
		{
			list.Add(delegate
			{
				LoadPrefabData(PrefabTyping.MINIGAME, "res://Scenes/Minigames/Staging/", deepSearch: true);
			});
			list.Add(delegate
			{
				LoadPrefabData(PrefabTyping.MINIGAME, "res://Scenes/Minigames/WorkInProgress/", deepSearch: true);
			});
		}
		list.Add(LoadMods);
		_totalLoadSteps = list.Count;
		_completedLoadSteps = 0;
		foreach (Action item in list)
		{
			item();
			_completedLoadSteps++;
			EmitSignal(SignalName.ResourceCacheProgressChanged, (float)_completedLoadSteps / (float)_totalLoadSteps * 100f);
			await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		}
		IsLoaded = true;
		GD.Print("[ResourceCache] LoadData complete — emitting ResourceCacheLoaded.");
		EmitSignal(SignalName.ResourceCacheLoaded);
	}

	public void LoadItemData(ResourceTyping ResourceType, string path, bool deepSearch = false)
	{
		Godot.Collections.Dictionary<ResourceTyping, Godot.Collections.Dictionary<string, Resource>> dictionary = new Godot.Collections.Dictionary<ResourceTyping, Godot.Collections.Dictionary<string, Resource>>();
		LoadItemDataInternal(ResourceType, path, deepSearch, dictionary);
		foreach (KeyValuePair<ResourceTyping, Godot.Collections.Dictionary<string, Resource>> item in dictionary)
		{
			if (resourcesLoaded.ContainsKey(item.Key))
			{
				foreach (KeyValuePair<string, Resource> item2 in item.Value)
				{
					if (!resourcesLoaded[item.Key].ContainsKey(item2.Key))
					{
						resourcesLoaded[item.Key][item2.Key] = item2.Value;
					}
					else
					{
						GD.PrintErr("Duplicate key '" + item2.Key + "' skipped during merge.");
					}
				}
			}
			else
			{
				resourcesLoaded[item.Key] = item.Value;
			}
		}
	}

	private void LoadItemDataInternal(ResourceTyping ResourceType, string path, bool deepSearch, Godot.Collections.Dictionary<ResourceTyping, Godot.Collections.Dictionary<string, Resource>> resources)
	{
		DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess == null)
		{
			return;
		}
		string[] directories;
		if (deepSearch)
		{
			directories = dirAccess.GetDirectories();
			foreach (string text in directories)
			{
				LoadItemDataInternal(ResourceType, path + text + "/", deepSearch: true, resources);
			}
		}
		string[] files = dirAccess.GetFiles();
		if (files == null)
		{
			return;
		}
		directories = files;
		foreach (string instance in directories)
		{
			Resource resource = GD.Load<Resource>(path + instance.TrimSuffix(".remap"));
			if (resource == null)
			{
				continue;
			}
			if (resource is ItemDataRes itemDataRes)
			{
				if (itemDataRes.itemID == "INVALID_ID: Please Set Name")
				{
					GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Invalid Item Data Name");
					break;
				}
				if (resources.ContainsKey(ResourceType))
				{
					if (resources[ResourceType].ContainsKey(itemDataRes.itemID))
					{
						GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Duplicate Item Data Name");
						break;
					}
					resources[ResourceType][itemDataRes.itemID] = resource;
				}
				else
				{
					resources[ResourceType] = new Godot.Collections.Dictionary<string, Resource> { { itemDataRes.itemID, resource } };
				}
			}
			if (resource is CharacterInfoDataRes characterInfoDataRes)
			{
				if (characterInfoDataRes.itemID == "INVALID_ID: Please Set Name")
				{
					GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Invalid Item Data Name");
					break;
				}
				if (resources.ContainsKey(ResourceType))
				{
					if (resources[ResourceType].ContainsKey(characterInfoDataRes.itemID))
					{
						GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Duplicate Item Data Name");
						break;
					}
					resources[ResourceType][characterInfoDataRes.itemID] = resource;
				}
				else
				{
					resources[ResourceType] = new Godot.Collections.Dictionary<string, Resource> { { characterInfoDataRes.itemID, resource } };
				}
			}
			if (resource is GalleryDataRes galleryDataRes)
			{
				if (galleryDataRes.itemID == "INVALID_ID: Please Set Name")
				{
					GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Invalid Item Data Name");
					break;
				}
				if (resources.ContainsKey(ResourceType))
				{
					if (resources[ResourceType].ContainsKey(galleryDataRes.itemID))
					{
						GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Duplicate Item Data Name");
						break;
					}
					resources[ResourceType][galleryDataRes.itemID] = resource;
				}
				else
				{
					resources[ResourceType] = new Godot.Collections.Dictionary<string, Resource> { { galleryDataRes.itemID, resource } };
				}
			}
			if (resource is AttachDataRes attachDataRes && ResourceType == ResourceTyping.SPAM)
			{
				string itemID = attachDataRes.itemID;
				if (itemID == "INVALID_ID: Please Set Name")
				{
					GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Invalid Attach Data Name");
					break;
				}
				if (resources.ContainsKey(ResourceType))
				{
					if (resources[ResourceType].ContainsKey(itemID))
					{
						GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Duplicate Item Data Name");
						break;
					}
					resources[ResourceType][itemID] = resource;
				}
				else
				{
					resources[ResourceType] = new Godot.Collections.Dictionary<string, Resource> { { itemID, resource } };
				}
			}
			if (resource is AttachDataRes attachDataRes2 && ResourceType == ResourceTyping.H_SCENES)
			{
				string itemID2 = attachDataRes2.itemID;
				if (itemID2 == "INVALID_ID: Please Set Name")
				{
					GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Invalid Attach Data Name");
					break;
				}
				if (resources.ContainsKey(ResourceType))
				{
					if (resources[ResourceType].ContainsKey(itemID2))
					{
						GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Duplicate Item Data Name");
						break;
					}
					resources[ResourceType][itemID2] = resource;
				}
				else
				{
					resources[ResourceType] = new Godot.Collections.Dictionary<string, Resource> { { itemID2, resource } };
				}
			}
			if (resource is TA_WorldDataRes tA_WorldDataRes)
			{
				if (tA_WorldDataRes.itemID == "INVALID_ID: Please Set Name")
				{
					GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Invalid Item Data Name");
					break;
				}
				if (resources.ContainsKey(ResourceType))
				{
					if (resources[ResourceType].ContainsKey(tA_WorldDataRes.itemID))
					{
						GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Duplicate Item Data Name");
						break;
					}
					resources[ResourceType][tA_WorldDataRes.itemID] = resource;
				}
				else
				{
					resources[ResourceType] = new Godot.Collections.Dictionary<string, Resource> { { tA_WorldDataRes.itemID, resource } };
				}
			}
			if (!(resource is TAsk_AskDataRes tAsk_AskDataRes))
			{
				continue;
			}
			if (tAsk_AskDataRes.askingCharacter == null)
			{
				GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to null askingCharacter on TAsk_AskDataRes");
				break;
			}
			string itemID3 = tAsk_AskDataRes.askingCharacter.itemID;
			if (itemID3 == "INVALID_ID: Please Set Name")
			{
				GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Invalid Character ID on TAsk_AskDataRes");
				break;
			}
			if (resources.ContainsKey(ResourceType))
			{
				if (resources[ResourceType].ContainsKey(itemID3))
				{
					GD.PrintErr("Attempted Resource Load: (" + path + ") - Failed due to Duplicate Ask ID");
					break;
				}
				resources[ResourceType][itemID3] = resource;
			}
			else
			{
				resources[ResourceType] = new Godot.Collections.Dictionary<string, Resource> { { itemID3, resource } };
			}
		}
	}

	public void LoadPrefabData(PrefabTyping prefabType, string path, bool deepSearch = false)
	{
		Godot.Collections.Dictionary<PrefabTyping, Godot.Collections.Dictionary<string, PackedScene>> dictionary = new Godot.Collections.Dictionary<PrefabTyping, Godot.Collections.Dictionary<string, PackedScene>>();
		LoadPrefabDataInternal(prefabType, path, deepSearch, dictionary);
		foreach (KeyValuePair<PrefabTyping, Godot.Collections.Dictionary<string, PackedScene>> item in dictionary)
		{
			if (prefabsLoaded.ContainsKey(item.Key))
			{
				foreach (KeyValuePair<string, PackedScene> item2 in item.Value)
				{
					if (!prefabsLoaded[item.Key].ContainsKey(item2.Key))
					{
						prefabsLoaded[item.Key][item2.Key] = item2.Value;
					}
					else
					{
						GD.PrintErr("[ResourceCache] Duplicate prefab key '" + item2.Key + "' skipped during merge.");
					}
				}
			}
			else
			{
				prefabsLoaded[item.Key] = item.Value;
			}
		}
	}

	private void LoadPrefabDataInternal(PrefabTyping prefabType, string path, bool deepSearch, Godot.Collections.Dictionary<PrefabTyping, Godot.Collections.Dictionary<string, PackedScene>> prefabs)
	{
		DirAccess dirAccess = DirAccess.Open(path);
		if (dirAccess == null)
		{
			return;
		}
		string[] directories;
		if (deepSearch)
		{
			directories = dirAccess.GetDirectories();
			foreach (string text in directories)
			{
				if (_ignoredPrefabFolders.Contains(text))
				{
					if (OS.HasFeature("editor"))
					{
						GD.Print("[ResourceCache] Skipping ignored folder during prefab scan: " + path + text);
					}
				}
				else
				{
					LoadPrefabDataInternal(prefabType, path + text + "/", deepSearch: true, prefabs);
				}
			}
		}
		string[] files = dirAccess.GetFiles();
		if (files == null)
		{
			return;
		}
		directories = files;
		for (int i = 0; i < directories.Length; i++)
		{
			string text2 = directories[i].TrimSuffix(".remap");
			string text3 = text2.GetExtension().ToLower();
			if (text3 != "tscn" && text3 != "scn")
			{
				continue;
			}
			PackedScene packedScene = GD.Load<PackedScene>(path + text2);
			if (packedScene == null)
			{
				GD.PrintErr("[ResourceCache] Failed to load PackedScene: " + path + text2);
				continue;
			}
			string file = text2.GetBaseName().GetFile();
			if (!prefabs.ContainsKey(prefabType))
			{
				prefabs[prefabType] = new Godot.Collections.Dictionary<string, PackedScene>();
			}
			if (!prefabs[prefabType].ContainsKey(file))
			{
				prefabs[prefabType][file] = packedScene;
				GD.Print($"[ResourceCache] Loaded prefab '{file}' as {prefabType}");
			}
			else
			{
				GD.PrintErr($"[ResourceCache] Duplicate prefab ID '{file}' at '{path}' — skipped.");
			}
		}
	}

	private void LoadMods()
	{
		ReadModSaveData();
		string text = OS.GetExecutablePath().GetBaseDir().PathJoin("Mods");
		if (OS.HasFeature("editor"))
		{
			text = ProjectSettings.GlobalizePath("res://Mods");
		}
		if (!DirAccess.DirExistsAbsolute(text))
		{
			GD.Print("[ResourceCache] No Mods folder found.");
			return;
		}
		DirAccess dirAccess = DirAccess.Open(text);
		if (dirAccess == null)
		{
			GD.PrintErr("[ResourceCache] Could not open Mods folder.");
			return;
		}
		string[] files = dirAccess.GetFiles();
		foreach (string text2 in files)
		{
			string text3 = text2.GetExtension().ToLower();
			if (text3 != "pck" && text3 != "zip")
			{
				continue;
			}
			string pack = text.PathJoin(text2);
			HashSet<string> modTreeEntriesRecursive = GetModTreeEntriesRecursive("res://Mods/");
			if (!ProjectSettings.LoadResourcePack(pack))
			{
				GD.PrintErr("[ResourceCache] Failed to load mod pack: " + text2);
				continue;
			}
			GD.Print("[ResourceCache] Loaded mod pack: " + text2);
			HashSet<string> modTreeEntriesRecursive2 = GetModTreeEntriesRecursive("res://Mods/");
			modTreeEntriesRecursive2.ExceptWith(modTreeEntriesRecursive);
			List<string> filePaths = new List<string>(modTreeEntriesRecursive2);
			ModManifest modManifest = ReadModManifest("res://Manifest/Manifest.json", text2);
			modManifests[text2] = modManifest;
			if (!_modsEnabled)
			{
				GD.Print("[ResourceCache] Mods disabled — skipping resource load for '" + modManifest.Name + "'.");
				continue;
			}
			if (!_enabledModIDs.Contains(modManifest.ID))
			{
				GD.Print("[ResourceCache] Mod '" + modManifest.ID + "' not in enabled list — skipping.");
				continue;
			}
			GD.Print($"[ResourceCache] Mod '{modManifest.Name}' v{modManifest.Version} — loading resources.");
			LoadModFiles(filePaths);
		}
	}

	private HashSet<string> GetModTreeEntriesRecursive(string path)
	{
		HashSet<string> hashSet = new HashSet<string>();
		string[] array = ResourceLoader.ListDirectory(path);
		if (array == null || array.Length == 0)
		{
			return hashSet;
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.EndsWith("/"))
			{
				hashSet.UnionWith(GetModTreeEntriesRecursive(path + text));
			}
			else
			{
				hashSet.Add(path + text);
			}
		}
		return hashSet;
	}

	private void LoadModFiles(List<string> filePaths)
	{
		foreach (string filePath in filePaths)
		{
			string text = filePath.TrimSuffix(".remap");
			string text2 = text.GetExtension().ToLower();
			string file = text.GetFile();
			if (file.ToLower() == "readme.txt")
			{
				continue;
			}
			string path = text.GetBaseDir() + "/";
			if (text2 == "tscn" || text2 == "scn")
			{
				PrefabTyping prefabTyping = DetectPrefabTypeFromPath(path);
				if (prefabTyping == PrefabTyping.UNTYPED)
				{
					GD.Print("[ResourceCache] Mod scene skipped (no subfolder mapping): " + text);
					continue;
				}
				PackedScene packedScene = GD.Load<PackedScene>(text);
				if (packedScene == null)
				{
					GD.PrintErr("[ResourceCache] Mod scene failed to load: " + text);
					continue;
				}
				string file2 = text.GetBaseName().GetFile();
				if (!prefabsLoaded.ContainsKey(prefabTyping))
				{
					prefabsLoaded[prefabTyping] = new Godot.Collections.Dictionary<string, PackedScene>();
				}
				if (!prefabsLoaded[prefabTyping].ContainsKey(file2))
				{
					prefabsLoaded[prefabTyping][file2] = packedScene;
					GD.Print($"[ResourceCache] Mod registered scene '{file2}' as {prefabTyping}");
				}
				else
				{
					GD.PrintErr("[ResourceCache] Mod scene '" + file2 + "' conflicts with existing prefab — skipped.");
				}
				continue;
			}
			Resource resource = GD.Load<Resource>(text);
			if (resource == null)
			{
				continue;
			}
			ResourceTyping resourceTyping = DetectResourceType(resource);
			if (resourceTyping == ResourceTyping.UNTYPED)
			{
				GD.Print("[ResourceCache] Mod resource not a known data type, skipping registration: " + text);
				continue;
			}
			string resourceID = GetResourceID(resource, file, path);
			if (resourceID == null)
			{
				GD.PrintErr("[ResourceCache] Mod resource rejected (null/invalid ID): " + text);
				continue;
			}
			if (!resourcesLoaded.ContainsKey(resourceTyping))
			{
				resourcesLoaded[resourceTyping] = new Godot.Collections.Dictionary<string, Resource>();
			}
			resourcesLoaded[resourceTyping][resourceID] = resource;
			GD.Print($"[ResourceCache] Mod registered '{resourceID}' as {resourceTyping}");
		}
	}

	private static ResourceTyping DetectResourceType(Resource res)
	{
		if (res is ItemDataRes)
		{
			return ResourceTyping.ITEM;
		}
		if (res is CharacterInfoDataRes)
		{
			return ResourceTyping.CHARACTER;
		}
		if (res is GalleryDataRes)
		{
			return ResourceTyping.GALLERY;
		}
		if (res is AttachDataRes attachDataRes)
		{
			if (attachDataRes.attachmentTyping == AttachDataRes.AttachmentType.RANDOM_CLICKED_WINDOW)
			{
				return ResourceTyping.SPAM;
			}
			if (attachDataRes.attachmentTyping == AttachDataRes.AttachmentType.OVERRIDE)
			{
				return ResourceTyping.H_SCENES;
			}
		}
		if (res is TA_WorldDataRes)
		{
			return ResourceTyping.BRAINDACE_WORLDS;
		}
		if (res is TAsk_AskDataRes)
		{
			return ResourceTyping.ASK_CHARACTERS;
		}
		return ResourceTyping.UNTYPED;
	}

	private static PrefabTyping DetectPrefabTypeFromPath(string path)
	{
		string[] array = path.Split(new char[2] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
		foreach (string key in array)
		{
			if (_prefabSubfolderMap.TryGetValue(key, out var value))
			{
				return value;
			}
		}
		return PrefabTyping.UNTYPED;
	}

	private static string GetResourceID(Resource res, string fileName, string path)
	{
		if (res is ItemDataRes itemDataRes)
		{
			if (!(itemDataRes.itemID == "INVALID_ID: Please Set Name"))
			{
				return itemDataRes.itemID;
			}
			return null;
		}
		if (res is CharacterInfoDataRes characterInfoDataRes)
		{
			if (!(characterInfoDataRes.itemID == "INVALID_ID: Please Set Name"))
			{
				return characterInfoDataRes.itemID;
			}
			return null;
		}
		if (res is GalleryDataRes galleryDataRes)
		{
			if (!(galleryDataRes.itemID == "INVALID_ID: Please Set Name"))
			{
				return galleryDataRes.itemID;
			}
			return null;
		}
		if (res is AttachDataRes attachDataRes)
		{
			if (!(attachDataRes.itemID == "INVALID_ID: Please Set Name"))
			{
				return attachDataRes.itemID;
			}
			return null;
		}
		if (res is TA_WorldDataRes tA_WorldDataRes)
		{
			if (!(tA_WorldDataRes.itemID == "INVALID_ID: Please Set Name"))
			{
				return tA_WorldDataRes.itemID;
			}
			return null;
		}
		if (res is TAsk_AskDataRes tAsk_AskDataRes)
		{
			if (tAsk_AskDataRes.askingCharacter == null)
			{
				return null;
			}
			if (!(tAsk_AskDataRes.askingCharacter.itemID == "INVALID_ID: Please Set Name"))
			{
				return tAsk_AskDataRes.askingCharacter.itemID;
			}
			return null;
		}
		return null;
	}

	private ModManifest ReadModManifest(string resPath, string packFileName)
	{
		if (!ResourceLoader.Exists(resPath))
		{
			GD.PrintErr("[ResourceCache] No manifest.json found in mod pack '" + packFileName + "'. Using defaults.");
			return new ModManifest
			{
				ID = "UNKNOWN",
				Name = packFileName
			};
		}
		FileAccess fileAccess = FileAccess.Open(resPath, FileAccess.ModeFlags.Read);
		if (fileAccess == null)
		{
			GD.PrintErr("[ResourceCache] Could not open manifest.json in '" + packFileName + "'. Using defaults.");
			return new ModManifest
			{
				ID = "UNKNOWN",
				Name = packFileName
			};
		}
		string asText = fileAccess.GetAsText();
		fileAccess.Close();
		Json json = new Json();
		if (json.Parse(asText) != Error.Ok)
		{
			GD.PrintErr($"[ResourceCache] manifest.json in '{packFileName}' has invalid JSON (line {json.GetErrorLine()}): {json.GetErrorMessage()}");
			return new ModManifest
			{
				ID = "UNKNOWN",
				Name = packFileName
			};
		}
		Dictionary dictionary = json.Data.AsGodotDictionary();
		Variant value;
		Variant value2;
		Variant value3;
		Variant value4;
		ModManifest modManifest = new ModManifest
		{
			ID = (dictionary.TryGetValue("ID", out value) ? value.AsString() : "UNKNOWN"),
			Name = (dictionary.TryGetValue("Name", out value2) ? value2.AsString() : packFileName),
			Version = (dictionary.TryGetValue("Version", out value3) ? value3.AsString() : "0.0.0"),
			Description = (dictionary.TryGetValue("Description", out value4) ? value4.AsString() : "")
		};
		GD.Print($"[ResourceCache] Manifest read — ID:{modManifest.ID} Name:{modManifest.Name} v{modManifest.Version}");
		return modManifest;
	}

	private void ReadModSaveData()
	{
		if (_saveReadDone)
		{
			return;
		}
		_saveReadDone = true;
		string savePath = SaveHandler.GetSavePath();
		if (!FileAccess.FileExists(savePath))
		{
			GD.Print("[ResourceCache] No save file found — mods disabled.");
			return;
		}
		ConfigFile configFile = new ConfigFile();
		Error error = configFile.Load(savePath);
		if (error != Error.Ok)
		{
			GD.PrintErr($"[ResourceCache] Could not read save file for mod check: {error}");
			return;
		}
		if (!configFile.HasSectionKey("mods", "modsEnabled"))
		{
			GD.Print("[ResourceCache] No modsEnabled key in save — mods disabled.");
			return;
		}
		_modsEnabled = (bool)configFile.GetValue("mods", "modsEnabled", false);
		if (!_modsEnabled)
		{
			GD.Print("[ResourceCache] Mods are disabled in save.");
			return;
		}
		if (!configFile.HasSectionKey("mods", "enabledMods"))
		{
			GD.Print("[ResourceCache] modsEnabled is true but no enabledMods list found.");
			return;
		}
		foreach (string item in configFile.GetValue("mods", "enabledMods", new Array<string>()).AsGodotArray<string>())
		{
			_enabledModIDs.Add(item);
		}
		GD.Print($"[ResourceCache] Mods enabled. {_enabledModIDs.Count} mod(s) in enabled list.");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(11)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.LoadData, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.LoadItemData, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "ResourceType", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "path", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Bool, "deepSearch", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.LoadItemDataInternal, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "ResourceType", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "path", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Bool, "deepSearch", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Dictionary, "resources", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.LoadPrefabData, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "prefabType", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "path", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Bool, "deepSearch", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.LoadPrefabDataInternal, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "prefabType", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "path", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Bool, "deepSearch", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Dictionary, "prefabs", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.LoadMods, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.DetectResourceType, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "res", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.DetectPrefabTypeFromPath, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "path", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetResourceID, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "res", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.String, "fileName", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "path", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ReadModSaveData, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.LoadData && args.Count == 0)
		{
			LoadData();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LoadItemData && args.Count == 3)
		{
			LoadItemData(VariantUtils.ConvertTo<ResourceTyping>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<bool>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LoadItemDataInternal && args.Count == 4)
		{
			LoadItemDataInternal(VariantUtils.ConvertTo<ResourceTyping>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<bool>(in args[2]), VariantUtils.ConvertToDictionary<ResourceTyping, Godot.Collections.Dictionary<string, Resource>>(in args[3]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LoadPrefabData && args.Count == 3)
		{
			LoadPrefabData(VariantUtils.ConvertTo<PrefabTyping>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<bool>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LoadPrefabDataInternal && args.Count == 4)
		{
			LoadPrefabDataInternal(VariantUtils.ConvertTo<PrefabTyping>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<bool>(in args[2]), VariantUtils.ConvertToDictionary<PrefabTyping, Godot.Collections.Dictionary<string, PackedScene>>(in args[3]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LoadMods && args.Count == 0)
		{
			LoadMods();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.DetectResourceType && args.Count == 1)
		{
			ResourceTyping from = DetectResourceType(VariantUtils.ConvertTo<Resource>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.DetectPrefabTypeFromPath && args.Count == 1)
		{
			PrefabTyping from2 = DetectPrefabTypeFromPath(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.GetResourceID && args.Count == 3)
		{
			string from3 = GetResourceID(VariantUtils.ConvertTo<Resource>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<string>(in args[2]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.ReadModSaveData && args.Count == 0)
		{
			ReadModSaveData();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.DetectResourceType && args.Count == 1)
		{
			ResourceTyping from = DetectResourceType(VariantUtils.ConvertTo<Resource>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.DetectPrefabTypeFromPath && args.Count == 1)
		{
			PrefabTyping from2 = DetectPrefabTypeFromPath(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.GetResourceID && args.Count == 3)
		{
			string from3 = GetResourceID(VariantUtils.ConvertTo<Resource>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<string>(in args[2]));
			ret = VariantUtils.CreateFrom(in from3);
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
		if (method == MethodName.LoadData)
		{
			return true;
		}
		if (method == MethodName.LoadItemData)
		{
			return true;
		}
		if (method == MethodName.LoadItemDataInternal)
		{
			return true;
		}
		if (method == MethodName.LoadPrefabData)
		{
			return true;
		}
		if (method == MethodName.LoadPrefabDataInternal)
		{
			return true;
		}
		if (method == MethodName.LoadMods)
		{
			return true;
		}
		if (method == MethodName.DetectResourceType)
		{
			return true;
		}
		if (method == MethodName.DetectPrefabTypeFromPath)
		{
			return true;
		}
		if (method == MethodName.GetResourceID)
		{
			return true;
		}
		if (method == MethodName.ReadModSaveData)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.IsLoaded)
		{
			IsLoaded = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._saveReadDone)
		{
			_saveReadDone = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._modsEnabled)
		{
			_modsEnabled = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._loadStarted)
		{
			_loadStarted = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._totalLoadSteps)
		{
			_totalLoadSteps = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._completedLoadSteps)
		{
			_completedLoadSteps = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.IsLoaded)
		{
			bool from = IsLoaded;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName._saveReadDone)
		{
			value = VariantUtils.CreateFrom(in _saveReadDone);
			return true;
		}
		if (name == PropertyName._modsEnabled)
		{
			value = VariantUtils.CreateFrom(in _modsEnabled);
			return true;
		}
		if (name == PropertyName._loadStarted)
		{
			value = VariantUtils.CreateFrom(in _loadStarted);
			return true;
		}
		if (name == PropertyName._totalLoadSteps)
		{
			value = VariantUtils.CreateFrom(in _totalLoadSteps);
			return true;
		}
		if (name == PropertyName._completedLoadSteps)
		{
			value = VariantUtils.CreateFrom(in _completedLoadSteps);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Bool, PropertyName.IsLoaded, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._saveReadDone, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._modsEnabled, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._loadStarted, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._totalLoadSteps, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._completedLoadSteps, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName isLoaded = PropertyName.IsLoaded;
		bool from = IsLoaded;
		info.AddProperty(isLoaded, Variant.From(in from));
		info.AddProperty(PropertyName._saveReadDone, Variant.From(in _saveReadDone));
		info.AddProperty(PropertyName._modsEnabled, Variant.From(in _modsEnabled));
		info.AddProperty(PropertyName._loadStarted, Variant.From(in _loadStarted));
		info.AddProperty(PropertyName._totalLoadSteps, Variant.From(in _totalLoadSteps));
		info.AddProperty(PropertyName._completedLoadSteps, Variant.From(in _completedLoadSteps));
		info.AddSignalEventDelegate(SignalName.ResourceCacheLoaded, backing_ResourceCacheLoaded);
		info.AddSignalEventDelegate(SignalName.ResourceCacheProgressChanged, backing_ResourceCacheProgressChanged);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.IsLoaded, out var value))
		{
			IsLoaded = value.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._saveReadDone, out var value2))
		{
			_saveReadDone = value2.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._modsEnabled, out var value3))
		{
			_modsEnabled = value3.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._loadStarted, out var value4))
		{
			_loadStarted = value4.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._totalLoadSteps, out var value5))
		{
			_totalLoadSteps = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName._completedLoadSteps, out var value6))
		{
			_completedLoadSteps = value6.As<int>();
		}
		if (info.TryGetSignalEventDelegate<ResourceCacheLoadedEventHandler>(SignalName.ResourceCacheLoaded, out var value7))
		{
			backing_ResourceCacheLoaded = value7;
		}
		if (info.TryGetSignalEventDelegate<ResourceCacheProgressChangedEventHandler>(SignalName.ResourceCacheProgressChanged, out var value8))
		{
			backing_ResourceCacheProgressChanged = value8;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(2)
		{
			new MethodInfo(SignalName.ResourceCacheLoaded, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(SignalName.ResourceCacheProgressChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "progress", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	protected void EmitSignalResourceCacheLoaded()
	{
		EmitSignal(SignalName.ResourceCacheLoaded);
	}

	protected void EmitSignalResourceCacheProgressChanged(float progress)
	{
		EmitSignal(SignalName.ResourceCacheProgressChanged, progress);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.ResourceCacheLoaded && args.Count == 0)
		{
			backing_ResourceCacheLoaded?.Invoke();
		}
		else if (signal == SignalName.ResourceCacheProgressChanged && args.Count == 1)
		{
			backing_ResourceCacheProgressChanged?.Invoke(VariantUtils.ConvertTo<float>(in args[0]));
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.ResourceCacheLoaded)
		{
			return true;
		}
		if (signal == SignalName.ResourceCacheProgressChanged)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
