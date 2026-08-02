using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAdventure/TerminalAdventure.cs")]
public class TerminalAdventure : Node
{
	private class CheckpointData
	{
		public TA_RoomDataRes Room;

		public List<TA_ItemDataRes> Inventory;

		public System.Collections.Generic.Dictionary<TA_RoomDataRes, List<TA_ItemDataRes>> RoomItems;

		public System.Collections.Generic.Dictionary<TA_NPCDataRes, List<TA_ItemDataRes>> NpcItems;

		public List<string> Keys;

		public List<TA_NPCDataRes> Party;
	}

	public new class MethodName : Node.MethodName
	{
		public static readonly StringName ResetForNewWorld = "ResetForNewWorld";

		public static readonly StringName Start = "Start";

		public static readonly StringName WireExits = "WireExits";

		public static readonly StringName ParseAdventureInput = "ParseAdventureInput";

		public static readonly StringName HandleSubmit = "HandleSubmit";

		public static readonly StringName HandleGo = "HandleGo";

		public static readonly StringName HandleExamine = "HandleExamine";

		public static readonly StringName HandleGet = "HandleGet";

		public static readonly StringName HandleGive = "HandleGive";

		public static readonly StringName HandleAsk = "HandleAsk";

		public static readonly StringName HandleHint = "HandleHint";

		public static readonly StringName HandleMap = "HandleMap";

		public static readonly StringName HandleCheckpoint = "HandleCheckpoint";

		public static readonly StringName ProcessTopicSideEffects = "ProcessTopicSideEffects";

		public static readonly StringName HandleHello = "HandleHello";

		public static readonly StringName HandleUse = "HandleUse";

		public static readonly StringName HandleRecruit = "HandleRecruit";

		public static readonly StringName PrintKeys = "PrintKeys";

		public static readonly StringName Restart = "Restart";

		public static readonly StringName GrantKey = "GrantKey";

		public static readonly StringName HasKey = "HasKey";

		public static readonly StringName TryMoveToRoom = "TryMoveToRoom";

		public static readonly StringName TryEnterRoom = "TryEnterRoom";

		public static readonly StringName MoveToRoom = "MoveToRoom";

		public static readonly StringName PrintRoom = "PrintRoom";

		public static readonly StringName PrintExits = "PrintExits";

		public static readonly StringName PrintInventory = "PrintInventory";

		public static readonly StringName PrintParty = "PrintParty";

		public static readonly StringName MakeLink = "MakeLink";

		public static readonly StringName PrintHelp = "PrintHelp";

		public static readonly StringName FullReset = "FullReset";

		public static readonly StringName SpawnItem = "SpawnItem";

		public static readonly StringName SpawnScenePopup = "SpawnScenePopup";

		public static readonly StringName ResolveTopic = "ResolveTopic";

		public static readonly StringName TopicMatches = "TopicMatches";

		public static readonly StringName FindTopic = "FindTopic";

		public static readonly StringName FindTopicInList = "FindTopicInList";

		public static readonly StringName FindNPC = "FindNPC";

		public static readonly StringName RemoveRoomItem = "RemoveRoomItem";

		public static readonly StringName RemoveNPCItem = "RemoveNPCItem";

		public static readonly StringName InitMutableState = "InitMutableState";

		public static readonly StringName CommitLine = "CommitLine";

		public static readonly StringName EscapeBB = "EscapeBB";
	}

	public new class PropertyName : Node.PropertyName
	{
		public static readonly StringName Handler = "Handler";

		public static readonly StringName World = "World";

		public static readonly StringName _currentRoom = "_currentRoom";

		public static readonly StringName _listOfKeys = "_listOfKeys";

		public static readonly StringName _party = "_party";

		public static readonly StringName _started = "_started";

		public static readonly StringName _pendingGameOver = "_pendingGameOver";
	}

	public new class SignalName : Node.SignalName
	{
	}

	public TA_WorldDataRes World;

	private TA_RoomDataRes _currentRoom;

	private List<TA_ItemDataRes> _inventory = new List<TA_ItemDataRes>();

	private System.Collections.Generic.Dictionary<TA_RoomDataRes, List<TA_ItemDataRes>> _roomItems = new System.Collections.Generic.Dictionary<TA_RoomDataRes, List<TA_ItemDataRes>>();

	private System.Collections.Generic.Dictionary<TA_NPCDataRes, List<TA_ItemDataRes>> _npcItems = new System.Collections.Generic.Dictionary<TA_NPCDataRes, List<TA_ItemDataRes>>();

	private Array<string> _listOfKeys = new Array<string>();

	private Array<TA_NPCDataRes> _party = new Array<TA_NPCDataRes>();

	private CheckpointData _checkpoint;

	private bool _started;

	private bool _pendingGameOver;

	private const string CmdQuit = "quit";

	private const string CmdLook = "look";

	private const string CmdInventory = "inventory";

	private const string CmdHello = "hello";

	private const string CmdExamine = "examine";

	private const string CmdGo = "go";

	private const string CmdGet = "get";

	private const string CmdTake = "take";

	private const string CmdGrab = "grab";

	private const string CmdGive = "give";

	private const string CmdAsk = "ask";

	private const string CmdRestart = "restart";

	private const string CmdHelp = "help";

	private const string CmdSubmit = "submit";

	private const string CmdUse = "use";

	private const string CmdKeylist = "keys";

	private const string CmdRecruit = "recruit";

	private const string CmdParty = "party";

	private const string CmdHint = "hint";

	private const string CmdMap = "map";

	private const string CmdCheckpoint = "checkpoint";

	private TerminalHandler Handler => GetParent<TerminalHandler>();

	public void ResetForNewWorld()
	{
		_started = false;
		_inventory.Clear();
		_listOfKeys.Clear();
		_party.Clear();
		_roomItems.Clear();
		_npcItems.Clear();
		_currentRoom = null;
		_pendingGameOver = false;
	}

	public void Start()
	{
		if (!_started)
		{
			InitMutableState();
			_currentRoom = World.StartRoom;
			_inventory.Clear();
			_started = true;
			if (!string.IsNullOrWhiteSpace(World.IntroText))
			{
				CommitLine(World.IntroText);
			}
		}
		WireExits();
		PrintRoom(_currentRoom);
	}

	private void WireExits()
	{
		if (World == null)
		{
			return;
		}
		foreach (TA_ExitDataRes exit in World.Exits)
		{
			if (exit.From != null && exit.To != null)
			{
				exit.From.Exits[exit.Label] = exit.To;
			}
		}
	}

	public bool ParseAdventureInput(string rawInput)
	{
		string text = rawInput.Trim();
		if (text.Length == 0)
		{
			return true;
		}
		_pendingGameOver = false;
		string[] array = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		string text2 = array[0].ToLower();
		string text3 = ((array.Length > 1) ? string.Join(" ", array.Skip(1)).ToLower() : "");
		switch (text2)
		{
		case "quit":
		case "exit":
			return false;
		case "restart":
			Restart();
			return true;
		case "help":
			PrintHelp();
			return true;
		case "look":
			PrintRoom(_currentRoom);
			return true;
		case "inv":
		case "i":
		case "inventory":
			PrintInventory();
			return true;
		case "hello":
		case "greet":
		case "hi":
			HandleHello(text3);
			return true;
		case "examine":
		case "inspect":
		case "x":
		case "describe":
			HandleExamine(text3);
			return true;
		case "move":
		case "walk":
		case "go":
		case "travel":
			HandleGo(text3);
			return true;
		case "take":
		case "grab":
		case "pick":
		case "get":
			if (text2 == "pick" && text3.StartsWith("up "))
			{
				text3 = text3.Substring(3).Trim();
			}
			HandleGet(text3);
			return !_pendingGameOver;
		case "hand":
		case "give":
		case "offer":
			HandleGive(text3);
			return !_pendingGameOver;
		case "enquire":
		case "inquire":
		case "ask":
		case "query":
			HandleAsk(text3);
			return !_pendingGameOver;
		case "recruit":
		case "enlist":
			HandleRecruit(text3);
			return true;
		case "members":
		case "party":
		case "companions":
			PrintParty();
			return true;
		case "use":
			HandleUse(text3);
			return !_pendingGameOver;
		case "keys":
			PrintKeys();
			return true;
		case "submit":
			return HandleSubmit(text3);
		case "hint":
		case "clue":
		case "tip":
			HandleHint();
			return true;
		case "map":
			HandleMap();
			return true;
		case "checkpoint":
			HandleCheckpoint(text3);
			return true;
		case "load":
			HandleCheckpoint("load");
			return true;
		case "save":
			HandleCheckpoint("save");
			return true;
		default:
			if (TryMoveToRoom(text2 + ((text3.Length > 0) ? (" " + text3) : "")))
			{
				return true;
			}
			CommitLine("  [color=red]Speak actual Words pup, I don't understand that command '" + EscapeBB(text) + "'.[/color]");
			CommitLine("  Type HELP for a list of commands.");
			CommitLine("");
			return true;
		}
	}

	private bool HandleSubmit(string rest = "")
	{
		List<TA_NPCDataRes> visibleNPCs = GetVisibleNPCs(_currentRoom);
		if (visibleNPCs.Count == 0)
		{
			CommitLine("  This command isn't usable here.");
			CommitLine("");
			return true;
		}
		if (!string.IsNullOrWhiteSpace(rest))
		{
			TA_NPCDataRes tA_NPCDataRes = FindNPC(_currentRoom, rest);
			if (tA_NPCDataRes == null)
			{
				CommitLine("  There's no '" + EscapeBB(rest) + "' here.");
				CommitLine("");
				return true;
			}
			TA_TopicDataRes tA_TopicDataRes = ResolveTopic(tA_NPCDataRes, "submit");
			if (tA_TopicDataRes == null || tA_TopicDataRes.Response == null)
			{
				CommitLine("  This command isn't usable here.");
				CommitLine("");
				return true;
			}
			CommitLine("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + ":[/b]  " + tA_TopicDataRes.Response);
			CommitLine("");
			ProcessTopicSideEffects(tA_TopicDataRes);
			return !_pendingGameOver;
		}
		TA_NPCDataRes speaker;
		TA_TopicDataRes tA_TopicDataRes2 = ResolveTopicAcrossRoom(visibleNPCs, "submit", out speaker);
		if (tA_TopicDataRes2 != null && tA_TopicDataRes2.Response != null)
		{
			CommitLine("  [b]" + EscapeBB(speaker.NPCName) + ":[/b]  " + tA_TopicDataRes2.Response);
			CommitLine("");
			ProcessTopicSideEffects(tA_TopicDataRes2);
			return !_pendingGameOver;
		}
		CommitLine("  This command isn't usable here.");
		CommitLine("");
		return true;
	}

	private void HandleGo(string destination)
	{
		if (string.IsNullOrWhiteSpace(destination))
		{
			CommitLine("  Go where? Name a connected room or direction.");
			CommitLine("");
		}
		else if (!TryMoveToRoom(destination))
		{
			CommitLine("  You can't go '" + EscapeBB(destination) + "' from here.");
			PrintExits(_currentRoom);
			CommitLine("");
		}
	}

	private void HandleExamine(string target)
	{
		if (string.IsNullOrWhiteSpace(target))
		{
			CommitLine("  Examine what?");
			CommitLine("");
			return;
		}
		TA_ItemDataRes tA_ItemDataRes = FindItemInList(_inventory, target);
		if (tA_ItemDataRes != null)
		{
			CommitLine("  [b]" + EscapeBB(tA_ItemDataRes.ItemName) + "[/b]");
			CommitLine("  " + tA_ItemDataRes.Description);
			CommitLine("");
			SpawnScenePopup(tA_ItemDataRes.ExaminePopup, tA_ItemDataRes.ExaminePopupID);
			return;
		}
		TA_ItemDataRes tA_ItemDataRes2 = FindItemInList(GetRoomItems(_currentRoom), target);
		if (tA_ItemDataRes2 != null)
		{
			CommitLine("  [b]" + EscapeBB(tA_ItemDataRes2.ItemName) + "[/b]");
			CommitLine("  " + tA_ItemDataRes2.Description);
			CommitLine("");
			SpawnScenePopup(tA_ItemDataRes2.ExaminePopup, tA_ItemDataRes2.ExaminePopupID);
			return;
		}
		foreach (TA_NPCDataRes visibleNPC in GetVisibleNPCs(_currentRoom))
		{
			TA_ItemDataRes tA_ItemDataRes3 = FindItemInList(GetNPCItems(visibleNPC), target);
			if (tA_ItemDataRes3 != null)
			{
				CommitLine($"  [b]{EscapeBB(tA_ItemDataRes3.ItemName)}[/b]  [color=grey](held by {EscapeBB(visibleNPC.NPCName)})[/color]");
				CommitLine("  " + tA_ItemDataRes3.Description);
				CommitLine("");
				SpawnScenePopup(tA_ItemDataRes3.ExaminePopup, tA_ItemDataRes3.ExaminePopupID);
				return;
			}
		}
		TA_NPCDataRes tA_NPCDataRes = FindNPC(_currentRoom, target);
		if (tA_NPCDataRes != null)
		{
			CommitLine("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + "[/b]");
			CommitLine("  " + tA_NPCDataRes.Description);
			List<TA_ItemDataRes> nPCItems = GetNPCItems(tA_NPCDataRes);
			if (nPCItems.Count > 0)
			{
				CommitLine("  Carrying: " + string.Join(", ", nPCItems.Select((TA_ItemDataRes i) => i.ItemName)));
			}
			CommitLine("");
			SpawnScenePopup(tA_NPCDataRes.ScenePopup, tA_NPCDataRes.ScenePopupID);
			return;
		}
		if (_currentRoom != null)
		{
			switch (target)
			{
			default:
				if (!_currentRoom.RoomName.ToLower().Contains(target))
				{
					break;
				}
				goto case "room";
			case "room":
			case "here":
			case "around":
				PrintRoom(_currentRoom);
				return;
			}
		}
		CommitLine("  You see nothing noteworthy about '" + EscapeBB(target) + "'.");
		CommitLine("");
	}

	private void HandleGet(string target)
	{
		if (string.IsNullOrWhiteSpace(target))
		{
			CommitLine("  Take what?");
			CommitLine("");
			return;
		}
		foreach (TA_NPCDataRes visibleNPC in GetVisibleNPCs(_currentRoom))
		{
			List<TA_ItemDataRes> nPCItems = GetNPCItems(visibleNPC);
			TA_ItemDataRes tA_ItemDataRes = FindItemInList(nPCItems, target);
			if (tA_ItemDataRes == null)
			{
				continue;
			}
			if (!tA_ItemDataRes.Takeable)
			{
				CommitLine($"  {EscapeBB(visibleNPC.NPCName)} doesn't let you take the {EscapeBB(tA_ItemDataRes.ItemName)}.");
				CommitLine("");
				return;
			}
			RemoveNPCItem(visibleNPC, tA_ItemDataRes);
			_inventory.Add(tA_ItemDataRes);
			CommitLine($"  You take the {EscapeBB(tA_ItemDataRes.ItemName)} from {EscapeBB(visibleNPC.NPCName)}.");
			CommitLine("");
			if (!string.IsNullOrWhiteSpace(tA_ItemDataRes.GiveKey))
			{
				GrantKey(tA_ItemDataRes.GiveKey);
			}
			if (tA_ItemDataRes.GameOverOnPickup)
			{
				FullReset();
				_pendingGameOver = true;
			}
			return;
		}
		List<TA_ItemDataRes> roomItems = GetRoomItems(_currentRoom);
		TA_ItemDataRes tA_ItemDataRes2 = FindItemInList(roomItems, target);
		if (tA_ItemDataRes2 == null)
		{
			CommitLine("  There's no '" + EscapeBB(target) + "' here to take.");
			CommitLine("");
			return;
		}
		if (!tA_ItemDataRes2.Takeable)
		{
			CommitLine("  You can't take the " + EscapeBB(tA_ItemDataRes2.ItemName) + ".");
			CommitLine("");
			return;
		}
		RemoveRoomItem(_currentRoom, tA_ItemDataRes2);
		_inventory.Add(tA_ItemDataRes2);
		CommitLine("  You pick up the " + EscapeBB(tA_ItemDataRes2.ItemName) + ".");
		CommitLine("");
		if (!string.IsNullOrWhiteSpace(tA_ItemDataRes2.GiveKey))
		{
			GrantKey(tA_ItemDataRes2.GiveKey);
		}
		if (tA_ItemDataRes2.GameOverOnPickup)
		{
			FullReset();
			_pendingGameOver = true;
		}
	}

	private void HandleGive(string target)
	{
		if (string.IsNullOrWhiteSpace(target))
		{
			CommitLine("  Give what to whom? e.g. GIVE key to merchant");
			CommitLine("");
			return;
		}
		string text = target;
		string text2 = "";
		int num = target.IndexOf(" to ", StringComparison.OrdinalIgnoreCase);
		if (num >= 0)
		{
			text = target.Substring(0, num).Trim();
			text2 = target.Substring(num + 4).Trim();
		}
		else
		{
			string[] array = target.Split(' ');
			if (array.Length >= 2)
			{
				text2 = array[^1];
				text = string.Join(" ", array.Take(array.Length - 1));
			}
		}
		TA_ItemDataRes item = FindItemInList(_inventory, text);
		if (item == null)
		{
			CommitLine("  You don't have a '" + EscapeBB(text) + "'.");
			CommitLine("");
			return;
		}
		if (string.IsNullOrWhiteSpace(text2))
		{
			CommitLine("  Give it to whom?");
			CommitLine("");
			return;
		}
		TA_NPCDataRes tA_NPCDataRes = FindNPC(_currentRoom, text2);
		if (tA_NPCDataRes == null)
		{
			CommitLine("  There's no '" + EscapeBB(text2) + "' here to give that to.");
			CommitLine("");
			return;
		}
		TA_TradeDataRes matchedTrade = tA_NPCDataRes.Trades.FirstOrDefault((TA_TradeDataRes t) => t.WantedItem != null && item.ItemName.Equals(t.WantedItem.ItemName, StringComparison.OrdinalIgnoreCase));
		if (matchedTrade != null)
		{
			_inventory.Remove(item);
			if (!string.IsNullOrWhiteSpace(item.GiveKey) && _listOfKeys.Contains(item.GiveKey))
			{
				_listOfKeys.Remove(item.GiveKey);
			}
			TA_ItemDataRes tA_ItemDataRes = null;
			List<TA_ItemDataRes> nPCItems = GetNPCItems(tA_NPCDataRes);
			if (matchedTrade.TradeRewardItem != null)
			{
				tA_ItemDataRes = nPCItems.FirstOrDefault((TA_ItemDataRes i) => i == matchedTrade.TradeRewardItem);
				if (tA_ItemDataRes != null)
				{
					RemoveNPCItem(tA_NPCDataRes, tA_ItemDataRes);
					_inventory.Add(tA_ItemDataRes);
				}
			}
			string newValue = ((tA_ItemDataRes != null) ? tA_ItemDataRes.ItemName : "nothing");
			string text3 = matchedTrade.TradeDialogue.Replace("{item}", item.ItemName).Replace("{reward}", newValue);
			CommitLine("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + ":[/b]  " + text3);
			if (!string.IsNullOrWhiteSpace(matchedTrade.TradeRewardInfo))
			{
				CommitLine("");
				CommitLine("  " + matchedTrade.TradeRewardInfo);
			}
			CommitLine("");
			if (matchedTrade.GiveItem != null)
			{
				SpawnItem(matchedTrade.GiveItem);
			}
			else if (matchedTrade.GiveItemID != "" && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].ContainsKey(matchedTrade.GiveItemID) && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][matchedTrade.GiveItemID] is ItemDataRes itemData)
			{
				SpawnItem(itemData);
			}
			if (!string.IsNullOrWhiteSpace(matchedTrade.GiveKey))
			{
				GrantKey(matchedTrade.GiveKey);
			}
			SpawnScenePopup(matchedTrade.ScenePopup, matchedTrade.ScenePopupID);
			if (matchedTrade.GameOver)
			{
				FullReset();
				_pendingGameOver = true;
			}
		}
		else if (tA_NPCDataRes.RejectedItems.ContainsKey(item))
		{
			CommitLine("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + ":[/b]  " + tA_NPCDataRes.RejectedItems[item].Replace("{item}", item.ItemName));
			CommitLine("");
		}
		else
		{
			CommitLine("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + ":[/b]  " + tA_NPCDataRes.RefuseItemDialogue.Replace("{item}", item.ItemName));
			CommitLine("");
		}
	}

	private void HandleAsk(string rest)
	{
		if (string.IsNullOrWhiteSpace(rest))
		{
			CommitLine("  Ask whom about what? e.g. ASK merchant ABOUT sword  or  ASK ABOUT sword");
			CommitLine("");
			return;
		}
		List<TA_NPCDataRes> visibleNPCs = GetVisibleNPCs(_currentRoom);
		if (visibleNPCs.Count == 0)
		{
			CommitLine("  There's no one here to ask.");
			CommitLine("");
			return;
		}
		string text = "";
		string text2 = rest;
		int num = rest.IndexOf(" about ", StringComparison.OrdinalIgnoreCase);
		if (num >= 0)
		{
			string text3 = rest.Substring(0, num).Trim();
			text2 = rest.Substring(num + 7).Trim();
			if (text3.Length > 0 && !text3.Equals("about", StringComparison.OrdinalIgnoreCase))
			{
				text = text3;
			}
		}
		else
		{
			string[] array = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			for (int i = 1; i < array.Length; i++)
			{
				string text4 = string.Join(" ", array.Take(i));
				if (FindNPC(_currentRoom, text4) != null)
				{
					text = text4;
					text2 = string.Join(" ", array.Skip(i));
					break;
				}
			}
		}
		if (string.IsNullOrWhiteSpace(text2))
		{
			CommitLine("  Ask about what?");
			CommitLine("");
		}
		else if (!string.IsNullOrWhiteSpace(text))
		{
			TA_NPCDataRes tA_NPCDataRes = FindNPC(_currentRoom, text);
			if (tA_NPCDataRes == null)
			{
				CommitLine("  There's no '" + EscapeBB(text) + "' here.");
				CommitLine("");
				return;
			}
			TA_TopicDataRes tA_TopicDataRes = ResolveTopic(tA_NPCDataRes, text2);
			string text5 = tA_TopicDataRes?.Response;
			CommitLine((text5 != null) ? ("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + ":[/b]  " + text5) : ("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + ":[/b]  " + tA_NPCDataRes.UnknownTopicDialogue));
			CommitLine("");
			if (text5 != null && tA_TopicDataRes != null)
			{
				ProcessTopicSideEffects(tA_TopicDataRes);
			}
		}
		else
		{
			TA_NPCDataRes speaker;
			TA_TopicDataRes tA_TopicDataRes2 = ResolveTopicAcrossRoom(visibleNPCs, text2, out speaker);
			if (tA_TopicDataRes2 != null && tA_TopicDataRes2.Response != null)
			{
				CommitLine("  [b]" + EscapeBB(speaker.NPCName) + ":[/b]  " + tA_TopicDataRes2.Response);
				CommitLine("");
				ProcessTopicSideEffects(tA_TopicDataRes2);
			}
			else
			{
				TA_NPCDataRes tA_NPCDataRes2 = visibleNPCs[0];
				CommitLine("  [b]" + EscapeBB(tA_NPCDataRes2.NPCName) + ":[/b]  " + tA_NPCDataRes2.UnknownTopicDialogue);
				CommitLine("");
			}
		}
	}

	private void HandleHint()
	{
		if (_currentRoom == null || string.IsNullOrWhiteSpace(_currentRoom.Hint))
		{
			CommitLine("  There are no hints for this area.");
			CommitLine("");
		}
		else
		{
			CommitLine("  [color=yellow][b]Hint:[/b]  " + _currentRoom.Hint + "[/color]");
			CommitLine("");
		}
	}

	private void HandleMap()
	{
		if (World?.mapData == null)
		{
			CommitLine("  There is no map available for this adventure.");
			CommitLine("");
		}
		else
		{
			CommitLine("  [b]You consult the map.[/b]");
			CommitLine("");
			SpawnScenePopup(World.mapData, "");
		}
	}

	private void HandleCheckpoint(string rest)
	{
		string text = rest.Trim().ToLower();
		if (!(text == "save"))
		{
			if (text == "load")
			{
				if (_checkpoint == null)
				{
					CommitLine("  [color=red]No checkpoint to load.[/color]");
					CommitLine("");
					return;
				}
				_currentRoom = _checkpoint.Room;
				_inventory = _checkpoint.Inventory.ToList();
				_roomItems = _checkpoint.RoomItems.ToDictionary((KeyValuePair<TA_RoomDataRes, List<TA_ItemDataRes>> kvp) => kvp.Key, (KeyValuePair<TA_RoomDataRes, List<TA_ItemDataRes>> kvp) => kvp.Value.ToList());
				_npcItems = _checkpoint.NpcItems.ToDictionary((KeyValuePair<TA_NPCDataRes, List<TA_ItemDataRes>> kvp) => kvp.Key, (KeyValuePair<TA_NPCDataRes, List<TA_ItemDataRes>> kvp) => kvp.Value.ToList());
				_listOfKeys = new Array<string>(_checkpoint.Keys);
				_party = new Array<TA_NPCDataRes>(_checkpoint.Party);
				_started = true;
				CommitLine("  [color=green][b]Checkpoint loaded.[/b][/color]");
				CommitLine("");
				PrintRoom(_currentRoom);
			}
			else
			{
				CommitLine("  Usage:  CHECKPOINT SAVE  /  CHECKPOINT LOAD / SAVE / LOAD");
				CommitLine("");
			}
		}
		else
		{
			_checkpoint = new CheckpointData
			{
				Room = _currentRoom,
				Inventory = _inventory.ToList(),
				RoomItems = _roomItems.ToDictionary((KeyValuePair<TA_RoomDataRes, List<TA_ItemDataRes>> kvp) => kvp.Key, (KeyValuePair<TA_RoomDataRes, List<TA_ItemDataRes>> kvp) => kvp.Value.ToList()),
				NpcItems = _npcItems.ToDictionary((KeyValuePair<TA_NPCDataRes, List<TA_ItemDataRes>> kvp) => kvp.Key, (KeyValuePair<TA_NPCDataRes, List<TA_ItemDataRes>> kvp) => kvp.Value.ToList()),
				Keys = _listOfKeys.ToList(),
				Party = _party.ToList()
			};
			CommitLine("  [color=green][b]Checkpoint saved.[/b][/color]");
			CommitLine("");
		}
	}

	private void ProcessTopicSideEffects(TA_TopicDataRes topic)
	{
		if (topic.GiveItem != null)
		{
			SpawnItem(topic.GiveItem);
		}
		else if (topic?.GiveItemID != "" && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].ContainsKey(topic.GiveItemID) && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][topic.GiveItemID] is ItemDataRes itemData)
		{
			SpawnItem(itemData);
		}
		if (!string.IsNullOrWhiteSpace(topic.GiveKey))
		{
			GrantKey(topic.GiveKey);
		}
		SpawnScenePopup(topic.ScenePopup, topic.ScenePopupID);
		if (topic.GameOver)
		{
			FullReset();
			_pendingGameOver = true;
		}
	}

	private void HandleHello(string target)
	{
		if (_currentRoom == null || GetVisibleNPCs(_currentRoom).Count == 0)
		{
			CommitLine("  There's no one here to greet.");
			CommitLine("");
			return;
		}
		TA_NPCDataRes tA_NPCDataRes = null;
		if (string.IsNullOrWhiteSpace(target))
		{
			if (GetVisibleNPCs(_currentRoom).Count != 1)
			{
				CommitLine("  Who do you want to greet?");
				foreach (TA_NPCDataRes visibleNPC in GetVisibleNPCs(_currentRoom))
				{
					CommitLine("    - " + visibleNPC.NPCName);
				}
				CommitLine("");
				return;
			}
			tA_NPCDataRes = GetVisibleNPCs(_currentRoom)[0];
		}
		else
		{
			tA_NPCDataRes = FindNPC(_currentRoom, target);
		}
		if (tA_NPCDataRes == null)
		{
			CommitLine("  There's no '" + EscapeBB(target) + "' here.");
			CommitLine("");
		}
		else
		{
			CommitLine("  [b]" + EscapeBB(tA_NPCDataRes.NPCName) + ":[/b]  " + tA_NPCDataRes.GreetingDialogue);
			CommitLine("");
		}
	}

	private void HandleUse(string target)
	{
		if (string.IsNullOrWhiteSpace(target))
		{
			CommitLine("  Use what?");
			CommitLine("");
			return;
		}
		TA_ItemDataRes tA_ItemDataRes = FindItemInList(_inventory, target);
		if (tA_ItemDataRes == null)
		{
			tA_ItemDataRes = FindItemInList(GetRoomItems(_currentRoom), target);
		}
		if (tA_ItemDataRes == null)
		{
			CommitLine("  There's no '" + EscapeBB(target) + "' here to use.");
			CommitLine("");
			return;
		}
		if (string.IsNullOrWhiteSpace(tA_ItemDataRes.UseDescription))
		{
			CommitLine("  Nothing happens when you use " + EscapeBB(tA_ItemDataRes.ItemName) + ".");
			CommitLine("");
			return;
		}
		CommitLine("  [b]" + EscapeBB(tA_ItemDataRes.ItemName) + "[/b]");
		CommitLine("  " + tA_ItemDataRes.UseDescription);
		CommitLine("");
		if (!string.IsNullOrWhiteSpace(tA_ItemDataRes.GiveKey))
		{
			GrantKey(tA_ItemDataRes.GiveKey);
		}
		SpawnScenePopup(tA_ItemDataRes.UsePopup, tA_ItemDataRes.UsePopupID);
		if (tA_ItemDataRes.GameOverOnUse)
		{
			FullReset();
			_pendingGameOver = true;
		}
	}

	private void HandleRecruit(string target)
	{
		if (_currentRoom == null || GetVisibleNPCs(_currentRoom).Count == 0)
		{
			CommitLine("  There's no one here to recruit.");
			CommitLine("");
			return;
		}
		TA_NPCDataRes tA_NPCDataRes = null;
		if (string.IsNullOrWhiteSpace(target))
		{
			List<TA_NPCDataRes> list = (from n in GetVisibleNPCs(_currentRoom)
				where n.Recruitable && !_party.Contains(n)
				select n).ToList();
			if (list.Count != 1)
			{
				if (list.Count == 0)
				{
					CommitLine("  There's no one here willing to join you.");
					CommitLine("");
					return;
				}
				CommitLine("  Who do you want to recruit?");
				foreach (TA_NPCDataRes item in list)
				{
					CommitLine("    - " + item.NPCName);
				}
				CommitLine("");
				return;
			}
			tA_NPCDataRes = list[0];
		}
		else
		{
			tA_NPCDataRes = FindNPC(_currentRoom, target);
		}
		if (tA_NPCDataRes == null)
		{
			CommitLine("  There's no '" + EscapeBB(target) + "' here.");
			CommitLine("");
			return;
		}
		if (_party.Contains(tA_NPCDataRes))
		{
			CommitLine("  " + EscapeBB(tA_NPCDataRes.NPCName) + " is already in your party.");
			CommitLine("");
			return;
		}
		if (!tA_NPCDataRes.Recruitable)
		{
			CommitLine("  " + EscapeBB(tA_NPCDataRes.NPCName) + " has no interest in joining you.");
			CommitLine("");
			return;
		}
		if (!string.IsNullOrWhiteSpace(tA_NPCDataRes.RecruitableKey))
		{
			bool flag = false;
			foreach (TA_ItemDataRes item2 in _inventory)
			{
				if (item2.ItemName.Equals(tA_NPCDataRes.RecruitableKey, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				foreach (string listOfKey in _listOfKeys)
				{
					if (listOfKey.Equals(tA_NPCDataRes.RecruitableKey, StringComparison.OrdinalIgnoreCase))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				CommitLine("  " + EscapeBB(tA_NPCDataRes.NPCName) + " isn't ready to join you yet.");
				CommitLine("");
				return;
			}
		}
		_party.Add(tA_NPCDataRes);
		CommitLine("  " + EscapeBB(tA_NPCDataRes.NPCName) + " has joined your party!");
		CommitLine("");
	}

	private void PrintKeys()
	{
		if (_listOfKeys.Count == 0)
		{
			CommitLine("  [color=red]You have no keys.[/color]");
		}
		else
		{
			CommitLine("  [b]Keys obtained[/b]:");
			foreach (string listOfKey in _listOfKeys)
			{
				CommitLine("    - " + listOfKey);
			}
		}
		CommitLine("");
	}

	private void Restart()
	{
		InitMutableState();
		_currentRoom = World.StartRoom;
		_inventory.Clear();
		_listOfKeys.Clear();
		_party.Clear();
		CommitLine("  --- Adventure restarted. ---");
		CommitLine("");
		PrintRoom(_currentRoom);
	}

	private void GrantKey(string key)
	{
		if (!string.IsNullOrWhiteSpace(key) && !_listOfKeys.Contains(key))
		{
			_listOfKeys.Add(key);
			CommitLine("  [b]Obtained Key[/b]: " + key);
			CommitLine("");
		}
	}

	private bool HasKey(string key)
	{
		if (!string.IsNullOrWhiteSpace(key))
		{
			return _listOfKeys.Contains(key);
		}
		return true;
	}

	private bool TryMoveToRoom(string destination)
	{
		if (_currentRoom == null)
		{
			return false;
		}
		string text = destination.Trim().ToLower();
		foreach (KeyValuePair<string, TA_RoomDataRes> exit in _currentRoom.Exits)
		{
			if (exit.Key.ToLower() == text)
			{
				return TryEnterRoom(exit.Value);
			}
		}
		foreach (KeyValuePair<string, TA_RoomDataRes> exit2 in _currentRoom.Exits)
		{
			if (exit2.Value != null && exit2.Value.RoomName.ToLower() == text)
			{
				return TryEnterRoom(exit2.Value);
			}
		}
		TA_RoomDataRes room = null;
		int num = 0;
		foreach (KeyValuePair<string, TA_RoomDataRes> exit3 in _currentRoom.Exits)
		{
			if (exit3.Key.ToLower().StartsWith(text) || (exit3.Value != null && exit3.Value.RoomName.ToLower().StartsWith(text)))
			{
				room = exit3.Value;
				num++;
			}
		}
		if (num == 1)
		{
			return TryEnterRoom(room);
		}
		return false;
	}

	private bool TryEnterRoom(TA_RoomDataRes room)
	{
		if (room == null)
		{
			CommitLine("  [color=red]That path leads nowhere.[/color]");
			CommitLine("");
			return true;
		}
		if (!string.IsNullOrWhiteSpace(room.requiredKey) && !HasKey(room.requiredKey))
		{
			string text = room.lockedDescription.Replace("{KEY}", room.requiredKey);
			CommitLine("  " + text);
			CommitLine("");
			return true;
		}
		MoveToRoom(room);
		return true;
	}

	private void MoveToRoom(TA_RoomDataRes room)
	{
		if (room == null)
		{
			CommitLine("  [color=red]That path leads nowhere.[/color]");
			CommitLine("");
		}
		else
		{
			_currentRoom = room;
			PrintRoom(_currentRoom);
		}
	}

	private void PrintRoom(TA_RoomDataRes room)
	{
		if (room == null)
		{
			CommitLine("  [color=red]Error: current room is null.[/color]");
			return;
		}
		CommitLine("");
		CommitLine("  [b][u]" + EscapeBB(room.RoomName).ToUpper() + "[/u][/b]");
		CommitLine("  " + room.Description);
		CommitLine("");
		List<TA_ItemDataRes> roomItems = GetRoomItems(room);
		if (roomItems.Count > 0)
		{
			IEnumerable<string> values = roomItems.Select((TA_ItemDataRes i) => MakeLink("examine " + i.ItemName, i.ItemName));
			CommitLine("  [b]Objects of Interest[/b]: " + string.Join(", ", values));
		}
		List<TA_NPCDataRes> visibleNPCs = GetVisibleNPCs(room);
		if (visibleNPCs.Count > 0)
		{
			IEnumerable<string> values2 = visibleNPCs.Select((TA_NPCDataRes n) => MakeLink("examine " + n.NPCName, (_party.Contains(n) ? "(P) " : "") + n.NPCName));
			CommitLine("  [b]People[/b]: " + string.Join(", ", values2));
		}
		PrintExits(room);
		CommitLine("");
		SpawnScenePopup(room.ScenePopup, room.ScenePopupID);
	}

	private void PrintExits(TA_RoomDataRes room)
	{
		if (room.Exits.Count == 0)
		{
			CommitLine("  [b]Exits[/b]: none");
			return;
		}
		IEnumerable<string> values = from kvp in room.Exits
			where kvp.Value != null
			where !kvp.Value.HiddenUntilKey || HasKey(kvp.Value.RequiredVisibilityKey)
			select MakeLink("go " + kvp.Key, kvp.Key) + " (" + EscapeBB(kvp.Value.RoomName) + ")";
		CommitLine("  [b]Exits[/b]: " + string.Join("  |  ", values));
	}

	private void PrintInventory()
	{
		if (_inventory.Count == 0)
		{
			CommitLine("  Your hands are empty.");
		}
		else
		{
			CommitLine("  You are carrying:");
			foreach (TA_ItemDataRes item in _inventory)
			{
				CommitLine("    - " + MakeLink("examine " + item.ItemName, item.ItemName));
			}
		}
		CommitLine("");
	}

	private void PrintParty()
	{
		if (_party.Count == 0)
		{
			CommitLine("  You are travelling alone.");
		}
		else
		{
			CommitLine("  [b]Party Members[/b]:");
			foreach (TA_NPCDataRes item in _party)
			{
				CommitLine("    - " + MakeLink("examine " + item.NPCName, item.NPCName));
			}
		}
		CommitLine("");
	}

	private string MakeLink(string command, string label)
	{
		string value = command.Replace(" ", "~");
		return $"[url={value}][color=cyan]{EscapeBB(label)}[/color][/url]";
	}

	private void PrintHelp()
	{
		CommitLine("");
		CommitLine("  LOOK                          Describe the current room.");
		CommitLine("  GO <room/direction>           Move to a connected room.");
		CommitLine("  <room name>                   Shorthand movement.");
		CommitLine("  EXAMINE <target>              Inspect an item, NPC, or the room.");
		CommitLine("  INVENTORY  (or I)             List what you're carrying.");
		CommitLine("  TAKE <item>                   Pick up an item from the room or an NPC.");
		CommitLine("  GIVE <item> TO <npc>          Hand an item to a character.");
		CommitLine("  ASK <npc> ABOUT <topic>       Ask a specific character about something.");
		CommitLine("  ASK ABOUT <topic>             Ask anyone in the room about something.");
		CommitLine("  HELLO [npc]                   Greet a character.");
		CommitLine("  USE <item>                    Use an item from your inventory.");
		CommitLine("  KEYS                          List all keys you have obtained.");
		CommitLine("  RECRUIT [npc]                 Recruit a willing NPC to your party.");
		CommitLine("  PARTY                         List your current party members.");
		CommitLine("  SUBMIT [npc]                  Submit to the will of those around you.");
		CommitLine("  MAP                           Open the world map.");
		CommitLine("  HINT                          Get a hint for the current room.");
		CommitLine("  CHECKPOINT <Save/Load>        Saves or Loads your current progress. [color=red]Does NOT save between games[/color]");
		CommitLine("  RESTART                       Reset the adventure from the beginning.");
		CommitLine("  QUIT                          Return to the terminal.");
		CommitLine("");
	}

	private void FullReset()
	{
		InitMutableState();
		_currentRoom = World.StartRoom;
		_inventory.Clear();
		_listOfKeys.Clear();
		_party.Clear();
		_started = false;
	}

	private void SpawnItem(ItemDataRes itemData)
	{
		if (itemData != null && Main.Instance != null)
		{
			_ = (Vector2I)(itemData.itemSize * itemData.itemScale * Main.Instance.settingSpriteScaler);
			Main.Instance.CallItemSpawn(itemData, new Vector2I(Main.Instance.mainWindow.Position.X + Mathf.RoundToInt(Main.Instance.mainCharacter.trueSize.X / 2), Main.Instance.mainWindow.Position.Y + Mathf.RoundToInt(Main.Instance.mainCharacter.trueSize.Y / 2)));
		}
	}

	private void SpawnScenePopup(AttachDataRes scenePopup, string scenePopupID)
	{
		if (scenePopup != null)
		{
			Main.Instance?.CallCharacterAttachmentSpawn(scenePopup, unclearableAttachment: true);
		}
		else if (!string.IsNullOrEmpty(scenePopupID) && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.SPAM].ContainsKey(scenePopupID) && ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.SPAM][scenePopupID] is AttachDataRes objData)
		{
			Main.Instance?.CallCharacterAttachmentSpawn(objData, unclearableAttachment: true);
		}
	}

	private TA_TopicDataRes ResolveTopicAcrossRoom(List<TA_NPCDataRes> npcs, string topicQuery, out TA_NPCDataRes speaker)
	{
		foreach (TA_NPCDataRes npc in npcs)
		{
			TA_TopicDataRes tA_TopicDataRes = ResolveActorPresentTopic(npc, npcs, topicQuery);
			if (tA_TopicDataRes != null)
			{
				speaker = npc;
				return tA_TopicDataRes;
			}
		}
		foreach (TA_NPCDataRes npc2 in npcs)
		{
			if (_party.Contains(npc2))
			{
				TA_TopicDataRes tA_TopicDataRes2 = FindTopicInList(npc2.PartyDialogue, topicQuery);
				if (tA_TopicDataRes2 != null)
				{
					speaker = npc2;
					return tA_TopicDataRes2;
				}
			}
		}
		foreach (TA_NPCDataRes npc3 in npcs)
		{
			TA_TopicDataRes tA_TopicDataRes3 = FindTopic(npc3, topicQuery);
			if (tA_TopicDataRes3 != null)
			{
				speaker = npc3;
				return tA_TopicDataRes3;
			}
		}
		speaker = null;
		return null;
	}

	private TA_TopicDataRes ResolveActorPresentTopic(TA_NPCDataRes npc, List<TA_NPCDataRes> roomNpcs, string topicQuery)
	{
		if (npc.KeyOrNPCPresentDialogue == null || npc.KeyOrNPCPresentDialogue.Count == 0)
		{
			return null;
		}
		foreach (string item in (from n in roomNpcs
			where n != npc
			select n.NPCName).Concat(_listOfKeys))
		{
			foreach (string key in npc.KeyOrNPCPresentDialogue.Keys)
			{
				if (key.Equals(item, StringComparison.OrdinalIgnoreCase))
				{
					TA_TopicDataRes tA_TopicDataRes = npc.KeyOrNPCPresentDialogue[key];
					if (tA_TopicDataRes != null && TopicMatches(tA_TopicDataRes, topicQuery))
					{
						return tA_TopicDataRes;
					}
				}
			}
		}
		return null;
	}

	private TA_TopicDataRes ResolveTopic(TA_NPCDataRes npc, string topicQuery)
	{
		List<TA_NPCDataRes> visibleNPCs = GetVisibleNPCs(_currentRoom);
		TA_TopicDataRes tA_TopicDataRes = ResolveActorPresentTopic(npc, visibleNPCs, topicQuery);
		if (tA_TopicDataRes != null)
		{
			return tA_TopicDataRes;
		}
		if (_party.Contains(npc))
		{
			TA_TopicDataRes tA_TopicDataRes2 = FindTopicInList(npc.PartyDialogue, topicQuery);
			if (tA_TopicDataRes2 != null)
			{
				return tA_TopicDataRes2;
			}
		}
		return FindTopic(npc, topicQuery);
	}

	private bool TopicMatches(TA_TopicDataRes topic, string topicQuery)
	{
		if (string.IsNullOrWhiteSpace(topic.Keywords))
		{
			return false;
		}
		string q = topicQuery.Trim().ToLower();
		return topic.Keywords.Split('|', StringSplitOptions.RemoveEmptyEntries).Any((string kw) => q.Contains(kw.Trim().ToLower()));
	}

	private TA_TopicDataRes FindTopic(TA_NPCDataRes npc, string topicQuery)
	{
		foreach (TA_TopicDataRes askTopic in npc.AskTopics)
		{
			if (TopicMatches(askTopic, topicQuery))
			{
				return askTopic;
			}
		}
		return null;
	}

	private TA_TopicDataRes FindTopicInList(Array<TA_TopicDataRes> topics, string topicQuery)
	{
		foreach (TA_TopicDataRes topic in topics)
		{
			if (TopicMatches(topic, topicQuery))
			{
				return topic;
			}
		}
		return null;
	}

	private TA_ItemDataRes FindItemInList(IList<TA_ItemDataRes> list, string query)
	{
		string q = query.Trim().ToLower();
		TA_ItemDataRes tA_ItemDataRes = list.FirstOrDefault((TA_ItemDataRes i) => i.ItemName.ToLower() == q);
		if (tA_ItemDataRes != null)
		{
			return tA_ItemDataRes;
		}
		return list.FirstOrDefault((TA_ItemDataRes i) => i.ItemName.ToLower().Contains(q));
	}

	private TA_NPCDataRes FindNPC(TA_RoomDataRes room, string query)
	{
		if (room == null)
		{
			return null;
		}
		string q = query.Trim().ToLower();
		List<TA_NPCDataRes> visibleNPCs = GetVisibleNPCs(room);
		return visibleNPCs.FirstOrDefault((TA_NPCDataRes n) => n.NPCName.ToLower() == q) ?? visibleNPCs.FirstOrDefault((TA_NPCDataRes n) => n.NPCName.ToLower().Contains(q));
	}

	private List<TA_ItemDataRes> GetRoomItems(TA_RoomDataRes room)
	{
		if (!_roomItems.ContainsKey(room))
		{
			_roomItems[room] = room.Items.ToList();
		}
		return _roomItems[room].Where((TA_ItemDataRes i) => !i.HiddenUntilKey || HasKey(i.RequiredVisibilityKey)).ToList();
	}

	private List<TA_ItemDataRes> GetNPCItems(TA_NPCDataRes npc)
	{
		if (!_npcItems.ContainsKey(npc))
		{
			_npcItems[npc] = npc.HeldItems.ToList();
		}
		return _npcItems[npc].Where((TA_ItemDataRes i) => !i.HiddenUntilKey || HasKey(i.RequiredVisibilityKey)).ToList();
	}

	private List<TA_NPCDataRes> GetVisibleNPCs(TA_RoomDataRes room)
	{
		List<TA_NPCDataRes> first = _party.Where((TA_NPCDataRes n) => !Main.Instance.IsBlacklisted(n.taggedKinks)).ToList();
		List<TA_NPCDataRes> second = (from n in room.NPCs
			where !Main.Instance.IsBlacklisted(n.taggedKinks)
			where !n.HiddenUntilKey || HasKey(n.RequiredVisibilityKey)
			where !_party.Contains(n)
			select n).ToList();
		return first.Concat(second).ToList();
	}

	private void RemoveRoomItem(TA_RoomDataRes room, TA_ItemDataRes item)
	{
		if (_roomItems.TryGetValue(room, out var value))
		{
			value.Remove(item);
		}
	}

	private void RemoveNPCItem(TA_NPCDataRes npc, TA_ItemDataRes item)
	{
		if (_npcItems.TryGetValue(npc, out var value))
		{
			value.Remove(item);
		}
	}

	private void InitMutableState()
	{
		_roomItems.Clear();
		_npcItems.Clear();
		if (World.StartRoom == null)
		{
			return;
		}
		HashSet<TA_RoomDataRes> hashSet = new HashSet<TA_RoomDataRes>();
		Queue<TA_RoomDataRes> queue = new Queue<TA_RoomDataRes>();
		queue.Enqueue(World.StartRoom);
		while (queue.Count > 0)
		{
			TA_RoomDataRes tA_RoomDataRes = queue.Dequeue();
			if (!hashSet.Add(tA_RoomDataRes))
			{
				continue;
			}
			_roomItems[tA_RoomDataRes] = tA_RoomDataRes.Items.ToList();
			foreach (TA_NPCDataRes nPC in tA_RoomDataRes.NPCs)
			{
				_npcItems[nPC] = nPC.HeldItems.ToList();
			}
			foreach (TA_RoomDataRes value in tA_RoomDataRes.Exits.Values)
			{
				if (value != null && !hashSet.Contains(value))
				{
					queue.Enqueue(value);
				}
			}
		}
	}

	private void CommitLine(string text)
	{
		Handler?.CommitLine(text);
	}

	private string EscapeBB(string text)
	{
		return text.Replace("[", "[lb]");
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(43)
		{
			new MethodInfo(MethodName.ResetForNewWorld, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.Start, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.WireExits, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ParseAdventureInput, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "rawInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleSubmit, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "rest", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleGo, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "destination", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleExamine, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "target", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleGet, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "target", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleGive, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "target", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleAsk, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "rest", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleHint, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleMap, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleCheckpoint, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "rest", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ProcessTopicSideEffects, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "topic", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.HandleHello, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "target", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleUse, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "target", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleRecruit, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "target", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PrintKeys, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.Restart, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GrantKey, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "key", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HasKey, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "key", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.TryMoveToRoom, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "destination", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.TryEnterRoom, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "room", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.MoveToRoom, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "room", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.PrintRoom, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "room", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.PrintExits, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "room", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.PrintInventory, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.PrintParty, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.MakeLink, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "command", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "label", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PrintHelp, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.FullReset, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SpawnItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "itemData", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.SpawnScenePopup, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "scenePopup", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.String, "scenePopupID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ResolveTopic, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "npc", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.String, "topicQuery", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.TopicMatches, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "topic", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.String, "topicQuery", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FindTopic, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "npc", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.String, "topicQuery", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FindTopicInList, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Array, "topics", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "topicQuery", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FindNPC, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "room", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.String, "query", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.RemoveRoomItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "room", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.RemoveNPCItem, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "npc", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Object, "item", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.InitMutableState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CommitLine, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.EscapeBB, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.ResetForNewWorld && args.Count == 0)
		{
			ResetForNewWorld();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Start && args.Count == 0)
		{
			Start();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.WireExits && args.Count == 0)
		{
			WireExits();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ParseAdventureInput && args.Count == 1)
		{
			bool from = ParseAdventureInput(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.HandleSubmit && args.Count == 1)
		{
			bool from2 = HandleSubmit(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.HandleGo && args.Count == 1)
		{
			HandleGo(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleExamine && args.Count == 1)
		{
			HandleExamine(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleGet && args.Count == 1)
		{
			HandleGet(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleGive && args.Count == 1)
		{
			HandleGive(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleAsk && args.Count == 1)
		{
			HandleAsk(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleHint && args.Count == 0)
		{
			HandleHint();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleMap && args.Count == 0)
		{
			HandleMap();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleCheckpoint && args.Count == 1)
		{
			HandleCheckpoint(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ProcessTopicSideEffects && args.Count == 1)
		{
			ProcessTopicSideEffects(VariantUtils.ConvertTo<TA_TopicDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleHello && args.Count == 1)
		{
			HandleHello(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleUse && args.Count == 1)
		{
			HandleUse(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleRecruit && args.Count == 1)
		{
			HandleRecruit(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PrintKeys && args.Count == 0)
		{
			PrintKeys();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Restart && args.Count == 0)
		{
			Restart();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GrantKey && args.Count == 1)
		{
			GrantKey(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HasKey && args.Count == 1)
		{
			bool from3 = HasKey(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.TryMoveToRoom && args.Count == 1)
		{
			bool from4 = TryMoveToRoom(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.TryEnterRoom && args.Count == 1)
		{
			bool from5 = TryEnterRoom(VariantUtils.ConvertTo<TA_RoomDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from5);
			return true;
		}
		if (method == MethodName.MoveToRoom && args.Count == 1)
		{
			MoveToRoom(VariantUtils.ConvertTo<TA_RoomDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PrintRoom && args.Count == 1)
		{
			PrintRoom(VariantUtils.ConvertTo<TA_RoomDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PrintExits && args.Count == 1)
		{
			PrintExits(VariantUtils.ConvertTo<TA_RoomDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PrintInventory && args.Count == 0)
		{
			PrintInventory();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PrintParty && args.Count == 0)
		{
			PrintParty();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.MakeLink && args.Count == 2)
		{
			string from6 = MakeLink(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from6);
			return true;
		}
		if (method == MethodName.PrintHelp && args.Count == 0)
		{
			PrintHelp();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FullReset && args.Count == 0)
		{
			FullReset();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnItem && args.Count == 1)
		{
			SpawnItem(VariantUtils.ConvertTo<ItemDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnScenePopup && args.Count == 2)
		{
			SpawnScenePopup(VariantUtils.ConvertTo<AttachDataRes>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ResolveTopic && args.Count == 2)
		{
			TA_TopicDataRes from7 = ResolveTopic(VariantUtils.ConvertTo<TA_NPCDataRes>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from7);
			return true;
		}
		if (method == MethodName.TopicMatches && args.Count == 2)
		{
			bool from8 = TopicMatches(VariantUtils.ConvertTo<TA_TopicDataRes>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from8);
			return true;
		}
		if (method == MethodName.FindTopic && args.Count == 2)
		{
			TA_TopicDataRes from9 = FindTopic(VariantUtils.ConvertTo<TA_NPCDataRes>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from9);
			return true;
		}
		if (method == MethodName.FindTopicInList && args.Count == 2)
		{
			TA_TopicDataRes from10 = FindTopicInList(VariantUtils.ConvertToArray<TA_TopicDataRes>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from10);
			return true;
		}
		if (method == MethodName.FindNPC && args.Count == 2)
		{
			TA_NPCDataRes from11 = FindNPC(VariantUtils.ConvertTo<TA_RoomDataRes>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from11);
			return true;
		}
		if (method == MethodName.RemoveRoomItem && args.Count == 2)
		{
			RemoveRoomItem(VariantUtils.ConvertTo<TA_RoomDataRes>(in args[0]), VariantUtils.ConvertTo<TA_ItemDataRes>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RemoveNPCItem && args.Count == 2)
		{
			RemoveNPCItem(VariantUtils.ConvertTo<TA_NPCDataRes>(in args[0]), VariantUtils.ConvertTo<TA_ItemDataRes>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.InitMutableState && args.Count == 0)
		{
			InitMutableState();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CommitLine && args.Count == 1)
		{
			CommitLine(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.EscapeBB && args.Count == 1)
		{
			string from12 = EscapeBB(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from12);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.ResetForNewWorld)
		{
			return true;
		}
		if (method == MethodName.Start)
		{
			return true;
		}
		if (method == MethodName.WireExits)
		{
			return true;
		}
		if (method == MethodName.ParseAdventureInput)
		{
			return true;
		}
		if (method == MethodName.HandleSubmit)
		{
			return true;
		}
		if (method == MethodName.HandleGo)
		{
			return true;
		}
		if (method == MethodName.HandleExamine)
		{
			return true;
		}
		if (method == MethodName.HandleGet)
		{
			return true;
		}
		if (method == MethodName.HandleGive)
		{
			return true;
		}
		if (method == MethodName.HandleAsk)
		{
			return true;
		}
		if (method == MethodName.HandleHint)
		{
			return true;
		}
		if (method == MethodName.HandleMap)
		{
			return true;
		}
		if (method == MethodName.HandleCheckpoint)
		{
			return true;
		}
		if (method == MethodName.ProcessTopicSideEffects)
		{
			return true;
		}
		if (method == MethodName.HandleHello)
		{
			return true;
		}
		if (method == MethodName.HandleUse)
		{
			return true;
		}
		if (method == MethodName.HandleRecruit)
		{
			return true;
		}
		if (method == MethodName.PrintKeys)
		{
			return true;
		}
		if (method == MethodName.Restart)
		{
			return true;
		}
		if (method == MethodName.GrantKey)
		{
			return true;
		}
		if (method == MethodName.HasKey)
		{
			return true;
		}
		if (method == MethodName.TryMoveToRoom)
		{
			return true;
		}
		if (method == MethodName.TryEnterRoom)
		{
			return true;
		}
		if (method == MethodName.MoveToRoom)
		{
			return true;
		}
		if (method == MethodName.PrintRoom)
		{
			return true;
		}
		if (method == MethodName.PrintExits)
		{
			return true;
		}
		if (method == MethodName.PrintInventory)
		{
			return true;
		}
		if (method == MethodName.PrintParty)
		{
			return true;
		}
		if (method == MethodName.MakeLink)
		{
			return true;
		}
		if (method == MethodName.PrintHelp)
		{
			return true;
		}
		if (method == MethodName.FullReset)
		{
			return true;
		}
		if (method == MethodName.SpawnItem)
		{
			return true;
		}
		if (method == MethodName.SpawnScenePopup)
		{
			return true;
		}
		if (method == MethodName.ResolveTopic)
		{
			return true;
		}
		if (method == MethodName.TopicMatches)
		{
			return true;
		}
		if (method == MethodName.FindTopic)
		{
			return true;
		}
		if (method == MethodName.FindTopicInList)
		{
			return true;
		}
		if (method == MethodName.FindNPC)
		{
			return true;
		}
		if (method == MethodName.RemoveRoomItem)
		{
			return true;
		}
		if (method == MethodName.RemoveNPCItem)
		{
			return true;
		}
		if (method == MethodName.InitMutableState)
		{
			return true;
		}
		if (method == MethodName.CommitLine)
		{
			return true;
		}
		if (method == MethodName.EscapeBB)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.World)
		{
			World = VariantUtils.ConvertTo<TA_WorldDataRes>(in value);
			return true;
		}
		if (name == PropertyName._currentRoom)
		{
			_currentRoom = VariantUtils.ConvertTo<TA_RoomDataRes>(in value);
			return true;
		}
		if (name == PropertyName._listOfKeys)
		{
			_listOfKeys = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		if (name == PropertyName._party)
		{
			_party = VariantUtils.ConvertToArray<TA_NPCDataRes>(in value);
			return true;
		}
		if (name == PropertyName._started)
		{
			_started = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._pendingGameOver)
		{
			_pendingGameOver = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Handler)
		{
			TerminalHandler from = Handler;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.World)
		{
			value = VariantUtils.CreateFrom(in World);
			return true;
		}
		if (name == PropertyName._currentRoom)
		{
			value = VariantUtils.CreateFrom(in _currentRoom);
			return true;
		}
		if (name == PropertyName._listOfKeys)
		{
			value = VariantUtils.CreateFromArray(_listOfKeys);
			return true;
		}
		if (name == PropertyName._party)
		{
			value = VariantUtils.CreateFromArray(_party);
			return true;
		}
		if (name == PropertyName._started)
		{
			value = VariantUtils.CreateFrom(in _started);
			return true;
		}
		if (name == PropertyName._pendingGameOver)
		{
			value = VariantUtils.CreateFrom(in _pendingGameOver);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.World, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._currentRoom, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName._listOfKeys, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName._party, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._started, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._pendingGameOver, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.Handler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.World, Variant.From(in World));
		info.AddProperty(PropertyName._currentRoom, Variant.From(in _currentRoom));
		info.AddProperty(PropertyName._listOfKeys, Variant.CreateFrom(_listOfKeys));
		info.AddProperty(PropertyName._party, Variant.CreateFrom(_party));
		info.AddProperty(PropertyName._started, Variant.From(in _started));
		info.AddProperty(PropertyName._pendingGameOver, Variant.From(in _pendingGameOver));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.World, out var value))
		{
			World = value.As<TA_WorldDataRes>();
		}
		if (info.TryGetProperty(PropertyName._currentRoom, out var value2))
		{
			_currentRoom = value2.As<TA_RoomDataRes>();
		}
		if (info.TryGetProperty(PropertyName._listOfKeys, out var value3))
		{
			_listOfKeys = value3.AsGodotArray<string>();
		}
		if (info.TryGetProperty(PropertyName._party, out var value4))
		{
			_party = value4.AsGodotArray<TA_NPCDataRes>();
		}
		if (info.TryGetProperty(PropertyName._started, out var value5))
		{
			_started = value5.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._pendingGameOver, out var value6))
		{
			_pendingGameOver = value6.As<bool>();
		}
	}
}
