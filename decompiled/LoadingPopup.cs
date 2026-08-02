using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/LoadingPopup/LoadingPopup.cs")]
public class LoadingPopup : Window
{
	public new class MethodName : Window.MethodName
	{
		public new static readonly StringName _Ready = "_Ready";

		public new static readonly StringName _Process = "_Process";

		public static readonly StringName OnResourceCacheProgressChanged = "OnResourceCacheProgressChanged";

		public static readonly StringName OnResourceCacheLoaded = "OnResourceCacheLoaded";
	}

	public new class PropertyName : Window.PropertyName
	{
		public static readonly StringName progressBar = "progressBar";

		public static readonly StringName nextScenePath = "nextScenePath";

		public static readonly StringName tipLabel = "tipLabel";

		public static readonly StringName tooltips = "tooltips";

		public static readonly StringName progress = "progress";

		public static readonly StringName sceneLoadPct = "sceneLoadPct";

		public static readonly StringName cachePct = "cachePct";

		public static readonly StringName sceneReady = "sceneReady";

		public static readonly StringName cacheReady = "cacheReady";
	}

	public new class SignalName : Window.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	private ProgressBar progressBar;

	[Export(PropertyHint.None, "")]
	private string nextScenePath;

	[Export(PropertyHint.None, "")]
	private RichTextLabel tipLabel;

	[Export(PropertyHint.None, "")]
	private Array<string> tooltips = new Array<string>();

	private Array progress = new Array();

	private double sceneLoadPct;

	private double cachePct;

	private bool sceneReady;

	private bool cacheReady;

	public override void _Ready()
	{
		tipLabel.Text = "Tip: " + tooltips[GD.RandRange(0, tooltips.Count() - 1)];
		ResourceLoader.LoadThreadedRequest(nextScenePath, "", useSubThreads: false, ResourceLoader.CacheMode.Reuse);
		ResourceCache.Instance.ResourceCacheProgressChanged += OnResourceCacheProgressChanged;
		ResourceCache.Instance.ResourceCacheLoaded += OnResourceCacheLoaded;
	}

	public override void _Process(double delta)
	{
		switch (ResourceLoader.LoadThreadedGetStatus(nextScenePath, progress))
		{
		case ResourceLoader.ThreadLoadStatus.InProgress:
			sceneLoadPct = (double)progress[0] * 100.0;
			break;
		case ResourceLoader.ThreadLoadStatus.Loaded:
			sceneLoadPct = 100.0;
			sceneReady = true;
			break;
		}
		if (sceneReady)
		{
			ResourceCache.Instance.CallDeferred("LoadData");
		}
		progressBar.Value = (sceneLoadPct + cachePct) / 2.0;
		if (sceneReady && cacheReady)
		{
			SetProcess(enable: false);
			PackedScene packedScene = (PackedScene)ResourceLoader.LoadThreadedGet(nextScenePath);
			GetTree().ChangeSceneToPacked(packedScene);
		}
	}

	private void OnResourceCacheProgressChanged(float pct)
	{
		cachePct = pct;
	}

	private void OnResourceCacheLoaded()
	{
		cacheReady = true;
		cachePct = 100.0;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(4)
		{
			new MethodInfo(MethodName._Ready, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnResourceCacheProgressChanged, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "pct", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.OnResourceCacheLoaded, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
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
		if (method == MethodName.OnResourceCacheProgressChanged && args.Count == 1)
		{
			OnResourceCacheProgressChanged(VariantUtils.ConvertTo<float>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.OnResourceCacheLoaded && args.Count == 0)
		{
			OnResourceCacheLoaded();
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
		if (method == MethodName.OnResourceCacheProgressChanged)
		{
			return true;
		}
		if (method == MethodName.OnResourceCacheLoaded)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.progressBar)
		{
			progressBar = VariantUtils.ConvertTo<ProgressBar>(in value);
			return true;
		}
		if (name == PropertyName.nextScenePath)
		{
			nextScenePath = VariantUtils.ConvertTo<string>(in value);
			return true;
		}
		if (name == PropertyName.tipLabel)
		{
			tipLabel = VariantUtils.ConvertTo<RichTextLabel>(in value);
			return true;
		}
		if (name == PropertyName.tooltips)
		{
			tooltips = VariantUtils.ConvertToArray<string>(in value);
			return true;
		}
		if (name == PropertyName.progress)
		{
			progress = VariantUtils.ConvertTo<Array>(in value);
			return true;
		}
		if (name == PropertyName.sceneLoadPct)
		{
			sceneLoadPct = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.cachePct)
		{
			cachePct = VariantUtils.ConvertTo<double>(in value);
			return true;
		}
		if (name == PropertyName.sceneReady)
		{
			sceneReady = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.cacheReady)
		{
			cacheReady = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.progressBar)
		{
			value = VariantUtils.CreateFrom(in progressBar);
			return true;
		}
		if (name == PropertyName.nextScenePath)
		{
			value = VariantUtils.CreateFrom(in nextScenePath);
			return true;
		}
		if (name == PropertyName.tipLabel)
		{
			value = VariantUtils.CreateFrom(in tipLabel);
			return true;
		}
		if (name == PropertyName.tooltips)
		{
			value = VariantUtils.CreateFromArray(tooltips);
			return true;
		}
		if (name == PropertyName.progress)
		{
			value = VariantUtils.CreateFrom(in progress);
			return true;
		}
		if (name == PropertyName.sceneLoadPct)
		{
			value = VariantUtils.CreateFrom(in sceneLoadPct);
			return true;
		}
		if (name == PropertyName.cachePct)
		{
			value = VariantUtils.CreateFrom(in cachePct);
			return true;
		}
		if (name == PropertyName.sceneReady)
		{
			value = VariantUtils.CreateFrom(in sceneReady);
			return true;
		}
		if (name == PropertyName.cacheReady)
		{
			value = VariantUtils.CreateFrom(in cacheReady);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.progressBar, PropertyHint.NodeType, "ProgressBar", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.String, PropertyName.nextScenePath, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.tipLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.tooltips, PropertyHint.TypeString, "4/0:", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Array, PropertyName.progress, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.sceneLoadPct, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Float, PropertyName.cachePct, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.sceneReady, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.cacheReady, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.progressBar, Variant.From(in progressBar));
		info.AddProperty(PropertyName.nextScenePath, Variant.From(in nextScenePath));
		info.AddProperty(PropertyName.tipLabel, Variant.From(in tipLabel));
		info.AddProperty(PropertyName.tooltips, Variant.CreateFrom(tooltips));
		info.AddProperty(PropertyName.progress, Variant.From(in progress));
		info.AddProperty(PropertyName.sceneLoadPct, Variant.From(in sceneLoadPct));
		info.AddProperty(PropertyName.cachePct, Variant.From(in cachePct));
		info.AddProperty(PropertyName.sceneReady, Variant.From(in sceneReady));
		info.AddProperty(PropertyName.cacheReady, Variant.From(in cacheReady));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.progressBar, out var value))
		{
			progressBar = value.As<ProgressBar>();
		}
		if (info.TryGetProperty(PropertyName.nextScenePath, out var value2))
		{
			nextScenePath = value2.As<string>();
		}
		if (info.TryGetProperty(PropertyName.tipLabel, out var value3))
		{
			tipLabel = value3.As<RichTextLabel>();
		}
		if (info.TryGetProperty(PropertyName.tooltips, out var value4))
		{
			tooltips = value4.AsGodotArray<string>();
		}
		if (info.TryGetProperty(PropertyName.progress, out var value5))
		{
			progress = value5.As<Array>();
		}
		if (info.TryGetProperty(PropertyName.sceneLoadPct, out var value6))
		{
			sceneLoadPct = value6.As<double>();
		}
		if (info.TryGetProperty(PropertyName.cachePct, out var value7))
		{
			cachePct = value7.As<double>();
		}
		if (info.TryGetProperty(PropertyName.sceneReady, out var value8))
		{
			sceneReady = value8.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.cacheReady, out var value9))
		{
			cacheReady = value9.As<bool>();
		}
	}
}
