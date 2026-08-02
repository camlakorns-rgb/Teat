using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/DinoRunner/DR_GameHandler.cs")]
public class DR_GameHandler : MinigameBase
{
	public new class MethodName : MinigameBase.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName IsMouseOver = "IsMouseOver";

		public static readonly StringName calculateScore = "calculateScore";

		public static readonly StringName SpawnFloatingText = "SpawnFloatingText";

		public static readonly StringName restartGame = "restartGame";
	}

	public new class PropertyName : MinigameBase.PropertyName
	{
		public static readonly StringName ticketRatio = "ticketRatio";

		public static readonly StringName score = "score";

		public static readonly StringName scoreAccumulator = "scoreAccumulator";

		public static readonly StringName highScore = "highScore";

		public static readonly StringName gameSpeed = "gameSpeed";

		public static readonly StringName maxSpeed = "maxSpeed";

		public static readonly StringName baseScoreSpeed = "baseScoreSpeed";

		public static readonly StringName speedIncrement = "speedIncrement";

		public static readonly StringName jumpSound = "jumpSound";

		public static readonly StringName deathSound = "deathSound";

		public static readonly StringName scoreBoonSound = "scoreBoonSound";

		public static readonly StringName scoreMarkerSound = "scoreMarkerSound";

		public static readonly StringName scoreLabel = "scoreLabel";

		public static readonly StringName gameMasterNode = "gameMasterNode";

		public static readonly StringName dinoCharacter = "dinoCharacter";

		public static readonly StringName spawnedProps = "spawnedProps";

		public static readonly StringName spawner = "spawner";

		public static readonly StringName GameStart = "GameStart";

		public static readonly StringName GameStop = "GameStop";

		public static readonly StringName FloatingText = "FloatingText";

		public static readonly StringName cachedDinoPosition = "cachedDinoPosition";

		public static readonly StringName GameRunning = "GameRunning";
	}

	public new class SignalName : MinigameBase.SignalName
	{
	}

	[ExportGroup("Scores", "")]
	[Export(PropertyHint.None, "")]
	public float ticketRatio = 0.05f;

	public int score;

	public float scoreAccumulator;

	public int highScore;

	[ExportGroup("Speeds", "")]
	[Export(PropertyHint.None, "")]
	public float gameSpeed = 1f;

	[Export(PropertyHint.None, "")]
	public float maxSpeed = 2.5f;

	[Export(PropertyHint.None, "")]
	private float baseScoreSpeed = 8f;

	[Export(PropertyHint.None, "")]
	private float speedIncrement = 0.01f;

	[ExportGroup("Sounds", "")]
	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer jumpSound;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer deathSound;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer scoreBoonSound;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer scoreMarkerSound;

	[ExportGroup("Dependecies", "")]
	[Export(PropertyHint.None, "")]
	public RichTextLabel scoreLabel;

	[Export(PropertyHint.None, "")]
	public Node2D gameMasterNode;

	[Export(PropertyHint.None, "")]
	public Node2D dinoCharacter;

	[Export(PropertyHint.None, "")]
	public Node2D spawnedProps;

	[Export(PropertyHint.None, "")]
	public DR_Spawner spawner;

	[Export(PropertyHint.None, "")]
	public Control GameStart;

	[Export(PropertyHint.None, "")]
	public Control GameStop;

	[Export(PropertyHint.None, "")]
	public PackedScene FloatingText;

	public Vector2 cachedDinoPosition;

	public bool GameRunning;

	public override void _Ready()
	{
		base._Ready();
		Variant variant = LoadMinigame();
		highScore = ((variant.AsInt32() != -1) ? variant.AsInt32() : 0);
		GameStart.Visible = true;
		GameStop.Visible = false;
		gameMasterNode.ProcessMode = ProcessModeEnum.Disabled;
		cachedDinoPosition = dinoCharacter.Position;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (gameMasterNode.ProcessMode == ProcessModeEnum.Disabled)
		{
			if (Input.IsActionJustPressed("DR_Jump") && IsMouseOver() && !GameRunning && Main.Instance.Pause == null)
			{
				spawner.StartSpawner();
				restartGame();
			}
			return;
		}
		gameSpeed = Mathf.Min(gameSpeed + speedIncrement * (float)delta, maxSpeed);
		scoreAccumulator += baseScoreSpeed * gameSpeed * (float)delta;
		score = Mathf.FloorToInt(scoreAccumulator);
		if (score % 1000 == 0)
		{
			scoreMarkerSound.Play();
		}
		scoreLabel.Text = "[color=grey]HI: " + highScore.ToString().PadLeft(5, '0') + "[/color] " + score.ToString().PadLeft(5, '0');
	}

	private bool IsMouseOver()
	{
		return new Rect2(base.Position, base.Size).HasPoint(DisplayServer.MouseGetPosition());
	}

	public void calculateScore()
	{
		if (score > highScore)
		{
			highScore = score;
		}
		Main.Instance.userTickets += Mathf.FloorToInt((float)score * ticketRatio);
		int num = score;
		int num2 = ((num < 1000) ? ((num < 200) ? 1 : ((num >= 500) ? 3 : 2)) : ((num >= 5000) ? 5 : 4));
		int num3 = num2;
		Main.Instance.ClearAllAttachments();
		switch (num3)
		{
		case 1:
			Main.Instance.dialogueStack.Add(Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.MINIGAME_FAIL]);
			break;
		case 2:
			Main.Instance.dialogueStack.Add(Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.MINIGAME_OK]);
			break;
		case 3:
			Main.Instance.dialogueStack.Add(Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.MINIGAME_GOOD]);
			break;
		case 4:
			Main.Instance.dialogueStack.Add(Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.MINIGAME_GREAT]);
			break;
		case 5:
			Main.Instance.dialogueStack.Add(Main.Instance.mainCharacter.characterInformation.responseTexts[CharacterInfoDataRes.ResponseToSituation.MINIGAME_PERFECT]);
			break;
		}
		Main.Instance.PopDialogueInStack(skipTimer: true);
		Main.Instance.saveHandler.SaveSettings();
	}

	public void SpawnFloatingText(string text, Vector2 position)
	{
		if (FloatingText == null)
		{
			GD.PushWarning("FloatingText PackedScene is not assigned.");
			return;
		}
		Node2D node2D = FloatingText.Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
		if (node2D is FloatingText floatingText)
		{
			floatingText.Text = text;
		}
		node2D.Position = position;
		spawnedProps.AddChild(node2D, forceReadableName: false, InternalMode.Disabled);
	}

	public void restartGame()
	{
		GameStart.Visible = false;
		GameStop.Visible = false;
		gameSpeed = 1f;
		scoreAccumulator = 0f;
		score = 0;
		foreach (Node child in spawnedProps.GetChildren())
		{
			child.QueueFree();
		}
		dinoCharacter.Position = cachedDinoPosition;
		gameMasterNode.ProcessMode = ProcessModeEnum.Inherit;
		GameRunning = true;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(6)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.IsMouseOver, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.calculateScore, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SpawnFloatingText, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Vector2, "position", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.restartGame, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.IsMouseOver && args.Count == 0)
		{
			bool from = IsMouseOver();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.calculateScore && args.Count == 0)
		{
			calculateScore();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnFloatingText && args.Count == 2)
		{
			SpawnFloatingText(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<Vector2>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.restartGame && args.Count == 0)
		{
			restartGame();
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
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName.IsMouseOver)
		{
			return true;
		}
		if (method == MethodName.calculateScore)
		{
			return true;
		}
		if (method == MethodName.SpawnFloatingText)
		{
			return true;
		}
		if (method == MethodName.restartGame)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.ticketRatio)
		{
			ticketRatio = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.score)
		{
			score = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.scoreAccumulator)
		{
			scoreAccumulator = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.highScore)
		{
			highScore = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.gameSpeed)
		{
			gameSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.maxSpeed)
		{
			maxSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.baseScoreSpeed)
		{
			baseScoreSpeed = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.speedIncrement)
		{
			speedIncrement = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.jumpSound)
		{
			jumpSound = VariantUtils.ConvertTo<AudioStreamPlayer>(in value);
			return true;
		}
		if (name == PropertyName.deathSound)
		{
			deathSound = VariantUtils.ConvertTo<AudioStreamPlayer>(in value);
			return true;
		}
		if (name == PropertyName.scoreBoonSound)
		{
			scoreBoonSound = VariantUtils.ConvertTo<AudioStreamPlayer>(in value);
			return true;
		}
		if (name == PropertyName.scoreMarkerSound)
		{
			scoreMarkerSound = VariantUtils.ConvertTo<AudioStreamPlayer>(in value);
			return true;
		}
		if (name == PropertyName.scoreLabel)
		{
			scoreLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.gameMasterNode)
		{
			gameMasterNode = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName.dinoCharacter)
		{
			dinoCharacter = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName.spawnedProps)
		{
			spawnedProps = VariantUtils.ConvertTo<Node2D>(in value);
			return true;
		}
		if (name == PropertyName.spawner)
		{
			spawner = VariantUtils.ConvertTo<DR_Spawner>(in value);
			return true;
		}
		if (name == PropertyName.GameStart)
		{
			GameStart = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.GameStop)
		{
			GameStop = VariantUtils.ConvertTo<Control>(in value);
			return true;
		}
		if (name == PropertyName.FloatingText)
		{
			FloatingText = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.cachedDinoPosition)
		{
			cachedDinoPosition = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.GameRunning)
		{
			GameRunning = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.ticketRatio)
		{
			value = VariantUtils.CreateFrom(in ticketRatio);
			return true;
		}
		if (name == PropertyName.score)
		{
			value = VariantUtils.CreateFrom(in score);
			return true;
		}
		if (name == PropertyName.scoreAccumulator)
		{
			value = VariantUtils.CreateFrom(in scoreAccumulator);
			return true;
		}
		if (name == PropertyName.highScore)
		{
			value = VariantUtils.CreateFrom(in highScore);
			return true;
		}
		if (name == PropertyName.gameSpeed)
		{
			value = VariantUtils.CreateFrom(in gameSpeed);
			return true;
		}
		if (name == PropertyName.maxSpeed)
		{
			value = VariantUtils.CreateFrom(in maxSpeed);
			return true;
		}
		if (name == PropertyName.baseScoreSpeed)
		{
			value = VariantUtils.CreateFrom(in baseScoreSpeed);
			return true;
		}
		if (name == PropertyName.speedIncrement)
		{
			value = VariantUtils.CreateFrom(in speedIncrement);
			return true;
		}
		if (name == PropertyName.jumpSound)
		{
			value = VariantUtils.CreateFrom(in jumpSound);
			return true;
		}
		if (name == PropertyName.deathSound)
		{
			value = VariantUtils.CreateFrom(in deathSound);
			return true;
		}
		if (name == PropertyName.scoreBoonSound)
		{
			value = VariantUtils.CreateFrom(in scoreBoonSound);
			return true;
		}
		if (name == PropertyName.scoreMarkerSound)
		{
			value = VariantUtils.CreateFrom(in scoreMarkerSound);
			return true;
		}
		if (name == PropertyName.scoreLabel)
		{
			value = VariantUtils.CreateFrom(in scoreLabel);
			return true;
		}
		if (name == PropertyName.gameMasterNode)
		{
			value = VariantUtils.CreateFrom(in gameMasterNode);
			return true;
		}
		if (name == PropertyName.dinoCharacter)
		{
			value = VariantUtils.CreateFrom(in dinoCharacter);
			return true;
		}
		if (name == PropertyName.spawnedProps)
		{
			value = VariantUtils.CreateFrom(in spawnedProps);
			return true;
		}
		if (name == PropertyName.spawner)
		{
			value = VariantUtils.CreateFrom(in spawner);
			return true;
		}
		if (name == PropertyName.GameStart)
		{
			value = VariantUtils.CreateFrom(in GameStart);
			return true;
		}
		if (name == PropertyName.GameStop)
		{
			value = VariantUtils.CreateFrom(in GameStop);
			return true;
		}
		if (name == PropertyName.FloatingText)
		{
			value = VariantUtils.CreateFrom(in FloatingText);
			return true;
		}
		if (name == PropertyName.cachedDinoPosition)
		{
			value = VariantUtils.CreateFrom(in cachedDinoPosition);
			return true;
		}
		if (name == PropertyName.GameRunning)
		{
			value = VariantUtils.CreateFrom(in GameRunning);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Nil, "Scores", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.ticketRatio, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.score, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.scoreAccumulator, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.highScore, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Nil, "Speeds", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.gameSpeed, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.maxSpeed, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.baseScoreSpeed, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.speedIncrement, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Sounds", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.jumpSound, PropertyHint.NodeType, "AudioStreamPlayer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.deathSound, PropertyHint.NodeType, "AudioStreamPlayer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.scoreBoonSound, PropertyHint.NodeType, "AudioStreamPlayer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.scoreMarkerSound, PropertyHint.NodeType, "AudioStreamPlayer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Dependecies", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.scoreLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.gameMasterNode, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.dinoCharacter, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawnedProps, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spawner, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.GameStart, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.GameStop, PropertyHint.NodeType, "Control", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.FloatingText, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.cachedDinoPosition, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.GameRunning, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.ticketRatio, Variant.From(in ticketRatio));
		info.AddProperty(PropertyName.score, Variant.From(in score));
		info.AddProperty(PropertyName.scoreAccumulator, Variant.From(in scoreAccumulator));
		info.AddProperty(PropertyName.highScore, Variant.From(in highScore));
		info.AddProperty(PropertyName.gameSpeed, Variant.From(in gameSpeed));
		info.AddProperty(PropertyName.maxSpeed, Variant.From(in maxSpeed));
		info.AddProperty(PropertyName.baseScoreSpeed, Variant.From(in baseScoreSpeed));
		info.AddProperty(PropertyName.speedIncrement, Variant.From(in speedIncrement));
		info.AddProperty(PropertyName.jumpSound, Variant.From(in jumpSound));
		info.AddProperty(PropertyName.deathSound, Variant.From(in deathSound));
		info.AddProperty(PropertyName.scoreBoonSound, Variant.From(in scoreBoonSound));
		info.AddProperty(PropertyName.scoreMarkerSound, Variant.From(in scoreMarkerSound));
		info.AddProperty(PropertyName.scoreLabel, Variant.From(in scoreLabel));
		info.AddProperty(PropertyName.gameMasterNode, Variant.From(in gameMasterNode));
		info.AddProperty(PropertyName.dinoCharacter, Variant.From(in dinoCharacter));
		info.AddProperty(PropertyName.spawnedProps, Variant.From(in spawnedProps));
		info.AddProperty(PropertyName.spawner, Variant.From(in spawner));
		info.AddProperty(PropertyName.GameStart, Variant.From(in GameStart));
		info.AddProperty(PropertyName.GameStop, Variant.From(in GameStop));
		info.AddProperty(PropertyName.FloatingText, Variant.From(in FloatingText));
		info.AddProperty(PropertyName.cachedDinoPosition, Variant.From(in cachedDinoPosition));
		info.AddProperty(PropertyName.GameRunning, Variant.From(in GameRunning));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.ticketRatio, out var value))
		{
			ticketRatio = value.As<float>();
		}
		if (info.TryGetProperty(PropertyName.score, out var value2))
		{
			score = value2.As<int>();
		}
		if (info.TryGetProperty(PropertyName.scoreAccumulator, out var value3))
		{
			scoreAccumulator = value3.As<float>();
		}
		if (info.TryGetProperty(PropertyName.highScore, out var value4))
		{
			highScore = value4.As<int>();
		}
		if (info.TryGetProperty(PropertyName.gameSpeed, out var value5))
		{
			gameSpeed = value5.As<float>();
		}
		if (info.TryGetProperty(PropertyName.maxSpeed, out var value6))
		{
			maxSpeed = value6.As<float>();
		}
		if (info.TryGetProperty(PropertyName.baseScoreSpeed, out var value7))
		{
			baseScoreSpeed = value7.As<float>();
		}
		if (info.TryGetProperty(PropertyName.speedIncrement, out var value8))
		{
			speedIncrement = value8.As<float>();
		}
		if (info.TryGetProperty(PropertyName.jumpSound, out var value9))
		{
			jumpSound = value9.As<AudioStreamPlayer>();
		}
		if (info.TryGetProperty(PropertyName.deathSound, out var value10))
		{
			deathSound = value10.As<AudioStreamPlayer>();
		}
		if (info.TryGetProperty(PropertyName.scoreBoonSound, out var value11))
		{
			scoreBoonSound = value11.As<AudioStreamPlayer>();
		}
		if (info.TryGetProperty(PropertyName.scoreMarkerSound, out var value12))
		{
			scoreMarkerSound = value12.As<AudioStreamPlayer>();
		}
		if (info.TryGetProperty(PropertyName.scoreLabel, out var value13))
		{
			scoreLabel = value13.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.gameMasterNode, out var value14))
		{
			gameMasterNode = value14.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName.dinoCharacter, out var value15))
		{
			dinoCharacter = value15.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName.spawnedProps, out var value16))
		{
			spawnedProps = value16.As<Node2D>();
		}
		if (info.TryGetProperty(PropertyName.spawner, out var value17))
		{
			spawner = value17.As<DR_Spawner>();
		}
		if (info.TryGetProperty(PropertyName.GameStart, out var value18))
		{
			GameStart = value18.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.GameStop, out var value19))
		{
			GameStop = value19.As<Control>();
		}
		if (info.TryGetProperty(PropertyName.FloatingText, out var value20))
		{
			FloatingText = value20.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.cachedDinoPosition, out var value21))
		{
			cachedDinoPosition = value21.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.GameRunning, out var value22))
		{
			GameRunning = value22.As<bool>();
		}
	}
}
