using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/SubMenus/TerminalMenu/TerminalAsk/TerminalAsk.cs")]
public class TerminalAsk : Node
{
	public new class MethodName : Node.MethodName
	{
		public static readonly StringName Enter = "Enter";

		public static readonly StringName ParseAskInput = "ParseAskInput";

		public static readonly StringName ResolveConvo = "ResolveConvo";

		public static readonly StringName FindBestEntry = "FindBestEntry";

		public static readonly StringName ContainsWholeWord = "ContainsWholeWord";

		public static readonly StringName GetThreshold = "GetThreshold";

		public static readonly StringName PlayConvo = "PlayConvo";

		public static readonly StringName PlayDialogue = "PlayDialogue";

		public static readonly StringName PlayNoMatch = "PlayNoMatch";

		public static readonly StringName RunMatchAction = "RunMatchAction";

		public static readonly StringName PrintHelp = "PrintHelp";

		public static readonly StringName Levenshtein = "Levenshtein";

		public static readonly StringName CommitLine = "CommitLine";
	}

	public new class PropertyName : Node.PropertyName
	{
		public static readonly StringName AskData = "AskData";

		public static readonly StringName ActiveCompanion = "ActiveCompanion";

		public static readonly StringName Handler = "Handler";
	}

	public new class SignalName : Node.SignalName
	{
	}

	public TAsk_AskDataRes AskData { get; private set; }

	public ActorWindow ActiveCompanion { get; private set; }

	private TerminalHandler Handler => GetParent<TerminalHandler>();

	public void Enter(TAsk_AskDataRes data, ActorWindow companion)
	{
		AskData = data;
		ActiveCompanion = companion;
		CommitLine("");
		CommitLine(AskData.EnterText);
		CommitLine("");
	}

	public bool ParseAskInput(string rawInput)
	{
		if (AskData == null)
		{
			return false;
		}
		if (ActiveCompanion != null && !GodotObject.IsInstanceValid(ActiveCompanion))
		{
			CommitLine("  [color=red]They're no longer here.[/color]");
			CommitLine("");
			return false;
		}
		string text = rawInput.Trim();
		if (text.Length == 0)
		{
			return true;
		}
		string text2 = text.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
		if (!(text2 == "quit"))
		{
			if (text2 == "help")
			{
				PrintHelp();
				return true;
			}
			TAsk_EntryDataRes tAsk_EntryDataRes = FindBestEntry(text);
			if (tAsk_EntryDataRes != null)
			{
				ConvoDataRes convoDataRes = ResolveConvo(tAsk_EntryDataRes);
				if (convoDataRes != null)
				{
					RunMatchAction(tAsk_EntryDataRes);
					PlayConvo(convoDataRes);
				}
				else
				{
					PlayNoMatch();
				}
			}
			else
			{
				PlayNoMatch();
			}
			return true;
		}
		CommitLine("");
		CommitLine(AskData.ExitText);
		CommitLine("");
		return false;
	}

	private ConvoDataRes ResolveConvo(TAsk_EntryDataRes entry)
	{
		if (entry.AltConvo != null && entry.AltConvo.Count > 0)
		{
			Character character = Main.Instance?.mainCharacter;
			if (character != null)
			{
				foreach (KeyValuePair<string, ConvoDataRes> item in entry.AltConvo)
				{
					if (item.Value != null)
					{
						string text = item.Key.Trim();
						int num = text.LastIndexOf(' ');
						string tagName = text;
						int num2 = 0;
						if (num >= 0 && int.TryParse(text.Substring(num + 1), out var result))
						{
							tagName = text.Substring(0, num).TrimEnd();
							num2 = result;
						}
						TagDataRes tag = character.GetTag(tagName);
						if (tag != null && (num2 == 0 || tag.tagAmount >= num2))
						{
							return item.Value;
						}
					}
				}
			}
		}
		return entry.Convo;
	}

	private TAsk_EntryDataRes FindBestEntry(string input)
	{
		if (AskData == null)
		{
			return null;
		}
		string text = input.ToLower();
		foreach (TAsk_EntryDataRes entry in AskData.Entries)
		{
			if (entry == null || string.IsNullOrWhiteSpace(entry.Keywords))
			{
				continue;
			}
			foreach (string item in SplitKeywords(entry.Keywords))
			{
				if (text == item || ContainsWholeWord(text, item))
				{
					return entry;
				}
			}
		}
		TAsk_EntryDataRes tAsk_EntryDataRes = null;
		int num = int.MaxValue;
		string keyword = "";
		foreach (TAsk_EntryDataRes entry2 in AskData.Entries)
		{
			if (entry2 == null || string.IsNullOrWhiteSpace(entry2.Keywords))
			{
				continue;
			}
			foreach (string item2 in SplitKeywords(entry2.Keywords))
			{
				int num2 = Levenshtein(text, item2);
				if (num2 < num)
				{
					num = num2;
					tAsk_EntryDataRes = entry2;
					keyword = item2;
				}
			}
		}
		if (tAsk_EntryDataRes == null || num > GetThreshold(keyword))
		{
			return null;
		}
		return tAsk_EntryDataRes;
	}

	private static IEnumerable<string> SplitKeywords(string raw)
	{
		return from k in raw.Split('|', StringSplitOptions.RemoveEmptyEntries)
			select k.Trim().ToLower() into k
			where k.Length > 0
			select k;
	}

	private static bool ContainsWholeWord(string text, string word)
	{
		for (int num = text.IndexOf(word, StringComparison.Ordinal); num >= 0; num = text.IndexOf(word, num + 1, StringComparison.Ordinal))
		{
			bool num2 = num == 0 || !char.IsLetterOrDigit(text[num - 1]);
			bool flag = num + word.Length == text.Length || !char.IsLetterOrDigit(text[num + word.Length]);
			if (num2 && flag)
			{
				return true;
			}
		}
		return false;
	}

	private static int GetThreshold(string keyword)
	{
		int length = keyword.Length;
		if (length <= 3)
		{
			return 0;
		}
		if (length <= 5)
		{
			return 1;
		}
		if (length <= 8)
		{
			return 2;
		}
		return 3;
	}

	private void PlayConvo(ConvoDataRes convo)
	{
		if (convo == null || Main.Instance == null)
		{
			return;
		}
		Main.Instance.ClearAllAttachments();
		if (ActiveCompanion != null && GodotObject.IsInstanceValid(ActiveCompanion))
		{
			foreach (DialogueDataRes item in convo.convoStack)
			{
				DialogueDataRes dialogueDataRes = (DialogueDataRes)item.Duplicate();
				if (string.IsNullOrEmpty(dialogueDataRes.speakingActorID))
				{
					dialogueDataRes.speakingActorID = ActiveCompanion.characterActor.characterInformation.itemID;
				}
				Main.Instance.dialogueStack.Add(dialogueDataRes);
			}
		}
		else
		{
			Main.Instance.dialogueStack = convo.convoStack.Duplicate(deep: true);
		}
		Main.Instance.PopDialogueInStack(skipTimer: true);
		Main.Instance.isInConvo = true;
	}

	private void PlayDialogue(DialogueDataRes dialogue)
	{
		if (dialogue == null || Main.Instance == null)
		{
			return;
		}
		Main.Instance.ClearAllAttachments();
		if (ActiveCompanion != null && GodotObject.IsInstanceValid(ActiveCompanion))
		{
			DialogueDataRes dialogueDataRes = (DialogueDataRes)dialogue.Duplicate();
			if (string.IsNullOrEmpty(dialogueDataRes.speakingActorID))
			{
				dialogueDataRes.speakingActorID = ActiveCompanion.characterActor.characterInformation.itemID;
			}
			Main.Instance.dialogueStack.Add(dialogueDataRes);
		}
		else
		{
			Main.Instance.dialogueStack.Add(dialogue);
		}
		Main.Instance.PopDialogueInStack(skipTimer: true);
		Main.Instance.isInConvo = true;
	}

	private void PlayNoMatch()
	{
		if (AskData == null || AskData.NoMatchDialogue.Count == 0)
		{
			CommitLine("  ...");
			CommitLine("");
			return;
		}
		DialogueDataRes dialogueDataRes = Main.Instance?.PickDialogue(AskData.NoMatchDialogue);
		if (dialogueDataRes != null)
		{
			PlayDialogue(dialogueDataRes);
			return;
		}
		CommitLine("  ...");
		CommitLine("");
	}

	private async void RunMatchAction(TAsk_EntryDataRes entry)
	{
		if (entry.dialogueTask == TAsk_EntryDataRes.CodeTies.QUIT)
		{
			await ToSignal(GetTree().CreateTimer(2.0), SceneTreeTimer.SignalName.Timeout);
			GetTree().Quit();
		}
	}

	private void PrintHelp()
	{
		CommitLine("");
		CommitLine("  [b]ASK MODE — type anything to speak.[/b]");
		CommitLine("  QUIT                   Return to the terminal.");
		CommitLine("  HELP                   Show this listing.");
		CommitLine("");
		if (AskData == null || !AskData.Entries.Any((TAsk_EntryDataRes e) => e != null && !string.IsNullOrWhiteSpace(e.DisplayHint)))
		{
			return;
		}
		CommitLine("  [b]Public topics[/b] (you may find more on your own):");
		foreach (TAsk_EntryDataRes entry in AskData.Entries)
		{
			if (entry != null && !string.IsNullOrWhiteSpace(entry.DisplayHint))
			{
				CommitLine("    - " + entry.DisplayHint);
			}
		}
		CommitLine("");
	}

	private static int Levenshtein(string a, string b)
	{
		if (a.Length == 0)
		{
			return b.Length;
		}
		if (b.Length == 0)
		{
			return a.Length;
		}
		int[,] array = new int[a.Length + 1, b.Length + 1];
		for (int i = 0; i <= a.Length; i++)
		{
			array[i, 0] = i;
		}
		for (int j = 0; j <= b.Length; j++)
		{
			array[0, j] = j;
		}
		for (int k = 1; k <= a.Length; k++)
		{
			for (int l = 1; l <= b.Length; l++)
			{
				int num = ((a[k - 1] != b[l - 1]) ? 1 : 0);
				array[k, l] = Math.Min(Math.Min(array[k - 1, l] + 1, array[k, l - 1] + 1), array[k - 1, l - 1] + num);
			}
		}
		return array[a.Length, b.Length];
	}

	private void CommitLine(string text)
	{
		Handler?.CommitLine(text);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(13)
		{
			new MethodInfo(MethodName.Enter, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "data", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false),
				new PropertyInfo(Variant.Type.Object, "companion", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Window"), exported: false)
			}, null),
			new MethodInfo(MethodName.ParseAskInput, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "rawInput", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ResolveConvo, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "entry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.FindBestEntry, new PropertyInfo(Variant.Type.Object, "", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "input", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.ContainsWholeWord, new PropertyInfo(Variant.Type.Bool, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "word", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.GetThreshold, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "keyword", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.PlayConvo, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "convo", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.PlayDialogue, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "dialogue", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.PlayNoMatch, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.RunMatchAction, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "entry", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Resource"), exported: false)
			}, null),
			new MethodInfo(MethodName.PrintHelp, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, null, null),
			new MethodInfo(MethodName.Levenshtein, new PropertyInfo(Variant.Type.Int, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal | MethodFlags.Static, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "a", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false),
				new PropertyInfo(Variant.Type.String, "b", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null),
			new MethodInfo(MethodName.CommitLine, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.String, "text", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.Enter && args.Count == 2)
		{
			Enter(VariantUtils.ConvertTo<TAsk_AskDataRes>(in args[0]), VariantUtils.ConvertTo<ActorWindow>(in args[1]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.ParseAskInput && args.Count == 1)
		{
			bool from = ParseAskInput(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.ResolveConvo && args.Count == 1)
		{
			ConvoDataRes from2 = ResolveConvo(VariantUtils.ConvertTo<TAsk_EntryDataRes>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.FindBestEntry && args.Count == 1)
		{
			TAsk_EntryDataRes from3 = FindBestEntry(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		if (method == MethodName.ContainsWholeWord && args.Count == 2)
		{
			bool from4 = ContainsWholeWord(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from4);
			return true;
		}
		if (method == MethodName.GetThreshold && args.Count == 1)
		{
			int from5 = GetThreshold(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from5);
			return true;
		}
		if (method == MethodName.PlayConvo && args.Count == 1)
		{
			PlayConvo(VariantUtils.ConvertTo<ConvoDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlayDialogue && args.Count == 1)
		{
			PlayDialogue(VariantUtils.ConvertTo<DialogueDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PlayNoMatch && args.Count == 0)
		{
			PlayNoMatch();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.RunMatchAction && args.Count == 1)
		{
			RunMatchAction(VariantUtils.ConvertTo<TAsk_EntryDataRes>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.PrintHelp && args.Count == 0)
		{
			PrintHelp();
			ret = default(godot_variant);
			return true;
		}
		if (method == MethodName.Levenshtein && args.Count == 2)
		{
			int from6 = Levenshtein(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from6);
			return true;
		}
		if (method == MethodName.CommitLine && args.Count == 1)
		{
			CommitLine(VariantUtils.ConvertTo<string>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static bool InvokeGodotClassStaticMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.ContainsWholeWord && args.Count == 2)
		{
			bool from = ContainsWholeWord(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (method == MethodName.GetThreshold && args.Count == 1)
		{
			int from2 = GetThreshold(VariantUtils.ConvertTo<string>(in args[0]));
			ret = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (method == MethodName.Levenshtein && args.Count == 2)
		{
			int from3 = Levenshtein(VariantUtils.ConvertTo<string>(in args[0]), VariantUtils.ConvertTo<string>(in args[1]));
			ret = VariantUtils.CreateFrom(in from3);
			return true;
		}
		ret = default(godot_variant);
		return false;
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.Enter)
		{
			return true;
		}
		if (method == MethodName.ParseAskInput)
		{
			return true;
		}
		if (method == MethodName.ResolveConvo)
		{
			return true;
		}
		if (method == MethodName.FindBestEntry)
		{
			return true;
		}
		if (method == MethodName.ContainsWholeWord)
		{
			return true;
		}
		if (method == MethodName.GetThreshold)
		{
			return true;
		}
		if (method == MethodName.PlayConvo)
		{
			return true;
		}
		if (method == MethodName.PlayDialogue)
		{
			return true;
		}
		if (method == MethodName.PlayNoMatch)
		{
			return true;
		}
		if (method == MethodName.RunMatchAction)
		{
			return true;
		}
		if (method == MethodName.PrintHelp)
		{
			return true;
		}
		if (method == MethodName.Levenshtein)
		{
			return true;
		}
		if (method == MethodName.CommitLine)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool SetGodotClassPropertyValue(in godot_string_name name, in godot_variant value)
	{
		if (name == PropertyName.AskData)
		{
			AskData = VariantUtils.ConvertTo<TAsk_AskDataRes>(in value);
			return true;
		}
		if (name == PropertyName.ActiveCompanion)
		{
			ActiveCompanion = VariantUtils.ConvertTo<ActorWindow>(in value);
			return true;
		}
		return base.SetGodotClassPropertyValue(in name, in value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool GetGodotClassPropertyValue(in godot_string_name name, out godot_variant value)
	{
		if (name == PropertyName.AskData)
		{
			TAsk_AskDataRes from = AskData;
			value = VariantUtils.CreateFrom(in from);
			return true;
		}
		if (name == PropertyName.ActiveCompanion)
		{
			ActorWindow from2 = ActiveCompanion;
			value = VariantUtils.CreateFrom(in from2);
			return true;
		}
		if (name == PropertyName.Handler)
		{
			TerminalHandler from3 = Handler;
			value = VariantUtils.CreateFrom(in from3);
			return true;
		}
		return base.GetGodotClassPropertyValue(in name, out value);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<PropertyInfo> GetGodotPropertyList()
	{
		return new List<PropertyInfo>
		{
			new PropertyInfo(Variant.Type.Object, PropertyName.AskData, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.ActiveCompanion, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false),
			new PropertyInfo(Variant.Type.Object, PropertyName.Handler, PropertyHint.None, "", PropertyUsageFlags.ScriptVariable, exported: false)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
		StringName askData = PropertyName.AskData;
		TAsk_AskDataRes from = AskData;
		info.AddProperty(askData, Variant.From(in from));
		StringName activeCompanion = PropertyName.ActiveCompanion;
		ActorWindow from2 = ActiveCompanion;
		info.AddProperty(activeCompanion, Variant.From(in from2));
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
		if (info.TryGetProperty(PropertyName.AskData, out var value))
		{
			AskData = value.As<TAsk_AskDataRes>();
		}
		if (info.TryGetProperty(PropertyName.ActiveCompanion, out var value2))
		{
			ActiveCompanion = value2.As<ActorWindow>();
		}
	}
}
