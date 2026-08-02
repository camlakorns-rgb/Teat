using System;
using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/AttachmentScripts/AttachTextController.cs")]
public class AttachTextController : RichTextLabel
{
	[Signal]
	public delegate void TypewriterFinishedEventHandler();

	public new class MethodName : RichTextLabel.MethodName
	{
		public static readonly StringName StartTypewriter = "StartTypewriter";

		public new static readonly StringName _Process = "_Process";
	}

	public new class PropertyName : RichTextLabel.PropertyName
	{
		public static readonly StringName CharsPerSecond = "CharsPerSecond";

		public static readonly StringName textureSet = "textureSet";

		public static readonly StringName audioStreamPlayer = "audioStreamPlayer";

		public static readonly StringName typingTimer = "typingTimer";

		public static readonly StringName targetCharCount = "targetCharCount";

		public static readonly StringName isTyping = "isTyping";

		public static readonly StringName skipBeatStart = "skipBeatStart";

		public static readonly StringName skipBeat = "skipBeat";
	}

	public new class SignalName : RichTextLabel.SignalName
	{
		public static readonly StringName TypewriterFinished = "TypewriterFinished";
	}

	[Export(PropertyHint.None, "")]
	public float CharsPerSecond = 30f;

	[Export(PropertyHint.None, "")]
	public NinePatchRect textureSet;

	[Export(PropertyHint.None, "")]
	public AudioStreamPlayer audioStreamPlayer;

	private float typingTimer;

	private int targetCharCount;

	private bool isTyping;

	private int skipBeatStart = 2;

	private int skipBeat;

	private TypewriterFinishedEventHandler backing_TypewriterFinished;

	public event TypewriterFinishedEventHandler TypewriterFinished
	{
		add
		{
			backing_TypewriterFinished = (TypewriterFinishedEventHandler)Delegate.Combine(backing_TypewriterFinished, value);
		}
		remove
		{
			backing_TypewriterFinished = (TypewriterFinishedEventHandler)Delegate.Remove(backing_TypewriterFinished, value);
		}
	}

	public void StartTypewriter(string fullText, AudioStream passedSFX)
	{
		fullText = fullText.StripEdges();
		textureSet.Size -= new Vector2(0f, textureSet.PatchMarginBottom + textureSet.PatchMarginTop);
		if (passedSFX != null && Main.Instance.settingAudioOn)
		{
			audioStreamPlayer.Stream = passedSFX;
		}
		base.BbcodeEnabled = true;
		base.Text = fullText;
		base.VisibleCharactersBehavior = TextServer.VisibleCharactersBehavior.CharsAfterShaping;
		targetCharCount = GetTotalCharacterCount();
		base.VisibleCharacters = 0;
		typingTimer = 0f;
		isTyping = true;
	}

	public override void _Process(double delta)
	{
		if (!isTyping)
		{
			return;
		}
		typingTimer += (float)delta;
		float num = Mathf.Max(1f, CharsPerSecond);
		int num2 = Mathf.FloorToInt(typingTimer * num);
		if (num2 >= targetCharCount)
		{
			base.VisibleCharacters = -1;
			isTyping = false;
			EmitSignal(SignalName.TypewriterFinished);
			return;
		}
		if (base.VisibleCharacters != num2 && this.audioStreamPlayer.Stream != null && skipBeat == 0)
		{
			AudioStreamPlayer audioStreamPlayer = (AudioStreamPlayer)this.audioStreamPlayer.Duplicate();
			audioStreamPlayer.PitchScale += (float)GD.RandRange(-0.10000000149011612, 0.10000000149011612);
			if ("aeiou".Contains(GetParsedText()[num2]))
			{
				audioStreamPlayer.PitchScale += 0.2f;
			}
			GetTree().Root.AddChild(audioStreamPlayer, forceReadableName: false, InternalMode.Disabled);
			audioStreamPlayer.Finished += audioStreamPlayer.QueueFree;
			audioStreamPlayer.Play();
			skipBeat = skipBeatStart;
		}
		else if (base.VisibleCharacters != num2 && this.audioStreamPlayer.Stream != null)
		{
			skipBeat--;
		}
		base.VisibleCharacters = num2;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(2)
		{
			new MethodInfo(MethodName.StartTypewriter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "fullText", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.Object, "passedSFX", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("AudioStream"), exported: false)
			}, null),
			new MethodInfo(MethodName._Process, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Float, "delta", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.StartTypewriter && args.Count == 2)
		{
			StartTypewriter(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<AudioStream>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName._Process && args.Count == 1)
		{
			_Process(VariantUtils.ConvertTo<double>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.StartTypewriter)
		{
			return true;
		}
		if (method == MethodName._Process)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.CharsPerSecond)
		{
			CharsPerSecond = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.textureSet)
		{
			textureSet = VariantUtils.ConvertTo<NinePatchRect>(in value);
			return true;
		}
		if (name == PropertyName.audioStreamPlayer)
		{
			audioStreamPlayer = VariantUtils.ConvertTo<AudioStreamPlayer>(in value);
			return true;
		}
		if (name == PropertyName.typingTimer)
		{
			typingTimer = VariantUtils.ConvertTo<float>(in value);
			return true;
		}
		if (name == PropertyName.targetCharCount)
		{
			targetCharCount = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.isTyping)
		{
			isTyping = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		if (name == PropertyName.skipBeatStart)
		{
			skipBeatStart = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		if (name == PropertyName.skipBeat)
		{
			skipBeat = VariantUtils.ConvertTo<int>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.CharsPerSecond)
		{
			value = VariantUtils.CreateFrom(in CharsPerSecond);
			return true;
		}
		if (name == PropertyName.textureSet)
		{
			value = VariantUtils.CreateFrom(in textureSet);
			return true;
		}
		if (name == PropertyName.audioStreamPlayer)
		{
			value = VariantUtils.CreateFrom(in audioStreamPlayer);
			return true;
		}
		if (name == PropertyName.typingTimer)
		{
			value = VariantUtils.CreateFrom(in typingTimer);
			return true;
		}
		if (name == PropertyName.targetCharCount)
		{
			value = VariantUtils.CreateFrom(in targetCharCount);
			return true;
		}
		if (name == PropertyName.isTyping)
		{
			value = VariantUtils.CreateFrom(in isTyping);
			return true;
		}
		if (name == PropertyName.skipBeatStart)
		{
			value = VariantUtils.CreateFrom(in skipBeatStart);
			return true;
		}
		if (name == PropertyName.skipBeat)
		{
			value = VariantUtils.CreateFrom(in skipBeat);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Float, PropertyName.CharsPerSecond, PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.textureSet, PropertyHint.NodeType, "NinePatchRect", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.audioStreamPlayer, PropertyHint.NodeType, "AudioStreamPlayer", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Float, PropertyName.typingTimer, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.targetCharCount, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isTyping, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.skipBeatStart, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Int, PropertyName.skipBeat, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.CharsPerSecond, Variant.From(in CharsPerSecond));
		info.AddProperty(PropertyName.textureSet, Variant.From(in textureSet));
		info.AddProperty(PropertyName.audioStreamPlayer, Variant.From(in audioStreamPlayer));
		info.AddProperty(PropertyName.typingTimer, Variant.From(in typingTimer));
		info.AddProperty(PropertyName.targetCharCount, Variant.From(in targetCharCount));
		info.AddProperty(PropertyName.isTyping, Variant.From(in isTyping));
		info.AddProperty(PropertyName.skipBeatStart, Variant.From(in skipBeatStart));
		info.AddProperty(PropertyName.skipBeat, Variant.From(in skipBeat));
		info.AddSignalEventDelegate(SignalName.TypewriterFinished, backing_TypewriterFinished);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.CharsPerSecond, out var value))
		{
			CharsPerSecond = value.As<float>();
		}
		if (info.TryGetProperty(PropertyName.textureSet, out var value2))
		{
			textureSet = value2.As<NinePatchRect>();
		}
		if (info.TryGetProperty(PropertyName.audioStreamPlayer, out var value3))
		{
			audioStreamPlayer = value3.As<AudioStreamPlayer>();
		}
		if (info.TryGetProperty(PropertyName.typingTimer, out var value4))
		{
			typingTimer = value4.As<float>();
		}
		if (info.TryGetProperty(PropertyName.targetCharCount, out var value5))
		{
			targetCharCount = value5.As<int>();
		}
		if (info.TryGetProperty(PropertyName.isTyping, out var value6))
		{
			isTyping = value6.As<bool>();
		}
		if (info.TryGetProperty(PropertyName.skipBeatStart, out var value7))
		{
			skipBeatStart = value7.As<int>();
		}
		if (info.TryGetProperty(PropertyName.skipBeat, out var value8))
		{
			skipBeat = value8.As<int>();
		}
		if (info.TryGetSignalEventDelegate<TypewriterFinishedEventHandler>(SignalName.TypewriterFinished, out var value9))
		{
			backing_TypewriterFinished = value9;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotSignalList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(SignalName.TypewriterFinished, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null)
		};
	}

	protected void EmitSignalTypewriterFinished()
	{
		EmitSignal(SignalName.TypewriterFinished);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RaiseGodotClassSignalCallbacks(in godot_string_name signal, NativeVariantPtrArgs args)
	{
		if (signal == SignalName.TypewriterFinished && args.Count == 0)
		{
			backing_TypewriterFinished?.Invoke();
		}
		else
		{
			base.RaiseGodotClassSignalCallbacks(in signal, args);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassSignal(in godot_string_name signal)
	{
		if (signal == SignalName.TypewriterFinished)
		{
			return true;
		}
		return base.HasGodotClassSignal(in signal);
	}
}
