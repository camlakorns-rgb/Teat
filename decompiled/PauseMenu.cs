using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/PauseMenu/PauseMenu.cs")]
public class PauseMenu : Window
{
	public enum PauseState
	{
		UNTYPED,
		GUIDE
	}

	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName SetFontSizeOnChildren = "SetFontSizeOnChildren";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName BackToParentButton = "BackToParentButton";

		public static readonly StringName HeadToState = "HeadToState";

		public static readonly StringName SetupScreenPositions = "SetupScreenPositions";

		public static readonly StringName OnClose = "OnClose";

		public static readonly StringName CloseGame = "CloseGame";

		public static readonly StringName OpenRecipesMenu = "OpenRecipesMenu";

		public static readonly StringName OpenSettingsMenu = "OpenSettingsMenu";

		public static readonly StringName OpenGalleryMenu = "OpenGalleryMenu";

		public static readonly StringName OnMetaClicked = "OnMetaClicked";

		public static readonly StringName UpdateGuideText = "UpdateGuideText";

		public static readonly StringName FixKeyName = "FixKeyName";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName MenuHolder = "MenuHolder";

		public static readonly StringName TitleText = "TitleText";

		public static readonly StringName parentMenu = "parentMenu";

		public static readonly StringName guideBook = "guideBook";

		public static readonly StringName guideText = "guideText";

		public static readonly StringName recipeMenu = "recipeMenu";

		public static readonly StringName settingsMenu = "settingsMenu";

		public static readonly StringName galleryMenu = "galleryMenu";

		public static readonly StringName state = "state";

		public static readonly StringName MouseIn = "MouseIn";

		public static readonly StringName _pauseCooldown = "_pauseCooldown";

		public static readonly StringName openSettings = "openSettings";

		public static readonly StringName openRecipe = "openRecipe";

		public static readonly StringName presentConfirmation = "presentConfirmation";
	}

	public new class SignalName : Window.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private Node MenuHolder;

	[Export(PropertyHint.None, "")]
	private RichTextLabel TitleText;

	[Export(PropertyHint.None, "")]
	private Control parentMenu;

	[Export(PropertyHint.None, "")]
	private Control guideBook;

	[Export(PropertyHint.None, "")]
	private RichTextLabel guideText;

	[Export(PropertyHint.None, "")]
	private PackedScene recipeMenu;

	[Export(PropertyHint.None, "")]
	private PackedScene settingsMenu;

	[Export(PropertyHint.None, "")]
	private PackedScene galleryMenu;

	private PauseState state;

	private bool MouseIn;

	public float _pauseCooldown = 0.2f;

	public SettingsMenu openSettings;

	public RecipeMenuHandler openRecipe;

	private ConfirmationMenu presentConfirmation;

	public override void _Ready()
	{
		string text = ProjectSettings.GetSetting("application/config/version").AsString();
		TitleText.Text = TitleText.Text.Replace("{v#}", "v" + text);
		SetupScreenPositions();
		SetFontSizeOnChildren(this);
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

	public override void _Process(double delta)
	{
		if (_pauseCooldown > 0f)
		{
			_pauseCooldown -= (float)delta;
		}
		if (Input.IsActionJustPressed("PauseGame") && _pauseCooldown <= 0f)
		{
			OnClose();
		}
		switch (state)
		{
		case PauseState.UNTYPED:
			parentMenu.Position = parentMenu.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			guideBook.Position = guideBook.Position.Lerp(new Vector2(-base.Size.X, 0f), (float)delta * 10f);
			break;
		case PauseState.GUIDE:
			guideBook.Position = guideBook.Position.Lerp(new Vector2(0f, 0f), (float)delta * 10f);
			parentMenu.Position = parentMenu.Position.Lerp(new Vector2(base.Size.X, 0f), (float)delta * 10f);
			break;
		}
	}

	public void BackToParentButton()
	{
		state = PauseState.UNTYPED;
	}

	public void HeadToState(int stateInt)
	{
		if (presentConfirmation != null)
		{
			presentConfirmation.QueueFree();
			presentConfirmation = null;
		}
		state = (PauseState)stateInt;
		if (state == PauseState.GUIDE)
		{
			UpdateGuideText();
		}
	}

	public void SetupScreenPositions()
	{
		guideBook.Position = new Vector2(-base.Size.X, 0f);
		guideBook.Visible = true;
	}

	public void OnClose()
	{
		if (presentConfirmation != null)
		{
			presentConfirmation.QueueFree();
			presentConfirmation = null;
		}
		if (openSettings != null)
		{
			openSettings.CloseSettingsWindow();
			presentConfirmation = null;
		}
		foreach (MinigameBase spawnedMinigame in Main.Instance.spawnedMinigames)
		{
			spawnedMinigame.PauseGame(Pause: false);
		}
		GetTree().Paused = false;
		Main.Instance.mainWindow.GrabFocus();
		Main.Instance.Pause = null;
		QueueFree();
	}

	public void CloseGame()
	{
		if (presentConfirmation == null)
		{
			ConfirmationMenu confirmationMenu = Main.Instance.confirmationMenu.Instantiate<ConfirmationMenu>(PackedScene.GenEditState.Disabled);
			MenuHolder.AddChild(confirmationMenu, forceReadableName: false, InternalMode.Disabled);
			confirmationMenu.label.Text = "[font_size=32]Close Byte Fully?[/font_size]";
			confirmationMenu.Confirmed += delegate
			{
				GetTree().Quit();
			};
			confirmationMenu.Deny += delegate
			{
				presentConfirmation = null;
			};
			presentConfirmation = confirmationMenu;
		}
	}

	public void OpenRecipesMenu()
	{
		if (presentConfirmation != null)
		{
			presentConfirmation.QueueFree();
			presentConfirmation = null;
		}
		RecipeMenuHandler recipeMenuHandler = recipeMenu.Instantiate<RecipeMenuHandler>(PackedScene.GenEditState.Disabled);
		recipeMenuHandler.Size = new Vector2I(Mathf.RoundToInt((float)recipeMenuHandler.Size.X * Main.Instance.settingUIScaler), Mathf.RoundToInt((float)recipeMenuHandler.Size.Y * Main.Instance.settingUIScaler));
		MenuHolder.AddChild(recipeMenuHandler, forceReadableName: false, InternalMode.Disabled);
		openRecipe = recipeMenuHandler;
	}

	public void OpenSettingsMenu()
	{
		if (presentConfirmation != null)
		{
			presentConfirmation.QueueFree();
			presentConfirmation = null;
		}
		SettingsMenu settingsMenu = this.settingsMenu.Instantiate<SettingsMenu>(PackedScene.GenEditState.Disabled);
		settingsMenu.Size = new Vector2I(Mathf.RoundToInt((float)settingsMenu.Size.X * Main.Instance.settingUIScaler), Mathf.RoundToInt((float)settingsMenu.Size.Y * Main.Instance.settingUIScaler));
		MenuHolder.AddChild(settingsMenu, forceReadableName: false, InternalMode.Disabled);
		openSettings = settingsMenu;
		openSettings.parent = this;
	}

	public void OpenGalleryMenu()
	{
		if (presentConfirmation != null)
		{
			presentConfirmation.QueueFree();
			presentConfirmation = null;
		}
		GalleryWindow galleryWindow = galleryMenu.Instantiate<GalleryWindow>(PackedScene.GenEditState.Disabled);
		galleryWindow.Size = new Vector2I(Mathf.RoundToInt((float)galleryWindow.Size.X * Main.Instance.settingUIScaler), Mathf.RoundToInt((float)galleryWindow.Size.Y * Main.Instance.settingUIScaler));
		MenuHolder.AddChild(galleryWindow, forceReadableName: false, InternalMode.Disabled);
	}

	public void OnMetaClicked(Variant meta)
	{
		OS.ShellOpen(meta.ToString());
	}

	private void UpdateGuideText()
	{
		string text = Regex.Replace(guideText.Text, "\\{(\\w+)(?:\\((\\d+)\\))?\\}", delegate(Match match)
		{
			string value = match.Groups[1].Value;
			int num = (match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0);
			if (!InputMap.HasAction(value))
			{
				return "Unbound";
			}
			Array<InputEvent> array = InputMap.ActionGetEvents(value);
			int num2 = 0;
			foreach (InputEvent item in array)
			{
				if (item is InputEventKey inputEventKey)
				{
					if (num2 == num)
					{
						return FixKeyName(OS.GetKeycodeString(inputEventKey.PhysicalKeycode));
					}
					num2++;
				}
				else if (item is InputEventMouseButton inputEventMouseButton)
				{
					if (num2 == num)
					{
						MouseButton buttonIndex = inputEventMouseButton.ButtonIndex;
						MouseButton num3 = buttonIndex - 1;
						if ((ulong)num3 <= 8uL)
						{
							switch (num3)
							{
							case MouseButton.None:
								return "Left Click";
							case MouseButton.Left:
								return "Right Click";
							case MouseButton.Right:
								return "Middle Click";
							case MouseButton.Middle:
								return "Scroll Up";
							case MouseButton.WheelUp:
								return "Scroll Down";
							case MouseButton.WheelRight:
								return "Mouse 4";
							case MouseButton.Xbutton1:
								return "Mouse 5";
							}
						}
						return inputEventMouseButton.ButtonIndex.ToString();
					}
					num2++;
				}
			}
			return "Unbound";
		});
		guideText.Text = text;
	}

	private string FixKeyName(string keyName)
	{
		if (keyName == "QuoteLeft")
		{
			return "`";
		}
		return keyName;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(14)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetFontSizeOnChildren, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "parent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node"), exported: false)
			}, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.BackToParentButton, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.HeadToState, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Int, "stateInt", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetupScreenPositions, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnClose, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.CloseGame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OpenRecipesMenu, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OpenSettingsMenu, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OpenGalleryMenu, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnMetaClicked, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Nil, "meta", PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.NilIsVariant, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateGuideText, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.FixKeyName, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "keyName", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
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
		if (method == MethodName.SetFontSizeOnChildren && args.Count == 1)
		{
			SetFontSizeOnChildren(VariantUtils.ConvertTo<Node>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BackToParentButton && args.Count == 0)
		{
			BackToParentButton();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.HeadToState && args.Count == 1)
		{
			HeadToState(VariantUtils.ConvertTo<int>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupScreenPositions && args.Count == 0)
		{
			SetupScreenPositions();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnClose && args.Count == 0)
		{
			OnClose();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.CloseGame && args.Count == 0)
		{
			CloseGame();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OpenRecipesMenu && args.Count == 0)
		{
			OpenRecipesMenu();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OpenSettingsMenu && args.Count == 0)
		{
			OpenSettingsMenu();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OpenGalleryMenu && args.Count == 0)
		{
			OpenGalleryMenu();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnMetaClicked && args.Count == 1)
		{
			OnMetaClicked(VariantUtils.ConvertTo<Variant>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateGuideText && args.Count == 0)
		{
			UpdateGuideText();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.FixKeyName && args.Count == 1)
		{
			string from = FixKeyName(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
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
		if (method == MethodName.SetFontSizeOnChildren)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.BackToParentButton)
		{
			return true;
		}
		if (method == MethodName.HeadToState)
		{
			return true;
		}
		if (method == MethodName.SetupScreenPositions)
		{
			return true;
		}
		if (method == MethodName.OnClose)
		{
			return true;
		}
		if (method == MethodName.CloseGame)
		{
			return true;
		}
		if (method == MethodName.OpenRecipesMenu)
		{
			return true;
		}
		if (method == MethodName.OpenSettingsMenu)
		{
			return true;
		}
		if (method == MethodName.OpenGalleryMenu)
		{
			return true;
		}
		if (method == MethodName.OnMetaClicked)
		{
			return true;
		}
		if (method == MethodName.UpdateGuideText)
		{
			return true;
		}
		if (method == MethodName.FixKeyName)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.MenuHolder)
		{
			MenuHolder = VariantUtils.ConvertTo<Node>(in value);
			return true;
		}
		if (name == PropertyName.TitleText)
		{
			TitleText = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.parentMenu)
		{
			parentMenu = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.guideBook)
		{
			guideBook = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.guideText)
		{
			guideText = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.recipeMenu)
		{
			recipeMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.settingsMenu)
		{
			settingsMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.galleryMenu)
		{
			galleryMenu = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.state)
		{
			state = VariantUtils.ConvertTo<PauseState>(in value);
			return true;
		}
		if (name == PropertyName.MouseIn)
		{
			MouseIn = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName._pauseCooldown)
		{
			_pauseCooldown = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.openSettings)
		{
			openSettings = VariantUtils.ConvertTo<SettingsMenu>(in value);
			return true;
		}
		if (name == PropertyName.openRecipe)
		{
			openRecipe = VariantUtils.ConvertTo<RecipeMenuHandler>(in value);
			return true;
		}
		if (name == PropertyName.presentConfirmation)
		{
			presentConfirmation = VariantUtils.ConvertTo<ConfirmationMenu>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.MenuHolder)
		{
			value = VariantUtils.CreateFrom(in MenuHolder);
			return true;
		}
		if (name == PropertyName.TitleText)
		{
			value = VariantUtils.CreateFrom(in TitleText);
			return true;
		}
		if (name == PropertyName.parentMenu)
		{
			value = VariantUtils.CreateFrom(in parentMenu);
			return true;
		}
		if (name == PropertyName.guideBook)
		{
			value = VariantUtils.CreateFrom(in guideBook);
			return true;
		}
		if (name == PropertyName.guideText)
		{
			value = VariantUtils.CreateFrom(in guideText);
			return true;
		}
		if (name == PropertyName.recipeMenu)
		{
			value = VariantUtils.CreateFrom(in recipeMenu);
			return true;
		}
		if (name == PropertyName.settingsMenu)
		{
			value = VariantUtils.CreateFrom(in settingsMenu);
			return true;
		}
		if (name == PropertyName.galleryMenu)
		{
			value = VariantUtils.CreateFrom(in galleryMenu);
			return true;
		}
		if (name == PropertyName.state)
		{
			value = VariantUtils.CreateFrom(in state);
			return true;
		}
		if (name == PropertyName.MouseIn)
		{
			value = VariantUtils.CreateFrom(in MouseIn);
			return true;
		}
		if (name == PropertyName._pauseCooldown)
		{
			value = VariantUtils.CreateFrom(in _pauseCooldown);
			return true;
		}
		if (name == PropertyName.openSettings)
		{
			value = VariantUtils.CreateFrom(in openSettings);
			return true;
		}
		if (name == PropertyName.openRecipe)
		{
			value = VariantUtils.CreateFrom(in openRecipe);
			return true;
		}
		if (name == PropertyName.presentConfirmation)
		{
			value = VariantUtils.CreateFrom(in presentConfirmation);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.MenuHolder, PropertyHint.NodeType, "Node", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.TitleText, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.parentMenu, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.guideBook, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.guideText, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.recipeMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.settingsMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.galleryMenu, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.state, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.MouseIn, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName._pauseCooldown, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.openSettings, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.openRecipe, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.presentConfirmation, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.MenuHolder, Variant.From(in MenuHolder));
		info.AddProperty(PropertyName.TitleText, Variant.From(in TitleText));
		info.AddProperty(PropertyName.parentMenu, Variant.From(in parentMenu));
		info.AddProperty(PropertyName.guideBook, Variant.From(in guideBook));
		info.AddProperty(PropertyName.guideText, Variant.From(in guideText));
		info.AddProperty(PropertyName.recipeMenu, Variant.From(in recipeMenu));
		info.AddProperty(PropertyName.settingsMenu, Variant.From(in settingsMenu));
		info.AddProperty(PropertyName.galleryMenu, Variant.From(in galleryMenu));
		info.AddProperty(PropertyName.state, Variant.From(in state));
		info.AddProperty(PropertyName.MouseIn, Variant.From(in MouseIn));
		info.AddProperty(PropertyName._pauseCooldown, Variant.From(in _pauseCooldown));
		info.AddProperty(PropertyName.openSettings, Variant.From(in openSettings));
		info.AddProperty(PropertyName.openRecipe, Variant.From(in openRecipe));
		info.AddProperty(PropertyName.presentConfirmation, Variant.From(in presentConfirmation));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.MenuHolder, out var value))
		{
			MenuHolder = value.As<Node>();
		}
		if (info.TryGetProperty(PropertyName.TitleText, out var value2))
		{
			TitleText = value2.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.parentMenu, out var value3))
		{
			parentMenu = value3.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.guideBook, out var value4))
		{
			guideBook = value4.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.guideText, out var value5))
		{
			guideText = value5.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.recipeMenu, out var value6))
		{
			recipeMenu = value6.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.settingsMenu, out var value7))
		{
			settingsMenu = value7.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.galleryMenu, out var value8))
		{
			galleryMenu = value8.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.state, out var value9))
		{
			state = value9.As<PauseState>();
		}
		if (info.TryGetProperty(PropertyName.MouseIn, out var value10))
		{
			MouseIn = value10.As<bool>();
		}
		if (info.TryGetProperty(PropertyName._pauseCooldown, out var value11))
		{
			_pauseCooldown = value11.As<float>();
		}
		if (info.TryGetProperty(PropertyName.openSettings, out var value12))
		{
			openSettings = value12.As<SettingsMenu>();
		}
		if (info.TryGetProperty(PropertyName.openRecipe, out var value13))
		{
			openRecipe = value13.As<RecipeMenuHandler>();
		}
		if (info.TryGetProperty(PropertyName.presentConfirmation, out var value14))
		{
			presentConfirmation = value14.As<ConfirmationMenu>();
		}
	}
}
