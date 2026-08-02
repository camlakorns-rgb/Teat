using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/ItemScripts/ItemWindow.cs")]
public class ItemWindow : Window
{
	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName DEBUG_LoadInScene = "DEBUG_LoadInScene";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName SetupItemWindow = "SetupItemWindow";

		public static readonly StringName FollowMouse = "FollowMouse";

		public static readonly StringName MoveItem = "MoveItem";

		public static readonly StringName UpdateCombinationShaders = "UpdateCombinationShaders";

		public static readonly StringName CombineItem = "CombineItem";

		public static readonly StringName PopItem = "PopItem";

		public static readonly StringName UseOnMainActor = "UseOnMainActor";

		public static readonly StringName UseOnOtherActor = "UseOnOtherActor";

		public static readonly StringName QueueDialogue = "QueueDialogue";

		public static readonly StringName UsePickedUpItem = "UsePickedUpItem";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName itemObject = "itemObject";

		public static readonly StringName spriteHolder = "spriteHolder";

		public static readonly StringName SpawnOnLoad = "SpawnOnLoad";

		public static readonly StringName cachedScreenIndex = "cachedScreenIndex";

		public static readonly StringName cachedScreenRect = "cachedScreenRect";

		public static readonly StringName cachedTaskbarPos = "cachedTaskbarPos";

		public static readonly StringName CurrentlyPickedUp = "CurrentlyPickedUp";

		public static readonly StringName isSetup = "isSetup";

		public static readonly StringName mouseOffset = "mouseOffset";

		public static readonly StringName selected = "selected";

		public static readonly StringName tiedShaderWindows = "tiedShaderWindows";

		public static readonly StringName itemWindowVelocity = "itemWindowVelocity";

		public static readonly StringName isThrown = "isThrown";

		public static readonly StringName itemLastMousePos = "itemLastMousePos";

		public static readonly StringName itemMouseVelocity = "itemMouseVelocity";
	}

	public new class SignalName : Window.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public ItemObjectHandler itemObject;

	[Export(PropertyHint.None, "")]
	public Node2D spriteHolder;

	[ExportGroup("Debug", "")]
	[Export(PropertyHint.None, "")]
	public bool SpawnOnLoad;

	private int cachedScreenIndex = -1;

	private Rect2I cachedScreenRect;

	private int cachedTaskbarPos;

	public bool CurrentlyPickedUp;

	private bool isSetup;

	private Vector2 mouseOffset = Vector2.Zero;

	private bool selected;

	private Array<ItemWindow> tiedShaderWindows = new Array<ItemWindow>();

	private Vector2 itemWindowVelocity = Vector2.Zero;

	private bool isThrown;

	private const float itemWindowGravity = 1400f;

	private const float itemWindowDamping = 0.995f;

	private Vector2 itemLastMousePos = Vector2.Zero;

	private Vector2 itemMouseVelocity = Vector2.Zero;

	public override void _Ready()
	{
		if (SpawnOnLoad && itemObject != null)
		{
			CallDeferred("DEBUG_LoadInScene");
		}
	}

	private void DEBUG_LoadInScene()
	{
		Main.Instance.spawnedItems.Add(this);
		SetupItemWindow();
	}

	public override void _Process(double delta)
	{
		if (!isSetup)
		{
			return;
		}
		if (CurrentlyPickedUp)
		{
			base.Visible = false;
			base.MousePassthrough = true;
			return;
		}
		base.MousePassthrough = Main.Instance.settingPassivePlayMode;
		if (selected)
		{
			FollowMouse();
		}
		else
		{
			int currentScreen = base.CurrentScreen;
			if (currentScreen != cachedScreenIndex)
			{
				cachedScreenIndex = currentScreen;
				cachedScreenRect = DisplayServer.ScreenGetUsableRect(currentScreen);
				cachedTaskbarPos = cachedScreenRect.End.Y - itemObject.trueSize.Y;
			}
			if (isThrown)
			{
				float num = (float)delta;
				itemWindowVelocity.Y += 1400f * num;
				itemWindowVelocity.X *= Mathf.Pow(0.995f, num * Main.Instance.windowAirResist);
				Vector2I position = new Vector2I(base.Position.X + Mathf.RoundToInt(itemWindowVelocity.X * num), base.Position.Y + Mathf.RoundToInt(itemWindowVelocity.Y * num));
				int num2 = Mathf.Clamp(position.X, cachedScreenRect.Position.X, cachedScreenRect.End.X - itemObject.trueSize.X);
				if (num2 != position.X)
				{
					itemWindowVelocity.X *= Main.Instance.windowBounceDamping;
					position.X = num2;
				}
				if (position.Y >= cachedTaskbarPos - 2)
				{
					position.Y = cachedTaskbarPos - 2;
					isThrown = false;
					itemWindowVelocity = Vector2.Zero;
				}
				base.Position = position;
			}
			else
			{
				int num3 = cachedTaskbarPos - 2;
				if (base.Position.Y < num3)
				{
					base.Position += new Vector2I(0, Mathf.RoundToInt(980f * (float)delta));
				}
				else if (base.Position.Y != num3)
				{
					base.Position = new Vector2I(base.Position.X, num3);
				}
			}
		}
		MoveItem();
		PopItem();
		if (!base.Visible)
		{
			base.Visible = true;
		}
	}

	public void SetupItemWindow(ItemDataRes itemData = null)
	{
		if (itemData != null)
		{
			itemObject.itemInformation = itemData;
		}
		itemObject.trueSize = (Vector2I)(itemObject.itemInformation.itemSize * itemObject.itemInformation.itemScale * Main.Instance.settingItemScaler);
		itemObject.SetupItem();
		base.MinSize = itemObject.trueSize;
		base.Size = base.MinSize;
		base.ProcessMode = ProcessModeEnum.Inherit;
		isSetup = true;
		if (!Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.ITEMS].Contains(itemObject.itemInformation.itemID))
		{
			Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.ITEMS].Add(itemObject.itemInformation.itemID);
			Main.Instance.saveHandler.SaveSettings();
		}
	}

	private void FollowMouse()
	{
		Vector2I point = DisplayServer.MouseGetPosition();
		Vector2 vector = DisplayServer.MouseGetPosition();
		itemMouseVelocity = (vector - itemLastMousePos) / (float)GetProcessDeltaTime();
		itemLastMousePos = vector;
		int screenCount = DisplayServer.GetScreenCount();
		for (int i = 0; i < screenCount; i++)
		{
			Rect2I rect2I = DisplayServer.ScreenGetUsableRect(i);
			if (rect2I.HasPoint(point))
			{
				int x = Mathf.Clamp((int)((float)point.X + mouseOffset.X), rect2I.Position.X, rect2I.Position.X + rect2I.Size.X - itemObject.trueSize.X);
				int y = Mathf.Clamp((int)((float)point.Y + mouseOffset.Y), rect2I.Position.Y, rect2I.End.Y - itemObject.trueSize.Y);
				base.Position = new Vector2I(x, y);
				return;
			}
		}
		base.Position = new Vector2I((int)((float)point.X + mouseOffset.X), (int)((float)point.Y + mouseOffset.Y));
	}

	private void MoveItem()
	{
		if (Input.IsActionPressed("Move") && !Main.Instance.SomethingHasBeenGrabbed && new Rect2I(base.Position, base.Size).HasPoint(DisplayServer.MouseGetPosition()))
		{
			if (itemObject.itemInformation.possiblePickUpDialogue.Count() > 0 && Main.Instance.mainCharacter.Visible && (float)GD.RandRange(0, 100) < itemObject.itemInformation.PerchentChanceOfDialogue)
			{
				DialogueDataRes dialogueDataRes = Main.Instance.PickDialogue(itemObject.itemInformation.possiblePickUpDialogue);
				if (dialogueDataRes != null)
				{
					Main.Instance.dialogueStack.Add(dialogueDataRes);
					Main.Instance.PopDialogueInStack(skipTimer: true);
				}
			}
			selected = true;
			Main.Instance.SomethingHasBeenGrabbed = true;
			Main.Instance.mainWindow.AlwaysOnTop = false;
			mouseOffset = DisplayServer.WindowGetPosition(GetWindowId()) - DisplayServer.MouseGetPosition();
			itemLastMousePos = DisplayServer.MouseGetPosition();
			itemMouseVelocity = Vector2.Zero;
			UpdateCombinationShaders(enable: true);
		}
		if (!Input.IsActionJustReleased("Move") || !Main.Instance.SomethingHasBeenGrabbed)
		{
			return;
		}
		if (selected)
		{
			if (Main.Instance.settingWindowThrowPhysics)
			{
				itemWindowVelocity = itemMouseVelocity * Main.Instance.mouseVelocityScaler;
				itemWindowVelocity.Y = Mathf.Clamp(itemWindowVelocity.Y, -1600f, float.MaxValue);
				isThrown = true;
			}
			UpdateCombinationShaders(enable: false);
			Rect2I rect2I = new Rect2I(Main.Instance.mainWindow.Position, Main.Instance.mainWindow.Size);
			Rect2I rect2I2 = new Rect2I(base.Position, base.Size);
			if (rect2I.Intersects(rect2I2) && Main.Instance.mainCharacter.Visible)
			{
				UseOnMainActor();
				if (!itemObject.itemInformation.isReusable)
				{
					selected = false;
					Main.Instance.TweenGrabRelease();
					Main.Instance.mainWindow.AlwaysOnTop = true;
					Main.Instance.spawnedItems.Remove(this);
					CallDeferred("queue_free");
				}
				else if (!Main.Instance.mainCharacter.Visible)
				{
					base.Visible = false;
				}
			}
			else
			{
				bool flag = false;
				if (itemObject.itemInformation.possibleUsableAIs.Count() > 0)
				{
					foreach (ActorWindow spawnedActor in Main.Instance.spawnedActors)
					{
						if (!GodotObject.IsInstanceValid(spawnedActor))
						{
							continue;
						}
						AiItemDataRes aiItemDataRes = null;
						foreach (AiItemDataRes possibleUsableAI in itemObject.itemInformation.possibleUsableAIs)
						{
							if (possibleUsableAI.targetActorsID == spawnedActor.characterActor.characterInformation._itemID)
							{
								aiItemDataRes = possibleUsableAI;
								break;
							}
						}
						if (aiItemDataRes == null)
						{
							continue;
						}
						Rect2I b = new Rect2I(spawnedActor.Position, spawnedActor.Size);
						if (rect2I2.Intersects(b) && spawnedActor.Visible)
						{
							UseOnOtherActor(spawnedActor, aiItemDataRes);
							flag = true;
							if (!itemObject.itemInformation.isReusable)
							{
								selected = false;
								Main.Instance.TweenGrabRelease();
								Main.Instance.mainWindow.AlwaysOnTop = true;
								Main.Instance.spawnedItems.Remove(this);
								CallDeferred("queue_free");
							}
							break;
						}
					}
				}
				if (!flag)
				{
					CombineItem(rect2I2);
				}
			}
		}
		selected = false;
		Main.Instance.TweenGrabRelease();
		Main.Instance.mainWindow.AlwaysOnTop = true;
	}

	private void UpdateCombinationShaders(bool enable)
	{
		if (!enable)
		{
			foreach (ItemWindow tiedShaderWindow in tiedShaderWindows)
			{
				ShaderMaterial obj = (ShaderMaterial)tiedShaderWindow.spriteHolder.Material;
				Color color = (Color)obj.GetShaderParameter("color");
				color.A = 0f;
				obj.SetShaderParameter("color", color);
			}
			tiedShaderWindows.Clear();
			return;
		}
		tiedShaderWindows.Clear();
		foreach (ItemWindow item in GetTree().GetNodesInGroup("ItemTag"))
		{
			if (item == this)
			{
				continue;
			}
			bool flag = false;
			foreach (CombinationDataRes possibleCombination in itemObject.itemInformation.possibleCombinations)
			{
				if (possibleCombination.requiredItem == item.itemObject.itemInformation && possibleCombination.outputItem != null)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (CombinationDataRes possibleCombination2 in item.itemObject.itemInformation.possibleCombinations)
				{
					if (possibleCombination2.requiredItem == itemObject.itemInformation && possibleCombination2.outputItem != null)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				ShaderMaterial obj2 = (ShaderMaterial)item.spriteHolder.Material;
				Color color2 = (Color)obj2.GetShaderParameter("color");
				color2.A = 1f;
				obj2.SetShaderParameter("color", color2);
				tiedShaderWindows.Add(item);
			}
		}
	}

	private void CombineItem(Rect2I thisRect)
	{
		foreach (ItemWindow spawnedItem in Main.Instance.spawnedItems)
		{
			if (spawnedItem == this)
			{
				continue;
			}
			Rect2I b = new Rect2I(spawnedItem.Position, spawnedItem.Size);
			if (!thisRect.Intersects(b))
			{
				continue;
			}
			CombinationDataRes combinationDataRes = null;
			foreach (CombinationDataRes possibleCombination in itemObject.itemInformation.possibleCombinations)
			{
				if (possibleCombination.requiredItem == spawnedItem.itemObject.itemInformation && possibleCombination.outputItem != null)
				{
					combinationDataRes = possibleCombination;
					break;
				}
			}
			if (combinationDataRes == null)
			{
				foreach (CombinationDataRes possibleCombination2 in spawnedItem.itemObject.itemInformation.possibleCombinations)
				{
					if (possibleCombination2.requiredItem == itemObject.itemInformation && possibleCombination2.outputItem != null)
					{
						combinationDataRes = possibleCombination2;
						break;
					}
				}
			}
			if (combinationDataRes != null)
			{
				Vector2I vector2I = (Vector2I)(combinationDataRes.outputItem.itemSize * combinationDataRes.outputItem.itemScale * Main.Instance.settingItemScaler);
				Vector2I spawningPosition = new Vector2I(Main.Instance.screenDataHandler.ClampAcrossAllScreensX((base.Position.X + spawnedItem.Position.X) / 2, vector2I.X), Main.Instance.screenDataHandler.ClampAcrossAllScreensY((base.Position.Y + spawnedItem.Position.Y) / 2 - 64));
				Main.Instance.CallItemSpawn(combinationDataRes.outputItem, spawningPosition);
				Main.Instance.spawnedItems.Remove(spawnedItem);
				spawnedItem.CallDeferred("queue_free");
				selected = false;
				Main.Instance.TweenGrabRelease();
				Main.Instance.mainWindow.AlwaysOnTop = true;
				Main.Instance.spawnedItems.Remove(this);
				CallDeferred("queue_free");
				break;
			}
		}
	}

	private void PopItem()
	{
		if (Input.IsActionJustPressed("Pet") && !selected && new Rect2I(base.Position, base.Size).HasPoint(DisplayServer.MouseGetPosition()))
		{
			Main.Instance.spawnedItems.Remove(this);
			CallDeferred("queue_free");
		}
	}

	private void UseOnMainActor()
	{
		GD.Print("Using Item: " + itemObject.itemInformation.itemID);
		Main.Instance.ClearAllAttachments();
		foreach (ItemDataRes.ItemTask itemTask in itemObject.itemInformation.itemTasks)
		{
			ItemDataRes.ItemTask from = itemTask;
			switch (from)
			{
			case ItemDataRes.ItemTask.RUN_ANIMATION:
				Main.Instance.CallCharacterForcedAnimation(itemObject.itemInformation.AnimationTies[GD.RandRange(0, itemObject.itemInformation.AnimationTies.Count() - 1)], (float)GD.RandRange(itemObject.itemInformation.RandomTimerAmount.X, itemObject.itemInformation.RandomTimerAmount.Y));
				break;
			case ItemDataRes.ItemTask.RUN_ATTACH:
			{
				if (itemObject.itemInformation.RandomAmountToAttach == -1)
				{
					foreach (AttachDataRes attachedDatum in itemObject.itemInformation.attachedData)
					{
						Main.Instance.CallCharacterAttachmentSpawn(attachedDatum);
					}
					break;
				}
				Array<AttachDataRes> array3 = new Array<AttachDataRes>(itemObject.itemInformation.attachedData.Where((AttachDataRes a) => a != null && !Main.Instance.IsBlacklisted(a.taggedKinks)).ToList());
				HashSet<AttachDataRes> hashSet = new HashSet<AttachDataRes>();
				int j = 0;
				Array<string> seenScenes = Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.NSFW_SCENES];
				float unseenWeightBonus = 50f;
				for (; j < itemObject.itemInformation.RandomAmountToAttach; j++)
				{
					Array<AttachDataRes> array4 = new Array<AttachDataRes>();
					foreach (AttachDataRes item in array3)
					{
						if (!hashSet.Contains(item))
						{
							array4.Add(item);
						}
					}
					if (array4.Count == 0)
					{
						GD.PrintErr("RUN_ATTACH: Not enough attachments to spawn!");
						break;
					}
					float num = array4.Sum(delegate(AttachDataRes a)
					{
						string baseName2 = a.ResourcePath.GetFile().GetBaseName();
						float num8 = (seenScenes.Contains(baseName2) ? 0f : unseenWeightBonus);
						return a.attachmentAppeanceWeight + num8;
					});
					float num2 = (float)GD.RandRange(0.0, num);
					float num3 = 0f;
					AttachDataRes attachDataRes3 = array4[array4.Count - 1];
					foreach (AttachDataRes item2 in array4)
					{
						string baseName = item2.ResourcePath.GetFile().GetBaseName();
						float num4 = (seenScenes.Contains(baseName) ? 0f : unseenWeightBonus);
						num3 += item2.attachmentAppeanceWeight + num4;
						if (num2 <= num3)
						{
							attachDataRes3 = item2;
							break;
						}
					}
					hashSet.Add(attachDataRes3);
					Main.Instance.CallCharacterAttachmentSpawn(attachDataRes3);
				}
				break;
			}
			case ItemDataRes.ItemTask.RUN_DIALOGUE:
				QueueDialogue();
				break;
			case ItemDataRes.ItemTask.SPAWN_ENEMY:
			{
				CharacterInfoDataRes choosenActor = itemObject.itemInformation.possibleSpawnedActors[GD.RandRange(0, itemObject.itemInformation.possibleSpawnedActors.Count() - 1)];
				if (choosenActor.AITyping == CharacterInfoDataRes.AITypes.COMPANION && (Main.Instance.spawnedCompanions.Any((ActorWindow companion) => companion.characterActor.characterInformation == choosenActor) || Main.Instance.spawnedCompanions.Count >= Main.Instance.companionLimit))
				{
					Main.Instance.dialogueStack = new Array<DialogueDataRes> { Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.COMPANION_LIMIT] };
					Main.Instance.PopDialogueInStack(skipTimer: true);
					return;
				}
				Main.Instance.CallActorSpawn(choosenActor);
				break;
			}
			case ItemDataRes.ItemTask.SPAWN_ITEM:
			{
				Vector2I spawningPosition = new Vector2I(Main.Instance.mainWindow.Position.X + Mathf.RoundToInt(Main.Instance.mainCharacter.trueSize.X / 2), Main.Instance.mainWindow.Position.Y + Mathf.RoundToInt(Main.Instance.mainCharacter.trueSize.Y / 2));
				if (itemObject.itemInformation.spawningItem.Count == 0)
				{
					GD.PrintErr("SPAWN_ITEM: No items configured to spawn!");
					break;
				}
				if (itemObject.itemInformation.ItemAmountToSpawn == -1)
				{
					foreach (KeyValuePair<ItemDataRes, float> item3 in itemObject.itemInformation.spawningItem)
					{
						Main.Instance.CallItemSpawn(item3.Key, spawningPosition);
					}
					break;
				}
				for (int k = 0; k < itemObject.itemInformation.ItemAmountToSpawn; k++)
				{
					float num5 = itemObject.itemInformation.spawningItem.Values.Sum();
					if (num5 <= 0f)
					{
						GD.PrintErr("SPAWN_ITEM: Item pool has no valid weight!");
						break;
					}
					float num6 = (float)GD.RandRange(0.0, num5);
					float num7 = 0f;
					ItemDataRes spawningItem = null;
					foreach (KeyValuePair<ItemDataRes, float> item4 in itemObject.itemInformation.spawningItem)
					{
						num7 += item4.Value;
						if (num6 <= num7)
						{
							spawningItem = item4.Key;
							break;
						}
					}
					Main.Instance.CallItemSpawn(spawningItem, spawningPosition);
				}
				break;
			}
			case ItemDataRes.ItemTask.SPAWN_POPUP:
			{
				Array<AttachDataRes> array = new Array<AttachDataRes>();
				if (itemObject.itemInformation.popupData != null && itemObject.itemInformation.popupData.Count > 0)
				{
					foreach (AttachDataRes popupDatum in itemObject.itemInformation.popupData)
					{
						if (popupDatum != null && !Main.Instance.IsBlacklisted(popupDatum.taggedKinks))
						{
							array.Add(popupDatum);
						}
					}
				}
				else if (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.SPAM))
				{
					foreach (KeyValuePair<string, Resource> item5 in ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.SPAM])
					{
						if (item5.Value is AttachDataRes attachDataRes && !Main.Instance.IsBlacklisted(attachDataRes.taggedKinks) && !attachDataRes.excludePopup)
						{
							array.Add(attachDataRes);
						}
					}
				}
				if (array.Count == 0)
				{
					GD.PrintErr("SPAWN_POPUP: No attachment data available to spawn!");
					break;
				}
				if (itemObject.itemInformation.RandomAmountToSpawn == -1)
				{
					foreach (AttachDataRes item6 in array)
					{
						Main.Instance.CallCharacterAttachmentSpawn(item6, unclearableAttachment: true);
					}
					break;
				}
				Godot.Collections.Dictionary<AttachDataRes, int> dictionary = new Godot.Collections.Dictionary<AttachDataRes, int>();
				for (int i = 0; i < itemObject.itemInformation.RandomAmountToSpawn; i++)
				{
					Array<AttachDataRes> array2 = new Array<AttachDataRes>();
					foreach (AttachDataRes item7 in array)
					{
						if (item7 != null && (!dictionary.ContainsKey(item7) || dictionary[item7] < itemObject.itemInformation.maxDuplicates))
						{
							array2.Add(item7);
						}
					}
					if (array2.Count == 0)
					{
						GD.PrintErr("Not Enough Items to Spawn!");
						break;
					}
					AttachDataRes attachDataRes2 = array2[GD.RandRange(0, array2.Count - 1)];
					if (!dictionary.ContainsKey(attachDataRes2))
					{
						dictionary[attachDataRes2] = 0;
					}
					dictionary[attachDataRes2]++;
					Main.Instance.CallCharacterAttachmentSpawn(attachDataRes2, unclearableAttachment: true);
				}
				break;
			}
			case ItemDataRes.ItemTask.SPAWN_MINIGAME:
				Main.Instance.CallMinigameSpawn(itemObject.itemInformation.minigameID);
				break;
			}
			SignalEventBus.Instance.EmitSignal(SignalEventBus.SignalName.ItemUsedOnMainActor, itemObject.itemInformation, Variant.From(in from));
		}
		if (itemObject.itemInformation.TagPair.Count() <= 0)
		{
			return;
		}
		foreach (TagDataRes item8 in itemObject.itemInformation.TagPair)
		{
			switch (item8.tagAction)
			{
			case TagDataRes.actionEnum.ADD:
				Main.Instance.mainCharacter.AddTag(item8);
				break;
			case TagDataRes.actionEnum.REMOVE:
				Main.Instance.mainCharacter.RemoveTag(item8);
				break;
			}
		}
	}

	private void UseOnOtherActor(ActorWindow target, AiItemDataRes aiData)
	{
		GD.Print("Using Item on Actor AS Sub Actor: " + itemObject.itemInformation.itemID);
		foreach (ItemDataRes.ItemTask itemTask in aiData.itemTasks)
		{
			ItemDataRes.ItemTask from = itemTask;
			switch (from)
			{
			case ItemDataRes.ItemTask.RUN_ANIMATION:
				target.characterActor.ForceMainBodyState(ActorCharacter.MainBodyStates.Forced_Animation, aiData.animationName);
				break;
			case ItemDataRes.ItemTask.RUN_ATTACH:
			{
				if (aiData.RandomAmountToAttach == -1)
				{
					foreach (AttachDataRes attachedSubDatum in aiData.attachedSubData)
					{
						Main.Instance.CallCharacterAttachmentSpawn(attachedSubDatum, unclearableAttachment: false, target);
					}
					break;
				}
				Array<AttachDataRes> array3 = new Array<AttachDataRes>(aiData.attachedSubData.Where((AttachDataRes a) => a != null && !Main.Instance.IsBlacklisted(a.taggedKinks)).ToList());
				HashSet<AttachDataRes> hashSet = new HashSet<AttachDataRes>();
				int k = 0;
				Array<string> seenScenes = Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.NSFW_SCENES];
				float unseenWeightBonus = 50f;
				for (; k < aiData.RandomAmountToAttach; k++)
				{
					Array<AttachDataRes> array4 = new Array<AttachDataRes>();
					foreach (AttachDataRes item in array3)
					{
						if (!hashSet.Contains(item))
						{
							array4.Add(item);
						}
					}
					if (array4.Count == 0)
					{
						GD.PrintErr("RUN_ATTACH (AI): Not enough attachments to spawn!");
						break;
					}
					float num4 = array4.Sum(delegate(AttachDataRes a)
					{
						string baseName2 = a.ResourcePath.GetFile().GetBaseName();
						float num8 = (seenScenes.Contains(baseName2) ? 0f : unseenWeightBonus);
						return a.attachmentAppeanceWeight + num8;
					});
					float num5 = (float)GD.RandRange(0.0, num4);
					float num6 = 0f;
					AttachDataRes attachDataRes2 = array4[array4.Count - 1];
					foreach (AttachDataRes item2 in array4)
					{
						string baseName = item2.ResourcePath.GetFile().GetBaseName();
						float num7 = (seenScenes.Contains(baseName) ? 0f : unseenWeightBonus);
						num6 += item2.attachmentAppeanceWeight + num7;
						if (num5 <= num6)
						{
							attachDataRes2 = item2;
							break;
						}
					}
					hashSet.Add(attachDataRes2);
					Main.Instance.CallCharacterAttachmentSpawn(attachDataRes2, unclearableAttachment: false, target);
				}
				break;
			}
			case ItemDataRes.ItemTask.RUN_DIALOGUE:
			{
				AiItemDataRes aiItemDataRes = (AiItemDataRes)aiData.Duplicate(deep: true);
				foreach (DialogueDataRes item3 in aiItemDataRes.dialogueSubStack)
				{
					if (item3.speakingActorID == "")
					{
						item3.speakingActorID = target.characterActor.characterInformation.itemID;
					}
				}
				QueueDialogue(aiItemDataRes);
				break;
			}
			case ItemDataRes.ItemTask.SPAWN_ENEMY:
			{
				CharacterInfoDataRes chosenActor = aiData.possibleSpawnedSubActors[GD.RandRange(0, aiData.possibleSpawnedSubActors.Count() - 1)];
				if (chosenActor.AITyping == CharacterInfoDataRes.AITypes.COMPANION && (Main.Instance.spawnedCompanions.Any((ActorWindow companion) => companion.characterActor.characterInformation == chosenActor) || Main.Instance.spawnedCompanions.Count >= Main.Instance.companionLimit))
				{
					Main.Instance.dialogueStack = new Array<DialogueDataRes> { Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.COMPANION_LIMIT] };
					Main.Instance.PopDialogueInStack(skipTimer: true);
					return;
				}
				Main.Instance.CallActorSpawn(chosenActor, new Vector2I(target.Position.X, target.Position.Y));
				break;
			}
			case ItemDataRes.ItemTask.SPAWN_ITEM:
			{
				Vector2I spawningPosition = new Vector2I(target.Position.X + Mathf.RoundToInt(target.Size.X / 2), target.Position.Y + Mathf.RoundToInt(target.Size.Y / 2));
				if (aiData.spawningItem.Count == 0)
				{
					GD.PrintErr("SPAWN_ITEM (AI): No items configured to spawn!");
					break;
				}
				if (aiData.ItemAmountToSpawn == -1)
				{
					foreach (KeyValuePair<ItemDataRes, float> item4 in aiData.spawningItem)
					{
						Main.Instance.CallItemSpawn(item4.Key, spawningPosition);
					}
					break;
				}
				for (int j = 0; j < aiData.ItemAmountToSpawn; j++)
				{
					float num = aiData.spawningItem.Values.Sum();
					if (num <= 0f)
					{
						GD.PrintErr("SPAWN_ITEM (AI): Item pool has no valid weight!");
						break;
					}
					float num2 = (float)GD.RandRange(0.0, num);
					float num3 = 0f;
					ItemDataRes spawningItem = null;
					foreach (KeyValuePair<ItemDataRes, float> item5 in aiData.spawningItem)
					{
						num3 += item5.Value;
						if (num2 <= num3)
						{
							spawningItem = item5.Key;
							break;
						}
					}
					Main.Instance.CallItemSpawn(spawningItem, spawningPosition);
				}
				break;
			}
			case ItemDataRes.ItemTask.SPAWN_POPUP:
			{
				if (itemObject.itemInformation.RandomAmountToSpawn == -1)
				{
					foreach (AttachDataRes popupDatum in itemObject.itemInformation.popupData)
					{
						if (!Main.Instance.IsBlacklisted(popupDatum.taggedKinks))
						{
							Main.Instance.CallCharacterAttachmentSpawn(popupDatum, unclearableAttachment: true);
						}
					}
					break;
				}
				Array<AttachDataRes> array = new Array<AttachDataRes>(itemObject.itemInformation.popupData.Where((AttachDataRes a) => !Main.Instance.IsBlacklisted(a.taggedKinks)).ToList());
				Godot.Collections.Dictionary<AttachDataRes, int> dictionary = new Godot.Collections.Dictionary<AttachDataRes, int>();
				for (int i = 0; i < itemObject.itemInformation.RandomAmountToSpawn; i++)
				{
					Array<AttachDataRes> array2 = new Array<AttachDataRes>();
					foreach (AttachDataRes item6 in array)
					{
						if (!dictionary.ContainsKey(item6) || dictionary[item6] < itemObject.itemInformation.maxDuplicates)
						{
							array2.Add(item6);
						}
					}
					if (array2.Count == 0)
					{
						GD.PrintErr("Not Enough Items to Spawn!");
						return;
					}
					AttachDataRes attachDataRes = array2[GD.RandRange(0, array2.Count - 1)];
					if (!dictionary.ContainsKey(attachDataRes))
					{
						dictionary[attachDataRes] = 0;
					}
					dictionary[attachDataRes]++;
					Main.Instance.CallCharacterAttachmentSpawn(attachDataRes, unclearableAttachment: true);
				}
				break;
			}
			case ItemDataRes.ItemTask.SPAWN_MINIGAME:
				Main.Instance.CallMinigameSpawn(itemObject.itemInformation.minigameID);
				break;
			case ItemDataRes.ItemTask.DESPAWN_SUB_ACTOR:
				target.CallDeferred("queue_free");
				break;
			case ItemDataRes.ItemTask.AGGRO_SUB_ACTOR:
			{
				bool flag = true;
				if (!Main.Instance.mainCharacter.Visible)
				{
					flag = false;
				}
				if ((target.characterActor.characterInformation.overrideAnimation == null || target.characterActor.characterInformation.overrideAnimation.Count() == 0) && (aiData.aggroAnimations == null || aiData.aggroAnimations.Count() == 0))
				{
					flag = false;
				}
				if (!target.Visible)
				{
					flag = false;
				}
				if (target.inUse || target.inUseByAttachment)
				{
					flag = false;
				}
				if (flag)
				{
					target.inAggro = true;
					if (aiData.aggroAnimations == null || aiData.aggroAnimations.Count() <= 0)
					{
						break;
					}
					target.possibleAggroOverrideAnimations.Clear();
					foreach (AttachDataRes aggroAnimation in aiData.aggroAnimations)
					{
						target.possibleAggroOverrideAnimations.Add((AttachDataRes)aggroAnimation.Duplicate(deep: true));
					}
				}
				else
				{
					Main.Instance.CallItemSpawn(itemObject.itemInformation, new Vector2I(target.Position.X + Mathf.RoundToInt(target.Size.X / 2), target.Position.Y + Mathf.RoundToInt(target.Size.Y / 2)));
					if (Main.Instance.mainCharacter.Visible)
					{
						Main.Instance.dialogueStack.Clear();
						Main.Instance.dialogueStack.Add(new DialogueDataRes("Sorry, I can't accept that right now!", target.characterActor.characterInformation.itemID, new Color("ffffff")));
						Main.Instance.PopDialogueInStack(skipTimer: true);
					}
				}
				break;
			}
			case ItemDataRes.ItemTask.ENEMY_SUB_ACTOR:
			{
				CharacterInfoDataRes spawningActor = aiData.possibleSpawnedSubActors[GD.RandRange(0, aiData.possibleSpawnedSubActors.Count() - 1)];
				Main.Instance.CallActorSpawn(spawningActor, Vector2I.Zero, target);
				break;
			}
			}
			SignalEventBus.Instance.EmitSignal(SignalEventBus.SignalName.ItemUsedOnSubActor, itemObject.itemInformation, Variant.From(in from));
		}
		if (itemObject.itemInformation.TagPair.Count() <= 0)
		{
			return;
		}
		foreach (TagDataRes item7 in itemObject.itemInformation.TagPair)
		{
			switch (item7.tagAction)
			{
			case TagDataRes.actionEnum.ADD:
				Main.Instance.mainCharacter.AddTag(item7);
				break;
			case TagDataRes.actionEnum.REMOVE:
				Main.Instance.mainCharacter.RemoveTag(item7);
				break;
			}
		}
	}

	private void QueueDialogue(AiItemDataRes aiData = null)
	{
		Main.Instance.dialogueStack.Clear();
		Array<DialogueDataRes> array = ((aiData != null) ? aiData.dialogueSubStack : itemObject.itemInformation.dialogueStack);
		if (aiData?.RandomDialogueSubAssignment ?? itemObject.itemInformation.RandomDialogueAssignment)
		{
			DialogueDataRes dialogueDataRes = Main.Instance.PickDialogue(array);
			if (dialogueDataRes != null)
			{
				Main.Instance.dialogueStack = new Array<DialogueDataRes> { dialogueDataRes };
				Main.Instance.PopDialogueInStack(skipTimer: true);
			}
			return;
		}
		Main.Instance.dialogueStack = new Array<DialogueDataRes>(array.Where(delegate(DialogueDataRes d)
		{
			if (Main.Instance.IsBlacklisted(d.taggedKinks))
			{
				return false;
			}
			foreach (TagDataRes requiredTag in d.RequiredTags)
			{
				TagDataRes tag = Main.Instance.mainCharacter.GetTag(requiredTag.tagName);
				if (tag == null || (requiredTag.tagAmount != 0 && tag.tagAmount < requiredTag.tagAmount))
				{
					return false;
				}
			}
			return true;
		}).ToList());
		if (Main.Instance.dialogueStack.Count > 0)
		{
			Main.Instance.PopDialogueInStack(skipTimer: true);
		}
	}

	public void UsePickedUpItem()
	{
		UseOnMainActor();
		if (!itemObject.itemInformation.isReusable)
		{
			Main.Instance.spawnedItems.Remove(this);
			if (GodotObject.IsInstanceValid(this) && !IsQueuedForDeletion())
			{
				CallDeferred("queue_free");
			}
		}
		else if (!Main.Instance.mainCharacter.Visible)
		{
			base.Visible = false;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(13)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.DEBUG_LoadInScene, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetupItemWindow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "itemData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.FollowMouse, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.MoveItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateCombinationShaders, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "enable", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CombineItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Rect2I, "thisRect", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PopItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UseOnMainActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UseOnOtherActor, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "target", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false),
				new PropertyInfo(Variant.Type.Object, "aiData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.QueueDialogue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "aiData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.UsePickedUpItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.DEBUG_LoadInScene && args.Count == 0)
		{
			DEBUG_LoadInScene();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupItemWindow && args.Count == 1)
		{
			SetupItemWindow(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FollowMouse && args.Count == 0)
		{
			FollowMouse();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MoveItem && args.Count == 0)
		{
			MoveItem();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateCombinationShaders && args.Count == 1)
		{
			UpdateCombinationShaders(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CombineItem && args.Count == 1)
		{
			CombineItem(VariantUtils.ConvertTo<Rect2I>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PopItem && args.Count == 0)
		{
			PopItem();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UseOnMainActor && args.Count == 0)
		{
			UseOnMainActor();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UseOnOtherActor && args.Count == 2)
		{
			UseOnOtherActor(VariantUtils.ConvertTo<ActorWindow>(in args[0]), VariantUtils.ConvertTo<AiItemDataRes>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.QueueDialogue && args.Count == 1)
		{
			QueueDialogue(VariantUtils.ConvertTo<AiItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UsePickedUpItem && args.Count == 0)
		{
			UsePickedUpItem();
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
		if (method == MethodName.DEBUG_LoadInScene)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.SetupItemWindow)
		{
			return true;
		}
		if (method == MethodName.FollowMouse)
		{
			return true;
		}
		if (method == MethodName.MoveItem)
		{
			return true;
		}
		if (method == MethodName.UpdateCombinationShaders)
		{
			return true;
		}
		if (method == MethodName.CombineItem)
		{
			return true;
		}
		if (method == MethodName.PopItem)
		{
			return true;
		}
		if (method == MethodName.UseOnMainActor)
		{
			return true;
		}
		if (method == MethodName.UseOnOtherActor)
		{
			return true;
		}
		if (method == MethodName.QueueDialogue)
		{
			return true;
		}
		if (method == MethodName.UsePickedUpItem)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.itemObject)
		{
			itemObject = VariantUtils.ConvertTo<ItemObjectHandler>(in value);
			return true;
		}
		if (name == PropertyName.spriteHolder)
		{
			spriteHolder = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName.SpawnOnLoad)
		{
			SpawnOnLoad = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.cachedScreenIndex)
		{
			cachedScreenIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.cachedScreenRect)
		{
			cachedScreenRect = VariantUtils.ConvertTo<Rect2I>(in value);
			return true;
		}
		if (name == PropertyName.cachedTaskbarPos)
		{
			cachedTaskbarPos = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.CurrentlyPickedUp)
		{
			CurrentlyPickedUp = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.isSetup)
		{
			isSetup = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.mouseOffset)
		{
			mouseOffset = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.selected)
		{
			selected = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.tiedShaderWindows)
		{
			tiedShaderWindows = VariantUtils.ConvertToArray<ItemWindow>(in value);
			return true;
		}
		if (name == PropertyName.itemWindowVelocity)
		{
			itemWindowVelocity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.isThrown)
		{
			isThrown = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.itemLastMousePos)
		{
			itemLastMousePos = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.itemMouseVelocity)
		{
			itemMouseVelocity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.itemObject)
		{
			value = VariantUtils.CreateFrom(in itemObject);
			return true;
		}
		if (name == PropertyName.spriteHolder)
		{
			value = VariantUtils.CreateFrom(in spriteHolder);
			return true;
		}
		if (name == PropertyName.SpawnOnLoad)
		{
			value = VariantUtils.CreateFrom(in SpawnOnLoad);
			return true;
		}
		if (name == PropertyName.cachedScreenIndex)
		{
			value = VariantUtils.CreateFrom(in cachedScreenIndex);
			return true;
		}
		if (name == PropertyName.cachedScreenRect)
		{
			value = VariantUtils.CreateFrom(in cachedScreenRect);
			return true;
		}
		if (name == PropertyName.cachedTaskbarPos)
		{
			value = VariantUtils.CreateFrom(in cachedTaskbarPos);
			return true;
		}
		if (name == PropertyName.CurrentlyPickedUp)
		{
			value = VariantUtils.CreateFrom(in CurrentlyPickedUp);
			return true;
		}
		if (name == PropertyName.isSetup)
		{
			value = VariantUtils.CreateFrom(in isSetup);
			return true;
		}
		if (name == PropertyName.mouseOffset)
		{
			value = VariantUtils.CreateFrom(in mouseOffset);
			return true;
		}
		if (name == PropertyName.selected)
		{
			value = VariantUtils.CreateFrom(in selected);
			return true;
		}
		if (name == PropertyName.tiedShaderWindows)
		{
			value = VariantUtils.CreateFromArray(tiedShaderWindows);
			return true;
		}
		if (name == PropertyName.itemWindowVelocity)
		{
			value = VariantUtils.CreateFrom(in itemWindowVelocity);
			return true;
		}
		if (name == PropertyName.isThrown)
		{
			value = VariantUtils.CreateFrom(in isThrown);
			return true;
		}
		if (name == PropertyName.itemLastMousePos)
		{
			value = VariantUtils.CreateFrom(in itemLastMousePos);
			return true;
		}
		if (name == PropertyName.itemMouseVelocity)
		{
			value = VariantUtils.CreateFrom(in itemMouseVelocity);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.itemObject, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spriteHolder, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Debug", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.SpawnOnLoad, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.cachedScreenIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Rect2I, PropertyName.cachedScreenRect, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.cachedTaskbarPos, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.CurrentlyPickedUp, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isSetup, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.mouseOffset, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.selected, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.tiedShaderWindows, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.itemWindowVelocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isThrown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.itemLastMousePos, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.itemMouseVelocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.itemObject, Variant.From(in itemObject));
		info.AddProperty(PropertyName.spriteHolder, Variant.From(in spriteHolder));
		info.AddProperty(PropertyName.SpawnOnLoad, Variant.From(in SpawnOnLoad));
		info.AddProperty(PropertyName.cachedScreenIndex, Variant.From(in cachedScreenIndex));
		info.AddProperty(PropertyName.cachedScreenRect, Variant.From(in cachedScreenRect));
		info.AddProperty(PropertyName.cachedTaskbarPos, Variant.From(in cachedTaskbarPos));
		info.AddProperty(PropertyName.CurrentlyPickedUp, Variant.From(in CurrentlyPickedUp));
		info.AddProperty(PropertyName.isSetup, Variant.From(in isSetup));
		info.AddProperty(PropertyName.mouseOffset, Variant.From(in mouseOffset));
		info.AddProperty(PropertyName.selected, Variant.From(in selected));
		info.AddProperty(PropertyName.tiedShaderWindows, Variant.CreateFrom(tiedShaderWindows));
		info.AddProperty(PropertyName.itemWindowVelocity, Variant.From(in itemWindowVelocity));
		info.AddProperty(PropertyName.isThrown, Variant.From(in isThrown));
		info.AddProperty(PropertyName.itemLastMousePos, Variant.From(in itemLastMousePos));
		info.AddProperty(PropertyName.itemMouseVelocity, Variant.From(in itemMouseVelocity));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.itemObject, out var value))
		{
			itemObject = value.As<ItemObjectHandler>();
		}
		if (info.TryGetProperty(PropertyName.spriteHolder, out var value2))
		{
			spriteHolder = value2.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName.SpawnOnLoad, out var value3))
		{
			SpawnOnLoad = value3.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.cachedScreenIndex, out var value4))
		{
			cachedScreenIndex = value4.As<int>();
		}
		if (info.TryGetProperty(PropertyName.cachedScreenRect, out var value5))
		{
			cachedScreenRect = value5.As<Rect2I>();
		}
		if (info.TryGetProperty(PropertyName.cachedTaskbarPos, out var value6))
		{
			cachedTaskbarPos = value6.As<int>();
		}
		if (info.TryGetProperty(PropertyName.CurrentlyPickedUp, out var value7))
		{
			CurrentlyPickedUp = value7.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.isSetup, out var value8))
		{
			isSetup = value8.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.mouseOffset, out var value9))
		{
			mouseOffset = value9.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.selected, out var value10))
		{
			selected = value10.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.tiedShaderWindows, out var value11))
		{
			tiedShaderWindows = value11.AsGodotArray<ItemWindow>();
		}
		if (info.TryGetProperty(PropertyName.itemWindowVelocity, out var value12))
		{
			itemWindowVelocity = value12.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.isThrown, out var value13))
		{
			isThrown = value13.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.itemLastMousePos, out var value14))
		{
			itemLastMousePos = value14.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.itemMouseVelocity, out var value15))
		{
			itemMouseVelocity = value15.As<Vector2>();
		}
	}
}
