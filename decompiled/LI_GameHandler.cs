using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/Minigames/LovenseIdle/LI_GameHandler.cs")]
public class LI_GameHandler : MinigameBase
{
	public new class MethodName : MinigameBase.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName DoPassiveTick = "DoPassiveTick";

		public static readonly StringName RegisterClicker = "RegisterClicker";

		public static readonly StringName OnClickerPressed = "OnClickerPressed";

		public static readonly StringName AwardHearts = "AwardHearts";

		public static readonly StringName OnLoopUpgradePressed = "OnLoopUpgradePressed";

		public static readonly StringName OnFloatUpgradePressed = "OnFloatUpgradePressed";

		public static readonly StringName OnPowerUpgradePressed = "OnPowerUpgradePressed";

		public static readonly StringName OnPrestigeUpgradePressed = "OnPrestigeUpgradePressed";

		public static readonly StringName SpawnToyClicker = "SpawnToyClicker";

		public static readonly StringName GetLoopPassiveRate = "GetLoopPassiveRate";

		public static readonly StringName GetPowerCapValue = "GetPowerCapValue";

		public static readonly StringName GetFloatsGracePeriod = "GetFloatsGracePeriod";

		public static readonly StringName GetFloatsDecayRate = "GetFloatsDecayRate";

		public static readonly StringName GetPrestigeMultiplier = "GetPrestigeMultiplier";

		public static readonly StringName RefreshFloatStatsOnAllClickers = "RefreshFloatStatsOnAllClickers";

		public static readonly StringName UpdateHeartsLabel = "UpdateHeartsLabel";

		public static readonly StringName UpdateAllLabels = "UpdateAllLabels";
	}

	public new class PropertyName : MinigameBase.PropertyName
	{
		public static readonly StringName LineBoxCords = "LineBoxCords";

		public static readonly StringName heartLine = "heartLine";

		public static readonly StringName Upgrades = "Upgrades";

		public static readonly StringName Hearts = "Hearts";

		public static readonly StringName Record = "Record";

		public static readonly StringName clickerHolder = "clickerHolder";

		public static readonly StringName clickerScene = "clickerScene";

		public static readonly StringName LoopUpgradeButton = "LoopUpgradeButton";

		public static readonly StringName FloatUpgradeButton = "FloatUpgradeButton";

		public static readonly StringName PowerUpgradeButton = "PowerUpgradeButton";

		public static readonly StringName PrestigeUpgradeButton = "PrestigeUpgradeButton";

		public static readonly StringName LoopUpgradeCostLabel = "LoopUpgradeCostLabel";

		public static readonly StringName FloatsUpgradeCostLabel = "FloatsUpgradeCostLabel";

		public static readonly StringName PowerUpgradeCostLabel = "PowerUpgradeCostLabel";

		public static readonly StringName ToyUpgradeCostLabel = "ToyUpgradeCostLabel";

		public static readonly StringName baseLoopRate = "baseLoopRate";

		public static readonly StringName loopRatePerLevel = "loopRatePerLevel";

		public static readonly StringName baseGracePeriod = "baseGracePeriod";

		public static readonly StringName graceIncreasePerLevel = "graceIncreasePerLevel";

		public static readonly StringName baseDecayRate = "baseDecayRate";

		public static readonly StringName decayReductionPerLevel = "decayReductionPerLevel";

		public static readonly StringName minDecayRate = "minDecayRate";

		public static readonly StringName basePowerCap = "basePowerCap";

		public static readonly StringName powerCapPerLevel = "powerCapPerLevel";

		public static readonly StringName baseLoopUpgradeCost = "baseLoopUpgradeCost";

		public static readonly StringName baseFloatsUpgradeCost = "baseFloatsUpgradeCost";

		public static readonly StringName basePowerUpgradeCost = "basePowerUpgradeCost";

		public static readonly StringName baseToyUpgradeCost = "baseToyUpgradeCost";

		public static readonly StringName currentHearts = "currentHearts";

		public static readonly StringName upgradedLoops = "upgradedLoops";

		public static readonly StringName loopCostScaler = "loopCostScaler";

		public static readonly StringName loopUpgradeCost = "loopUpgradeCost";

		public static readonly StringName upgradedFloats = "upgradedFloats";

		public static readonly StringName floatsCostScaler = "floatsCostScaler";

		public static readonly StringName floatsUpgradeCost = "floatsUpgradeCost";

		public static readonly StringName upgradedPower = "upgradedPower";

		public static readonly StringName powerCostScaler = "powerCostScaler";

		public static readonly StringName powerUpgradeCost = "powerUpgradeCost";

		public static readonly StringName currentPrestige = "currentPrestige";

		public static readonly StringName prestigeCostScaler = "prestigeCostScaler";

		public static readonly StringName prestigeUpgradeCost = "prestigeUpgradeCost";

		public static readonly StringName passiveTickAccumulator = "passiveTickAccumulator";
	}

	public new class SignalName : MinigameBase.SignalName
	{
	}

	[ExportGroup("Overall Power", "")]
	[Export(PropertyHint.None, "")]
	public Vector2I LineBoxCords;

	[Export(PropertyHint.None, "")]
	private Line2D heartLine;

	[ExportGroup("Displayed Text", "")]
	[Export(PropertyHint.None, "")]
	private RichTextLabel Upgrades;

	[Export(PropertyHint.None, "")]
	private RichTextLabel Hearts;

	[Export(PropertyHint.None, "")]
	private RichTextLabel Record;

	[ExportGroup("Clicker Buttons", "")]
	[Export(PropertyHint.None, "")]
	private GridContainer clickerHolder;

	[Export(PropertyHint.None, "")]
	private PackedScene clickerScene;

	[ExportGroup("Upgrade Buttons", "")]
	[Export(PropertyHint.None, "")]
	private BaseButton LoopUpgradeButton;

	[Export(PropertyHint.None, "")]
	private BaseButton FloatUpgradeButton;

	[Export(PropertyHint.None, "")]
	private BaseButton PowerUpgradeButton;

	[Export(PropertyHint.None, "")]
	private BaseButton PrestigeUpgradeButton;

	[Export(PropertyHint.None, "")]
	private RichTextLabel LoopUpgradeCostLabel;

	[Export(PropertyHint.None, "")]
	private RichTextLabel FloatsUpgradeCostLabel;

	[Export(PropertyHint.None, "")]
	private RichTextLabel PowerUpgradeCostLabel;

	[Export(PropertyHint.None, "")]
	private RichTextLabel ToyUpgradeCostLabel;

	[ExportGroup("Balance", "")]
	[Export(PropertyHint.None, "")]
	private float baseLoopRate = 0.5f;

	[Export(PropertyHint.None, "")]
	private float loopRatePerLevel = 0.5f;

	[Export(PropertyHint.None, "")]
	private float baseGracePeriod = 1f;

	[Export(PropertyHint.None, "")]
	private float graceIncreasePerLevel = 0.25f;

	[Export(PropertyHint.None, "")]
	private float baseDecayRate = 2f;

	[Export(PropertyHint.None, "")]
	private float decayReductionPerLevel = 0.1f;

	[Export(PropertyHint.None, "")]
	private float minDecayRate = 0.1f;

	[Export(PropertyHint.None, "")]
	private float basePowerCap = 1f;

	[Export(PropertyHint.None, "")]
	private float powerCapPerLevel = 1f;

	[Export(PropertyHint.None, "")]
	private int baseLoopUpgradeCost = 10;

	[Export(PropertyHint.None, "")]
	private int baseFloatsUpgradeCost = 10;

	[Export(PropertyHint.None, "")]
	private int basePowerUpgradeCost = 10;

	[Export(PropertyHint.None, "")]
	private int baseToyUpgradeCost = 500;

	private float currentHearts;

	private int upgradedLoops;

	private float loopCostScaler = 1.15f;

	private int loopUpgradeCost;

	private int upgradedFloats;

	private float floatsCostScaler = 1.15f;

	private int floatsUpgradeCost;

	private int upgradedPower;

	private float powerCostScaler = 1.2f;

	private int powerUpgradeCost;

	private int currentPrestige;

	private float prestigeCostScaler = 2f;

	private int prestigeUpgradeCost;

	private float passiveTickAccumulator;

	private readonly List<LI_Clicker> activeClickers = new List<LI_Clicker>();

	public override void _Ready()
	{
		base._Ready();
		loopUpgradeCost = baseLoopUpgradeCost;
		floatsUpgradeCost = baseFloatsUpgradeCost;
		powerUpgradeCost = basePowerUpgradeCost;
		prestigeUpgradeCost = baseToyUpgradeCost;
		if (LoopUpgradeButton != null)
		{
			LoopUpgradeButton.Pressed += OnLoopUpgradePressed;
		}
		if (FloatUpgradeButton != null)
		{
			FloatUpgradeButton.Pressed += OnFloatUpgradePressed;
		}
		if (PowerUpgradeButton != null)
		{
			PowerUpgradeButton.Pressed += OnPowerUpgradePressed;
		}
		if (PrestigeUpgradeButton != null)
		{
			PrestigeUpgradeButton.Pressed += OnPrestigeUpgradePressed;
		}
		foreach (Node child in clickerHolder.GetChildren())
		{
			if (child is LI_Clicker clicker)
			{
				RegisterClicker(clicker);
			}
		}
		RefreshFloatStatsOnAllClickers();
		UpdateAllLabels();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		passiveTickAccumulator += (float)delta;
		while (passiveTickAccumulator >= 1f)
		{
			passiveTickAccumulator -= 1f;
			DoPassiveTick();
		}
	}

	private void DoPassiveTick()
	{
		float num = GetLoopPassiveRate();
		foreach (LI_Clicker activeClicker in activeClickers)
		{
			num += activeClicker.currentHeartsPerSecond;
		}
		AwardHearts(num);
	}

	private void RegisterClicker(LI_Clicker clicker)
	{
		activeClickers.Add(clicker);
		clicker.UpdateFloatStats(GetFloatsGracePeriod(), GetFloatsDecayRate());
		clicker.ClickerPressed += delegate
		{
			OnClickerPressed(clicker);
		};
	}

	private void OnClickerPressed(LI_Clicker clicker)
	{
		float powerCapValue = GetPowerCapValue();
		AwardHearts(powerCapValue);
		clicker.ApplyClick(powerCapValue);
	}

	private void AwardHearts(float amount)
	{
		if (!(amount <= 0f))
		{
			currentHearts += amount;
			UpdateHeartsLabel();
		}
	}

	private void OnLoopUpgradePressed()
	{
		if (!(currentHearts < (float)loopUpgradeCost))
		{
			currentHearts -= loopUpgradeCost;
			upgradedLoops++;
			loopUpgradeCost = Mathf.RoundToInt((float)loopUpgradeCost * loopCostScaler);
			UpdateAllLabels();
		}
	}

	private void OnFloatUpgradePressed()
	{
		if (!(currentHearts < (float)floatsUpgradeCost))
		{
			currentHearts -= floatsUpgradeCost;
			upgradedFloats++;
			floatsUpgradeCost = Mathf.RoundToInt((float)floatsUpgradeCost * floatsCostScaler);
			RefreshFloatStatsOnAllClickers();
			UpdateAllLabels();
		}
	}

	private void OnPowerUpgradePressed()
	{
		if (!(currentHearts < (float)powerUpgradeCost))
		{
			currentHearts -= powerUpgradeCost;
			upgradedPower++;
			powerUpgradeCost = Mathf.RoundToInt((float)powerUpgradeCost * powerCostScaler);
			UpdateAllLabels();
		}
	}

	private void OnPrestigeUpgradePressed()
	{
		if (!(currentHearts < (float)prestigeUpgradeCost))
		{
			currentHearts -= prestigeUpgradeCost;
			currentPrestige++;
			upgradedLoops = 0;
			upgradedFloats = 0;
			upgradedPower = 0;
			loopUpgradeCost = baseLoopUpgradeCost;
			floatsUpgradeCost = baseFloatsUpgradeCost;
			powerUpgradeCost = basePowerUpgradeCost;
			prestigeUpgradeCost = Mathf.RoundToInt((float)prestigeUpgradeCost * prestigeCostScaler);
			RefreshFloatStatsOnAllClickers();
			SpawnToyClicker();
			UpdateAllLabels();
		}
	}

	private void SpawnToyClicker()
	{
		if (clickerScene != null && clickerHolder != null)
		{
			LI_Clicker lI_Clicker = clickerScene.Instantiate<LI_Clicker>(PackedScene.GenEditState.Disabled);
			clickerHolder.AddChild(lI_Clicker, forceReadableName: false, InternalMode.Disabled);
			RegisterClicker(lI_Clicker);
		}
	}

	private float GetLoopPassiveRate()
	{
		return (baseLoopRate + (float)upgradedLoops * loopRatePerLevel) * GetPrestigeMultiplier();
	}

	private float GetPowerCapValue()
	{
		return (basePowerCap + (float)upgradedPower * powerCapPerLevel) * GetPrestigeMultiplier();
	}

	private float GetFloatsGracePeriod()
	{
		return baseGracePeriod + (float)upgradedFloats * graceIncreasePerLevel;
	}

	private float GetFloatsDecayRate()
	{
		return Mathf.Max(minDecayRate, baseDecayRate - (float)upgradedFloats * decayReductionPerLevel);
	}

	private float GetPrestigeMultiplier()
	{
		return 1f + (float)currentPrestige * 0.5f;
	}

	private void RefreshFloatStatsOnAllClickers()
	{
		float floatsGracePeriod = GetFloatsGracePeriod();
		float floatsDecayRate = GetFloatsDecayRate();
		foreach (LI_Clicker activeClicker in activeClickers)
		{
			activeClicker.UpdateFloatStats(floatsGracePeriod, floatsDecayRate);
		}
	}

	private void UpdateHeartsLabel()
	{
		if (Hearts != null)
		{
			Hearts.Text = $"{currentHearts:0}";
		}
	}

	private void UpdateAllLabels()
	{
		UpdateHeartsLabel();
		if (LoopUpgradeCostLabel != null)
		{
			LoopUpgradeCostLabel.Text = $"{loopUpgradeCost}";
		}
		if (FloatsUpgradeCostLabel != null)
		{
			FloatsUpgradeCostLabel.Text = $"{floatsUpgradeCost}";
		}
		if (PowerUpgradeCostLabel != null)
		{
			PowerUpgradeCostLabel.Text = $"{powerUpgradeCost}";
		}
		if (ToyUpgradeCostLabel != null)
		{
			ToyUpgradeCostLabel.Text = $"{prestigeUpgradeCost}";
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(19)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.DoPassiveTick, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RegisterClicker, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "clicker", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Panel"), exported: false)
			}, null),
			new MethodInfo(MethodName.OnClickerPressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "clicker", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Panel"), exported: false)
			}, null),
			new MethodInfo(MethodName.AwardHearts, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "amount", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnLoopUpgradePressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnFloatUpgradePressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnPowerUpgradePressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.OnPrestigeUpgradePressed, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SpawnToyClicker, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetLoopPassiveRate, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetPowerCapValue, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetFloatsGracePeriod, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetFloatsDecayRate, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.GetPrestigeMultiplier, new PropertyInfo(Variant.Type.Float, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RefreshFloatStatsOnAllClickers, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateHeartsLabel, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.UpdateAllLabels, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.DoPassiveTick && args.Count == 0)
		{
			DoPassiveTick();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RegisterClicker && args.Count == 1)
		{
			RegisterClicker(VariantUtils.ConvertTo<LI_Clicker>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnClickerPressed && args.Count == 1)
		{
			OnClickerPressed(VariantUtils.ConvertTo<LI_Clicker>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.AwardHearts && args.Count == 1)
		{
			AwardHearts(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnLoopUpgradePressed && args.Count == 0)
		{
			OnLoopUpgradePressed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnFloatUpgradePressed && args.Count == 0)
		{
			OnFloatUpgradePressed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnPowerUpgradePressed && args.Count == 0)
		{
			OnPowerUpgradePressed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnPrestigeUpgradePressed && args.Count == 0)
		{
			OnPrestigeUpgradePressed();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SpawnToyClicker && args.Count == 0)
		{
			SpawnToyClicker();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.GetLoopPassiveRate && args.Count == 0)
		{
			float from = GetLoopPassiveRate();
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.GetPowerCapValue && args.Count == 0)
		{
			float from2 = GetPowerCapValue();
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.GetFloatsGracePeriod && args.Count == 0)
		{
			float from3 = GetFloatsGracePeriod();
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.GetFloatsDecayRate && args.Count == 0)
		{
			float from4 = GetFloatsDecayRate();
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.GetPrestigeMultiplier && args.Count == 0)
		{
			float from5 = GetPrestigeMultiplier();
			ret = VariantUtils.CreateFrom(in from5);
			return true;
		}
		if (method == MethodName.RefreshFloatStatsOnAllClickers && args.Count == 0)
		{
			RefreshFloatStatsOnAllClickers();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateHeartsLabel && args.Count == 0)
		{
			UpdateHeartsLabel();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.UpdateAllLabels && args.Count == 0)
		{
			UpdateAllLabels();
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
		if (method == MethodName.DoPassiveTick)
		{
			return true;
		}
		if (method == MethodName.RegisterClicker)
		{
			return true;
		}
		if (method == MethodName.OnClickerPressed)
		{
			return true;
		}
		if (method == MethodName.AwardHearts)
		{
			return true;
		}
		if (method == MethodName.OnLoopUpgradePressed)
		{
			return true;
		}
		if (method == MethodName.OnFloatUpgradePressed)
		{
			return true;
		}
		if (method == MethodName.OnPowerUpgradePressed)
		{
			return true;
		}
		if (method == MethodName.OnPrestigeUpgradePressed)
		{
			return true;
		}
		if (method == MethodName.SpawnToyClicker)
		{
			return true;
		}
		if (method == MethodName.GetLoopPassiveRate)
		{
			return true;
		}
		if (method == MethodName.GetPowerCapValue)
		{
			return true;
		}
		if (method == MethodName.GetFloatsGracePeriod)
		{
			return true;
		}
		if (method == MethodName.GetFloatsDecayRate)
		{
			return true;
		}
		if (method == MethodName.GetPrestigeMultiplier)
		{
			return true;
		}
		if (method == MethodName.RefreshFloatStatsOnAllClickers)
		{
			return true;
		}
		if (method == MethodName.UpdateHeartsLabel)
		{
			return true;
		}
		if (method == MethodName.UpdateAllLabels)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.LineBoxCords)
		{
			LineBoxCords = VariantUtils.ConvertTo<Vector2I>(in value);
			return true;
		}
		if (name == PropertyName.heartLine)
		{
			heartLine = VariantUtils.ConvertTo<Line2D>(in value);
			return true;
		}
		if (name == PropertyName.Upgrades)
		{
			Upgrades = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.Hearts)
		{
			Hearts = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.Record)
		{
			Record = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.clickerHolder)
		{
			clickerHolder = VariantUtils.ConvertTo<GridContainer>(in value);
			return true;
		}
		if (name == PropertyName.clickerScene)
		{
			clickerScene = VariantUtils.ConvertTo<PackedScene>(in value);
			return true;
		}
		if (name == PropertyName.LoopUpgradeButton)
		{
			LoopUpgradeButton = VariantUtils.ConvertTo<BaseButton>(in value);
			return true;
		}
		if (name == PropertyName.FloatUpgradeButton)
		{
			FloatUpgradeButton = VariantUtils.ConvertTo<BaseButton>(in value);
			return true;
		}
		if (name == PropertyName.PowerUpgradeButton)
		{
			PowerUpgradeButton = VariantUtils.ConvertTo<BaseButton>(in value);
			return true;
		}
		if (name == PropertyName.PrestigeUpgradeButton)
		{
			PrestigeUpgradeButton = VariantUtils.ConvertTo<BaseButton>(in value);
			return true;
		}
		if (name == PropertyName.LoopUpgradeCostLabel)
		{
			LoopUpgradeCostLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.FloatsUpgradeCostLabel)
		{
			FloatsUpgradeCostLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.PowerUpgradeCostLabel)
		{
			PowerUpgradeCostLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.ToyUpgradeCostLabel)
		{
			ToyUpgradeCostLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.baseLoopRate)
		{
			baseLoopRate = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.loopRatePerLevel)
		{
			loopRatePerLevel = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.baseGracePeriod)
		{
			baseGracePeriod = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.graceIncreasePerLevel)
		{
			graceIncreasePerLevel = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.baseDecayRate)
		{
			baseDecayRate = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.decayReductionPerLevel)
		{
			decayReductionPerLevel = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.minDecayRate)
		{
			minDecayRate = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.basePowerCap)
		{
			basePowerCap = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.powerCapPerLevel)
		{
			powerCapPerLevel = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.baseLoopUpgradeCost)
		{
			baseLoopUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.baseFloatsUpgradeCost)
		{
			baseFloatsUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.basePowerUpgradeCost)
		{
			basePowerUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.baseToyUpgradeCost)
		{
			baseToyUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.currentHearts)
		{
			currentHearts = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.upgradedLoops)
		{
			upgradedLoops = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.loopCostScaler)
		{
			loopCostScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.loopUpgradeCost)
		{
			loopUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.upgradedFloats)
		{
			upgradedFloats = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.floatsCostScaler)
		{
			floatsCostScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.floatsUpgradeCost)
		{
			floatsUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.upgradedPower)
		{
			upgradedPower = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.powerCostScaler)
		{
			powerCostScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.powerUpgradeCost)
		{
			powerUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.currentPrestige)
		{
			currentPrestige = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.prestigeCostScaler)
		{
			prestigeCostScaler = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.prestigeUpgradeCost)
		{
			prestigeUpgradeCost = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.passiveTickAccumulator)
		{
			passiveTickAccumulator = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.LineBoxCords)
		{
			value = VariantUtils.CreateFrom(in LineBoxCords);
			return true;
		}
		if (name == PropertyName.heartLine)
		{
			value = VariantUtils.CreateFrom(in heartLine);
			return true;
		}
		if (name == PropertyName.Upgrades)
		{
			value = VariantUtils.CreateFrom(in Upgrades);
			return true;
		}
		if (name == PropertyName.Hearts)
		{
			value = VariantUtils.CreateFrom(in Hearts);
			return true;
		}
		if (name == PropertyName.Record)
		{
			value = VariantUtils.CreateFrom(in Record);
			return true;
		}
		if (name == PropertyName.clickerHolder)
		{
			value = VariantUtils.CreateFrom(in clickerHolder);
			return true;
		}
		if (name == PropertyName.clickerScene)
		{
			value = VariantUtils.CreateFrom(in clickerScene);
			return true;
		}
		if (name == PropertyName.LoopUpgradeButton)
		{
			value = VariantUtils.CreateFrom(in LoopUpgradeButton);
			return true;
		}
		if (name == PropertyName.FloatUpgradeButton)
		{
			value = VariantUtils.CreateFrom(in FloatUpgradeButton);
			return true;
		}
		if (name == PropertyName.PowerUpgradeButton)
		{
			value = VariantUtils.CreateFrom(in PowerUpgradeButton);
			return true;
		}
		if (name == PropertyName.PrestigeUpgradeButton)
		{
			value = VariantUtils.CreateFrom(in PrestigeUpgradeButton);
			return true;
		}
		if (name == PropertyName.LoopUpgradeCostLabel)
		{
			value = VariantUtils.CreateFrom(in LoopUpgradeCostLabel);
			return true;
		}
		if (name == PropertyName.FloatsUpgradeCostLabel)
		{
			value = VariantUtils.CreateFrom(in FloatsUpgradeCostLabel);
			return true;
		}
		if (name == PropertyName.PowerUpgradeCostLabel)
		{
			value = VariantUtils.CreateFrom(in PowerUpgradeCostLabel);
			return true;
		}
		if (name == PropertyName.ToyUpgradeCostLabel)
		{
			value = VariantUtils.CreateFrom(in ToyUpgradeCostLabel);
			return true;
		}
		if (name == PropertyName.baseLoopRate)
		{
			value = VariantUtils.CreateFrom(in baseLoopRate);
			return true;
		}
		if (name == PropertyName.loopRatePerLevel)
		{
			value = VariantUtils.CreateFrom(in loopRatePerLevel);
			return true;
		}
		if (name == PropertyName.baseGracePeriod)
		{
			value = VariantUtils.CreateFrom(in baseGracePeriod);
			return true;
		}
		if (name == PropertyName.graceIncreasePerLevel)
		{
			value = VariantUtils.CreateFrom(in graceIncreasePerLevel);
			return true;
		}
		if (name == PropertyName.baseDecayRate)
		{
			value = VariantUtils.CreateFrom(in baseDecayRate);
			return true;
		}
		if (name == PropertyName.decayReductionPerLevel)
		{
			value = VariantUtils.CreateFrom(in decayReductionPerLevel);
			return true;
		}
		if (name == PropertyName.minDecayRate)
		{
			value = VariantUtils.CreateFrom(in minDecayRate);
			return true;
		}
		if (name == PropertyName.basePowerCap)
		{
			value = VariantUtils.CreateFrom(in basePowerCap);
			return true;
		}
		if (name == PropertyName.powerCapPerLevel)
		{
			value = VariantUtils.CreateFrom(in powerCapPerLevel);
			return true;
		}
		if (name == PropertyName.baseLoopUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in baseLoopUpgradeCost);
			return true;
		}
		if (name == PropertyName.baseFloatsUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in baseFloatsUpgradeCost);
			return true;
		}
		if (name == PropertyName.basePowerUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in basePowerUpgradeCost);
			return true;
		}
		if (name == PropertyName.baseToyUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in baseToyUpgradeCost);
			return true;
		}
		if (name == PropertyName.currentHearts)
		{
			value = VariantUtils.CreateFrom(in currentHearts);
			return true;
		}
		if (name == PropertyName.upgradedLoops)
		{
			value = VariantUtils.CreateFrom(in upgradedLoops);
			return true;
		}
		if (name == PropertyName.loopCostScaler)
		{
			value = VariantUtils.CreateFrom(in loopCostScaler);
			return true;
		}
		if (name == PropertyName.loopUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in loopUpgradeCost);
			return true;
		}
		if (name == PropertyName.upgradedFloats)
		{
			value = VariantUtils.CreateFrom(in upgradedFloats);
			return true;
		}
		if (name == PropertyName.floatsCostScaler)
		{
			value = VariantUtils.CreateFrom(in floatsCostScaler);
			return true;
		}
		if (name == PropertyName.floatsUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in floatsUpgradeCost);
			return true;
		}
		if (name == PropertyName.upgradedPower)
		{
			value = VariantUtils.CreateFrom(in upgradedPower);
			return true;
		}
		if (name == PropertyName.powerCostScaler)
		{
			value = VariantUtils.CreateFrom(in powerCostScaler);
			return true;
		}
		if (name == PropertyName.powerUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in powerUpgradeCost);
			return true;
		}
		if (name == PropertyName.currentPrestige)
		{
			value = VariantUtils.CreateFrom(in currentPrestige);
			return true;
		}
		if (name == PropertyName.prestigeCostScaler)
		{
			value = VariantUtils.CreateFrom(in prestigeCostScaler);
			return true;
		}
		if (name == PropertyName.prestigeUpgradeCost)
		{
			value = VariantUtils.CreateFrom(in prestigeUpgradeCost);
			return true;
		}
		if (name == PropertyName.passiveTickAccumulator)
		{
			value = VariantUtils.CreateFrom(in passiveTickAccumulator);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal new static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Nil, "Overall Power", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.LineBoxCords, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.heartLine, PropertyHint.NodeType, "Line2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Displayed Text", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.Upgrades, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.Hearts, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.Record, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Clicker Buttons", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.clickerHolder, PropertyHint.NodeType, "GridContainer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.clickerScene, PropertyHint.ResourceType, "PackedScene", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Upgrade Buttons", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.LoopUpgradeButton, PropertyHint.NodeType, "BaseButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.FloatUpgradeButton, PropertyHint.NodeType, "BaseButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.PowerUpgradeButton, PropertyHint.NodeType, "BaseButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.PrestigeUpgradeButton, PropertyHint.NodeType, "BaseButton", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.LoopUpgradeCostLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.FloatsUpgradeCostLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.PowerUpgradeCostLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.ToyUpgradeCostLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "Balance", PropertyHint.None, "", PropertyUsageFlags.Group, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.baseLoopRate, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.loopRatePerLevel, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.baseGracePeriod, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.graceIncreasePerLevel, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.baseDecayRate, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.decayReductionPerLevel, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.minDecayRate, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.basePowerCap, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.powerCapPerLevel, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.baseLoopUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.baseFloatsUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.basePowerUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Int, PropertyName.baseToyUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.currentHearts, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.upgradedLoops, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.loopCostScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.loopUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.upgradedFloats, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.floatsCostScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.floatsUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.upgradedPower, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.powerCostScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.powerUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.currentPrestige, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.prestigeCostScaler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.prestigeUpgradeCost, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.passiveTickAccumulator, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.LineBoxCords, Variant.From(in LineBoxCords));
		info.AddProperty(PropertyName.heartLine, Variant.From(in heartLine));
		info.AddProperty(PropertyName.Upgrades, Variant.From(in Upgrades));
		info.AddProperty(PropertyName.Hearts, Variant.From(in Hearts));
		info.AddProperty(PropertyName.Record, Variant.From(in Record));
		info.AddProperty(PropertyName.clickerHolder, Variant.From(in clickerHolder));
		info.AddProperty(PropertyName.clickerScene, Variant.From(in clickerScene));
		info.AddProperty(PropertyName.LoopUpgradeButton, Variant.From(in LoopUpgradeButton));
		info.AddProperty(PropertyName.FloatUpgradeButton, Variant.From(in FloatUpgradeButton));
		info.AddProperty(PropertyName.PowerUpgradeButton, Variant.From(in PowerUpgradeButton));
		info.AddProperty(PropertyName.PrestigeUpgradeButton, Variant.From(in PrestigeUpgradeButton));
		info.AddProperty(PropertyName.LoopUpgradeCostLabel, Variant.From(in LoopUpgradeCostLabel));
		info.AddProperty(PropertyName.FloatsUpgradeCostLabel, Variant.From(in FloatsUpgradeCostLabel));
		info.AddProperty(PropertyName.PowerUpgradeCostLabel, Variant.From(in PowerUpgradeCostLabel));
		info.AddProperty(PropertyName.ToyUpgradeCostLabel, Variant.From(in ToyUpgradeCostLabel));
		info.AddProperty(PropertyName.baseLoopRate, Variant.From(in baseLoopRate));
		info.AddProperty(PropertyName.loopRatePerLevel, Variant.From(in loopRatePerLevel));
		info.AddProperty(PropertyName.baseGracePeriod, Variant.From(in baseGracePeriod));
		info.AddProperty(PropertyName.graceIncreasePerLevel, Variant.From(in graceIncreasePerLevel));
		info.AddProperty(PropertyName.baseDecayRate, Variant.From(in baseDecayRate));
		info.AddProperty(PropertyName.decayReductionPerLevel, Variant.From(in decayReductionPerLevel));
		info.AddProperty(PropertyName.minDecayRate, Variant.From(in minDecayRate));
		info.AddProperty(PropertyName.basePowerCap, Variant.From(in basePowerCap));
		info.AddProperty(PropertyName.powerCapPerLevel, Variant.From(in powerCapPerLevel));
		info.AddProperty(PropertyName.baseLoopUpgradeCost, Variant.From(in baseLoopUpgradeCost));
		info.AddProperty(PropertyName.baseFloatsUpgradeCost, Variant.From(in baseFloatsUpgradeCost));
		info.AddProperty(PropertyName.basePowerUpgradeCost, Variant.From(in basePowerUpgradeCost));
		info.AddProperty(PropertyName.baseToyUpgradeCost, Variant.From(in baseToyUpgradeCost));
		info.AddProperty(PropertyName.currentHearts, Variant.From(in currentHearts));
		info.AddProperty(PropertyName.upgradedLoops, Variant.From(in upgradedLoops));
		info.AddProperty(PropertyName.loopCostScaler, Variant.From(in loopCostScaler));
		info.AddProperty(PropertyName.loopUpgradeCost, Variant.From(in loopUpgradeCost));
		info.AddProperty(PropertyName.upgradedFloats, Variant.From(in upgradedFloats));
		info.AddProperty(PropertyName.floatsCostScaler, Variant.From(in floatsCostScaler));
		info.AddProperty(PropertyName.floatsUpgradeCost, Variant.From(in floatsUpgradeCost));
		info.AddProperty(PropertyName.upgradedPower, Variant.From(in upgradedPower));
		info.AddProperty(PropertyName.powerCostScaler, Variant.From(in powerCostScaler));
		info.AddProperty(PropertyName.powerUpgradeCost, Variant.From(in powerUpgradeCost));
		info.AddProperty(PropertyName.currentPrestige, Variant.From(in currentPrestige));
		info.AddProperty(PropertyName.prestigeCostScaler, Variant.From(in prestigeCostScaler));
		info.AddProperty(PropertyName.prestigeUpgradeCost, Variant.From(in prestigeUpgradeCost));
		info.AddProperty(PropertyName.passiveTickAccumulator, Variant.From(in passiveTickAccumulator));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.LineBoxCords, out var value))
		{
			LineBoxCords = value.As<Vector2I>();
		}
		if (info.TryGetProperty(PropertyName.heartLine, out var value2))
		{
			heartLine = value2.As<Line2D>();
		}
		if (info.TryGetProperty(PropertyName.Upgrades, out var value3))
		{
			Upgrades = value3.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.Hearts, out var value4))
		{
			Hearts = value4.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.Record, out var value5))
		{
			Record = value5.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.clickerHolder, out var value6))
		{
			clickerHolder = value6.As<GridContainer>();
		}
		if (info.TryGetProperty(PropertyName.clickerScene, out var value7))
		{
			clickerScene = value7.As<PackedScene>();
		}
		if (info.TryGetProperty(PropertyName.LoopUpgradeButton, out var value8))
		{
			LoopUpgradeButton = value8.As<BaseButton>();
		}
		if (info.TryGetProperty(PropertyName.FloatUpgradeButton, out var value9))
		{
			FloatUpgradeButton = value9.As<BaseButton>();
		}
		if (info.TryGetProperty(PropertyName.PowerUpgradeButton, out var value10))
		{
			PowerUpgradeButton = value10.As<BaseButton>();
		}
		if (info.TryGetProperty(PropertyName.PrestigeUpgradeButton, out var value11))
		{
			PrestigeUpgradeButton = value11.As<BaseButton>();
		}
		if (info.TryGetProperty(PropertyName.LoopUpgradeCostLabel, out var value12))
		{
			LoopUpgradeCostLabel = value12.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.FloatsUpgradeCostLabel, out var value13))
		{
			FloatsUpgradeCostLabel = value13.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.PowerUpgradeCostLabel, out var value14))
		{
			PowerUpgradeCostLabel = value14.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.ToyUpgradeCostLabel, out var value15))
		{
			ToyUpgradeCostLabel = value15.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.baseLoopRate, out var value16))
		{
			baseLoopRate = value16.As<float>();
		}
		if (info.TryGetProperty(PropertyName.loopRatePerLevel, out var value17))
		{
			loopRatePerLevel = value17.As<float>();
		}
		if (info.TryGetProperty(PropertyName.baseGracePeriod, out var value18))
		{
			baseGracePeriod = value18.As<float>();
		}
		if (info.TryGetProperty(PropertyName.graceIncreasePerLevel, out var value19))
		{
			graceIncreasePerLevel = value19.As<float>();
		}
		if (info.TryGetProperty(PropertyName.baseDecayRate, out var value20))
		{
			baseDecayRate = value20.As<float>();
		}
		if (info.TryGetProperty(PropertyName.decayReductionPerLevel, out var value21))
		{
			decayReductionPerLevel = value21.As<float>();
		}
		if (info.TryGetProperty(PropertyName.minDecayRate, out var value22))
		{
			minDecayRate = value22.As<float>();
		}
		if (info.TryGetProperty(PropertyName.basePowerCap, out var value23))
		{
			basePowerCap = value23.As<float>();
		}
		if (info.TryGetProperty(PropertyName.powerCapPerLevel, out var value24))
		{
			powerCapPerLevel = value24.As<float>();
		}
		if (info.TryGetProperty(PropertyName.baseLoopUpgradeCost, out var value25))
		{
			baseLoopUpgradeCost = value25.As<int>();
		}
		if (info.TryGetProperty(PropertyName.baseFloatsUpgradeCost, out var value26))
		{
			baseFloatsUpgradeCost = value26.As<int>();
		}
		if (info.TryGetProperty(PropertyName.basePowerUpgradeCost, out var value27))
		{
			basePowerUpgradeCost = value27.As<int>();
		}
		if (info.TryGetProperty(PropertyName.baseToyUpgradeCost, out var value28))
		{
			baseToyUpgradeCost = value28.As<int>();
		}
		if (info.TryGetProperty(PropertyName.currentHearts, out var value29))
		{
			currentHearts = value29.As<float>();
		}
		if (info.TryGetProperty(PropertyName.upgradedLoops, out var value30))
		{
			upgradedLoops = value30.As<int>();
		}
		if (info.TryGetProperty(PropertyName.loopCostScaler, out var value31))
		{
			loopCostScaler = value31.As<float>();
		}
		if (info.TryGetProperty(PropertyName.loopUpgradeCost, out var value32))
		{
			loopUpgradeCost = value32.As<int>();
		}
		if (info.TryGetProperty(PropertyName.upgradedFloats, out var value33))
		{
			upgradedFloats = value33.As<int>();
		}
		if (info.TryGetProperty(PropertyName.floatsCostScaler, out var value34))
		{
			floatsCostScaler = value34.As<float>();
		}
		if (info.TryGetProperty(PropertyName.floatsUpgradeCost, out var value35))
		{
			floatsUpgradeCost = value35.As<int>();
		}
		if (info.TryGetProperty(PropertyName.upgradedPower, out var value36))
		{
			upgradedPower = value36.As<int>();
		}
		if (info.TryGetProperty(PropertyName.powerCostScaler, out var value37))
		{
			powerCostScaler = value37.As<float>();
		}
		if (info.TryGetProperty(PropertyName.powerUpgradeCost, out var value38))
		{
			powerUpgradeCost = value38.As<int>();
		}
		if (info.TryGetProperty(PropertyName.currentPrestige, out var value39))
		{
			currentPrestige = value39.As<int>();
		}
		if (info.TryGetProperty(PropertyName.prestigeCostScaler, out var value40))
		{
			prestigeCostScaler = value40.As<float>();
		}
		if (info.TryGetProperty(PropertyName.prestigeUpgradeCost, out var value41))
		{
			prestigeUpgradeCost = value41.As<int>();
		}
		if (info.TryGetProperty(PropertyName.passiveTickAccumulator, out var value42))
		{
			passiveTickAccumulator = value42.As<float>();
		}
	}
}
