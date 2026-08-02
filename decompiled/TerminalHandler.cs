using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalHandler.cs")]
public class TerminalHandler : Control
{
	public new class MethodName : Control.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName ApplyFonts = "ApplyFonts";

		public static readonly StringName LoadTerminalCommands = "LoadTerminalCommands";

		public new static readonly StringName _Input = "_Input";

		public static readonly StringName HandleSubmit = "HandleSubmit";

		public static readonly StringName PrintBootHeader = "PrintBootHeader";

		public static readonly StringName RebuildOutput = "RebuildOutput";

		public static readonly StringName ScrollOutputToBottom = "ScrollOutputToBottom";

		public static readonly StringName RebuildInputLine = "RebuildInputLine";

		public static readonly StringName BuildInputLine = "BuildInputLine";

		public static readonly StringName EscapeBB = "EscapeBB";

		public static readonly StringName CommitLine = "CommitLine";

		public static readonly StringName ClearOutput = "ClearOutput";

		public static readonly StringName OnInputTrapChanged = "OnInputTrapChanged";

		public static readonly StringName RefocusInput = "RefocusInput";

		public static readonly StringName CyclePastCommand = "CyclePastCommand";

		public static readonly StringName ResetTabCycle = "ResetTabCycle";

		public static readonly StringName HandleTabComplete = "HandleTabComplete";

		public static readonly StringName ParseInput = "ParseInput";

		public static readonly StringName EnterAdventureMode = "EnterAdventureMode";

		public static readonly StringName FindWorldKey = "FindWorldKey";

		public static readonly StringName LeaveAdventureMode = "LeaveAdventureMode";

		public static readonly StringName GetAdventureNode = "GetAdventureNode";

		public static readonly StringName EnterAskMode = "EnterAskMode";

		public static readonly StringName FindAskData = "FindAskData";

		public static readonly StringName GetAskNode = "GetAskNode";

		public static readonly StringName HandleAdminCommand = "HandleAdminCommand";

		public static readonly StringName DenyAccess = "DenyAccess";

		public static readonly StringName HandleSpawnItemCommand = "HandleSpawnItemCommand";

		public static readonly StringName HandleSpawnActorCommand = "HandleSpawnActorCommand";

		public static readonly StringName HandleSpawnPopupCommand = "HandleSpawnPopupCommand";

		public static readonly StringName HandleSpawnMinigameCommand = "HandleSpawnMinigameCommand";

		public static readonly StringName HandleSpawnSceneCommand = "HandleSpawnSceneCommand";

		public static readonly StringName HandleSetUsernameCommand = "HandleSetUsernameCommand";

		public static readonly StringName HandleUnlockGalleryCommand = "HandleUnlockGalleryCommand";

		public static readonly StringName EnableCindeshMode = "EnableCindeshMode";

		public static readonly StringName DisplayHelpCommands = "DisplayHelpCommands";

		public static readonly StringName TickToDeath = "TickToDeath";

		public static readonly StringName EasterEggStringHandler = "EasterEggStringHandler";

		public static readonly StringName MakeLink = "MakeLink";

		public static readonly StringName OnMetaClicked = "OnMetaClicked";
	}

	public new class PropertyName : Control.PropertyName
	{
		public static readonly StringName Prefix = "Prefix";

		public static readonly StringName WorldCache = "WorldCache";

		public static readonly StringName AskCache = "AskCache";

		public static readonly StringName windowParent = "windowParent";

		public static readonly StringName outputScroll = "outputScroll";

		public static readonly StringName outputLabel = "outputLabel";

		public static readonly StringName inputLabel = "inputLabel";

		public static readonly StringName TerminalEasterEggs = "TerminalEasterEggs";

		public static readonly StringName TerminalFont = "TerminalFont";

		public static readonly StringName TerminalFontBold = "TerminalFontBold";

		public static readonly StringName TerminalFontMono = "TerminalFontMono";

		public static readonly StringName TerminalFontSize = "TerminalFontSize";

		public static readonly StringName DefaultWorldID = "DefaultWorldID";

		public static readonly StringName adminEnabled = "adminEnabled";

		public static readonly StringName AllCommands = "AllCommands";

		public static readonly StringName _inputTrap = "_inputTrap";

		public static readonly StringName settingConfirmMenu = "settingConfirmMenu";

		public static readonly StringName _committedText = "_committedText";

		public static readonly StringName _pastCommands = "_pastCommands";

		public static readonly StringName _currentPastCommand = "_currentPastCommand";

		public static readonly StringName DeathCounter = "DeathCounter";

		public static readonly StringName _tabMatches = "_tabMatches";

		public static readonly StringName _tabMatchIndex = "_tabMatchIndex";

		public static readonly StringName _tabPrefix = "_tabPrefix";

		public static readonly StringName _tabCycling = "_tabCycling";

		public static readonly StringName _caretVisible = "_caretVisible";

		public static readonly StringName _caretTimer = "_caretTimer";

		public static readonly StringName _adventureMode = "_adventureMode";

		public static readonly StringName _askMode = "_askMode";

		public static readonly StringName _dynamicCommands = "_dynamicCommands";
	}

	public new class SignalName : Control.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public TerminalWindow windowParent;

	[Export(PropertyHint.None, "")]
	public ScrollContainer outputScroll;

	[Export(PropertyHint.None, "")]
	public RichTextLabel outputLabel;

	[Export(PropertyHint.None, "")]
	public RichTextLabel inputLabel;

	[Export(PropertyHint.None, "")]
	public Array<TerminalEEDataRes> TerminalEasterEggs = new Array<TerminalEEDataRes>();

	[Export(PropertyHint.None, "")]
	public FontFile TerminalFont;

	[Export(PropertyHint.None, "")]
	public FontFile TerminalFontBold;

	[Export(PropertyHint.None, "")]
	public FontFile TerminalFontMono;

	[Export(PropertyHint.None, "")]
	public int TerminalFontSize = 16;

	[ExportGroup("Brain Dance Worlds", "")]
	[Export(PropertyHint.None, "")]
	public string DefaultWorldID = "";

	public bool adminEnabled;

	private const string HelpCommand = "help";

	private const string QuitCommand = "exit";

	private const string ClearCommand = "clear";

	private const string ConfigCommand = "config";

	private const string SetUserNameCommand = "set_username";

	private const string EnableCindeshModeCommand = "enable_cindesh_mode";

	private const string RootCommand = "root";

	private const string SpawnItemCommand = "force_spawn_item";

	private const string SpawnActorCommand = "force_spawn_actor";

	private const string SpawnPopupCommand = "force_spawn_popup";

	private const string SpawnMinigameCommand = "force_spawn_minigame";

	private const string SpawnSceneCommand = "force_spawn_scene";

	private const string UnlockGalleryCommand = "unlock_gallery";

	private const string AdventureCommand = "enter_brain_dance";

	private const string AskCommand = "ask_mode";

	private const string AdminsPassword = "LookUponMyKnotAndDespair";

	private readonly Array<string> AllCommands = new Array<string>
	{
		"help", "exit", "clear", "config", "enable_cindesh_mode", "root", "force_spawn_item", "force_spawn_actor", "force_spawn_popup", "enter_brain_dance",
		"set_username", "unlock_gallery", "ask_mode", "force_spawn_minigame"
	};

	private LineEdit _inputTrap;

	private ConfirmationMenu settingConfirmMenu;

	private string _committedText = "";

	private Array<string> _pastCommands = new Array<string>();

	private int _currentPastCommand;

	private int DeathCounter = 4;

	private Array<string> _tabMatches = new Array<string>();

	private int _tabMatchIndex;

	private string _tabPrefix = "";

	private bool _tabCycling;

	private bool _caretVisible = true;

	private Timer _caretTimer;

	private bool _adventureMode;

	private bool _askMode;

	private const string MainCharacterAskID = "Byte_0";

	private Godot.Collections.Dictionary<string, Node> _dynamicCommands = new Godot.Collections.Dictionary<string, Node>();

	private string Prefix
	{
		get
		{
			if (!_adventureMode)
			{
				if (!_askMode)
				{
					return "C:/FEN_OS/Users/" + Main.Instance.userInfoName + ">";
				}
				return Main.Instance.userInfoName + ">";
			}
			return Main.Instance.userInfoName + ">";
		}
	}

	private Godot.Collections.Dictionary<string, Resource> WorldCache
	{
		get
		{
			if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.BRAINDACE_WORLDS))
			{
				return new Godot.Collections.Dictionary<string, Resource>();
			}
			return ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.BRAINDACE_WORLDS];
		}
	}

	private Godot.Collections.Dictionary<string, Resource> AskCache
	{
		get
		{
			if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ASK_CHARACTERS))
			{
				return new Godot.Collections.Dictionary<string, Resource>();
			}
			return ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ASK_CHARACTERS];
		}
	}

	public override void _Ready()
	{
		SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect, LayoutPresetMode.Minsize);
		adminEnabled = Main.Instance.AdminAccess;
		outputLabel.ScrollFollowing = false;
		outputLabel.FocusMode = FocusModeEnum.None;
		outputLabel.MouseFilter = MouseFilterEnum.Pass;
		outputLabel.SelectionEnabled = true;
		outputLabel.ContextMenuEnabled = false;
		outputLabel.MetaClicked += OnMetaClicked;
		ApplyFonts(outputLabel);
		inputLabel.BbcodeEnabled = true;
		inputLabel.FocusMode = FocusModeEnum.None;
		inputLabel.MouseFilter = MouseFilterEnum.Ignore;
		ApplyFonts(inputLabel);
		_inputTrap = new LineEdit();
		_inputTrap.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect, LayoutPresetMode.Minsize);
		_inputTrap.Position = Vector2.Zero;
		_inputTrap.Modulate = new Color(0f, 0f, 0f, 0f);
		_inputTrap.AddThemeStyleboxOverride("normal", new StyleBoxEmpty());
		_inputTrap.AddThemeStyleboxOverride("focus", new StyleBoxEmpty());
		_inputTrap.AddThemeStyleboxOverride("read_only", new StyleBoxEmpty());
		_inputTrap.CaretBlink = false;
		_inputTrap.TextChanged += OnInputTrapChanged;
		_inputTrap.FocusMode = FocusModeEnum.All;
		_inputTrap.MouseFilter = MouseFilterEnum.Ignore;
		_inputTrap.ProcessMode = ProcessModeEnum.Always;
		AddChild(_inputTrap, forceReadableName: false, InternalMode.Disabled);
		_caretTimer = new Timer();
		_caretTimer.WaitTime = 0.5;
		_caretTimer.Autostart = true;
		_caretTimer.Timeout += delegate
		{
			_caretVisible = !_caretVisible;
			if (outputLabel.GetSelectedText() == "")
			{
				RebuildInputLine();
			}
		};
		AddChild(_caretTimer, forceReadableName: false, InternalMode.Disabled);
		PrintBootHeader();
		RebuildOutput();
		RebuildInputLine();
		LoadTerminalCommands();
		_inputTrap.GrabFocus();
		if (OS.HasFeature("editor"))
		{
			adminEnabled = true;
			Main.Instance.AdminAccess = true;
		}
	}

	private void ApplyFonts(RichTextLabel label)
	{
		if (TerminalFont != null)
		{
			label.AddThemeFontOverride("normal_font", TerminalFont);
			label.AddThemeFontSizeOverride("normal_font_size", Mathf.RoundToInt(TerminalFontSize));
			label.AddThemeFontOverride("bold_font", TerminalFontBold);
			label.AddThemeFontSizeOverride("bold_font_size", Mathf.RoundToInt(TerminalFontSize));
			label.AddThemeFontOverride("mono_font", TerminalFont);
			label.AddThemeFontSizeOverride("mono_font_size", Mathf.RoundToInt(TerminalFontSize));
		}
	}

	private void LoadTerminalCommands()
	{
		if (!ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.TERMINAL_COMMANDS))
		{
			return;
		}
		foreach (KeyValuePair<string, PackedScene> item in ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.TERMINAL_COMMANDS])
		{
			string key = item.Key;
			PackedScene value = item.Value;
			if (value == null)
			{
				continue;
			}
			Node node = value.Instantiate(PackedScene.GenEditState.Disabled);
			Variant variant = node.Get("Command");
			string text = ((variant.VariantType == Variant.Type.String) ? variant.AsString() : "");
			if (string.IsNullOrWhiteSpace(text))
			{
				GD.PrintErr("[TerminalHandler] Prefab '" + key + "' has no 'Command' property set. Skipping.");
				node.QueueFree();
				continue;
			}
			string text2 = text.ToLower();
			if (!node.HasMethod(text))
			{
				GD.PrintErr($"[TerminalHandler] Prefab '{key}' declares Command '{text}' but has no matching method. Skipping.");
				node.QueueFree();
			}
			else if (_dynamicCommands.ContainsKey(text2) || AllCommands.Contains(text2))
			{
				GD.PrintErr($"[TerminalHandler] Command '{text2}' from prefab '{key}' conflicts with an existing command. Skipping.");
				node.QueueFree();
			}
			else
			{
				node.Set("terminal", this);
				AddChild(node, forceReadableName: false, InternalMode.Disabled);
				_dynamicCommands[text2] = node;
				AllCommands.Add(text2);
				GD.Print($"[TerminalHandler] Registered terminal command '{text2}' from prefab '{key}'.");
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton { Pressed: not false })
		{
			if (outputLabel.GetSelectedText() == "")
			{
				_inputTrap.GrabFocus();
			}
		}
		else
		{
			if (!(@event is InputEventKey { Pressed: not false } inputEventKey))
			{
				return;
			}
			if (outputLabel.GetSelectedText() == "")
			{
				_inputTrap.GrabFocus();
			}
			Key keycode = inputEventKey.Keycode;
			if (keycode != Key.C)
			{
				Key num = keycode - 4194306;
				if ((ulong)num <= 16uL)
				{
					switch (num)
					{
					case (Key)3L:
					case (Key)4L:
						GetViewport().SetInputAsHandled();
						HandleSubmit(_inputTrap.Text);
						return;
					case (Key)14L:
						GetViewport().SetInputAsHandled();
						CyclePastCommand(-1);
						return;
					case (Key)16L:
						GetViewport().SetInputAsHandled();
						CyclePastCommand(1);
						return;
					case Key.None:
						GetViewport().SetInputAsHandled();
						if (!_adventureMode && _inputTrap.Text.Length > 0)
						{
							HandleTabComplete();
						}
						return;
					case (Key)11L:
					case (Key)12L:
					case (Key)13L:
					case (Key)15L:
						CallDeferred("RebuildInputLine");
						return;
					}
				}
				if (inputEventKey.Keycode != Key.Tab)
				{
					ResetTabCycle();
				}
			}
			else if (inputEventKey.CtrlPressed && outputLabel.GetSelectedText() != "")
			{
				DisplayServer.ClipboardSet(outputLabel.GetSelectedText());
				GetViewport().SetInputAsHandled();
			}
		}
	}

	private void HandleSubmit(string text)
	{
		ResetTabCycle();
		string text2 = text.Trim();
		_inputTrap.Clear();
		RebuildInputLine();
		if (text2.Length == 0)
		{
			CommitLine(Prefix);
			RebuildOutput();
			return;
		}
		CommitLine(Prefix + text2);
		if (!_pastCommands.Contains(text2))
		{
			_pastCommands.Add(text2);
		}
		_currentPastCommand = _pastCommands.Count;
		if (settingConfirmMenu != null)
		{
			CommitLine("  Confirmation Window Open; Please Confirm or Deny the Popup before entering more commands");
			CommitLine("");
			RebuildOutput();
			return;
		}
		if (_adventureMode)
		{
			string[] array = text2.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			if (array.Length >= 1 && array[0].ToLower() == "enter_brain_dance")
			{
				LeaveAdventureMode();
				GetAdventureNode()?.ResetForNewWorld();
				EnterAdventureMode((array.Length > 1) ? array[1] : "");
				RebuildOutput();
				return;
			}
		}
		if (_adventureMode)
		{
			TerminalAdventure adventureNode = GetAdventureNode();
			if (adventureNode != null)
			{
				if (!adventureNode.ParseAdventureInput(text2))
				{
					LeaveAdventureMode();
				}
			}
			else
			{
				CommitLine("  [Error] TerminalAdventure node not found.");
				CommitLine("");
			}
			RebuildOutput();
		}
		else if (_askMode)
		{
			TerminalAsk askNode = GetAskNode();
			if (askNode != null)
			{
				if (!askNode.ParseAskInput(text2))
				{
					_askMode = false;
				}
			}
			else
			{
				CommitLine("  [Error] TerminalAsk node not found.");
				CommitLine("");
				_askMode = false;
			}
			RebuildOutput();
		}
		else
		{
			if (!ParseInput(text2))
			{
				CommitLine("  Command not recognised. Type '" + "help".ToUpper() + "' to list all available commands.");
				CommitLine("");
			}
			RebuildOutput();
		}
	}

	private void PrintBootHeader()
	{
		CommitLine("FEN-DOS version 0.27");
		CommitLine("Copyright (C) 1999 Short Side Systems");
		Dictionary datetimeDictFromSystem = Time.GetDatetimeDictFromSystem();
		CommitLine($"Current date is {(int)datetimeDictFromSystem["year"]}-{(int)datetimeDictFromSystem["month"]:D2}-{(int)datetimeDictFromSystem["day"]:D2}");
		CommitLine("");
		CommitLine("For assistance, type '" + "help".ToUpper() + "'.");
		CommitLine("[color=red]Hello " + Main.Instance.userInfoName + ". May you have fun digging through my flesh, apologies if I get... bitey[/color]");
		CommitLine("");
	}

	private void RebuildOutput()
	{
		VScrollBar vScrollBar = outputScroll.GetVScrollBar();
		int num = ((vScrollBar.MaxValue > vScrollBar.Page) ? ((int)(vScrollBar.MaxValue - vScrollBar.Page)) : 0);
		bool num2 = outputScroll.ScrollVertical >= num - 4;
		outputLabel.Text = _committedText + "\n";
		if (num2)
		{
			ScrollOutputToBottom();
		}
	}

	private async void ScrollOutputToBottom()
	{
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		VScrollBar vScrollBar = outputScroll.GetVScrollBar();
		outputScroll.ScrollVertical = (int)vScrollBar.MaxValue;
	}

	private void RebuildInputLine()
	{
		inputLabel.Text = BuildInputLine();
	}

	private string BuildInputLine()
	{
		string text = _inputTrap.Text;
		bool num = _inputTrap.HasSelection();
		int caretColumn = _inputTrap.CaretColumn;
		string prefix = Prefix;
		if (num)
		{
			int selectionFromColumn = _inputTrap.GetSelectionFromColumn();
			int selectionToColumn = _inputTrap.GetSelectionToColumn();
			prefix += EscapeBB(text.Substring(0, selectionFromColumn));
			prefix = prefix + "[bgcolor=#cccccc][color=#000000]" + EscapeBB(text.Substring(selectionFromColumn, selectionToColumn - selectionFromColumn)) + "[/color][/bgcolor]";
			prefix += EscapeBB(text.Substring(selectionToColumn));
			if (_caretVisible)
			{
				prefix += "|";
			}
		}
		else
		{
			bool flag = caretColumn >= text.Length;
			string text2 = EscapeBB(text.Substring(0, caretColumn));
			string text3 = EscapeBB(text.Substring(caretColumn));
			prefix += text2;
			string text4 = (_caretVisible ? "#cccccc" : "#000000");
			prefix += (flag ? ("[color=" + text4 + "]_[/color]") : ("[color=" + text4 + "]|[/color]"));
			prefix += text3;
		}
		return prefix;
	}

	private string EscapeBB(string text)
	{
		return text.Replace("[", "[lb]");
	}

	internal void CommitLine(string text)
	{
		_committedText = _committedText + text + "\n";
	}

	private void ClearOutput()
	{
		_committedText = "";
		RebuildOutput();
	}

	private void OnInputTrapChanged(string newText)
	{
		ResetTabCycle();
		_caretVisible = true;
		_caretTimer.Start();
		if (outputLabel.GetSelectedText() == "")
		{
			RebuildInputLine();
		}
	}

	private void RefocusInput()
	{
		GetTree().CreateTimer(0.0).Timeout += delegate
		{
			_inputTrap.GrabFocus();
		};
	}

	private void CyclePastCommand(int direction)
	{
		if (_pastCommands.Count != 0)
		{
			_currentPastCommand = Mathf.Clamp(_currentPastCommand + direction, 0, _pastCommands.Count);
			if (_currentPastCommand == _pastCommands.Count)
			{
				_inputTrap.Text = "";
			}
			else
			{
				_inputTrap.Text = _pastCommands[_currentPastCommand];
				_inputTrap.CaretColumn = _inputTrap.Text.Length;
			}
			_caretVisible = true;
			_caretTimer.Start();
			RebuildInputLine();
		}
	}

	private void ResetTabCycle()
	{
		_tabMatches.Clear();
		_tabMatchIndex = 0;
		_tabPrefix = "";
		_tabCycling = false;
	}

	private void HandleTabComplete()
	{
		string text = _inputTrap.Text;
		string[] array = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			return;
		}
		if (array.Length == 1 && !text.EndsWith(" "))
		{
			string text2 = array[0].ToLower();
			if (!_tabCycling || _tabPrefix != text2)
			{
				_tabMatches.Clear();
				_tabMatchIndex = 0;
				_tabPrefix = text2;
				foreach (string allCommand in AllCommands)
				{
					if (allCommand.StartsWith(text2))
					{
						_tabMatches.Add(allCommand);
					}
				}
				_tabCycling = _tabMatches.Count > 0;
			}
			if (_tabMatches.Count != 0)
			{
				_inputTrap.Text = _tabMatches[_tabMatchIndex];
				_inputTrap.CaretColumn = _inputTrap.Text.Length;
				_tabMatchIndex = (_tabMatchIndex + 1) % _tabMatches.Count;
				RebuildInputLine();
			}
		}
		else
		{
			if (array.Length < 1)
			{
				return;
			}
			string text3 = array[0].ToLower();
			string text4 = ((array.Length >= 2) ? array[1] : "");
			ResourceCache.ResourceTyping? resourceTyping = null;
			switch (text3)
			{
			case "force_spawn_item":
				resourceTyping = ResourceCache.ResourceTyping.ITEM;
				break;
			case "force_spawn_actor":
				resourceTyping = ResourceCache.ResourceTyping.CHARACTER;
				break;
			case "force_spawn_minigame":
				if (!_tabCycling || _tabPrefix != text4)
				{
					_tabMatches.Clear();
					_tabMatchIndex = 0;
					_tabPrefix = text4;
					if (ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.MINIGAME))
					{
						foreach (string key in ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.MINIGAME].Keys)
						{
							if (key.StartsWith(text4, StringComparison.OrdinalIgnoreCase))
							{
								_tabMatches.Add(key);
							}
						}
					}
					_tabCycling = _tabMatches.Count > 0;
				}
				if (_tabMatches.Count != 0)
				{
					_inputTrap.Text = text3 + " " + _tabMatches[_tabMatchIndex];
					_inputTrap.CaretColumn = _inputTrap.Text.Length;
					_tabMatchIndex = (_tabMatchIndex + 1) % _tabMatches.Count;
					RebuildOutput();
				}
				return;
			case "enter_brain_dance":
				resourceTyping = null;
				break;
			}
			if (text3 == "enter_brain_dance")
			{
				if (!_tabCycling || _tabPrefix != text4)
				{
					_tabMatches.Clear();
					_tabMatchIndex = 0;
					_tabPrefix = text4;
					foreach (string key2 in WorldCache.Keys)
					{
						if (key2.ToLower().StartsWith(text4.ToLower()))
						{
							_tabMatches.Add(key2);
						}
					}
					_tabCycling = _tabMatches.Count > 0;
				}
				if (_tabMatches.Count != 0)
				{
					_inputTrap.Text = text3 + " " + _tabMatches[_tabMatchIndex];
					_inputTrap.CaretColumn = _inputTrap.Text.Length;
					_tabMatchIndex = (_tabMatchIndex + 1) % _tabMatches.Count;
					RebuildOutput();
				}
			}
			else if (text3 == "ask_mode")
			{
				if (!_tabCycling || _tabPrefix != text4)
				{
					_tabMatches.Clear();
					_tabMatchIndex = 0;
					_tabPrefix = text4;
					foreach (string key3 in AskCache.Keys)
					{
						if (key3.ToLower().StartsWith(text4.ToLower()))
						{
							_tabMatches.Add(key3);
						}
					}
					_tabCycling = _tabMatches.Count > 0;
				}
				if (_tabMatches.Count != 0)
				{
					_inputTrap.Text = text3 + " " + _tabMatches[_tabMatchIndex];
					_inputTrap.CaretColumn = _inputTrap.Text.Length;
					_tabMatchIndex = (_tabMatchIndex + 1) % _tabMatches.Count;
					RebuildOutput();
				}
			}
			else
			{
				if (!resourceTyping.HasValue)
				{
					return;
				}
				if (!_tabCycling || _tabPrefix != text4)
				{
					_tabMatches.Clear();
					_tabMatchIndex = 0;
					_tabPrefix = text4;
					if (ResourceCache.resourcesLoaded.ContainsKey(resourceTyping.Value))
					{
						foreach (string key4 in ResourceCache.resourcesLoaded[resourceTyping.Value].Keys)
						{
							if (key4.StartsWith(text4, StringComparison.OrdinalIgnoreCase))
							{
								_tabMatches.Add(key4);
							}
						}
					}
					_tabCycling = _tabMatches.Count > 0;
				}
				if (_tabMatches.Count != 0)
				{
					_inputTrap.Text = text3 + " " + _tabMatches[_tabMatchIndex];
					_inputTrap.CaretColumn = _inputTrap.Text.Length;
					_tabMatchIndex = (_tabMatchIndex + 1) % _tabMatches.Count;
					RebuildOutput();
				}
			}
		}
	}

	private bool ParseInput(string input)
	{
		string[] array = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 0)
		{
			return false;
		}
		string text = array[0].ToLower();
		switch (text)
		{
		case "help":
			DisplayHelpCommands();
			return true;
		case "clear":
			ClearOutput();
			return true;
		case "exit":
			GetTree().Quit();
			return true;
		case "config":
		{
			string userDataDir = OS.GetUserDataDir();
			CommitLine($"  Config location: [url=open_folder~{userDataDir}][color=cyan]{EscapeBB(userDataDir)}[/color][/url]");
			CommitLine("");
			return true;
		}
		case "enable_cindesh_mode":
			EnableCindeshMode(forcedEnabled: false);
			return true;
		case "set_username":
			HandleSetUsernameCommand(array);
			return true;
		case "root":
			HandleAdminCommand(array);
			return true;
		case "force_spawn_item":
			HandleSpawnItemCommand(array);
			return true;
		case "force_spawn_actor":
			HandleSpawnActorCommand(array);
			return true;
		case "force_spawn_popup":
			HandleSpawnPopupCommand(array);
			return true;
		case "force_spawn_minigame":
			HandleSpawnMinigameCommand(array);
			return true;
		case "force_spawn_scene":
			HandleSpawnSceneCommand(array);
			return true;
		case "enter_brain_dance":
			EnterAdventureMode((array.Length > 1) ? array[1] : "");
			return true;
		case "ask_mode":
			EnterAskMode((array.Length > 1) ? array[1] : "");
			return true;
		case "unlock_gallery":
			HandleUnlockGalleryCommand();
			return true;
		default:
			if (_dynamicCommands.ContainsKey(text))
			{
				_dynamicCommands[text].Call(text, array);
				return true;
			}
			return EasterEggStringHandler(array);
		}
	}

	private void EnterAdventureMode(string worldID = "")
	{
		TerminalAdventure adventureNode = GetAdventureNode();
		if (adventureNode == null)
		{
			GD.PrintErr("No Adventure Node");
			return;
		}
		if (worldID == "/ls")
		{
			CommitLine("  -- Brain Dance IDs --");
			if (WorldCache.Count > 0)
			{
				foreach (string key in WorldCache.Keys)
				{
					CommitLine("  " + MakeLink("enter_brain_dance " + key, key));
				}
			}
			else
			{
				CommitLine("  No Brain Dances found.");
			}
			CommitLine("");
			return;
		}
		TA_WorldDataRes tA_WorldDataRes = null;
		if (string.IsNullOrWhiteSpace(worldID))
		{
			if (!string.IsNullOrWhiteSpace(DefaultWorldID))
			{
				string text = FindWorldKey(DefaultWorldID);
				if (text != null)
				{
					tA_WorldDataRes = (TA_WorldDataRes)WorldCache[text];
				}
			}
			if (tA_WorldDataRes == null)
			{
				using IEnumerator<Resource> enumerator2 = WorldCache.Values.GetEnumerator();
				if (enumerator2.MoveNext())
				{
					tA_WorldDataRes = (TA_WorldDataRes)enumerator2.Current;
				}
			}
			if (tA_WorldDataRes == null)
			{
				CommitLine("  No Brain Dance found. Set a DefaultWorldID or add entries to TerminalWorlds.");
				CommitLine("");
				return;
			}
		}
		else
		{
			string text2 = FindWorldKey(worldID);
			if (text2 == null)
			{
				CommitLine("  [color=red]Unknown Brain Dance '[/color]" + EscapeBB(worldID) + "[color=red]'. Type ENTER_BRAIN_DANCE /ls to list available.[/color]");
				CommitLine("");
				return;
			}
			tA_WorldDataRes = (TA_WorldDataRes)WorldCache[text2];
		}
		if (adventureNode.World != tA_WorldDataRes)
		{
			adventureNode.ResetForNewWorld();
		}
		adventureNode.World = tA_WorldDataRes;
		_adventureMode = true;
		CommitLine("");
		CommitLine("  [color=cyan]-- ENTERING BRAIN DANCE MODE -- type QUIT to return to terminal --[/color]");
		CommitLine("");
		adventureNode.Start();
	}

	private string FindWorldKey(string id)
	{
		foreach (string key in WorldCache.Keys)
		{
			if (key.Equals(id, StringComparison.OrdinalIgnoreCase))
			{
				return key;
			}
		}
		return null;
	}

	private void LeaveAdventureMode()
	{
		_adventureMode = false;
		CommitLine("");
		CommitLine("  [color=cyan]-- RETURNED TO TERMINAL --[/color]");
		CommitLine("");
	}

	private TerminalAdventure GetAdventureNode()
	{
		foreach (Node child in GetChildren())
		{
			if (child is TerminalAdventure result)
			{
				return result;
			}
		}
		return null;
	}

	private void EnterAskMode(string companionID = "")
	{
		TerminalAsk askNode = GetAskNode();
		if (askNode == null)
		{
			CommitLine("  [color=red][Ask] No TerminalAsk node found as a child.[/color]");
			CommitLine("");
			return;
		}
		if (companionID == "/ls")
		{
			CommitLine("  -- Ask NPCs --");
			bool flag = false;
			foreach (string key in AskCache.Keys)
			{
				if (key == "Byte_0")
				{
					CommitLine("  " + MakeLink("ask_mode " + key, key) + " (main character)");
					flag = true;
					continue;
				}
				foreach (ActorWindow spawnedCompanion in Main.Instance.spawnedCompanions)
				{
					if (GodotObject.IsInstanceValid(spawnedCompanion) && spawnedCompanion.characterActor.characterInformation.itemID == key)
					{
						CommitLine("  " + MakeLink("ask_mode " + key, key));
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				CommitLine("  No Ask NPCs are available right now.");
			}
			CommitLine("");
			return;
		}
		if (string.IsNullOrWhiteSpace(companionID) || companionID.Equals("Byte_0", StringComparison.OrdinalIgnoreCase))
		{
			if (!AskCache.ContainsKey("Byte_0"))
			{
				CommitLine("  [color=red]No Ask data configured for the main character. Add a 'Byte_0' resource to res://Resources/AskMode/.[/color]");
				CommitLine("");
			}
			else
			{
				_askMode = true;
				askNode.Enter((TAsk_AskDataRes)AskCache["Byte_0"], null);
			}
			return;
		}
		if (!AskCache.ContainsKey(companionID))
		{
			CommitLine("  [color=red]No Ask data found for '[/color]" + EscapeBB(companionID) + "[color=red]'. Type ASK_MODE /ls to list available.[/color]");
			CommitLine("");
			return;
		}
		TAsk_AskDataRes data = (TAsk_AskDataRes)AskCache[companionID];
		ActorWindow actorWindow = null;
		foreach (ActorWindow spawnedCompanion2 in Main.Instance.spawnedCompanions)
		{
			if (GodotObject.IsInstanceValid(spawnedCompanion2) && spawnedCompanion2.characterActor.characterInformation.itemID == companionID)
			{
				actorWindow = spawnedCompanion2;
				break;
			}
		}
		if (actorWindow == null)
		{
			CommitLine("  [color=red]" + EscapeBB(companionID) + " isn't here right now.[/color]");
			CommitLine("");
		}
		else
		{
			_askMode = true;
			askNode.Enter(data, actorWindow);
		}
	}

	private TAsk_AskDataRes FindAskData(string id)
	{
		if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ASK_CHARACTERS))
		{
			return null;
		}
		Godot.Collections.Dictionary<string, Resource> dictionary = ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ASK_CHARACTERS];
		if (!dictionary.ContainsKey(id))
		{
			return null;
		}
		return (TAsk_AskDataRes)dictionary[id];
	}

	private TerminalAsk GetAskNode()
	{
		foreach (Node child in GetChildren())
		{
			if (child is TerminalAsk result)
			{
				return result;
			}
		}
		return null;
	}

	private void HandleAdminCommand(string[] splitInput)
	{
		if (splitInput.Length < 2)
		{
			CommitLine("  Access denied. Password required: Root <password>");
			CommitLine("");
		}
		else if (splitInput[1] != "LookUponMyKnotAndDespair")
		{
			CommitLine("  Access denied. Incorrect password. [color=red]" + TickToDeath() + "[/color]");
			if (DeathCounter == 0)
			{
				EnableCindeshMode();
			}
			else
			{
				CommitLine("");
			}
		}
		else
		{
			adminEnabled = !adminEnabled;
			Main.Instance.AdminAccess = adminEnabled;
			CommitLine(adminEnabled ? "  Access granted. Root Access granted." : "  Root Access revoked.");
			CommitLine("");
		}
	}

	private void DenyAccess()
	{
		CommitLine("  Access denied. [color=red]" + TickToDeath() + "[/color]");
		if (DeathCounter == 0)
		{
			EnableCindeshMode();
		}
		else
		{
			CommitLine("");
		}
	}

	private void HandleSpawnItemCommand(string[] splitInput)
	{
		if (!adminEnabled)
		{
			DenyAccess();
			return;
		}
		if (splitInput.Length < 2)
		{
			CommitLine("  Usage: FORCE_SPAWN_ITEM <item_id>  |  FORCE_SPAWN_ITEM /ls to list all IDs. | FORCE_SPAWN_ITEM <item_id> /path to see Resource Path for Modding.");
			CommitLine("");
			return;
		}
		if (splitInput[1] == "/ls")
		{
			CommitLine("  -- Item IDs --");
			if (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM))
			{
				foreach (string key in ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Keys)
				{
					CommitLine("  " + MakeLink("force_spawn_item " + key, key));
				}
			}
			else
			{
				CommitLine("  No items found in cache.");
			}
			CommitLine("");
			return;
		}
		string text = splitInput[1];
		if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || !ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].ContainsKey(text))
		{
			CommitLine("  Error: Item ID '" + text + "' not found.");
			CommitLine("");
			return;
		}
		ItemDataRes itemDataRes = (ItemDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM][text];
		if (splitInput.Length >= 3 && splitInput[2] == "/path")
		{
			CommitLine("  " + text + ": " + itemDataRes.ResourcePath);
			CommitLine("");
			return;
		}
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
		CommitLine("  Spawned item: " + text);
		CommitLine("");
	}

	private void HandleSpawnActorCommand(string[] splitInput)
	{
		if (!adminEnabled)
		{
			DenyAccess();
			return;
		}
		if (splitInput.Length < 2)
		{
			CommitLine("  Usage: FORCE_SPAWN_ACTOR <actor_id>  |  FORCE_SPAWN_ACTOR /ls to list all IDs. | FORCE_SPAWN_ACTOR <actor_id> /path to see Resource Path for Modding.");
			CommitLine("");
			return;
		}
		if (splitInput[1] == "/ls")
		{
			CommitLine("  -- Actor IDs --");
			if (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.CHARACTER))
			{
				foreach (string key in ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER].Keys)
				{
					CommitLine("  " + MakeLink("force_spawn_actor " + key, key));
				}
			}
			else
			{
				CommitLine("  No actors found in cache.");
			}
			CommitLine("");
			return;
		}
		string text = splitInput[1];
		if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.CHARACTER) || !ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER].ContainsKey(text))
		{
			CommitLine("  Error: Actor ID '" + text + "' not found.");
			CommitLine("");
			return;
		}
		CharacterInfoDataRes characterInfoDataRes = (CharacterInfoDataRes)ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][text];
		if (splitInput.Length >= 3 && splitInput[2] == "/path")
		{
			CommitLine("  " + text + ": " + characterInfoDataRes.ResourcePath);
			CommitLine("");
		}
		else
		{
			Main.Instance.CallActorSpawn(characterInfoDataRes);
			CommitLine("  Spawned actor: " + text);
			CommitLine("");
		}
	}

	private void HandleSpawnPopupCommand(string[] splitInput)
	{
		if (!adminEnabled)
		{
			DenyAccess();
			return;
		}
		if (Main.Instance.settingRemovePopups)
		{
			CommitLine("  [color=red]Popup's are currently Disabled in the Settings![/color]");
			CommitLine("");
			return;
		}
		Godot.Collections.Dictionary<string, Resource> dictionary = (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.SPAM) ? ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.SPAM] : new Godot.Collections.Dictionary<string, Resource>());
		if (splitInput.Length < 2)
		{
			CommitLine("  Usage: FORCE_SPAWN_POPUP <popup_id>  |  FORCE_SPAWN_POPUP /ls to list all IDs. | FORCE_SPAWN_POPUP <popup_id> /path to see Resource Path for Modding.");
			CommitLine("");
			return;
		}
		if (splitInput[1] == "/ls")
		{
			CommitLine("  -- Popup IDs --");
			if (dictionary.Count > 0)
			{
				foreach (string key in dictionary.Keys)
				{
					CommitLine("  " + MakeLink("force_spawn_popup " + key, key));
				}
			}
			else
			{
				CommitLine("  No popups found in cache.");
			}
			CommitLine("");
			return;
		}
		string text = splitInput[1];
		if (!dictionary.ContainsKey(text))
		{
			CommitLine("  Error: Popup ID '" + text + "' not found.");
			CommitLine("");
			return;
		}
		AttachDataRes attachDataRes = (AttachDataRes)dictionary[text];
		if (splitInput.Length >= 3 && splitInput[2] == "/path")
		{
			CommitLine("  " + text + ": " + attachDataRes.ResourcePath);
			CommitLine("");
		}
		else
		{
			Main.Instance.CallCharacterAttachmentSpawn((AttachDataRes)dictionary[text], unclearableAttachment: true);
			CommitLine("  Spawned popup: " + text);
			CommitLine("");
		}
	}

	private void HandleSpawnMinigameCommand(string[] splitInput)
	{
		if (!adminEnabled)
		{
			DenyAccess();
		}
		else if (splitInput.Length < 2)
		{
			CommitLine("  Usage: FORCE_SPAWN_MINIGAME <minigame_id>  |  FORCE_SPAWN_MINIGAME /ls to list all IDs.");
			CommitLine("");
		}
		else if (splitInput[1] == "/ls")
		{
			CommitLine("  -- Minigame IDs --");
			if (ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.MINIGAME))
			{
				foreach (string key in ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.MINIGAME].Keys)
				{
					CommitLine("  " + MakeLink("force_spawn_minigame " + key, key));
				}
			}
			else
			{
				CommitLine("  No minigames found in cache.");
			}
			CommitLine("");
		}
		else
		{
			string text = splitInput[1];
			if (!ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.MINIGAME) || !ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.MINIGAME].ContainsKey(text))
			{
				CommitLine("  Error: Minigame ID '" + text + "' not found.");
				CommitLine("");
			}
			else
			{
				Main.Instance.CallMinigameSpawn(text);
				CommitLine("  Spawned minigame: " + text);
				CommitLine("");
			}
		}
	}

	private void HandleSpawnSceneCommand(string[] splitInput)
	{
		if (!adminEnabled)
		{
			DenyAccess();
		}
		else if (splitInput.Length < 2)
		{
			CommitLine("  Usage: FORCE_SPAWN_SCENE <scene_id>  |  FORCE_SPAWN_SCENE /ls to list all IDs.");
			CommitLine("");
		}
		else if (splitInput[1] == "/ls")
		{
			CommitLine("  -- Scene IDs --");
			if (ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.UNTYPED))
			{
				foreach (string key in ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.UNTYPED].Keys)
				{
					CommitLine("  " + MakeLink("force_spawn_scene " + key, key));
				}
			}
			else
			{
				CommitLine("  No Scene found in cache.");
			}
			CommitLine("");
		}
		else
		{
			string text = splitInput[1];
			if (!ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.UNTYPED) || !ResourceCache.prefabsLoaded[ResourceCache.PrefabTyping.UNTYPED].ContainsKey(text))
			{
				CommitLine("  Error: Scene ID '" + text + "' not found.");
				CommitLine("");
			}
			else
			{
				Main.Instance.CallPackedSceneSpawn(text);
				CommitLine("  Spawned Scene: " + text);
				CommitLine("");
			}
		}
	}

	private void HandleSetUsernameCommand(string[] splitInput)
	{
		if (splitInput.Length < 2)
		{
			CommitLine("  Usage: Set_Username <Input Name String>");
			CommitLine("");
			return;
		}
		string userInfoName = splitInput[1];
		Main.Instance.userInfoName = userInfoName;
		Main.Instance.saveHandler.SaveSettings();
		CommitLine("  Name Set: " + Main.Instance.userInfoName);
		CommitLine("");
	}

	private void HandleUnlockGalleryCommand()
	{
		if (!adminEnabled)
		{
			DenyAccess();
			return;
		}
		int num = 0;
		if (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM))
		{
			Array<string> array = Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.ITEMS];
			foreach (string key in ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Keys)
			{
				if (!array.Contains(key))
				{
					array.Add(key);
					num++;
				}
			}
		}
		Godot.Collections.Dictionary<string, Resource> obj = (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.SPAM) ? ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.SPAM] : new Godot.Collections.Dictionary<string, Resource>());
		Array<string> array2 = Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.POP_UPS];
		foreach (string key2 in obj.Keys)
		{
			if (!array2.Contains(key2))
			{
				array2.Add(key2);
				num++;
			}
		}
		if (ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.H_SCENES))
		{
			Array<string> array3 = Main.Instance.SeenObjects[SaveHandler.SeenObjectTypes.NSFW_SCENES];
			foreach (string key3 in ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.H_SCENES].Keys)
			{
				if (!array3.Contains(key3))
				{
					array3.Add(key3);
					num++;
				}
			}
		}
		Main.Instance.saveHandler.SaveSettings();
		CommitLine($"  Gallery unlocked. {num} new entries added.");
		CommitLine("");
	}

	private void UnlockFromFolder(string folderPath, Array<string> bucket, ref int totalAdded)
	{
		using DirAccess dirAccess = DirAccess.Open(folderPath);
		if (dirAccess == null)
		{
			GD.PrintErr("[UnlockGallery] Could not open folder: " + folderPath);
			return;
		}
		dirAccess.ListDirBegin();
		string next = dirAccess.GetNext();
		while (next != "")
		{
			if (!dirAccess.CurrentIsDir())
			{
				string baseName = next.TrimSuffix(".remap").GetBaseName().GetBaseName();
				if (!bucket.Contains(baseName))
				{
					bucket.Add(baseName);
					totalAdded++;
				}
			}
			next = dirAccess.GetNext();
		}
		dirAccess.ListDirEnd();
	}

	private void EnableCindeshMode(bool forcedEnabled = true)
	{
		if (Main.Instance.settingBlacklistedContent.Contains(SaveHandler.Kinks.CUCKING))
		{
			CommitLine("");
			if (forcedEnabled)
			{
				Main.Instance.settingBlacklistedContent.Remove(SaveHandler.Kinks.CUCKING);
				CommitLine("  Cucking Enabled. [color=red]  You bite my Hand. Now sit there and watch pup.[/color]");
				Main.Instance.userInfoName = "[color=cyan]Cuck[/color]";
				CommitLine("");
				Main.Instance.saveHandler.SaveSettings();
				return;
			}
			ConfirmationMenu confirmationMenu = Main.Instance.confirmationMenu.Instantiate<ConfirmationMenu>(PackedScene.GenEditState.Disabled);
			AddChild(confirmationMenu, forceReadableName: false, InternalMode.Disabled);
			confirmationMenu.label.Text = "[font_size=20]Cindesh Mode enables Cucking. It will be harder to turn off. [color=red]Are you sure?[/color][/font_size]";
			confirmationMenu.Confirmed += delegate
			{
				Main.Instance.settingBlacklistedContent.Remove(SaveHandler.Kinks.CUCKING);
				CommitLine("  Cucking Enabled. [color=red]  You asked for this Pup, now sit there and watch.[/color]");
				CommitLine("");
				Main.Instance.saveHandler.SaveSettings();
				settingConfirmMenu = null;
			};
			confirmationMenu.Deny += delegate
			{
				CommitLine("  Setting Confirmation Denyed. [color=red]  Good Choice.[/color]");
				CommitLine("");
				settingConfirmMenu = null;
			};
			settingConfirmMenu = confirmationMenu;
		}
		else
		{
			CommitLine("");
			CommitLine("[color=red]  Pup, you already have this setting enabled. If you can't turn this off, then you get to watch.[/color]");
			CommitLine("");
		}
	}

	private void DisplayHelpCommands()
	{
		CommitLine("");
		CommitLine("clear".ToUpper() + "                        Clear the terminal window.");
		CommitLine("exit".ToUpper() + "                         Terminate the pet program.");
		CommitLine("config".ToUpper() + "                       Shows the location of the Program Config.");
		CommitLine("set_username".ToUpper() + "                  Sets the User's given name.");
		CommitLine("enable_cindesh_mode".ToUpper() + "            Enables Cindesh's Heavier Content. [color=red]You have been warned[/color]");
		CommitLine("enter_brain_dance".ToUpper() + " <id | /ls>         Launch into a Brain Dance. [color=red]Perhaps you may find info here dog?[/color]");
		CommitLine("ask_mode".ToUpper() + " <id | /ls>               Enter Ask Mode with a character!");
		CommitLine("");
		CommitLine("root".ToUpper() + " <password>              Grants Root Access.");
		CommitLine("force_spawn_item".ToUpper() + " <id | /ls | id /path>         Force spawn an item by ID. [Root Access Required]");
		CommitLine("force_spawn_actor".ToUpper() + " <id | /ls | id /path>        Force spawn an actor by ID. [Root Access Required]");
		CommitLine("force_spawn_popup".ToUpper() + " <id | /ls | id /path>        Force spawn a popup by ID. [Root Access Required]");
		CommitLine("force_spawn_minigame".ToUpper() + " <id | /ls>                Force spawn a minigame by ID. [Root Access Required]");
		if (ResourceCache.prefabsLoaded.ContainsKey(ResourceCache.PrefabTyping.UNTYPED))
		{
			CommitLine("force_spawn_scene".ToUpper() + " <id | /ls>                Force spawn a generic UI Scene by ID. [Root Access Required]");
		}
		CommitLine("unlock_gallery".ToUpper() + "                Unlocks all gallery/scene entries. [Root Access Required]");
		if (_dynamicCommands.Count > 0)
		{
			CommitLine("");
			CommitLine("  < Additional Commands >");
			foreach (KeyValuePair<string, Node> dynamicCommand in _dynamicCommands)
			{
				string value = dynamicCommand.Key.ToUpper();
				Variant variant = dynamicCommand.Value.Get("HelpText");
				string value2 = ((variant.VariantType == Variant.Type.String && variant.AsString() != "") ? EscapeBB(variant.AsString()) : "No description provided.");
				CommitLine($"{value,-28} {value2}");
			}
		}
		CommitLine("");
		CommitLine("  Not all commands are listed here. [color=red]Explore Mutt[/color]");
		CommitLine("  For more information on tools see the command-line reference in no where.");
		CommitLine("  [color=red]You aren't supposed to be here...[/color]");
		CommitLine("");
	}

	private string TickToDeath()
	{
		DeathCounter--;
		return DeathCounter switch
		{
			3 => "You lack permissions Dog", 
			2 => "The hand that mocked them, and the heart that fed;", 
			1 => "And on the pedestal, these words appear:", 
			0 => "My Name is Fen, The Handler.", 
			_ => "Dude...", 
		};
	}

	private bool EasterEggStringHandler(string[] splitInput)
	{
		TerminalEEDataRes terminalEEDataRes = TerminalEasterEggs.FirstOrDefault((TerminalEEDataRes ee) => ee.EEName.Equals(splitInput[0].ToLower(), StringComparison.OrdinalIgnoreCase));
		if (terminalEEDataRes == null || terminalEEDataRes.possibleStrings.Count == 0)
		{
			return false;
		}
		Random random = new Random();
		CommitLine(" " + terminalEEDataRes.possibleStrings[random.Next(terminalEEDataRes.possibleStrings.Count)] + " ");
		CommitLine("");
		return true;
	}

	private string MakeLink(string command, string label)
	{
		string value = command.Replace(" ", "~");
		return $"[url={value}][color=cyan]{EscapeBB(label)}[/color][/url]";
	}

	private void OnMetaClicked(Variant meta)
	{
		string text = meta.AsString();
		if (text.StartsWith("open_folder~"))
		{
			OS.ShellOpen(text.Substring("open_folder~".Length));
			return;
		}
		string text2 = text.Replace("~", " ");
		HandleSubmit(text2);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(41)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ApplyFonts, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "label", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("RichTextLabel"), exported: false)
			}, null),
			new MethodInfo(MethodName.LoadTerminalCommands, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "event", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
			}, null),
			new MethodInfo(MethodName.HandleSubmit, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PrintBootHeader, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RebuildOutput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ScrollOutputToBottom, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RebuildInputLine, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BuildInputLine, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.EscapeBB, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CommitLine, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ClearOutput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnInputTrapChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "newText", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.RefocusInput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CyclePastCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "direction", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ResetTabCycle, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleTabComplete, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ParseInput, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "input", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.EnterAdventureMode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "worldID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FindWorldKey, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "id", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.LeaveAdventureMode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetAdventureNode, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.EnterAskMode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "companionID", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FindAskData, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "id", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetAskNode, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleAdminCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.DenyAccess, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HandleSpawnItemCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleSpawnActorCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleSpawnPopupCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleSpawnMinigameCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleSpawnSceneCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleSetUsernameCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.HandleUnlockGalleryCommand, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.EnableCindeshMode, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "forcedEnabled", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.DisplayHelpCommands, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.TickToDeath, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.EasterEggStringHandler, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.PackedStringArray, "splitInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.MakeLink, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "command", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "label", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnMetaClicked, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Nil, "meta", PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.NilIsVariant, exported: false)
			}, null)
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
		if (method == MethodName.ApplyFonts && args.Count == 1)
		{
			ApplyFonts(VariantUtils.ConvertTo<RichTextLabel>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.LoadTerminalCommands && args.Count == 0)
		{
			LoadTerminalCommands();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleSubmit && args.Count == 1)
		{
			HandleSubmit(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PrintBootHeader && args.Count == 0)
		{
			PrintBootHeader();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RebuildOutput && args.Count == 0)
		{
			RebuildOutput();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ScrollOutputToBottom && args.Count == 0)
		{
			ScrollOutputToBottom();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RebuildInputLine && args.Count == 0)
		{
			RebuildInputLine();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BuildInputLine && args.Count == 0)
		{
			string from = BuildInputLine();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.EscapeBB && args.Count == 1)
		{
			string from2 = EscapeBB(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.CommitLine && args.Count == 1)
		{
			CommitLine(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ClearOutput && args.Count == 0)
		{
			ClearOutput();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnInputTrapChanged && args.Count == 1)
		{
			OnInputTrapChanged(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RefocusInput && args.Count == 0)
		{
			RefocusInput();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CyclePastCommand && args.Count == 1)
		{
			CyclePastCommand(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ResetTabCycle && args.Count == 0)
		{
			ResetTabCycle();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleTabComplete && args.Count == 0)
		{
			HandleTabComplete();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ParseInput && args.Count == 1)
		{
			bool from3 = ParseInput(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.EnterAdventureMode && args.Count == 1)
		{
			EnterAdventureMode(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FindWorldKey && args.Count == 1)
		{
			string from4 = FindWorldKey(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.LeaveAdventureMode && args.Count == 0)
		{
			LeaveAdventureMode();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetAdventureNode && args.Count == 0)
		{
			TerminalAdventure from5 = GetAdventureNode();
			ret = VariantUtils.CreateFrom(in from5);
			return true;
		}
		if (method == MethodName.EnterAskMode && args.Count == 1)
		{
			EnterAskMode(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FindAskData && args.Count == 1)
		{
			TAsk_AskDataRes from6 = FindAskData(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from6);
			return true;
		}
		if (method == MethodName.GetAskNode && args.Count == 0)
		{
			TerminalAsk from7 = GetAskNode();
			ret = VariantUtils.CreateFrom(in from7);
			return true;
		}
		if (method == MethodName.HandleAdminCommand && args.Count == 1)
		{
			HandleAdminCommand(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.DenyAccess && args.Count == 0)
		{
			DenyAccess();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleSpawnItemCommand && args.Count == 1)
		{
			HandleSpawnItemCommand(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleSpawnActorCommand && args.Count == 1)
		{
			HandleSpawnActorCommand(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleSpawnPopupCommand && args.Count == 1)
		{
			HandleSpawnPopupCommand(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleSpawnMinigameCommand && args.Count == 1)
		{
			HandleSpawnMinigameCommand(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleSpawnSceneCommand && args.Count == 1)
		{
			HandleSpawnSceneCommand(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleSetUsernameCommand && args.Count == 1)
		{
			HandleSetUsernameCommand(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HandleUnlockGalleryCommand && args.Count == 0)
		{
			HandleUnlockGalleryCommand();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.EnableCindeshMode && args.Count == 1)
		{
			EnableCindeshMode(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.DisplayHelpCommands && args.Count == 0)
		{
			DisplayHelpCommands();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.TickToDeath && args.Count == 0)
		{
			string from8 = TickToDeath();
			ret = VariantUtils.CreateFrom(in from8);
			return true;
		}
		if (method == MethodName.EasterEggStringHandler && args.Count == 1)
		{
			bool from9 = EasterEggStringHandler(VariantUtils.ConvertTo<string[]>(in args[0]));
			ret = VariantUtils.CreateFrom(in from9);
			return true;
		}
		if (method == MethodName.MakeLink && args.Count == 2)
		{
			string from10 = MakeLink(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from10);
			return true;
		}
		if (method == MethodName.OnMetaClicked && args.Count == 1)
		{
			OnMetaClicked(VariantUtils.ConvertTo<Variant>(in args[0]));
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
		if (method == MethodName.ApplyFonts)
		{
			return true;
		}
		if (method == MethodName.LoadTerminalCommands)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.HandleSubmit)
		{
			return true;
		}
		if (method == MethodName.PrintBootHeader)
		{
			return true;
		}
		if (method == MethodName.RebuildOutput)
		{
			return true;
		}
		if (method == MethodName.ScrollOutputToBottom)
		{
			return true;
		}
		if (method == MethodName.RebuildInputLine)
		{
			return true;
		}
		if (method == MethodName.BuildInputLine)
		{
			return true;
		}
		if (method == MethodName.EscapeBB)
		{
			return true;
		}
		if (method == MethodName.CommitLine)
		{
			return true;
		}
		if (method == MethodName.ClearOutput)
		{
			return true;
		}
		if (method == MethodName.OnInputTrapChanged)
		{
			return true;
		}
		if (method == MethodName.RefocusInput)
		{
			return true;
		}
		if (method == MethodName.CyclePastCommand)
		{
			return true;
		}
		if (method == MethodName.ResetTabCycle)
		{
			return true;
		}
		if (method == MethodName.HandleTabComplete)
		{
			return true;
		}
		if (method == MethodName.ParseInput)
		{
			return true;
		}
		if (method == MethodName.EnterAdventureMode)
		{
			return true;
		}
		if (method == MethodName.FindWorldKey)
		{
			return true;
		}
		if (method == MethodName.LeaveAdventureMode)
		{
			return true;
		}
		if (method == MethodName.GetAdventureNode)
		{
			return true;
		}
		if (method == MethodName.EnterAskMode)
		{
			return true;
		}
		if (method == MethodName.FindAskData)
		{
			return true;
		}
		if (method == MethodName.GetAskNode)
		{
			return true;
		}
		if (method == MethodName.HandleAdminCommand)
		{
			return true;
		}
		if (method == MethodName.DenyAccess)
		{
			return true;
		}
		if (method == MethodName.HandleSpawnItemCommand)
		{
			return true;
		}
		if (method == MethodName.HandleSpawnActorCommand)
		{
			return true;
		}
		if (method == MethodName.HandleSpawnPopupCommand)
		{
			return true;
		}
		if (method == MethodName.HandleSpawnMinigameCommand)
		{
			return true;
		}
		if (method == MethodName.HandleSpawnSceneCommand)
		{
			return true;
		}
		if (method == MethodName.HandleSetUsernameCommand)
		{
			return true;
		}
		if (method == MethodName.HandleUnlockGalleryCommand)
		{
			return true;
		}
		if (method == MethodName.EnableCindeshMode)
		{
			return true;
		}
		if (method == MethodName.DisplayHelpCommands)
		{
			return true;
		}
		if (method == MethodName.TickToDeath)
		{
			return true;
		}
		if (method == MethodName.EasterEggStringHandler)
		{
			return true;
		}
		if (method == MethodName.MakeLink)
		{
			return true;
		}
		if (method == MethodName.OnMetaClicked)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.windowParent)
		{
			windowParent = VariantUtils.ConvertTo<TerminalWindow>(in value);
			return true;
		}
		if (name == PropertyName.outputScroll)
		{
			outputScroll = VariantUtils.ConvertTo<ScrollContainer>(in value);
			return true;
		}
		if (name == PropertyName.outputLabel)
		{
			outputLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.inputLabel)
		{
			inputLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.TerminalEasterEggs)
		{
			TerminalEasterEggs = VariantUtils.ConvertToArray<TerminalEEDataRes>(in value);
			return true;
		}
		if (name == PropertyName.TerminalFont)
		{
			TerminalFont = VariantUtils.ConvertTo<FontFile>(in value);
			return true;
		}
		if (name == PropertyName.TerminalFontBold)
		{
			TerminalFontBold = VariantUtils.ConvertTo<FontFile>(in value);
			return true;
		}
		if (name == PropertyName.TerminalFontMono)
		{
			TerminalFontMono = VariantUtils.ConvertTo<FontFile>(in value);
			return true;
		}
		if (name == PropertyName.TerminalFontSize)
		{
			TerminalFontSize = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.DefaultWorldID)
		{
			DefaultWorldID = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.adminEnabled)
		{
			adminEnabled = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._inputTrap)
		{
			_inputTrap = VariantUtils.ConvertTo<LineEdit>(in value);
			return true;
		}
		if (name == PropertyName.settingConfirmMenu)
		{
			settingConfirmMenu = VariantUtils.ConvertTo<ConfirmationMenu>(in value);
			return true;
		}
		if (name == PropertyName._committedText)
		{
			_committedText = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName._pastCommands)
		{
			_pastCommands = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		if (name == PropertyName._currentPastCommand)
		{
			_currentPastCommand = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.DeathCounter)
		{
			DeathCounter = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._tabMatches)
		{
			_tabMatches = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		if (name == PropertyName._tabMatchIndex)
		{
			_tabMatchIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._tabPrefix)
		{
			_tabPrefix = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName._tabCycling)
		{
			_tabCycling = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._caretVisible)
		{
			_caretVisible = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._caretTimer)
		{
			_caretTimer = VariantUtils.ConvertTo<Timer>(in value);
			return true;
		}
		if (name == PropertyName._adventureMode)
		{
			_adventureMode = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._askMode)
		{
			_askMode = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._dynamicCommands)
		{
			_dynamicCommands = VariantUtils.ConvertToDictionary<string, Node>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.Prefix)
		{
			string from = Prefix;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.WorldCache)
		{
			value = VariantUtils.CreateFromDictionary(WorldCache);
			return true;
		}
		if (name == PropertyName.AskCache)
		{
			value = VariantUtils.CreateFromDictionary(AskCache);
			return true;
		}
		if (name == PropertyName.windowParent)
		{
			value = VariantUtils.CreateFrom(in windowParent);
			return true;
		}
		if (name == PropertyName.outputScroll)
		{
			value = VariantUtils.CreateFrom(in outputScroll);
			return true;
		}
		if (name == PropertyName.outputLabel)
		{
			value = VariantUtils.CreateFrom(in outputLabel);
			return true;
		}
		if (name == PropertyName.inputLabel)
		{
			value = VariantUtils.CreateFrom(in inputLabel);
			return true;
		}
		if (name == PropertyName.TerminalEasterEggs)
		{
			value = VariantUtils.CreateFromArray(TerminalEasterEggs);
			return true;
		}
		if (name == PropertyName.TerminalFont)
		{
			value = VariantUtils.CreateFrom(in TerminalFont);
			return true;
		}
		if (name == PropertyName.TerminalFontBold)
		{
			value = VariantUtils.CreateFrom(in TerminalFontBold);
			return true;
		}
		if (name == PropertyName.TerminalFontMono)
		{
			value = VariantUtils.CreateFrom(in TerminalFontMono);
			return true;
		}
		if (name == PropertyName.TerminalFontSize)
		{
			value = VariantUtils.CreateFrom(in TerminalFontSize);
			return true;
		}
		if (name == PropertyName.DefaultWorldID)
		{
			value = VariantUtils.CreateFrom(in DefaultWorldID);
			return true;
		}
		if (name == PropertyName.adminEnabled)
		{
			value = VariantUtils.CreateFrom(in adminEnabled);
			return true;
		}
		if (name == PropertyName.AllCommands)
		{
			value = VariantUtils.CreateFromArray(AllCommands);
			return true;
		}
		if (name == PropertyName._inputTrap)
		{
			value = VariantUtils.CreateFrom(in _inputTrap);
			return true;
		}
		if (name == PropertyName.settingConfirmMenu)
		{
			value = VariantUtils.CreateFrom(in settingConfirmMenu);
			return true;
		}
		if (name == PropertyName._committedText)
		{
			value = VariantUtils.CreateFrom(in _committedText);
			return true;
		}
		if (name == PropertyName._pastCommands)
		{
			value = VariantUtils.CreateFromArray(_pastCommands);
			return true;
		}
		if (name == PropertyName._currentPastCommand)
		{
			value = VariantUtils.CreateFrom(in _currentPastCommand);
			return true;
		}
		if (name == PropertyName.DeathCounter)
		{
			value = VariantUtils.CreateFrom(in DeathCounter);
			return true;
		}
		if (name == PropertyName._tabMatches)
		{
			value = VariantUtils.CreateFromArray(_tabMatches);
			return true;
		}
		if (name == PropertyName._tabMatchIndex)
		{
			value = VariantUtils.CreateFrom(in _tabMatchIndex);
			return true;
		}
		if (name == PropertyName._tabPrefix)
		{
			value = VariantUtils.CreateFrom(in _tabPrefix);
			return true;
		}
		if (name == PropertyName._tabCycling)
		{
			value = VariantUtils.CreateFrom(in _tabCycling);
			return true;
		}
		if (name == PropertyName._caretVisible)
		{
			value = VariantUtils.CreateFrom(in _caretVisible);
			return true;
		}
		if (name == PropertyName._caretTimer)
		{
			value = VariantUtils.CreateFrom(in _caretTimer);
			return true;
		}
		if (name == PropertyName._adventureMode)
		{
			value = VariantUtils.CreateFrom(in _adventureMode);
			return true;
		}
		if (name == PropertyName._askMode)
		{
			value = VariantUtils.CreateFrom(in _askMode);
			return true;
		}
		if (name == PropertyName._dynamicCommands)
		{
			value = VariantUtils.CreateFromDictionary(_dynamicCommands);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.windowParent, PropertyHint.NodeType, "Window", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.outputScroll, PropertyHint.NodeType, "ScrollContainer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.outputLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.inputLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.TerminalEasterEggs, PropertyHint.TypeString, "24/17:TerminalEEDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.TerminalFont, PropertyHint.ResourceType, "FontFile", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.TerminalFontBold, PropertyHint.ResourceType, "FontFile", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.TerminalFontMono, PropertyHint.ResourceType, "FontFile", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.TerminalFontSize, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Brain Dance Worlds", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.DefaultWorldID, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Bool, PropertyName.adminEnabled, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName.AllCommands, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._inputTrap, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.settingConfirmMenu, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName._committedText, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName._pastCommands, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._currentPastCommand, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.DeathCounter, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Array, PropertyName._tabMatches, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._tabMatchIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName._tabPrefix, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._tabCycling, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._caretVisible, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._caretTimer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._adventureMode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName._askMode, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName.Prefix, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.WorldCache, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName.AskCache, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Dictionary, PropertyName._dynamicCommands, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.windowParent, Variant.From(in windowParent));
		info.AddProperty(PropertyName.outputScroll, Variant.From(in outputScroll));
		info.AddProperty(PropertyName.outputLabel, Variant.From(in outputLabel));
		info.AddProperty(PropertyName.inputLabel, Variant.From(in inputLabel));
		info.AddProperty(PropertyName.TerminalEasterEggs, Variant.CreateFrom(TerminalEasterEggs));
		info.AddProperty(PropertyName.TerminalFont, Variant.From(in TerminalFont));
		info.AddProperty(PropertyName.TerminalFontBold, Variant.From(in TerminalFontBold));
		info.AddProperty(PropertyName.TerminalFontMono, Variant.From(in TerminalFontMono));
		info.AddProperty(PropertyName.TerminalFontSize, Variant.From(in TerminalFontSize));
		info.AddProperty(PropertyName.DefaultWorldID, Variant.From(in DefaultWorldID));
		info.AddProperty(PropertyName.adminEnabled, Variant.From(in adminEnabled));
		info.AddProperty(PropertyName._inputTrap, Variant.From(in _inputTrap));
		info.AddProperty(PropertyName.settingConfirmMenu, Variant.From(in settingConfirmMenu));
		info.AddProperty(PropertyName._committedText, Variant.From(in _committedText));
		info.AddProperty(PropertyName._pastCommands, Variant.CreateFrom(_pastCommands));
		info.AddProperty(PropertyName._currentPastCommand, Variant.From(in _currentPastCommand));
		info.AddProperty(PropertyName.DeathCounter, Variant.From(in DeathCounter));
		info.AddProperty(PropertyName._tabMatches, Variant.CreateFrom(_tabMatches));
		info.AddProperty(PropertyName._tabMatchIndex, Variant.From(in _tabMatchIndex));
		info.AddProperty(PropertyName._tabPrefix, Variant.From(in _tabPrefix));
		info.AddProperty(PropertyName._tabCycling, Variant.From(in _tabCycling));
		info.AddProperty(PropertyName._caretVisible, Variant.From(in _caretVisible));
		info.AddProperty(PropertyName._caretTimer, Variant.From(in _caretTimer));
		info.AddProperty(PropertyName._adventureMode, Variant.From(in _adventureMode));
		info.AddProperty(PropertyName._askMode, Variant.From(in _askMode));
		info.AddProperty(PropertyName._dynamicCommands, Variant.CreateFrom(_dynamicCommands));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.windowParent, out var value))
		{
			windowParent = value.As<TerminalWindow>();
		}
		if (info.TryGetProperty(PropertyName.outputScroll, out var value2))
		{
			outputScroll = value2.As<ScrollContainer>();
		}
		if (info.TryGetProperty(PropertyName.outputLabel, out var value3))
		{
			outputLabel = value3.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.inputLabel, out var value4))
		{
			inputLabel = value4.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.TerminalEasterEggs, out var value5))
		{
			TerminalEasterEggs = value5.AsGodotArray<TerminalEEDataRes>();
		}
		if (info.TryGetProperty(PropertyName.TerminalFont, out var value6))
		{
			TerminalFont = value6.As<FontFile>();
		}
		if (info.TryGetProperty(PropertyName.TerminalFontBold, out var value7))
		{
			TerminalFontBold = value7.As<FontFile>();
		}
		if (info.TryGetProperty(PropertyName.TerminalFontMono, out var value8))
		{
			TerminalFontMono = value8.As<FontFile>();
		}
		if (info.TryGetProperty(PropertyName.TerminalFontSize, out var value9))
		{
			TerminalFontSize = value9.As<int>();
		}
		if (info.TryGetProperty(PropertyName.DefaultWorldID, out var value10))
		{
			DefaultWorldID = value10.As<string>();
		}
		if (info.TryGetProperty(PropertyName.adminEnabled, out var value11))
		{
			adminEnabled = value11.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._inputTrap, out var value12))
		{
			_inputTrap = value12.As<LineEdit>();
		}
		if (info.TryGetProperty(PropertyName.settingConfirmMenu, out var value13))
		{
			settingConfirmMenu = value13.As<ConfirmationMenu>();
		}
		if (info.TryGetProperty(PropertyName._committedText, out var value14))
		{
			_committedText = value14.As<string>();
		}
		if (info.TryGetProperty(PropertyName._pastCommands, out var value15))
		{
			_pastCommands = value15.AsGodotArray<string>();
		}
		if (info.TryGetProperty(PropertyName._currentPastCommand, out var value16))
		{
			_currentPastCommand = value16.As<int>();
		}
		if (info.TryGetProperty(PropertyName.DeathCounter, out var value17))
		{
			DeathCounter = value17.As<int>();
		}
		if (info.TryGetProperty(PropertyName._tabMatches, out var value18))
		{
			_tabMatches = value18.AsGodotArray<string>();
		}
		if (info.TryGetProperty(PropertyName._tabMatchIndex, out var value19))
		{
			_tabMatchIndex = value19.As<int>();
		}
		if (info.TryGetProperty(PropertyName._tabPrefix, out var value20))
		{
			_tabPrefix = value20.As<string>();
		}
		if (info.TryGetProperty(PropertyName._tabCycling, out var value21))
		{
			_tabCycling = value21.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._caretVisible, out var value22))
		{
			_caretVisible = value22.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._caretTimer, out var value23))
		{
			_caretTimer = value23.As<Timer>();
		}
		if (info.TryGetProperty(PropertyName._adventureMode, out var value24))
		{
			_adventureMode = value24.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._askMode, out var value25))
		{
			_askMode = value25.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._dynamicCommands, out var value26))
		{
			_dynamicCommands = value26.AsGodotDictionary<string, Node>();
		}
	}
}
