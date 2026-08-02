using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Godot.Bridge;
using Godot.NativeInterop;

[ScriptPath("res://Scripts/CharacterScripts/SpriteParentController.cs")]
public class SpriteParentController : Node2D
{
	public new class MethodName : Node2D.MethodName
	{
		public static readonly StringName setupCharacterSprites = "setupCharacterSprites";
	}

	public new class PropertyName : Node2D.PropertyName
	{
	}

	public new class SignalName : Node2D.SignalName
	{
	}

	public void setupCharacterSprites(Character parent)
	{
		foreach (CharAnimDataRes characterAnimationLayer in parent.characterInformation.characterAnimationLayers)
		{
			AnimatedSprite2D animatedSprite2D = new AnimatedSprite2D();
			animatedSprite2D.SpriteFrames = characterAnimationLayer.CharacterAnimationData;
			animatedSprite2D.Offset = characterAnimationLayer.spriteSpecificOffset;
			animatedSprite2D.TextureFilter = TextureFilterEnum.Nearest;
			if (characterAnimationLayer.DefaultAnimation == "INVALID_DEFAULT_ANIMATION")
			{
				GD.PrintErr(characterAnimationLayer.animationType.ToString() + " - Is not properly setting it's Default Animation!!!");
			}
			animatedSprite2D.Play(characterAnimationLayer.DefaultAnimation);
			animatedSprite2D.ZIndex = characterAnimationLayer.ZLayer;
			AddChild(animatedSprite2D, forceReadableName: false, InternalMode.Disabled);
			switch (characterAnimationLayer.animationType)
			{
			case CharAnimDataRes.AnimationTyping.MAIN_BODY:
				parent.MainBody = animatedSprite2D;
				break;
			case CharAnimDataRes.AnimationTyping.FACE_BODY:
				parent.FaceBody = animatedSprite2D;
				parent.FaceBody.AnimationFinished += parent.OnBlinkFinished;
				break;
			case CharAnimDataRes.AnimationTyping.ATTACHED_BODY:
				parent.attachedBodies.Add(animatedSprite2D, characterAnimationLayer);
				break;
			}
		}
		if (GetChild(0) is AnimatedSprite2D animatedSprite2D2)
		{
			base.Scale = parent.characterInformation.characterScale * parent.GetParent<Main>().settingSpriteScaler;
			base.Position = animatedSprite2D2.SpriteFrames.GetFrameTexture(animatedSprite2D2.Animation, 0).GetSize() / 2f * base.Scale;
			base.Position += parent.characterInformation.characterOffset * base.Scale;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static List<MethodInfo> GetGodotMethodList()
	{
		return new List<MethodInfo>(1)
		{
			new MethodInfo(MethodName.setupCharacterSprites, new PropertyInfo(Variant.Type.Nil, "", PropertyHint.None, "", PropertyUsageFlags.Default, exported: false), MethodFlags.Normal, new List<PropertyInfo>
			{
				new PropertyInfo(Variant.Type.Object, "parent", PropertyHint.None, "", PropertyUsageFlags.Default, new StringName("Node2D"), exported: false)
			}, null)
		};
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool InvokeGodotClassMethod(in godot_string_name method, NativeVariantPtrArgs args, out godot_variant ret)
	{
		if (method == MethodName.setupCharacterSprites && args.Count == 1)
		{
			setupCharacterSprites(VariantUtils.ConvertTo<Character>(in args[0]));
			ret = default(godot_variant);
			return true;
		}
		return base.InvokeGodotClassMethod(in method, args, out ret);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override bool HasGodotClassMethod(in godot_string_name method)
	{
		if (method == MethodName.setupCharacterSprites)
		{
			return true;
		}
		return base.HasGodotClassMethod(in method);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void SaveGodotObjectData(GodotSerializationInfo info)
	{
		base.SaveGodotObjectData(info);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void RestoreGodotObjectData(GodotSerializationInfo info)
	{
		base.RestoreGodotObjectData(info);
	}
}
