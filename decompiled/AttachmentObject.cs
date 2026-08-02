using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Godot;
using Godot.Bridge;
using Godot.Collections;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/AttachmentScripts/AttachmentObject.cs")]
public class AttachmentObject : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName SetupImageAttachment = "SetupImageAttachment";

		public static readonly StringName SetupTextAttachment = "SetupTextAttachment";

		public static readonly StringName ResolveTextPlaceholders = "ResolveTextPlaceholders";

		public static readonly StringName FormatVariantAsText = "FormatVariantAsText";

		public static readonly StringName LoadAudioStreamSafe = "LoadAudioStreamSafe";

		public static readonly StringName FlipHorizontal = "FlipHorizontal";
	}

	public new class PropertyName : Node2D.PropertyName
	{
		public static readonly StringName attachedItemInformation = "attachedItemInformation";

		public static readonly StringName spriteParentController = "spriteParentController";

		public static readonly StringName textLabel = "textLabel";

		public static readonly StringName dialogueDataRes = "dialogueDataRes";

		public static readonly StringName trueSize = "trueSize";

		public static readonly StringName isFlipped = "isFlipped";
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	[Export(PropertyHint.None, "")]
	public AttachDataRes attachedItemInformation;

	[ExportSubgroup("IMAGE_INFO", "")]
	[Export(PropertyHint.None, "")]
	public AttachSpriteController spriteParentController;

	[ExportSubgroup("TEXT_INFO", "")]
	[Export(PropertyHint.None, "")]
	public AttachTextController textLabel;

	public DialogueDataRes dialogueDataRes;

	public Vector2I trueSize;

	private bool isFlipped;

	public void SetupImageAttachment()
	{
		trueSize = (Vector2I)(attachedItemInformation.attachmentImageSize * attachedItemInformation.attachmentScale * Main.Instance.settingSpriteScaler);
		spriteParentController.setupItemSprites(this);
	}

	public void SetupTextAttachment(DialogueDataRes passedText)
	{
		dialogueDataRes = passedText;
		string text = ResolveTextPlaceholders(dialogueDataRes.Dialogue);
		textLabel.BbcodeEnabled = true;
		textLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		textLabel.FitContent = true;
		textLabel.VisibleCharacters = -1;
		if (passedText.fontFile != null)
		{
			textLabel.AddThemeFontOverride("normal_font", passedText.fontFile);
		}
		textLabel.CustomMinimumSize = new Vector2(dialogueDataRes.MinimumTextSizePerLine, 0f);
		textLabel.Size = new Vector2(dialogueDataRes.MinimumTextSizePerLine, 0f);
		textLabel.Text = text;
		textLabel.ResetSize();
		Vector2 minimumSize = textLabel.GetMinimumSize();
		textLabel.Scale = attachedItemInformation.attachmentScale * Main.Instance.settingSpriteScaler;
		trueSize = new Vector2I((int)(dialogueDataRes.MinimumTextSizePerLine * attachedItemInformation.attachmentScale.X * Main.Instance.settingSpriteScaler), (int)(minimumSize.Y * attachedItemInformation.attachmentScale.Y * Main.Instance.settingSpriteScaler));
		textLabel.CharsPerSecond = dialogueDataRes.TypingSpeed;
		textLabel.VisibleCharacters = 0;
		if (dialogueDataRes.overrideColor != new Color("ffffff"))
		{
			textLabel.textureSet.Modulate = dialogueDataRes.overrideColor;
		}
		else if (!string.IsNullOrEmpty(dialogueDataRes.speakingActorID))
		{
			if (dialogueDataRes.speakingActorID == Main.Instance.mainCharacter.characterInformation._itemID)
			{
				textLabel.textureSet.Modulate = Main.Instance.mainCharacter.characterInformation.characterColor;
			}
			else if (ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.CHARACTER][dialogueDataRes.speakingActorID] is CharacterInfoDataRes characterInfoDataRes)
			{
				textLabel.textureSet.Modulate = characterInfoDataRes.characterColor;
			}
		}
		else
		{
			textLabel.textureSet.Modulate = Main.Instance.mainCharacter.characterInformation.characterColor;
		}
		textLabel.StartTypewriter(text, LoadAudioStreamSafe(dialogueDataRes.audioStreamUID));
		textLabel.TypewriterFinished += GetParent<AttachObjWindow>().OnTextFinish;
	}

	private string ResolveTextPlaceholders(string text)
	{
		text = text.Replace("{USER}", Main.Instance.userInfoName);
		text = ResolveTagPattern(text, "TAG_DURATION", delegate(string tagName)
		{
			TagDataRes tag3 = Main.Instance.mainCharacter.GetTag(tagName);
			return (tag3 == null) ? "0" : Mathf.RoundToInt(tag3.tagDuration).ToString();
		});
		text = ResolveTagPattern(text, "TAG_VALUE", delegate(string tagName)
		{
			TagDataRes tag2 = Main.Instance.mainCharacter.GetTag(tagName);
			GD.Print(tag2.tagAmount);
			return (tag2 == null) ? "0" : tag2.tagAmount.ToString();
		});
		text = ResolveTagPattern(text, "TAG", delegate(string tagName)
		{
			TagDataRes tag = Main.Instance.mainCharacter.GetTag(tagName);
			return (tag == null) ? tagName : Regex.Replace(tag.tagName, "(?<=[a-z])(?=[A-Z])", " ");
		});
		text = ResolveTagPattern(text, "MINIGAME_DATA", (string minigameName) => (Main.Instance.minigameData == null || !Main.Instance.minigameData.TryGetValue(minigameName, out var value)) ? ("[color=red][ERROR: No Data Found!][/color] - Sorry " + Main.Instance.userInfoName) : FormatVariantAsText(value));
		return text;
	}

	private string ResolveTagPattern(string text, string keyword, Func<string, string> resolver)
	{
		string text2 = "{" + keyword + "(";
		int startIndex = 0;
		while (true)
		{
			int num = text.IndexOf(text2, startIndex, StringComparison.Ordinal);
			if (num == -1)
			{
				break;
			}
			int num2 = num + text2.Length;
			int num3 = text.IndexOf(")}", num2, StringComparison.Ordinal);
			if (num3 == -1)
			{
				break;
			}
			string arg = text.Substring(num2, num3 - num2).Trim();
			string text3 = resolver(arg);
			text = text.Substring(0, num) + text3 + text.Substring(num3 + ")}".Length);
			startIndex = num + text3.Length;
		}
		return text;
	}

	private string FormatVariantAsText(Variant data)
	{
		switch (data.VariantType)
		{
		case Variant.Type.Dictionary:
		{
			Dictionary dictionary = data.AsGodotDictionary();
			List<string> list2 = new List<string>();
			foreach (Variant key in dictionary.Keys)
			{
				list2.Add($"{key}: {FormatVariantAsText(dictionary[key])}");
			}
			return string.Join(", ", list2);
		}
		case Variant.Type.Array:
		{
			Godot.Collections.Array array = data.AsGodotArray();
			List<string> list = new List<string>();
			foreach (Variant item in array)
			{
				list.Add(FormatVariantAsText(item));
			}
			return string.Join(", ", list);
		}
		case Variant.Type.Nil:
			return "None";
		default:
			return data.ToString();
		}
	}

	private AudioStream LoadAudioStreamSafe(string uid)
	{
		if (string.IsNullOrEmpty(uid))
		{
			return null;
		}
		if (!ResourceLoader.Exists(uid))
		{
			return null;
		}
		return ResourceLoader.Load<AudioStream>(uid, null, ResourceLoader.CacheMode.Reuse);
	}

	public void FlipHorizontal(bool isLeft)
	{
		if (isLeft != isFlipped)
		{
			switch (attachedItemInformation.attachmentTyping)
			{
			case AttachDataRes.AttachmentType.OVERRIDE:
				spriteParentController.FlipImage(isLeft);
				break;
			case AttachDataRes.AttachmentType.IMAGE:
				spriteParentController.FlipImage(isLeft);
				break;
			}
			isFlipped = isLeft;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(6)
		{
			new MethodInfo(MethodName.SetupImageAttachment, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.SetupTextAttachment, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "passedText", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.ResolveTextPlaceholders, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FormatVariantAsText, new PropertyInfo(Variant.Type.String, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Nil, "data", PropertyHint.None, "", PropertyUsageFlags.Default | PropertyUsageFlags.NilIsVariant, exported: false)
			}, null),
			new MethodInfo(MethodName.LoadAudioStreamSafe, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("AudioStream"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "uid", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.FlipHorizontal, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Bool, "isLeft", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.SetupImageAttachment && args.Count == 0)
		{
			SetupImageAttachment();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.SetupTextAttachment && args.Count == 1)
		{
			SetupTextAttachment(VariantUtils.ConvertTo<DialogueDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ResolveTextPlaceholders && args.Count == 1)
		{
			string from = ResolveTextPlaceholders(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.FormatVariantAsText && args.Count == 1)
		{
			string from2 = FormatVariantAsText(VariantUtils.ConvertTo<Variant>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.LoadAudioStreamSafe && args.Count == 1)
		{
			AudioStream from3 = LoadAudioStreamSafe(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.FlipHorizontal && args.Count == 1)
		{
			FlipHorizontal(VariantUtils.ConvertTo<bool>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.SetupImageAttachment)
		{
			return true;
		}
		if (method == MethodName.SetupTextAttachment)
		{
			return true;
		}
		if (method == MethodName.ResolveTextPlaceholders)
		{
			return true;
		}
		if (method == MethodName.FormatVariantAsText)
		{
			return true;
		}
		if (method == MethodName.LoadAudioStreamSafe)
		{
			return true;
		}
		if (method == MethodName.FlipHorizontal)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.attachedItemInformation)
		{
			attachedItemInformation = VariantUtils.ConvertTo<AttachDataRes>(in value);
			return true;
		}
		if (name == PropertyName.spriteParentController)
		{
			spriteParentController = VariantUtils.ConvertTo<AttachSpriteController>(in value);
			return true;
		}
		if (name == PropertyName.textLabel)
		{
			textLabel = VariantUtils.ConvertTo<AttachTextController>(in value);
			return true;
		}
		if (name == PropertyName.dialogueDataRes)
		{
			dialogueDataRes = VariantUtils.ConvertTo<DialogueDataRes>(in value);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			trueSize = VariantUtils.ConvertTo<Vector2I>(in value);
			return true;
		}
		if (name == PropertyName.isFlipped)
		{
			isFlipped = VariantUtils.ConvertTo<bool>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.attachedItemInformation)
		{
			value = VariantUtils.CreateFrom(in attachedItemInformation);
			return true;
		}
		if (name == PropertyName.spriteParentController)
		{
			value = VariantUtils.CreateFrom(in spriteParentController);
			return true;
		}
		if (name == PropertyName.textLabel)
		{
			value = VariantUtils.CreateFrom(in textLabel);
			return true;
		}
		if (name == PropertyName.dialogueDataRes)
		{
			value = VariantUtils.CreateFrom(in dialogueDataRes);
			return true;
		}
		if (name == PropertyName.trueSize)
		{
			value = VariantUtils.CreateFrom(in trueSize);
			return true;
		}
		if (name == PropertyName.isFlipped)
		{
			value = VariantUtils.CreateFrom(in isFlipped);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.attachedItemInformation, PropertyHint.ResourceType, "AttachDataRes", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "IMAGE_INFO", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.spriteParentController, PropertyHint.NodeType, "Node2D", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Nil, "TEXT_INFO", PropertyHint.None, "", PropertyUsageFlags.Subgroup, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.textLabel, PropertyHint.NodeType, "RichTextLabel", PropertyUsageFlags.Default | PropertyUsageFlags.ScriptVariable, exported: true),
			new PropertyInfo(Variant.Type.Object, PropertyName.dialogueDataRes, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Vector2I, PropertyName.trueSize, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Bool, PropertyName.isFlipped, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		info.AddProperty(PropertyName.attachedItemInformation, Variant.From(in attachedItemInformation));
		info.AddProperty(PropertyName.spriteParentController, Variant.From(in spriteParentController));
		info.AddProperty(PropertyName.textLabel, Variant.From(in textLabel));
		info.AddProperty(PropertyName.dialogueDataRes, Variant.From(in dialogueDataRes));
		info.AddProperty(PropertyName.trueSize, Variant.From(in trueSize));
		info.AddProperty(PropertyName.isFlipped, Variant.From(in isFlipped));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.attachedItemInformation, out var value))
		{
			attachedItemInformation = value.As<AttachDataRes>();
		}
		if (info.TryGetProperty(PropertyName.spriteParentController, out var value2))
		{
			spriteParentController = value2.As<AttachSpriteController>();
		}
		if (info.TryGetProperty(PropertyName.textLabel, out var value3))
		{
			textLabel = value3.As<AttachTextController>();
		}
		if (info.TryGetProperty(PropertyName.dialogueDataRes, out var value4))
		{
			dialogueDataRes = value4.As<DialogueDataRes>();
		}
		if (info.TryGetProperty(PropertyName.trueSize, out var value5))
		{
			trueSize = value5.As<Vector2I>();
		}
		if (info.TryGetProperty(PropertyName.isFlipped, out var value6))
		{
			isFlipped = value6.As<bool>();
		}
	}
}
