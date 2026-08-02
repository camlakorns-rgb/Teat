using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/CatchHer/CatchHerGameLogic.cs")]
public class CatchHerGameLogic : Node2D
{
	private enum GameState
	{
		WaitClick,
		Playing,
		Won,
		Lost
	}

	public new class MethodName : Node2D.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public static readonly StringName SetupCamera = "SetupCamera";

		public static readonly StringName BuildPlatforms = "BuildPlatforms";

		public static readonly StringName SpawnPlatform = "SpawnPlatform";

		public static readonly StringName SetupPlayer = "SetupPlayer";

		public static readonly StringName SetupEnemy = "SetupEnemy";

		public static readonly StringName SetupUI = "SetupUI";

		public new static readonly StringName _Process = "_Process";

		public new static readonly StringName _Input = "_Input";

		public static readonly StringName UpdateTimer = "UpdateTimer";

		public static readonly StringName UpdatePlayer = "UpdatePlayer";

		public static readonly StringName UpdateEnemy = "UpdateEnemy";

		public static readonly StringName StartEnemyJump = "StartEnemyJump";

		public static readonly StringName OnPlayerHitboxEntered = "OnPlayerHitboxEntered";

		public static readonly StringName Win = "Win";

		public static readonly StringName Lose = "Lose";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName PlayerScene = "PlayerScene";

		public static readonly StringName EnemyScene = "EnemyScene";

		public static readonly StringName Bg = "Bg";

		public static readonly StringName HintLabel = "HintLabel";

		public static readonly StringName TimerLabel = "TimerLabel";

		public static readonly StringName currentState = "currentState";

		public static readonly StringName screenSize = "screenSize";

		public static readonly StringName timeRemaining = "timeRemaining";

		public static readonly StringName camera = "camera";

		public static readonly StringName playerVelocity = "playerVelocity";

		public static readonly StringName playerHitbox = "playerHitbox";

		public static readonly StringName enemyHitbox = "enemyHitbox";

		public static readonly StringName enemyCurrentPlatformIndex = "enemyCurrentPlatformIndex";

		public static readonly StringName enemyIsJumping = "enemyIsJumping";

		public static readonly StringName enemyJumpOrigin = "enemyJumpOrigin";

		public static readonly StringName enemyJumpDestination = "enemyJumpDestination";

		public static readonly StringName enemyJumpProgress = "enemyJumpProgress";

		public static readonly StringName enemyTimeUntilNextJump = "enemyTimeUntilNextJump";
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public CharacterBody2D PlayerScene;

	[Export(PropertyHint.None, "")]
	public CharacterBody2D EnemyScene;

	[Export(PropertyHint.None, "")]
	public ColorRect Bg;

	[Export(PropertyHint.None, "")]
	public RichTextLabel HintLabel;

	[Export(PropertyHint.None, "")]
	public RichTextLabel TimerLabel;

	private const float Gravity = 1800f;

	private const float PlayerSpeed = 280f;

	private const float PlayerJumpForce = -920f;

	private const float PlatformWidth = 180f;

	private const float PlatformHeight = 8f;

	private const float PlatformGapMin = 160f;

	private const float PlatformGapMax = 210f;

	private const float EnemyJumpDuration = 0.55f;

	private const float EnemyJumpDelay = 1.2f;

	private const float CountdownDuration = 30f;

	private const int StartingPlatformCount = 18;

	private GameState currentState;

	private Vector2 screenSize;

	private float timeRemaining = 30f;

	private Camera2D camera;

	private readonly List<StaticBody2D> platforms = new List<StaticBody2D>();

	private Vector2 playerVelocity;

	private Area2D playerHitbox;

	private Area2D enemyHitbox;

	private int enemyCurrentPlatformIndex = 3;

	private bool enemyIsJumping;

	private Vector2 enemyJumpOrigin;

	private Vector2 enemyJumpDestination;

	private float enemyJumpProgress;

	private float enemyTimeUntilNextJump = 1.2f;

	public override void _Ready()
	{
		screenSize = GetViewport().GetVisibleRect().Size;
		BuildPlatforms();
		SetupPlayer();
		SetupEnemy();
		SetupCamera();
		SetupUI();
	}

	private void SetupCamera()
	{
		camera = new Camera2D();
		camera.AnchorMode = Camera2D.AnchorModeEnum.DragCenter;
		camera.PositionSmoothingEnabled = false;
		AddChild(camera, forceReadableName: false, InternalMode.Disabled);
		camera.MakeCurrent();
		camera.GlobalPosition = new Vector2(screenSize.X * 0.5f, screenSize.Y * 0.5f);
		camera.PositionSmoothingEnabled = true;
		camera.PositionSmoothingSpeed = 5f;
	}

	private void BuildPlatforms()
	{
		RandomNumberGenerator randomNumberGenerator = new RandomNumberGenerator();
		randomNumberGenerator.Randomize();
		float num = screenSize.Y - 8f;
		SpawnPlatform(screenSize.X * 0.5f, num, screenSize.X * 0.9f);
		for (int i = 1; i < 18; i++)
		{
			float num2 = randomNumberGenerator.RandfRange(160f, 210f);
			num -= num2;
			float centerX = randomNumberGenerator.RandfRange(110f, screenSize.X - 90f - 20f);
			SpawnPlatform(centerX, num, 180f);
		}
		float num3 = platforms[platforms.Count - 1].Position.Y - screenSize.Y;
		float y = screenSize.Y - 8f - num3 + screenSize.Y;
		Bg.Position = new Vector2(0f, num3);
		Bg.SetDeferred(Control.PropertyName.Size, new Vector2(screenSize.X, y));
	}

	private void SpawnPlatform(float centerX, float centerY, float width)
	{
		StaticBody2D staticBody2D = new StaticBody2D();
		AddChild(staticBody2D, forceReadableName: false, InternalMode.Disabled);
		CollisionShape2D collisionShape2D = new CollisionShape2D();
		RectangleShape2D shape = new RectangleShape2D
		{
			Size = new Vector2(width, 8f)
		};
		collisionShape2D.Shape = shape;
		collisionShape2D.OneWayCollision = true;
		staticBody2D.AddChild(collisionShape2D, forceReadableName: false, InternalMode.Disabled);
		ColorRect node = new ColorRect
		{
			Size = new Vector2(width, 8f),
			Position = new Vector2((0f - width) * 0.5f, -4f),
			Color = new Color(0.3f, 0.72f, 0.45f)
		};
		staticBody2D.AddChild(node, forceReadableName: false, InternalMode.Disabled);
		staticBody2D.Position = new Vector2(centerX, centerY);
		platforms.Add(staticBody2D);
	}

	private void SetupPlayer()
	{
		PlayerScene.UpDirection = Vector2.Up;
		playerHitbox = PlayerScene.GetChild<Area2D>(1);
		playerHitbox.AreaEntered += OnPlayerHitboxEntered;
		StaticBody2D staticBody2D = platforms[0];
		PlayerScene.Position = new Vector2(staticBody2D.Position.X, staticBody2D.Position.Y - 8f - 8f);
	}

	private void SetupEnemy()
	{
		enemyHitbox = EnemyScene.GetChild<Area2D>(1);
		EnemyScene.Position = platforms[enemyCurrentPlatformIndex].Position;
	}

	private void SetupUI()
	{
		HintLabel.Visible = true;
		HintLabel.Text = "[center]Press anywhere to begin the Chase[/center]";
		HintLabel.GetParent<Panel>().Visible = true;
		TimerLabel.Visible = false;
		TimerLabel.GetParent<Panel>().Visible = false;
		TimerLabel.Text = "";
	}

	public override void _Process(double delta)
	{
		switch (currentState)
		{
		case GameState.Playing:
			UpdateTimer((float)delta);
			UpdatePlayer((float)delta);
			UpdateEnemy((float)delta);
			break;
		case GameState.Won:
		case GameState.Lost:
			if (Input.IsMouseButtonPressed(MouseButton.Left))
			{
				GetTree().ReloadCurrentScene();
			}
			break;
		}
	}

	public override void _Input(InputEvent ev)
	{
		if (currentState == GameState.WaitClick && ev is InputEventMouseButton inputEventMouseButton && inputEventMouseButton.ButtonIndex == MouseButton.Left && inputEventMouseButton.Pressed)
		{
			currentState = GameState.Playing;
			HintLabel.Visible = false;
			HintLabel.GetParent<Panel>().Visible = false;
			TimerLabel.Visible = true;
			TimerLabel.GetParent<Panel>().Visible = true;
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}

	private void UpdateTimer(float dt)
	{
		timeRemaining -= dt;
		TimerLabel.Text = $"CATCH HER! - {Mathf.Max(timeRemaining, 0f):F1}s";
		if (timeRemaining <= 0f)
		{
			Lose();
		}
	}

	private void UpdatePlayer(float dt)
	{
		playerVelocity.Y += 1800f * dt;
		float num = 0f;
		if (Input.IsKeyPressed(Key.D))
		{
			num += 1f;
		}
		if (Input.IsKeyPressed(Key.A))
		{
			num -= 1f;
		}
		playerVelocity.X = num * 280f;
		if (PlayerScene.IsOnFloor() && (Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.W)))
		{
			playerVelocity.Y = -920f;
		}
		PlayerScene.Velocity = playerVelocity;
		PlayerScene.MoveAndSlide();
		if (PlayerScene.IsOnFloor())
		{
			playerVelocity.Y = 0f;
		}
		if (PlayerScene.IsOnCeiling())
		{
			playerVelocity.Y = 0f;
		}
		Vector2 position = PlayerScene.Position;
		if (position.X < 0f)
		{
			position.X = screenSize.X;
		}
		if (position.X > screenSize.X)
		{
			position.X = 0f;
		}
		PlayerScene.Position = position;
		if (currentState == GameState.Playing)
		{
			float num2 = screenSize.Y * 0.55f;
			float y = ((PlayerScene.GlobalPosition.Y < num2) ? PlayerScene.GlobalPosition.Y : (screenSize.Y * 0.5f));
			camera.GlobalPosition = new Vector2(screenSize.X * 0.5f, y);
		}
	}

	private void UpdateEnemy(float dt)
	{
		if (enemyIsJumping)
		{
			enemyJumpProgress += dt;
			float num = Mathf.Min(enemyJumpProgress / 0.55f, 1f);
			float x = Mathf.Lerp(enemyJumpOrigin.X, enemyJumpDestination.X, num);
			float y = Mathf.Lerp(enemyJumpOrigin.Y, enemyJumpDestination.Y, num) - Mathf.Sin(num * (float)Math.PI) * 60f;
			EnemyScene.Position = new Vector2(x, y);
			if (num >= 1f)
			{
				EnemyScene.Position = enemyJumpDestination;
				enemyIsJumping = false;
				enemyTimeUntilNextJump = 1.2f;
			}
		}
		else
		{
			enemyTimeUntilNextJump -= dt;
			if (enemyTimeUntilNextJump <= 0f)
			{
				StartEnemyJump();
			}
		}
	}

	private void StartEnemyJump()
	{
		int count = platforms.Count;
		if (count == 0)
		{
			return;
		}
		float y = platforms[enemyCurrentPlatformIndex].Position.Y;
		List<int> list = new List<int>();
		for (int i = 0; i < count; i++)
		{
			if (i != enemyCurrentPlatformIndex)
			{
				float num = y - platforms[i].Position.Y;
				if (num > 20f && num < 462f)
				{
					list.Add(i);
				}
			}
		}
		if (list.Count == 0)
		{
			for (int j = 0; j < count; j++)
			{
				if (j != enemyCurrentPlatformIndex && (platforms[j].Position - platforms[enemyCurrentPlatformIndex].Position).Length() < 525f)
				{
					list.Add(j);
				}
			}
		}
		if (list.Count != 0)
		{
			int index = list[GD.RandRange(0, list.Count - 1)];
			enemyJumpOrigin = EnemyScene.Position;
			enemyJumpDestination = platforms[index].Position;
			enemyCurrentPlatformIndex = index;
			enemyIsJumping = true;
			enemyJumpProgress = 0f;
		}
	}

	private void OnPlayerHitboxEntered(Area2D other)
	{
		if (currentState == GameState.Playing && other == enemyHitbox)
		{
			Win();
		}
	}

	private void Win()
	{
		currentState = GameState.Won;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		TimerLabel.Visible = false;
		TimerLabel.GetParent<Panel>().Visible = false;
		HintLabel.Visible = true;
		HintLabel.GetParent<Panel>().Visible = true;
		HintLabel.Text = "[center][b]YOU CAUGHT HER![/b]";
	}

	private void Lose()
	{
		currentState = GameState.Lost;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		TimerLabel.Visible = false;
		TimerLabel.GetParent<Panel>().Visible = false;
		HintLabel.Visible = true;
		HintLabel.GetParent<Panel>().Visible = true;
		HintLabel.Text = "[center][b]TIME'S UP![/b]";
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(16)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupCamera, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.BuildPlatforms, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SpawnPlatform, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "centerX", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "centerY", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Float, "width", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.SetupPlayer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupEnemy, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupUI, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName._Input, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "ev", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("InputEvent"), exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateTimer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "dt", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdatePlayer, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "dt", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.UpdateEnemy, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "dt", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.StartEnemyJump, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnPlayerHitboxEntered, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "other", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Area2D"), exported: false)
			}, null),
			new MethodInfo(MethodName.Win, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.Lose, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.SetupCamera && args.Count == 0)
		{
			SetupCamera();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.BuildPlatforms && args.Count == 0)
		{
			BuildPlatforms();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnPlatform && args.Count == 3)
		{
			SpawnPlatform(VariantUtils.ConvertTo<float>(in args[0]), VariantUtils.ConvertTo<float>(in args[1]), VariantUtils.ConvertTo<float>(in args[2]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupPlayer && args.Count == 0)
		{
			SetupPlayer();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupEnemy && args.Count == 0)
		{
			SetupEnemy();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupUI && args.Count == 0)
		{
			SetupUI();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Input && args.Count == 1)
		{
			_Input(VariantUtils.ConvertTo<InputEvent>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateTimer && args.Count == 1)
		{
			UpdateTimer(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdatePlayer && args.Count == 1)
		{
			UpdatePlayer(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateEnemy && args.Count == 1)
		{
			UpdateEnemy(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.StartEnemyJump && args.Count == 0)
		{
			StartEnemyJump();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnPlayerHitboxEntered && args.Count == 1)
		{
			OnPlayerHitboxEntered(VariantUtils.ConvertTo<Area2D>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Win && args.Count == 0)
		{
			Win();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Lose && args.Count == 0)
		{
			Lose();
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
		if (method == MethodName.SetupCamera)
		{
			return true;
		}
		if (method == MethodName.BuildPlatforms)
		{
			return true;
		}
		if (method == MethodName.SpawnPlatform)
		{
			return true;
		}
		if (method == MethodName.SetupPlayer)
		{
			return true;
		}
		if (method == MethodName.SetupEnemy)
		{
			return true;
		}
		if (method == MethodName.SetupUI)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		if (method == MethodName._Input)
		{
			return true;
		}
		if (method == MethodName.UpdateTimer)
		{
			return true;
		}
		if (method == MethodName.UpdatePlayer)
		{
			return true;
		}
		if (method == MethodName.UpdateEnemy)
		{
			return true;
		}
		if (method == MethodName.StartEnemyJump)
		{
			return true;
		}
		if (method == MethodName.OnPlayerHitboxEntered)
		{
			return true;
		}
		if (method == MethodName.Win)
		{
			return true;
		}
		if (method == MethodName.Lose)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.PlayerScene)
		{
			PlayerScene = VariantUtils.ConvertTo<CharacterBody2D>(in value);
			return true;
		}
		if (name == PropertyName.EnemyScene)
		{
			EnemyScene = VariantUtils.ConvertTo<CharacterBody2D>(in value);
			return true;
		}
		if (name == PropertyName.Bg)
		{
			Bg = VariantUtils.ConvertTo<ColorRect>(in value);
			return true;
		}
		if (name == PropertyName.HintLabel)
		{
			HintLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.TimerLabel)
		{
			TimerLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.currentState)
		{
			currentState = VariantUtils.ConvertTo<GameState>(in value);
			return true;
		}
		if (name == PropertyName.screenSize)
		{
			screenSize = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.timeRemaining)
		{
			timeRemaining = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.camera)
		{
			camera = VariantUtils.ConvertTo<Camera2D>(in value);
			return true;
		}
		if (name == PropertyName.playerVelocity)
		{
			playerVelocity = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.playerHitbox)
		{
			playerHitbox = VariantUtils.ConvertTo<Area2D>(in value);
			return true;
		}
		if (name == PropertyName.enemyHitbox)
		{
			enemyHitbox = VariantUtils.ConvertTo<Area2D>(in value);
			return true;
		}
		if (name == PropertyName.enemyCurrentPlatformIndex)
		{
			enemyCurrentPlatformIndex = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.enemyIsJumping)
		{
			enemyIsJumping = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.enemyJumpOrigin)
		{
			enemyJumpOrigin = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.enemyJumpDestination)
		{
			enemyJumpDestination = VariantUtils.ConvertTo<Vector2>(in value);
			return true;
		}
		if (name == PropertyName.enemyJumpProgress)
		{
			enemyJumpProgress = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.enemyTimeUntilNextJump)
		{
			enemyTimeUntilNextJump = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.PlayerScene)
		{
			value = VariantUtils.CreateFrom(in PlayerScene);
			return true;
		}
		if (name == PropertyName.EnemyScene)
		{
			value = VariantUtils.CreateFrom(in EnemyScene);
			return true;
		}
		if (name == PropertyName.Bg)
		{
			value = VariantUtils.CreateFrom(in Bg);
			return true;
		}
		if (name == PropertyName.HintLabel)
		{
			value = VariantUtils.CreateFrom(in HintLabel);
			return true;
		}
		if (name == PropertyName.TimerLabel)
		{
			value = VariantUtils.CreateFrom(in TimerLabel);
			return true;
		}
		if (name == PropertyName.currentState)
		{
			value = VariantUtils.CreateFrom(in currentState);
			return true;
		}
		if (name == PropertyName.screenSize)
		{
			value = VariantUtils.CreateFrom(in screenSize);
			return true;
		}
		if (name == PropertyName.timeRemaining)
		{
			value = VariantUtils.CreateFrom(in timeRemaining);
			return true;
		}
		if (name == PropertyName.camera)
		{
			value = VariantUtils.CreateFrom(in camera);
			return true;
		}
		if (name == PropertyName.playerVelocity)
		{
			value = VariantUtils.CreateFrom(in playerVelocity);
			return true;
		}
		if (name == PropertyName.playerHitbox)
		{
			value = VariantUtils.CreateFrom(in playerHitbox);
			return true;
		}
		if (name == PropertyName.enemyHitbox)
		{
			value = VariantUtils.CreateFrom(in enemyHitbox);
			return true;
		}
		if (name == PropertyName.enemyCurrentPlatformIndex)
		{
			value = VariantUtils.CreateFrom(in enemyCurrentPlatformIndex);
			return true;
		}
		if (name == PropertyName.enemyIsJumping)
		{
			value = VariantUtils.CreateFrom(in enemyIsJumping);
			return true;
		}
		if (name == PropertyName.enemyJumpOrigin)
		{
			value = VariantUtils.CreateFrom(in enemyJumpOrigin);
			return true;
		}
		if (name == PropertyName.enemyJumpDestination)
		{
			value = VariantUtils.CreateFrom(in enemyJumpDestination);
			return true;
		}
		if (name == PropertyName.enemyJumpProgress)
		{
			value = VariantUtils.CreateFrom(in enemyJumpProgress);
			return true;
		}
		if (name == PropertyName.enemyTimeUntilNextJump)
		{
			value = VariantUtils.CreateFrom(in enemyTimeUntilNextJump);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.PlayerScene, PropertyHint.NodeType, "CharacterBody2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.EnemyScene, PropertyHint.NodeType, "CharacterBody2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.Bg, PropertyHint.NodeType, "ColorRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.HintLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.TimerLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.currentState, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.screenSize, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.timeRemaining, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.camera, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.playerVelocity, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.playerHitbox, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.enemyHitbox, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.enemyCurrentPlatformIndex, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.enemyIsJumping, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.enemyJumpOrigin, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2, PropertyName.enemyJumpDestination, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.enemyJumpProgress, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.enemyTimeUntilNextJump, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.PlayerScene, Variant.From(in PlayerScene));
		info.AddProperty(PropertyName.EnemyScene, Variant.From(in EnemyScene));
		info.AddProperty(PropertyName.Bg, Variant.From(in Bg));
		info.AddProperty(PropertyName.HintLabel, Variant.From(in HintLabel));
		info.AddProperty(PropertyName.TimerLabel, Variant.From(in TimerLabel));
		info.AddProperty(PropertyName.currentState, Variant.From(in currentState));
		info.AddProperty(PropertyName.screenSize, Variant.From(in screenSize));
		info.AddProperty(PropertyName.timeRemaining, Variant.From(in timeRemaining));
		info.AddProperty(PropertyName.camera, Variant.From(in camera));
		info.AddProperty(PropertyName.playerVelocity, Variant.From(in playerVelocity));
		info.AddProperty(PropertyName.playerHitbox, Variant.From(in playerHitbox));
		info.AddProperty(PropertyName.enemyHitbox, Variant.From(in enemyHitbox));
		info.AddProperty(PropertyName.enemyCurrentPlatformIndex, Variant.From(in enemyCurrentPlatformIndex));
		info.AddProperty(PropertyName.enemyIsJumping, Variant.From(in enemyIsJumping));
		info.AddProperty(PropertyName.enemyJumpOrigin, Variant.From(in enemyJumpOrigin));
		info.AddProperty(PropertyName.enemyJumpDestination, Variant.From(in enemyJumpDestination));
		info.AddProperty(PropertyName.enemyJumpProgress, Variant.From(in enemyJumpProgress));
		info.AddProperty(PropertyName.enemyTimeUntilNextJump, Variant.From(in enemyTimeUntilNextJump));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.PlayerScene, out var value))
		{
			PlayerScene = value.As<CharacterBody2D>();
		}
		if (info.TryGetProperty(PropertyName.EnemyScene, out var value2))
		{
			EnemyScene = value2.As<CharacterBody2D>();
		}
		if (info.TryGetProperty(PropertyName.Bg, out var value3))
		{
			Bg = value3.As<ColorRect>();
		}
		if (info.TryGetProperty(PropertyName.HintLabel, out var value4))
		{
			HintLabel = value4.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.TimerLabel, out var value5))
		{
			TimerLabel = value5.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.currentState, out var value6))
		{
			currentState = value6.As<GameState>();
		}
		if (info.TryGetProperty(PropertyName.screenSize, out var value7))
		{
			screenSize = value7.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.timeRemaining, out var value8))
		{
			timeRemaining = value8.As<float>();
		}
		if (info.TryGetProperty(PropertyName.camera, out var value9))
		{
			camera = value9.As<Camera2D>();
		}
		if (info.TryGetProperty(PropertyName.playerVelocity, out var value10))
		{
			playerVelocity = value10.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.playerHitbox, out var value11))
		{
			playerHitbox = value11.As<Area2D>();
		}
		if (info.TryGetProperty(PropertyName.enemyHitbox, out var value12))
		{
			enemyHitbox = value12.As<Area2D>();
		}
		if (info.TryGetProperty(PropertyName.enemyCurrentPlatformIndex, out var value13))
		{
			enemyCurrentPlatformIndex = value13.As<int>();
		}
		if (info.TryGetProperty(PropertyName.enemyIsJumping, out var value14))
		{
			enemyIsJumping = value14.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.enemyJumpOrigin, out var value15))
		{
			enemyJumpOrigin = value15.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.enemyJumpDestination, out var value16))
		{
			enemyJumpDestination = value16.As<Vector2>();
		}
		if (info.TryGetProperty(PropertyName.enemyJumpProgress, out var value17))
		{
			enemyJumpProgress = value17.As<float>();
		}
		if (info.TryGetProperty(PropertyName.enemyTimeUntilNextJump, out var value18))
		{
			enemyTimeUntilNextJump = value18.As<float>();
		}
	}
}
