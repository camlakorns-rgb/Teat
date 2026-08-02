using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/SettingsMenu/SettingsMenu.cs")]
public class SettingsMenu : Window
{
	public enum ScreenState
	{
		UNTYPED,
		SCALE,
		GAMEPLAY,
		AUDIO,
		INPUT,
		MOD
	}

	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName SetupScreenPositions = "SetupScreenPositions";

		public static readonly StringName SetFontSizeOnChildren = "SetFontSizeOnChildren";

		public static readonly StringName BackToParentButton = "BackToParentButton";

		public static readonly StringName CloseSettingsWindow = "CloseSettingsWindow";

		public static readonly StringName HeadToState = "HeadToState";

		public static readonly StringName SetupScaling = "SetupScaling";

		public static readonly StringName SetupRenderingSelect = "SetupRenderingSelect";

		public static readonly StringName ChangeRenderingDriver = "ChangeRenderingDriver";

		public static readonly StringName PlatformSuffix = "PlatformSuffix";

		public static readonly StringName ChangeActorScaling = "ChangeActorScaling";

		public static readonly StringName ChangeItemScaling = "ChangeItemScaling";

		public static readonly StringName ChangeUIScaling = "ChangeUIScaling";

		public static readonly StringName SetupGameplay = "SetupGameplay";

		public static readonly StringName ToggleItemSpawns = "ToggleItemSpawns";

		public static readonly StringName ToggleActorSpawns = "ToggleActorSpawns";

		public static readonly StringName TogglePassivePlay = "TogglePassivePlay";

		public static readonly StringName ToggleDisablePopups = "ToggleDisablePopups";

		public static readonly StringName ToggleDisableConvos = "ToggleDisableConvos";

		public static readonly StringName SetupAudio = "SetupAudio";

		public static readonly StringName ToggleAudioSetting = "ToggleAudioSetting";

		public static readonly StringName SetupInput = "SetupInput";

		public static readonly StringName ActionToDisplayName = "ActionToDisplayName";

		public static readonly StringName GetBindingText = "GetBindingText";

		public static readonly StringName InputEventToText = "InputEventToText";

		public static readonly StringName BeginRebind = "BeginRebind";

		public static readonly StringName FinishRebind = "FinishRebind";

		public new static readonly StringName _Input = "_Input";

		public static readonly StringName ResetInputBindings = "ResetInputBindings";

		public static readonly StringName SetupMods = "SetupMods";

		public static readonly StringName ToggleModsEnabled = "ToggleModsEnabled";

		public static readonly StringName PopulateModList = "PopulateModList";

		public static readonly StringName ClearModList = "ClearModList";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName parentControl = "parentControl";

		public static readonly StringName scaleControl = "scaleControl";

		public static readonly StringName gameplayControl = "gameplayControl";

		public static readonly StringName audioControl = "audioControl";

		public static readonly StringName inputControl = "inputControl";

		public static readonly StringName modControl = "modControl";

		public static readonly StringName renderingSelect = "renderingSelect";

		public static readonly StringName petScaleSelect = "petScaleSelect";

		public static readonly StringName itemScaleSelect = "itemScaleSelect";

		public static readonly StringName uiScaleSelect = "uiScaleSelect";

		public static readonly StringName ItemButton = "ItemButton";

		public static readonly StringName ActorButton = "ActorButton";

		public static readonly StringName AIButton = "AIButton";

		public static readonly StringName PopupButton = "PopupButton";

		public static readonly StringName ConvoButton = "ConvoButton";

		public static readonly StringName AudioButton = "AudioButton";

		public static readonly StringName scrollInputBox = "scrollInputBox";

		public static readonly StringName InputButtonPack = "InputButtonPack";

		public static readonly StringName scrollModBox = "scrollModBox";

		public static readonly StringName ToggleButtonPack = "ToggleButtonPack";

		public static readonly StringName confirmationMenu = "confirmationMenu";

		public static readonly StringName ModsEnabledButton = "ModsEnabledButton";

		public static readonly StringName parent = "parent";

		public static readonly StringName state = "state";

		public static readonly StringName originalActorIndex = "originalActorIndex";

		public static readonly StringName originalItemIndex = "originalItemIndex";

		public static readonly StringName originalUIIndex = "originalUIIndex";

		public static readonly StringName flagForReload = "flagForReload";

		public static readonly StringName flagForFullReload = "flagForFullReload";

		public static readonly StringName IsAwaitingRebind = "IsAwaitingRebind";

		public static readonly StringName _awaitingRebindButton = "_awaitingRebindButton";

		public static readonly StringName _awaitingRebindAction = "_awaitingRebindAction";

		public static readonly StringName _awaitingRebindIndex = "_awaitingRebindIndex";

		public static readonly StringName _originalButtonText = "_originalButtonText";
	}

	public new class SignalName : Window.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private Control parentControl;

	[Export(PropertyHint.None, "")]
	private Control scaleControl;

	[Export(PropertyHint.None, "")]
	private Control gameplayControl;

	[Export(PropertyHint.None, "")]
	private Control audioControl;

	[Export(PropertyHint.None, "")]
	private Control inputControl;

	[Export(PropertyHint.None, "")]
	private Control modControl;

	[ExportGroup("Scale Settings", "")]
	[Export(PropertyHint.None, "")]
	private OptionButton renderingSelect;

	[Export(PropertyHint.None, "")]
	private OptionButton petScaleSelect;

	[Export(PropertyHint.None, "")]
	private OptionButton itemScaleSelect;

	[Export(PropertyHint.None, "")]
	private OptionButton uiScaleSelect;

	[ExportGroup("Gameplay Settings", "")]
	[Export(PropertyHint.None, "")]
	private CheckButton ItemButton;

	[Export(PropertyHint.None, "")]
	private CheckButton ActorButton;

	[Export(PropertyHint.None, "")]
	private CheckButton AIButton;

	[Export(PropertyHint.None, "")]
	private CheckButton PopupButton;

	[Export(PropertyHint.None, "")]
	private CheckButton ConvoButton;

	[ExportGroup("Audio Settings", "")]
	[Export(PropertyHint.None, "")]
	private CheckButton AudioButton;

	[ExportGroup("Input Settings", "")]
	[Export(PropertyHint.None, "")]
	private Control scrollInputBox;

	[Export(PropertyHint.None, "")]
	private PackedScene InputButtonPack;

	[ExportGroup("Mod Settings", "")]
	[Export(PropertyHint.None, "")]
	private Control scrollModBox;

	[Export(PropertyHint.None, "")]
	private PackedScene ToggleButtonPack;

	[Export(PropertyHint.None, "")]
	private PackedScene confirmationMenu;

	[Export(PropertyHint.None, "")]
	private CheckButton ModsEnabledButton;

	public PauseMenu parent;

	private ScreenState state;

	private int originalActorIndex;

	private int originalItemIndex;

	private int originalUIIndex;

	private bool flagForReload;

	private bool flagForFullReload;

	public static readonly string[] TrackedActions = new string[9] { "Move", "Pet", "Despawn", "Sit", "Clothing_Up", "Clothing_Down", "Screen_Lock", "Magnifier", "Terminal" };

	public bool IsAwaitingRebind;

	private Button _awaitingRebindButton;

	private string _awaitingRebindAction;

	private int _awaitingRebindIndex;

	private string _originalButtonText;

	public override void _Ready()
	{
		SetupScaling();
		SetupGameplay();
		SetupAudio();
		SetupScreenPositions();
		SetupInput();
		SetupMods();
		SetFontSizeOnChildren(this);
	}

	public override void _Process(double delta)
	{
		switch (state)
		{
		case ScreenState.UNTYPED:
			parentControl.Position = parentControl.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			scaleControl.Position = scaleControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			gameplayControl.Position = gameplayControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			audioControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			inputControl.Position = inputControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			modControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			break;
		case ScreenState.SCALE:
			scaleControl.Position = scaleControl.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			parentControl.Position = parentControl.Position.Lerp(new Vector2(base.Size.X, 0f), (float)delta * 10f);
			gameplayControl.Position = gameplayControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			audioControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			inputControl.Position = inputControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			modControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			break;
		case ScreenState.GAMEPLAY:
			gameplayControl.Position = gameplayControl.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			parentControl.Position = parentControl.Position.Lerp(new Vector2(base.Size.X, 0f), (float)delta * 10f);
			scaleControl.Position = scaleControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			audioControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			inputControl.Position = inputControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			modControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			break;
		case ScreenState.AUDIO:
			audioControl.Position = audioControl.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			parentControl.Position = parentControl.Position.Lerp(new Vector2(base.Size.X, 0f), (float)delta * 10f);
			scaleControl.Position = scaleControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			gameplayControl.Position = gameplayControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			inputControl.Position = inputControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			modControl.Position = modControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			break;
		case ScreenState.INPUT:
			inputControl.Position = inputControl.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			parentControl.Position = parentControl.Position.Lerp(new Vector2(base.Size.X, 0f), (float)delta * 10f);
			scaleControl.Position = scaleControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			gameplayControl.Position = gameplayControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			audioControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			modControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			break;
		case ScreenState.MOD:
			modControl.Position = modControl.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			parentControl.Position = parentControl.Position.Lerp(new Vector2(base.Size.X, 0f), (float)delta * 10f);
			scaleControl.Position = scaleControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			gameplayControl.Position = gameplayControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			audioControl.Position = audioControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			inputControl.Position = inputControl.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			break;
		}
	}

	public void SetupScreenPositions()
	{
		parentControl.Position = new Vector2(0f, 0f);
		parentControl.Visible = true;
		scaleControl.Position = new Vector2(-base.Size.X, 0f);
		scaleControl.Visible = true;
		gameplayControl.Position = new Vector2(-base.Size.X, 0f);
		gameplayControl.Visible = true;
		audioControl.Position = new Vector2(-base.Size.X, 0f);
		audioControl.Visible = true;
		inputControl.Position = new Vector2(-base.Size.X, 0f);
		inputControl.Visible = true;
		modControl.Position = new Vector2(-base.Size.X, 0f);
		modControl.Visible = true;
	}

	private void SetFontSizeOnChildren(Node parent)
	{
		foreach (Node child in parent.GetChildren())
		{
			if (child is RichTextLabel richTextLabel)
			{
				string[] array = new string[5] { "normal_font_size", "bold_font_size", "italics_font_size", "bold_italics_font_size", "mono_font_size" };
				foreach (string text in array)
				{
					int themeFontSize = richTextLabel.GetThemeFontSize(text, "");
					richTextLabel.AddThemeFontSizeOverride(text, themeFontSize);
				}
				if (richTextLabel.BbcodeEnabled)
				{
					richTextLabel.Text = Regex.Replace(richTextLabel.Text, "\\[font_size=(\\d+)\\]", delegate(Match match)
					{
						int value = int.Parse(match.Groups[1].Value);
						return $"[font_size={value}]";
					});
				}
			}
			else if (child is Control control)
			{
				int themeFontSize2 = control.GetThemeFontSize("font_size", "");
				control.AddThemeFontSizeOverride("font_size", themeFontSize2);
			}
			if (child.GetChildCount() > 0)
			{
				SetFontSizeOnChildren(child);
			}
		}
	}

	public void BackToParentButton()
	{
		state = ScreenState.UNTYPED;
	}

	public void CloseSettingsWindow()
	{
		Main.Instance.saveHandler.SaveSettings();
		if (flagForReload)
		{
			GetTree().CallDeferred("reload_current_scene");
			GetTree().Paused = false;
		}
		else if (flagForFullReload)
		{
			OS.CreateInstance(OS.GetCmdlineArgs());
			GetTree().Quit();
		}
		parent.openSettings = null;
		QueueFree();
	}

	public void HeadToState(int stateInt)
	{
		state = (ScreenState)stateInt;
	}

	public void SetupScaling()
	{
		SetupRenderingSelect();
		for (int i = 0; i < petScaleSelect.ItemCount; i++)
		{
			if (petScaleSelect.GetItemText(i).TrimSuffix("%").ToFloat() / 100f == Main.Instance.settingSpriteScaler)
			{
				petScaleSelect.Selected = i;
				break;
			}
		}
		for (int j = 0; j < itemScaleSelect.ItemCount; j++)
		{
			if (itemScaleSelect.GetItemText(j).TrimSuffix("%").ToFloat() / 100f == Main.Instance.settingItemScaler)
			{
				itemScaleSelect.Selected = j;
				break;
			}
		}
		for (int k = 0; k < uiScaleSelect.ItemCount; k++)
		{
			if (uiScaleSelect.GetItemText(k).TrimSuffix("%").ToFloat() / 100f == Main.Instance.settingUIScaler)
			{
				uiScaleSelect.Selected = k;
				break;
			}
		}
		originalActorIndex = petScaleSelect.Selected;
		originalItemIndex = itemScaleSelect.Selected;
		originalUIIndex = uiScaleSelect.Selected;
	}

	public void SetupRenderingSelect()
	{
		string text = PlatformSuffix();
		string name = "rendering/renderer/rendering_method." + text;
		string name2 = "rendering/gl_compatibility/driver." + text;
		string name3 = "rendering/rendering_device/driver." + text;
		string text2 = (string)ProjectSettings.GetSetting(name, "gl_compatibility");
		string text3 = (string)ProjectSettings.GetSetting(name2, "opengl3");
		string text4 = (string)ProjectSettings.GetSetting(name3, "vulkan");
		if (text2 == "gl_compatibility")
		{
			renderingSelect.Selected = ((text3 == "opengl3_angle") ? 1 : 0);
		}
		else
		{
			renderingSelect.Selected = ((text4 == "d3d12") ? 2 : 3);
		}
	}

	public void ChangeRenderingDriver(int index)
	{
		string text = PlatformSuffix();
		string name = "rendering/renderer/rendering_method." + text;
		string name2 = "rendering/gl_compatibility/driver." + text;
		string name3 = "rendering/rendering_device/driver." + text;
		switch (index)
		{
		case 0:
			ProjectSettings.SetSetting(name, "gl_compatibility");
			ProjectSettings.SetSetting(name2, "opengl3");
			break;
		case 1:
			ProjectSettings.SetSetting(name, "gl_compatibility");
			ProjectSettings.SetSetting(name2, "opengl3_angle");
			break;
		case 2:
			ProjectSettings.SetSetting(name, "forward_plus");
			ProjectSettings.SetSetting(name3, "d3d12");
			break;
		case 3:
			ProjectSettings.SetSetting(name, "forward_plus");
			ProjectSettings.SetSetting(name3, "vulkan");
			break;
		}
		Error error = ProjectSettings.SaveCustom("res://override.cfg");
		if (error == Error.Ok)
		{
			GD.Print("Rendering settings saved to override.cfg");
		}
		else
		{
			GD.Print("Failed to save rendering settings. Error code: ", error);
		}
		flagForFullReload = true;
	}

	private static string PlatformSuffix()
	{
		switch (OS.GetName())
		{
		case "Windows":
			return "windows";
		case "Linux":
		case "FreeBSD":
		case "NetBSD":
		case "OpenBSD":
		case "BSD":
			return "linuxbsd";
		default:
			return "windows";
		}
	}

	public void ChangeActorScaling(int index)
	{
		Main.Instance.settingSpriteScaler = petScaleSelect.GetItemText(index).TrimSuffix("%").ToFloat() / 100f;
		Main.Instance.saveHandler.SaveSettings();
		flagForReload = originalActorIndex != index;
	}

	public void ChangeItemScaling(int index)
	{
		Main.Instance.settingItemScaler = itemScaleSelect.GetItemText(index).TrimSuffix("%").ToFloat() / 100f;
		Main.Instance.saveHandler.SaveSettings();
		flagForReload = originalItemIndex != index;
	}

	public void ChangeUIScaling(int index)
	{
		Main.Instance.settingUIScaler = uiScaleSelect.GetItemText(index).TrimSuffix("%").ToFloat() / 100f;
		Main.Instance.saveHandler.SaveSettings();
		flagForReload = originalUIIndex != index;
	}

	public void SetupGameplay()
	{
		ItemButton.ButtonPressed = Main.Instance.settingSpawnItems;
		if (ItemButton.ButtonPressed)
		{
			ItemButton.Text = "Yes";
		}
		else
		{
			ItemButton.Text = "No";
		}
		ActorButton.ButtonPressed = Main.Instance.settingSpawnActors;
		if (ActorButton.ButtonPressed)
		{
			ActorButton.Text = "Yes";
		}
		else
		{
			ActorButton.Text = "No";
		}
		AIButton.ButtonPressed = Main.Instance.settingPassivePlayMode;
		if (AIButton.ButtonPressed)
		{
			AIButton.Text = "On";
		}
		else
		{
			AIButton.Text = "Off";
		}
		PopupButton.ButtonPressed = Main.Instance.settingRemovePopups;
		if (PopupButton.ButtonPressed)
		{
			PopupButton.Text = "On";
		}
		else
		{
			PopupButton.Text = "Off";
		}
		ConvoButton.ButtonPressed = Main.Instance.settingRemoveConvos;
		if (ConvoButton.ButtonPressed)
		{
			ConvoButton.Text = "On";
		}
		else
		{
			ConvoButton.Text = "Off";
		}
	}

	public void ToggleItemSpawns()
	{
		Main.Instance.settingSpawnItems = ItemButton.ButtonPressed;
		if (ItemButton.ButtonPressed)
		{
			ItemButton.Text = "Yes";
		}
		else
		{
			ItemButton.Text = "No";
		}
		Main.Instance.saveHandler.SaveSettings();
	}

	public void ToggleActorSpawns()
	{
		Main.Instance.settingSpawnActors = ActorButton.ButtonPressed;
		if (ActorButton.ButtonPressed)
		{
			ActorButton.Text = "Yes";
		}
		else
		{
			ActorButton.Text = "No";
		}
		Main.Instance.saveHandler.SaveSettings();
	}

	public void TogglePassivePlay()
	{
		Main.Instance.settingPassivePlayMode = AIButton.ButtonPressed;
		if (AIButton.ButtonPressed)
		{
			AIButton.Text = "On";
		}
		else
		{
			AIButton.Text = "Off";
		}
		Main.Instance.saveHandler.SaveSettings();
	}

	public void ToggleDisablePopups()
	{
		Main.Instance.settingRemovePopups = PopupButton.ButtonPressed;
		if (PopupButton.ButtonPressed)
		{
			PopupButton.Text = "On";
		}
		else
		{
			PopupButton.Text = "Off";
		}
		Main.Instance.saveHandler.SaveSettings();
	}

	public void ToggleDisableConvos()
	{
		Main.Instance.settingRemoveConvos = ConvoButton.ButtonPressed;
		if (ConvoButton.ButtonPressed)
		{
			ConvoButton.Text = "On";
		}
		else
		{
			ConvoButton.Text = "Off";
		}
		Main.Instance.saveHandler.SaveSettings();
	}

	public void SetupAudio()
	{
		AudioButton.ButtonPressed = Main.Instance.settingAudioOn;
		if (AudioButton.ButtonPressed)
		{
			AudioButton.Text = "Yes";
		}
		else
		{
			AudioButton.Text = "No";
		}
	}

	public void ToggleAudioSetting()
	{
		Main.Instance.settingAudioOn = AudioButton.ButtonPressed;
		if (AudioButton.ButtonPressed)
		{
			AudioButton.Text = "Yes";
		}
		else
		{
			AudioButton.Text = "No";
		}
		Main.Instance.saveHandler.SaveSettings();
	}

	public void SetupInput()
	{
		Array<Node> children = scrollInputBox.GetChildren();
		for (int i = 1; i < children.Count; i++)
		{
			children[i].QueueFree();
		}
		string[] trackedActions = TrackedActions;
		foreach (string action in trackedActions)
		{
			if (InputMap.HasAction(action))
			{
				Node node = InputButtonPack.Instantiate(PackedScene.GenEditState.Disabled);
				scrollInputBox.AddChild(node, forceReadableName: false, InternalMode.Disabled);
				node.GetChild<RichTextLabel>(0).Text = ActionToDisplayName(action);
				Button input1 = node.GetChild<Button>(1);
				input1.GetChild<RichTextLabel>(0).Text = GetBindingText(action, 0);
				input1.Pressed += delegate
				{
					BeginRebind(input1, action, 0);
				};
				Button input2 = node.GetChild<Button>(2);
				input2.GetChild<RichTextLabel>(0).Text = GetBindingText(action, 1);
				input2.Pressed += delegate
				{
					BeginRebind(input2, action, 1);
				};
			}
		}
	}

	private string ActionToDisplayName(string action)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(action.Replace("_", " "));
	}

	private string GetBindingText(string action, int index)
	{
		Array<InputEvent> array = InputMap.ActionGetEvents(action);
		if (index < array.Count)
		{
			return InputEventToText(array[index]);
		}
		return "---";
	}

	private string InputEventToText(InputEvent inputEvent)
	{
		if (inputEvent is InputEventKey inputEventKey)
		{
			if (inputEventKey.PhysicalKeycode == Key.None)
			{
				return inputEventKey.AsText();
			}
			return OS.GetKeycodeString(inputEventKey.PhysicalKeycode);
		}
		if (inputEvent is InputEventMouseButton inputEventMouseButton)
		{
			return $"Mouse {inputEventMouseButton.ButtonIndex}";
		}
		return inputEvent.AsText();
	}

	private void BeginRebind(Button button, string action, int bindIndex)
	{
		if (_awaitingRebindButton != null)
		{
			_awaitingRebindButton.GetChild<RichTextLabel>(0).Text = _originalButtonText;
		}
		_awaitingRebindButton = button;
		_awaitingRebindAction = action;
		_awaitingRebindIndex = bindIndex;
		_originalButtonText = button.GetChild<RichTextLabel>(0).Text;
		IsAwaitingRebind = true;
		button.GetChild<RichTextLabel>(0).Text = "[font_size=32][ Press a Key ][/font_size]";
		button.ReleaseFocus();
	}

	private void FinishRebind(InputEvent newEvent)
	{
		if (_awaitingRebindButton != null)
		{
			Array<InputEvent> array = InputMap.ActionGetEvents(_awaitingRebindAction);
			if (_awaitingRebindIndex < array.Count)
			{
				InputMap.ActionEraseEvent(_awaitingRebindAction, array[_awaitingRebindIndex]);
			}
			InputMap.ActionAddEvent(_awaitingRebindAction, newEvent);
			_awaitingRebindButton.GetChild<RichTextLabel>(0).Text = InputEventToText(newEvent);
			_awaitingRebindButton = null;
			_awaitingRebindAction = null;
			_originalButtonText = null;
			IsAwaitingRebind = false;
			Main.Instance.saveHandler.SaveSettings();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (_awaitingRebindButton != null && (@event is InputEventKey { Pressed: not false, Echo: false } || @event is InputEventMouseButton { Pressed: not false }))
		{
			if (@event is InputEventKey inputEventKey2 && inputEventKey2.PhysicalKeycode == Key.Escape)
			{
				_awaitingRebindButton.GetChild<RichTextLabel>(0).Text = _originalButtonText;
				_awaitingRebindButton = null;
				_awaitingRebindAction = null;
				_originalButtonText = null;
				IsAwaitingRebind = false;
				parent._pauseCooldown = 0.1f;
				GetViewport().SetInputAsHandled();
			}
			else
			{
				FinishRebind(@event);
				GetViewport().SetInputAsHandled();
			}
		}
	}

	public void ResetInputBindings()
	{
		InputMap.LoadFromProjectSettings();
		Array<Node> children = scrollInputBox.GetChildren();
		for (int i = 1; i < children.Count; i++)
		{
			Node node = children[i];
			string text = TrackedActions[i - 1];
			if (InputMap.HasAction(text))
			{
				node.GetChild<Button>(1).GetChild<RichTextLabel>(0).Text = GetBindingText(text, 0);
				node.GetChild<Button>(2).GetChild<RichTextLabel>(0).Text = GetBindingText(text, 1);
			}
		}
		_awaitingRebindButton = null;
		_awaitingRebindAction = null;
		_originalButtonText = null;
		Main.Instance.saveHandler.SaveSettings();
	}

	public void SetupMods()
	{
		ModsEnabledButton.ButtonPressed = Main.Instance.settingMods;
		ModsEnabledButton.Text = (Main.Instance.settingMods ? "On" : "Off");
		if (Main.Instance.settingMods)
		{
			PopulateModList();
		}
	}

	public void ToggleModsEnabled()
	{
		if (ModsEnabledButton.ButtonPressed && !Main.Instance.settingMods)
		{
			ConfirmationMenu confirmationMenu = this.confirmationMenu.Instantiate<ConfirmationMenu>(PackedScene.GenEditState.Disabled);
			AddChild(confirmationMenu, forceReadableName: false, InternalMode.Disabled);
			confirmationMenu.UnpauseOnClose = false;
			confirmationMenu.label.Text = "[font_size=14]Modding can be dangerous; Do you agree to the MODs EULA?[/font_size]";
			confirmationMenu.Confirmed += delegate
			{
				Main.Instance.settingMods = true;
				ModsEnabledButton.ButtonPressed = true;
				ModsEnabledButton.Text = "On";
				Main.Instance.saveHandler.SaveSettings();
				PopulateModList();
			};
			confirmationMenu.Deny += delegate
			{
				ModsEnabledButton.ButtonPressed = false;
				ModsEnabledButton.Text = "Off";
				Main.Instance.settingMods = false;
			};
		}
		else
		{
			if (ModsEnabledButton.ButtonPressed)
			{
				Main.Instance.settingMods = true;
				ModsEnabledButton.Text = "On";
				PopulateModList();
			}
			else
			{
				Main.Instance.settingMods = false;
				ModsEnabledButton.Text = "Off";
				ClearModList();
				Main.Instance.settingEnabledMods.Clear();
				flagForFullReload = true;
			}
			Main.Instance.saveHandler.SaveSettings();
		}
	}

	private void PopulateModList()
	{
		ClearModList();
		foreach (KeyValuePair<string, ModManifest> modManifest in ResourceCache.modManifests)
		{
			ModManifest manifest = modManifest.Value;
			Node node = ToggleButtonPack.Instantiate(PackedScene.GenEditState.Disabled);
			scrollModBox.AddChild(node, forceReadableName: false, InternalMode.Disabled);
			node.GetChild<RichTextLabel>(0).Text = $"[font_size=22]{manifest.Name} v{manifest.Version}\n[/font_size][font_size=18]{manifest.Description}[/font_size]";
			Node child = node.GetChild<Node>(1);
			CheckButton checkBox = child.GetChild<CheckButton>(0);
			if (Main.Instance.settingEnabledMods.Contains(manifest.ID))
			{
				checkBox.ButtonPressed = true;
				checkBox.Text = "Enabled";
			}
			else
			{
				checkBox.ButtonPressed = false;
				checkBox.Text = "Disabled";
			}
			checkBox.Toggled += delegate(bool pressed)
			{
				if (pressed)
				{
					if (!Main.Instance.settingEnabledMods.Contains(manifest.ID))
					{
						Main.Instance.settingEnabledMods.Add(manifest.ID);
					}
					checkBox.Text = "Enabled";
				}
				else
				{
					Main.Instance.settingEnabledMods.Remove(manifest.ID);
					checkBox.Text = "Disabled";
				}
				Main.Instance.saveHandler.SaveSettings();
				flagForFullReload = true;
			};
		}
	}

	private void ClearModList()
	{
		Array<Node> children = scrollModBox.GetChildren();
		for (int i = 1; i < children.Count; i++)
		{
			children[i].QueueFree();
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(34)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetupScreenPositions, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetFontSizeOnChildren, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "parent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false)
			}, null),
			new MethodInfo(MethodName.BackToParentButton, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CloseSettingsWindow, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HeadToState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "stateInt", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetupScaling, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupRenderingSelect, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ChangeRenderingDriver, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "index", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PlatformSuffix, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, null, null),
			new MethodInfo(MethodName.ChangeActorScaling, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "index", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ChangeItemScaling, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "index", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ChangeUIScaling, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "index", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetupGameplay, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ToggleItemSpawns, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ToggleActorSpawns, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.TogglePassivePlay, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ToggleDisablePopups, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ToggleDisableConvos, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupAudio, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ToggleAudioSetting, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupInput, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ActionToDisplayName, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "action", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetBindingText, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "action", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Int, "index", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.InputEventToText, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "inputEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
			}, null),
			new MethodInfo(MethodName.BeginRebind, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "button", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Button"), exported: false),
				new PropertyInfo(Variant.Type.String, "action", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Int, "bindIndex", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FinishRebind, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "newEvent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
			}, null),
			new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "event", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
			}, null),
			new MethodInfo(MethodName.ResetInputBindings, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupMods, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ToggleModsEnabled, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.PopulateModList, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.ClearModList, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupScreenPositions && args.Count == 0)
		{
			SetupScreenPositions();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetFontSizeOnChildren && args.Count == 1)
		{
			SetFontSizeOnChildren(VariantUtils.ConvertTo<Node>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BackToParentButton && args.Count == 0)
		{
			BackToParentButton();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CloseSettingsWindow && args.Count == 0)
		{
			CloseSettingsWindow();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HeadToState && args.Count == 1)
		{
			HeadToState(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupScaling && args.Count == 0)
		{
			SetupScaling();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupRenderingSelect && args.Count == 0)
		{
			SetupRenderingSelect();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ChangeRenderingDriver && args.Count == 1)
		{
			ChangeRenderingDriver(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlatformSuffix && args.Count == 0)
		{
			string from = PlatformSuffix();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.ChangeActorScaling && args.Count == 1)
		{
			ChangeActorScaling(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ChangeItemScaling && args.Count == 1)
		{
			ChangeItemScaling(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ChangeUIScaling && args.Count == 1)
		{
			ChangeUIScaling(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupGameplay && args.Count == 0)
		{
			SetupGameplay();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ToggleItemSpawns && args.Count == 0)
		{
			ToggleItemSpawns();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ToggleActorSpawns && args.Count == 0)
		{
			ToggleActorSpawns();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.TogglePassivePlay && args.Count == 0)
		{
			TogglePassivePlay();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ToggleDisablePopups && args.Count == 0)
		{
			ToggleDisablePopups();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ToggleDisableConvos && args.Count == 0)
		{
			ToggleDisableConvos();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupAudio && args.Count == 0)
		{
			SetupAudio();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ToggleAudioSetting && args.Count == 0)
		{
			ToggleAudioSetting();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupInput && args.Count == 0)
		{
			SetupInput();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ActionToDisplayName && args.Count == 1)
		{
			string from2 = ActionToDisplayName(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.GetBindingText && args.Count == 2)
		{
			string from3 = GetBindingText(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<int>(in args[1]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.InputEventToText && args.Count == 1)
		{
			string from4 = InputEventToText(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.BeginRebind && args.Count == 3)
		{
			BeginRebind(VariantUtils.ConvertTo<Button>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]), VariantUtils.ConvertTo<int>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FinishRebind && args.Count == 1)
		{
			FinishRebind(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ResetInputBindings && args.Count == 0)
		{
			ResetInputBindings();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupMods && args.Count == 0)
		{
			SetupMods();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ToggleModsEnabled && args.Count == 0)
		{
			ToggleModsEnabled();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PopulateModList && args.Count == 0)
		{
			PopulateModList();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ClearModList && args.Count == 0)
		{
			ClearModList();
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.PlatformSuffix && args.Count == 0)
		{
			string from = PlatformSuffix();
			ret = VariantUtils.CreateFrom(in from);
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
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.SetupScreenPositions)
		{
			return true;
		}
		if (method == MethodName.SetFontSizeOnChildren)
		{
			return true;
		}
		if (method == MethodName.BackToParentButton)
		{
			return true;
		}
		if (method == MethodName.CloseSettingsWindow)
		{
			return true;
		}
		if (method == MethodName.HeadToState)
		{
			return true;
		}
		if (method == MethodName.SetupScaling)
		{
			return true;
		}
		if (method == MethodName.SetupRenderingSelect)
		{
			return true;
		}
		if (method == MethodName.ChangeRenderingDriver)
		{
			return true;
		}
		if (method == MethodName.PlatformSuffix)
		{
			return true;
		}
		if (method == MethodName.ChangeActorScaling)
		{
			return true;
		}
		if (method == MethodName.ChangeItemScaling)
		{
			return true;
		}
		if (method == MethodName.ChangeUIScaling)
		{
			return true;
		}
		if (method == MethodName.SetupGameplay)
		{
			return true;
		}
		if (method == MethodName.ToggleItemSpawns)
		{
			return true;
		}
		if (method == MethodName.ToggleActorSpawns)
		{
			return true;
		}
		if (method == MethodName.TogglePassivePlay)
		{
			return true;
		}
		if (method == MethodName.ToggleDisablePopups)
		{
			return true;
		}
		if (method == MethodName.ToggleDisableConvos)
		{
			return true;
		}
		if (method == MethodName.SetupAudio)
		{
			return true;
		}
		if (method == MethodName.ToggleAudioSetting)
		{
			return true;
		}
		if (method == MethodName.SetupInput)
		{
			return true;
		}
		if (method == MethodName.ActionToDisplayName)
		{
			return true;
		}
		if (method == MethodName.GetBindingText)
		{
			return true;
		}
		if (method == MethodName.InputEventToText)
		{
			return true;
		}
		if (method == MethodName.BeginRebind)
		{
			return true;
		}
		if (method == MethodName.FinishRebind)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.ResetInputBindings)
		{
			return true;
		}
		if (method == MethodName.SetupMods)
		{
			return true;
		}
		if (method == MethodName.ToggleModsEnabled)
		{
			return true;
		}
		if (method == MethodName.PopulateModList)
		{
			return true;
		}
		if (method == MethodName.ClearModList)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.parentControl)
		{
			parentControl = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.scaleControl)
		{
			scaleControl = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.gameplayControl)
		{
			gameplayControl = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.audioControl)
		{
			audioControl = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.inputControl)
		{
			inputControl = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.modControl)
		{
			modControl = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.renderingSelect)
		{
			renderingSelect = VariantUtils.ConvertTo<OptionButton>(in value);
			return true;
		}
		if (name == PropertyName.petScaleSelect)
		{
			petScaleSelect = VariantUtils.ConvertTo<OptionButton>(in value);
			return true;
		}
		if (name == PropertyName.itemScaleSelect)
		{
			itemScaleSelect = VariantUtils.ConvertTo<OptionButton>(in value);
			return true;
		}
		if (name == PropertyName.uiScaleSelect)
		{
			uiScaleSelect = VariantUtils.ConvertTo<OptionButton>(in value);
			return true;
		}
		if (name == PropertyName.ItemButton)
		{
			ItemButton = VariantUtils.ConvertTo<CheckButton>(in value);
			return true;
		}
		if (name == PropertyName.ActorButton)
		{
			ActorButton = VariantUtils.ConvertTo<CheckButton>(in value);
			return true;
		}
		if (name == PropertyName.AIButton)
		{
			AIButton = VariantUtils.ConvertTo<CheckButton>(in value);
			return true;
		}
		if (name == PropertyName.PopupButton)
		{
			PopupButton = VariantUtils.ConvertTo<CheckButton>(in value);
			return true;
		}
		if (name == PropertyName.ConvoButton)
		{
			ConvoButton = VariantUtils.ConvertTo<CheckButton>(in value);
			return true;
		}
		if (name == PropertyName.AudioButton)
		{
			AudioButton = VariantUtils.ConvertTo<CheckButton>(in value);
			return true;
		}
		if (name == PropertyName.scrollInputBox)
		{
			scrollInputBox = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.InputButtonPack)
		{
			InputButtonPack = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.scrollModBox)
		{
			scrollModBox = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.ToggleButtonPack)
		{
			ToggleButtonPack = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.confirmationMenu)
		{
			confirmationMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.ModsEnabledButton)
		{
			ModsEnabledButton = VariantUtils.ConvertTo<CheckButton>(in value);
			return true;
		}
		if (name == PropertyName.parent)
		{
			parent = VariantUtils.ConvertTo<PauseMenu>(in value);
			return true;
		}
		if (name == PropertyName.state)
		{
			state = VariantUtils.ConvertTo<ScreenState>(in value);
			return true;
		}
		if (name == PropertyName.originalActorIndex)
		{
			originalActorIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.originalItemIndex)
		{
			originalItemIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.originalUIIndex)
		{
			originalUIIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.flagForReload)
		{
			flagForReload = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.flagForFullReload)
		{
			flagForFullReload = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.IsAwaitingRebind)
		{
			IsAwaitingRebind = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._awaitingRebindButton)
		{
			_awaitingRebindButton = VariantUtils.ConvertTo<Button>(in value);
			return true;
		}
		if (name == PropertyName._awaitingRebindAction)
		{
			_awaitingRebindAction = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName._awaitingRebindIndex)
		{
			_awaitingRebindIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName._originalButtonText)
		{
			_originalButtonText = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.parentControl)
		{
			value = VariantUtils.CreateFrom(in parentControl);
			return true;
		}
		if (name == PropertyName.scaleControl)
		{
			value = VariantUtils.CreateFrom(in scaleControl);
			return true;
		}
		if (name == PropertyName.gameplayControl)
		{
			value = VariantUtils.CreateFrom(in gameplayControl);
			return true;
		}
		if (name == PropertyName.audioControl)
		{
			value = VariantUtils.CreateFrom(in audioControl);
			return true;
		}
		if (name == PropertyName.inputControl)
		{
			value = VariantUtils.CreateFrom(in inputControl);
			return true;
		}
		if (name == PropertyName.modControl)
		{
			value = VariantUtils.CreateFrom(in modControl);
			return true;
		}
		if (name == PropertyName.renderingSelect)
		{
			value = VariantUtils.CreateFrom(in renderingSelect);
			return true;
		}
		if (name == PropertyName.petScaleSelect)
		{
			value = VariantUtils.CreateFrom(in petScaleSelect);
			return true;
		}
		if (name == PropertyName.itemScaleSelect)
		{
			value = VariantUtils.CreateFrom(in itemScaleSelect);
			return true;
		}
		if (name == PropertyName.uiScaleSelect)
		{
			value = VariantUtils.CreateFrom(in uiScaleSelect);
			return true;
		}
		if (name == PropertyName.ItemButton)
		{
			value = VariantUtils.CreateFrom(in ItemButton);
			return true;
		}
		if (name == PropertyName.ActorButton)
		{
			value = VariantUtils.CreateFrom(in ActorButton);
			return true;
		}
		if (name == PropertyName.AIButton)
		{
			value = VariantUtils.CreateFrom(in AIButton);
			return true;
		}
		if (name == PropertyName.PopupButton)
		{
			value = VariantUtils.CreateFrom(in PopupButton);
			return true;
		}
		if (name == PropertyName.ConvoButton)
		{
			value = VariantUtils.CreateFrom(in ConvoButton);
			return true;
		}
		if (name == PropertyName.AudioButton)
		{
			value = VariantUtils.CreateFrom(in AudioButton);
			return true;
		}
		if (name == PropertyName.scrollInputBox)
		{
			value = VariantUtils.CreateFrom(in scrollInputBox);
			return true;
		}
		if (name == PropertyName.InputButtonPack)
		{
			value = VariantUtils.CreateFrom(in InputButtonPack);
			return true;
		}
		if (name == PropertyName.scrollModBox)
		{
			value = VariantUtils.CreateFrom(in scrollModBox);
			return true;
		}
		if (name == PropertyName.ToggleButtonPack)
		{
			value = VariantUtils.CreateFrom(in ToggleButtonPack);
			return true;
		}
		if (name == PropertyName.confirmationMenu)
		{
			value = VariantUtils.CreateFrom(in confirmationMenu);
			return true;
		}
		if (name == PropertyName.ModsEnabledButton)
		{
			value = VariantUtils.CreateFrom(in ModsEnabledButton);
			return true;
		}
		if (name == PropertyName.parent)
		{
			value = VariantUtils.CreateFrom(in parent);
			return true;
		}
		if (name == PropertyName.state)
		{
			value = VariantUtils.CreateFrom(in state);
			return true;
		}
		if (name == PropertyName.originalActorIndex)
		{
			value = VariantUtils.CreateFrom(in originalActorIndex);
			return true;
		}
		if (name == PropertyName.originalItemIndex)
		{
			value = VariantUtils.CreateFrom(in originalItemIndex);
			return true;
		}
		if (name == PropertyName.originalUIIndex)
		{
			value = VariantUtils.CreateFrom(in originalUIIndex);
			return true;
		}
		if (name == PropertyName.flagForReload)
		{
			value = VariantUtils.CreateFrom(in flagForReload);
			return true;
		}
		if (name == PropertyName.flagForFullReload)
		{
			value = VariantUtils.CreateFrom(in flagForFullReload);
			return true;
		}
		if (name == PropertyName.IsAwaitingRebind)
		{
			value = VariantUtils.CreateFrom(in IsAwaitingRebind);
			return true;
		}
		if (name == PropertyName._awaitingRebindButton)
		{
			value = VariantUtils.CreateFrom(in _awaitingRebindButton);
			return true;
		}
		if (name == PropertyName._awaitingRebindAction)
		{
			value = VariantUtils.CreateFrom(in _awaitingRebindAction);
			return true;
		}
		if (name == PropertyName._awaitingRebindIndex)
		{
			value = VariantUtils.CreateFrom(in _awaitingRebindIndex);
			return true;
		}
		if (name == PropertyName._originalButtonText)
		{
			value = VariantUtils.CreateFrom(in _originalButtonText);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.parentControl, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.scaleControl, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.gameplayControl, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.audioControl, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.inputControl, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.modControl, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Scale Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.renderingSelect, PropertyHint.NodeType, "OptionButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.petScaleSelect, PropertyHint.NodeType, "OptionButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.itemScaleSelect, PropertyHint.NodeType, "OptionButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.uiScaleSelect, PropertyHint.NodeType, "OptionButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Gameplay Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ItemButton, PropertyHint.NodeType, "CheckButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ActorButton, PropertyHint.NodeType, "CheckButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.AIButton, PropertyHint.NodeType, "CheckButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.PopupButton, PropertyHint.NodeType, "CheckButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ConvoButton, PropertyHint.NodeType, "CheckButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Audio Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.AudioButton, PropertyHint.NodeType, "CheckButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Input Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.scrollInputBox, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.InputButtonPack, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Mod Settings", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.scrollModBox, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ToggleButtonPack, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.confirmationMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ModsEnabledButton, PropertyHint.NodeType, "CheckButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.parent, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.state, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.originalActorIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.originalItemIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.originalUIIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.flagForReload, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.flagForFullReload, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.IsAwaitingRebind, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName._awaitingRebindButton, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName._awaitingRebindAction, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName._awaitingRebindIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.String, PropertyName._originalButtonText, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.parentControl, Variant.From(in parentControl));
		info.AddProperty(PropertyName.scaleControl, Variant.From(in scaleControl));
		info.AddProperty(PropertyName.gameplayControl, Variant.From(in gameplayControl));
		info.AddProperty(PropertyName.audioControl, Variant.From(in audioControl));
		info.AddProperty(PropertyName.inputControl, Variant.From(in inputControl));
		info.AddProperty(PropertyName.modControl, Variant.From(in modControl));
		info.AddProperty(PropertyName.renderingSelect, Variant.From(in renderingSelect));
		info.AddProperty(PropertyName.petScaleSelect, Variant.From(in petScaleSelect));
		info.AddProperty(PropertyName.itemScaleSelect, Variant.From(in itemScaleSelect));
		info.AddProperty(PropertyName.uiScaleSelect, Variant.From(in uiScaleSelect));
		info.AddProperty(PropertyName.ItemButton, Variant.From(in ItemButton));
		info.AddProperty(PropertyName.ActorButton, Variant.From(in ActorButton));
		info.AddProperty(PropertyName.AIButton, Variant.From(in AIButton));
		info.AddProperty(PropertyName.PopupButton, Variant.From(in PopupButton));
		info.AddProperty(PropertyName.ConvoButton, Variant.From(in ConvoButton));
		info.AddProperty(PropertyName.AudioButton, Variant.From(in AudioButton));
		info.AddProperty(PropertyName.scrollInputBox, Variant.From(in scrollInputBox));
		info.AddProperty(PropertyName.InputButtonPack, Variant.From(in InputButtonPack));
		info.AddProperty(PropertyName.scrollModBox, Variant.From(in scrollModBox));
		info.AddProperty(PropertyName.ToggleButtonPack, Variant.From(in ToggleButtonPack));
		info.AddProperty(PropertyName.confirmationMenu, Variant.From(in confirmationMenu));
		info.AddProperty(PropertyName.ModsEnabledButton, Variant.From(in ModsEnabledButton));
		info.AddProperty(PropertyName.parent, Variant.From(in parent));
		info.AddProperty(PropertyName.state, Variant.From(in state));
		info.AddProperty(PropertyName.originalActorIndex, Variant.From(in originalActorIndex));
		info.AddProperty(PropertyName.originalItemIndex, Variant.From(in originalItemIndex));
		info.AddProperty(PropertyName.originalUIIndex, Variant.From(in originalUIIndex));
		info.AddProperty(PropertyName.flagForReload, Variant.From(in flagForReload));
		info.AddProperty(PropertyName.flagForFullReload, Variant.From(in flagForFullReload));
		info.AddProperty(PropertyName.IsAwaitingRebind, Variant.From(in IsAwaitingRebind));
		info.AddProperty(PropertyName._awaitingRebindButton, Variant.From(in _awaitingRebindButton));
		info.AddProperty(PropertyName._awaitingRebindAction, Variant.From(in _awaitingRebindAction));
		info.AddProperty(PropertyName._awaitingRebindIndex, Variant.From(in _awaitingRebindIndex));
		info.AddProperty(PropertyName._originalButtonText, Variant.From(in _originalButtonText));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.parentControl, out var value))
		{
			parentControl = value.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.scaleControl, out var value2))
		{
			scaleControl = value2.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.gameplayControl, out var value3))
		{
			gameplayControl = value3.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.audioControl, out var value4))
		{
			audioControl = value4.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.inputControl, out var value5))
		{
			inputControl = value5.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.modControl, out var value6))
		{
			modControl = value6.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.renderingSelect, out var value7))
		{
			renderingSelect = value7.As<OptionButton>();
		}
		if (info.TryGetProperty(PropertyName.petScaleSelect, out var value8))
		{
			petScaleSelect = value8.As<OptionButton>();
		}
		if (info.TryGetProperty(PropertyName.itemScaleSelect, out var value9))
		{
			itemScaleSelect = value9.As<OptionButton>();
		}
		if (info.TryGetProperty(PropertyName.uiScaleSelect, out var value10))
		{
			uiScaleSelect = value10.As<OptionButton>();
		}
		if (info.TryGetProperty(PropertyName.ItemButton, out var value11))
		{
			ItemButton = value11.As<CheckButton>();
		}
		if (info.TryGetProperty(PropertyName.ActorButton, out var value12))
		{
			ActorButton = value12.As<CheckButton>();
		}
		if (info.TryGetProperty(PropertyName.AIButton, out var value13))
		{
			AIButton = value13.As<CheckButton>();
		}
		if (info.TryGetProperty(PropertyName.PopupButton, out var value14))
		{
			PopupButton = value14.As<CheckButton>();
		}
		if (info.TryGetProperty(PropertyName.ConvoButton, out var value15))
		{
			ConvoButton = value15.As<CheckButton>();
		}
		if (info.TryGetProperty(PropertyName.AudioButton, out var value16))
		{
			AudioButton = value16.As<CheckButton>();
		}
		if (info.TryGetProperty(PropertyName.scrollInputBox, out var value17))
		{
			scrollInputBox = value17.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.InputButtonPack, out var value18))
		{
			InputButtonPack = value18.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.scrollModBox, out var value19))
		{
			scrollModBox = value19.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.ToggleButtonPack, out var value20))
		{
			ToggleButtonPack = value20.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.confirmationMenu, out var value21))
		{
			confirmationMenu = value21.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.ModsEnabledButton, out var value22))
		{
			ModsEnabledButton = value22.As<CheckButton>();
		}
		if (info.TryGetProperty(PropertyName.parent, out var value23))
		{
			parent = value23.As<PauseMenu>();
		}
		if (info.TryGetProperty(PropertyName.state, out var value24))
		{
			state = value24.As<ScreenState>();
		}
		if (info.TryGetProperty(PropertyName.originalActorIndex, out var value25))
		{
			originalActorIndex = value25.As<int>();
		}
		if (info.TryGetProperty(PropertyName.originalItemIndex, out var value26))
		{
			originalItemIndex = value26.As<int>();
		}
		if (info.TryGetProperty(PropertyName.originalUIIndex, out var value27))
		{
			originalUIIndex = value27.As<int>();
		}
		if (info.TryGetProperty(PropertyName.flagForReload, out var value28))
		{
			flagForReload = value28.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.flagForFullReload, out var value29))
		{
			flagForFullReload = value29.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.IsAwaitingRebind, out var value30))
		{
			IsAwaitingRebind = value30.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._awaitingRebindButton, out var value31))
		{
			_awaitingRebindButton = value31.As<Button>();
		}
		if (info.TryGetProperty(PropertyName._awaitingRebindAction, out var value32))
		{
			_awaitingRebindAction = value32.As<string>();
		}
		if (info.TryGetProperty(PropertyName._awaitingRebindIndex, out var value33))
		{
			_awaitingRebindIndex = value33.As<int>();
		}
		if (info.TryGetProperty(PropertyName._originalButtonText, out var value34))
		{
			_originalButtonText = value34.As<string>();
		}
	}
}
