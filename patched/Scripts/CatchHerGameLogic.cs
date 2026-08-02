using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/CatchHer/CatchHerGameLogic.cs")]
public partial class CatchHerGameLogic : Node2D
{
    private enum GameState
    {
        WaitClick,
        Playing,
        Won,
        Lost
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

    private bool _mobileControlsCreated;
    private bool _mobileLeft;
    private bool _mobileRight;
    private bool _mobileJump;
    private Button _jumpButton;

    private void EnsureMobileControls()
    {
        if (_mobileControlsCreated)
        {
            return;
        }
        _mobileControlsCreated = true;
        _jumpButton = MobileUI.MakeGameButton("JUMP", Control.LayoutPreset.CenterBottom, new Vector2(-80, -120), new Vector2(160, 80));
        _jumpButton.ButtonDown += () => _mobileJump = true;
        _jumpButton.ButtonUp += () => _mobileJump = false;
        AddChild(_jumpButton);
    }

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
        if (Main._isMobile)
        {
            EnsureMobileControls();
        }
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
        if (Main._isMobile && ev is InputEventScreenTouch touchEvent)
        {
            Vector2 tpos = touchEvent.Position;
            bool onJump = _jumpButton != null && _jumpButton.GetGlobalRect().HasPoint(tpos);
            bool leftHalf = !onJump && tpos.X < (float)GetViewportRect().Size.X / 2f;
            if (touchEvent.Pressed)
            {
                if (onJump)
                {
                    _mobileJump = true;
                }
                else if (leftHalf)
                {
                    _mobileLeft = true;
                }
                else
                {
                    _mobileRight = true;
                }
            }
            else
            {
                if (onJump)
                {
                    _mobileJump = false;
                }
                else if (leftHalf)
                {
                    _mobileLeft = false;
                }
                else
                {
                    _mobileRight = false;
                }
            }
        }
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
        if (Input.IsKeyPressed(Key.D) || (Main._isMobile && _mobileRight))
        {
            num += 1f;
        }
        if (Input.IsKeyPressed(Key.A) || (Main._isMobile && _mobileLeft))
        {
            num -= 1f;
        }
        playerVelocity.X = num * 280f;
        if (PlayerScene.IsOnFloor() && (Input.IsKeyPressed(Key.Space) || Input.IsKeyPressed(Key.W) || (Main._isMobile && _mobileJump)))
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

}
