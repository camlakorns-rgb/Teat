using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/ItemScripts/ItemWindow.cs")]
public partial class ItemWindow : Window
{

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

	public Vector2 itemWindowVelocity = Vector2.Zero;

	public bool isThrown;

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
				if (position != base.Position) base.Position = position;
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
					Vector2I newPos = new Vector2I(base.Position.X, num3);
                if (newPos != base.Position) base.Position = newPos;
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
		if (Input.IsActionPressed("Move") && !Main.Instance.SomethingHasBeenGrabbed && new Rect2I(base.Position, base.Size).HasPoint(Main._isMobile ? (Vector2I)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()))
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

	public void UpdateCombinationShaders(bool enable)
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

	public void CombineItem(Rect2I thisRect)
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
		if (Input.IsActionJustPressed("Pet") && !selected && new Rect2I(base.Position, base.Size).HasPoint(Main._isMobile ? (Vector2I)Main.Instance.MobileMousePos() : DisplayServer.MouseGetPosition()))
		{
			Main.Instance.spawnedItems.Remove(this);
			CallDeferred("queue_free");
		}
	}

	public void UseOnMainActor()
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

	public void UseOnOtherActor(ActorWindow target, AiItemDataRes aiData)
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

}
