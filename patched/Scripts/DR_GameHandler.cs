using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/DinoRunner/DR_GameHandler.cs")]
public partial class DR_GameHandler : MinigameBase
{

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

    private bool _mobileButtonsCreated;

    private void EnsureMobileButtons()
    {
        if (_mobileButtonsCreated)
        {
            return;
        }
        _mobileButtonsCreated = true;
        Button jump = MobileUI.MakeGameButton("JUMP", Control.LayoutPreset.BottomLeft, new Vector2(24, -176), new Vector2(150, 64));
        jump.ButtonDown += () => Input.ActionPress("DR_Jump");
        jump.ButtonUp += () => Input.ActionRelease("DR_Jump");
        AddChild(jump);
        Button duck = MobileUI.MakeGameButton("DUCK", Control.LayoutPreset.BottomLeft, new Vector2(24, -100), new Vector2(150, 64));
        duck.ButtonDown += () => Input.ActionPress("DR_Duck");
        duck.ButtonUp += () => Input.ActionRelease("DR_Duck");
        AddChild(duck);
    }

    public override void _Ready()
    {
        base._Ready();
        if (Main._isMobile)
        {
            EnsureMobileButtons();
        }
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
        if (Main._isMobile)
        {
            return true;
        }
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

}
