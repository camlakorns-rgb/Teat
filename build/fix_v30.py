import os, re, sys

MAIN_PATH = "/home/user/Teat/patched/Scripts/Main.cs"
ITEM_PATH = "/home/user/Teat/patched/Scripts/ItemWindow.cs"
ACTOR_PATH = "/home/user/Teat/patched/Scripts/ActorWindow.cs"
ATTACH_PATH = "/home/user/Teat/patched/Scripts/AttachObjWindow.cs"

# --- Fix Main.cs ---
with open(MAIN_PATH, 'r', encoding='utf-8') as f:
    main = f.read()

# Add V30 marker if not present, replace V13/V29 markers to keep but add new
if "V30_" not in main:
    main = main.replace('public static readonly string V13_BUILD = "V13_ITEMDRAG_AWAYFIX_BUBBLEFIX";',
                        'public static readonly string V13_BUILD = "V13_ITEMDRAG_AWAYFIX_BUBBLEFIX";\n    public static readonly string V30_BUILD = "V30_BLACKSCREEN_ITEMSPAWN_FIX";\n    public static readonly string V30_BUILD2 = "V30_MOBILE_RENDERER_VISIBLE_FALSE";')

# Helper methods for logical visibility
# We'll replace IsPointOnAnyItem, IsPointOnAnyActor, FindItemAtPoint

# Replace IsPointOnAnyItem
old_is_point_item = """    public bool IsPointOnAnyItem(Vector2I p)
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            ItemWindow w = spawnedItems[i];
            if (GodotObject.IsInstanceValid(w) && w.Visible && !w.CurrentlyPickedUp && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }"""

new_is_point_item = """    public bool IsPointOnAnyItem(Vector2I p)
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            ItemWindow w = spawnedItems[i];
            if (!GodotObject.IsInstanceValid(w) || w.CurrentlyPickedUp)
                continue;
            bool logicallyVisible = _isMobile ? w.IsActiveForMobile : w.Visible;
            if (logicallyVisible && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }

    public bool IsItemLogicallyVisible(ItemWindow w)
    {
        if (!GodotObject.IsInstanceValid(w)) return false;
        if (_isMobile) return w.IsActiveForMobile;
        return w.Visible;
    }

    public bool IsActorLogicallyVisible(ActorWindow w)
    {
        if (!GodotObject.IsInstanceValid(w)) return false;
        if (_isMobile) return w.IsActiveForMobile;
        return w.Visible;
    }"""

if old_is_point_item in main:
    main = main.replace(old_is_point_item, new_is_point_item)
else:
    print("IsPointOnAnyItem old not found, trying fuzzy")
    # fallback regex
    pattern = r'public bool IsPointOnAnyItem\(Vector2I p\)\s*\{.*?return false;\s*\}'
    main = re.sub(pattern, new_is_point_item, main, flags=re.DOTALL)

# Fix IsPointOnAnyActor
old_actor = """    public bool IsPointOnAnyActor(Vector2I p)
    {
        for (int i = spawnedActors.Count - 1; i >= 0; i--)
        {
            ActorWindow w = spawnedActors[i];
            if (GodotObject.IsInstanceValid(w) && w.Visible && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }"""

new_actor = """    public bool IsPointOnAnyActor(Vector2I p)
    {
        for (int i = spawnedActors.Count - 1; i >= 0; i--)
        {
            ActorWindow w = spawnedActors[i];
            if (!GodotObject.IsInstanceValid(w)) continue;
            bool logicallyVisible = _isMobile ? w.IsActiveForMobile : w.Visible;
            if (logicallyVisible && new Rect2I(w.Position, w.Size).HasPoint(p))
                return true;
        }
        return false;
    }"""

if old_actor in main:
    main = main.replace(old_actor, new_actor)
else:
    print("IsPointOnAnyActor old not found")

# Fix FindItemAtPoint
old_find = """    private ItemWindow FindItemAtPoint(Vector2I pos)
    {
        // Iterate backwards to find topmost item (last in list = drawn on top)
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            ItemWindow w = spawnedItems[i];
            if (GodotObject.IsInstanceValid(w) && w.Visible && !w.CurrentlyPickedUp)
            {
                Rect2I itemRect = new Rect2I(w.Position, w.Size);
                if (itemRect.HasPoint(pos))
                    return w;
            }
        }
        return null;
    }"""

new_find = """    private ItemWindow FindItemAtPoint(Vector2I pos)
    {
        // Iterate backwards to find topmost item (last in list = drawn on top)
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            ItemWindow w = spawnedItems[i];
            if (!GodotObject.IsInstanceValid(w) || w.CurrentlyPickedUp) continue;
            bool logicallyVisible = _isMobile ? w.IsActiveForMobile : w.Visible;
            if (logicallyVisible)
            {
                Rect2I itemRect = new Rect2I(w.Position, w.Size);
                if (itemRect.HasPoint(pos))
                    return w;
            }
        }
        return null;
    }"""

if old_find in main:
    main = main.replace(old_find, new_find)
else:
    print("FindItemAtPoint not matched, trying regex")
    pattern = r'private ItemWindow FindItemAtPoint\(Vector2I pos\)\s*\{.*?return null;\s*\}'
    main = re.sub(pattern, new_find, main, flags=re.DOTALL)

# Fix TrySetTargetToItem - replace where item.Visible with IsItemLogicallyVisible
# We'll do a regex replacement for that specific LINQ query
main = main.replace(
    "        List<ItemWindow> list = (from item in spawnedItems\n            where GodotObject.IsInstanceValid(item)\n            where settingPassivePlayMode || !item.itemObject.itemInformation.NontargetablePickup\n            where !item.itemObject.itemInformation.NoPassivePickup\n            where item.Visible\n            select item).ToList();",
    "        List<ItemWindow> list = (from item in spawnedItems\n            where GodotObject.IsInstanceValid(item)\n            where settingPassivePlayMode || !item.itemObject.itemInformation.NontargetablePickup\n            where !item.itemObject.itemInformation.NoPassivePickup\n            where IsItemLogicallyVisible(item)\n            select item).ToList();"
)

main = main.replace(
    "        if (possibleArray != null)\n        {\n            list.Clear();\n            list = (from item in possibleArray\n                where GodotObject.IsInstanceValid(item)\n                where settingPassivePlayMode || !item.itemObject.itemInformation.NontargetablePickup\n                where !item.itemObject.itemInformation.NoPassivePickup\n                where item.Visible\n                select item).ToList();\n        }",
    "        if (possibleArray != null)\n        {\n            list.Clear();\n            list = (from item in possibleArray\n                where GodotObject.IsInstanceValid(item)\n                where settingPassivePlayMode || !item.itemObject.itemInformation.NontargetablePickup\n                where !item.itemObject.itemInformation.NoPassivePickup\n                where IsItemLogicallyVisible(item)\n                select item).ToList();\n        }"
)

# Fix CheckLandingInteraction
old_companion_query = "        List<ActorWindow> source = (from a in spawnedCompanions\n            where GodotObject.IsInstanceValid(a) && a.Visible && !a.inUse && !a.inUseByAttachment\n            orderby Mathf.Abs(a.Position.X + a.Size.X / 2 - centerX)\n            select a).ToList();"

new_companion_query = "        List<ActorWindow> source = (from a in spawnedCompanions\n            where GodotObject.IsInstanceValid(a) && IsActorLogicallyVisible(a) && !a.inUse && !a.inUseByAttachment\n            orderby Mathf.Abs(a.Position.X + a.Size.X / 2 - centerX)\n            select a).ToList();"

main = main.replace(old_companion_query, new_companion_query)

old_item_landing = "        foreach (ItemWindow spawnedItem in spawnedItems)\n        {\n            if (!GodotObject.IsInstanceValid(spawnedItem) || !spawnedItem.Visible || spawnedItem.CurrentlyPickedUp || !spawnedItem.itemObject.itemInformation.isUsableDroppedOn)\n            {\n                continue;\n            }"

new_item_landing = "        foreach (ItemWindow spawnedItem in spawnedItems)\n        {\n            if (!GodotObject.IsInstanceValid(spawnedItem) || !IsItemLogicallyVisible(spawnedItem) || spawnedItem.CurrentlyPickedUp || !spawnedItem.itemObject.itemInformation.isUsableDroppedOn)\n            {\n                continue;\n            }"

main = main.replace(old_item_landing, new_item_landing)

# Fix ResourceCache loading - add robust retry
# Find the block in _Ready that does AddChild MobileUI and spawner timers and LoadData
old_ready_snippet = """        if (_isMobile)
        {
            AddChild(new MobileUI());
            if (spawnerTimer != null && spawnerTimer.IsStopped())
            {
                spawnerTimer.WaitTime = 4f;
                spawnerTimer.Start();
            }
            if (spawnerActorTimer != null && spawnerActorTimer.IsStopped())
            {
                spawnerActorTimer.WaitTime = 20f;
                spawnerActorTimer.Start();
            }
            if (ResourceCache.Instance != null)
            {
                ResourceCache.Instance.CallDeferred("LoadData");
            }
        }"""

new_ready_snippet = """        if (_isMobile)
        {
            AddChild(new MobileUI());
            if (spawnerTimer != null && spawnerTimer.IsStopped())
            {
                spawnerTimer.WaitTime = 4f;
                spawnerTimer.Start();
            }
            if (spawnerActorTimer != null && spawnerActorTimer.IsStopped())
            {
                spawnerActorTimer.WaitTime = 20f;
                spawnerActorTimer.Start();
            }
            _resourceLoadRetries = 0;
            TryLoadResources();
        }"""

if old_ready_snippet in main:
    main = main.replace(old_ready_snippet, new_ready_snippet)
else:
    print("Ready snippet not found, trying partial")
    # try to find similar
    main = main.replace("if (ResourceCache.Instance != null)\n            {\n                ResourceCache.Instance.CallDeferred(\"LoadData\");\n            }", "_resourceLoadRetries = 0;\n            TryLoadResources();")

# Now add TryLoadResources method after _Ready or near. Find the _Ready closing bracket and insert method after.
# We'll insert after the _Ready method definition ends, before _Process.

# Add field and method definitions near top of class after existing fields.
# Insert after _itemLastDragTime field.

field_insert = "    private Vector2I _savedTouchPos;"
new_field_with_retry = """    private Vector2I _savedTouchPos;
    private int _resourceLoadRetries = 0;

    public void TryLoadResources()
    {
        if (ResourceCache.Instance != null)
        {
            bool needLoad = false;
            if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count == 0)
                needLoad = true;
            if (needLoad)
            {
                GD.Print("[Mobile] TryLoadResources attempt " + _resourceLoadRetries + " - calling LoadData");
                ResourceCache.Instance.CallDeferred("LoadData");
            }
            else
            {
                GD.Print("[Mobile] ResourceCache already loaded: ITEM=" + ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count);
                return;
            }
        }
        else
        {
            GD.Print("[Mobile] ResourceCache.Instance null, retrying...");
        }
        _resourceLoadRetries++;
        if (_resourceLoadRetries < 20)
        {
            GetTree().CreateTimer(0.6).Timeout += () => TryLoadResources();
        }
    }"""

if field_insert in main:
    main = main.replace(field_insert, new_field_with_retry)
else:
    print("field insert not found")

# Also fix OnSpawnerTimeout to retry LoadData if empty
old_spawner_retry = """            if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count == 0)
            {
                GD.PrintErr("[Spawner] ITEM resources not ready yet - retrying in 5s");
                spawnerTimer.WaitTime = 5f;
                spawnerTimer.Start();
                return;
            }"""

new_spawner_retry = """            if (!ResourceCache.resourcesLoaded.ContainsKey(ResourceCache.ResourceTyping.ITEM) || ResourceCache.resourcesLoaded[ResourceCache.ResourceTyping.ITEM].Count == 0)
            {
                GD.PrintErr("[Spawner] ITEM resources not ready yet - retrying in 5s (attempting LoadData again)");
                if (ResourceCache.Instance != null)
                {
                    ResourceCache.Instance.CallDeferred("LoadData");
                }
                else
                {
                    TryLoadResources();
                }
                spawnerTimer.WaitTime = 5f;
                spawnerTimer.Start();
                return;
            }"""

if old_spawner_retry in main:
    main = main.replace(old_spawner_retry, new_spawner_retry)

# Write back
with open(MAIN_PATH, 'w', encoding='utf-8') as f:
    f.write(main)

print("Main.cs fixed")

# --- Fix ItemWindow.cs ---
with open(ITEM_PATH, 'r', encoding='utf-8') as f:
    item = f.read()

# Replace SetupItemWindow mobile part to use Visible=false and add IsActive properties
# Add public properties near top after fields

# Check if properties exist
if "public bool IsActiveForMobile" not in item:
    # insert after private Node2D _mobileSpriteRoot;
    item = item.replace(
        "    // Mobile renderer: render item as sprite at scene root to avoid Window flickering\n\tprivate Node2D _mobileSpriteRoot;\n\tprivate AnimatedSprite2D _mobileSprite;",
        "    // Mobile renderer: render item as sprite at scene root to avoid Window flickering\n\tprivate Node2D _mobileSpriteRoot;\n\tprivate AnimatedSprite2D _mobileSprite;\n\n\tpublic bool IsSetup => isSetup;\n\tpublic bool IsActiveForMobile => isSetup && !CurrentlyPickedUp;"
    )

# Fix _Process mobile sprite visibility and sync

old_process_sync = """\t\tif (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
\t\t{
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.Visible = base.Visible && !CurrentlyPickedUp;
\t\t\tif (_mobileSprite != null && GodotObject.IsInstanceValid(_mobileSprite))
\t\t\t{
\t\t\t\tif (itemObject != null && itemObject.spriteParentController != null && itemObject.spriteParentController.GetChildCount() > 0 && itemObject.spriteParentController.GetChild(0) is AnimatedSprite2D srcSprite)
\t\t\t\t{
\t\t\t\t\tif (_mobileSprite.SpriteFrames != srcSprite.SpriteFrames)
\t\t\t\t\t\t_mobileSprite.SpriteFrames = srcSprite.SpriteFrames;
\t\t\t\t\t_mobileSprite.Animation = srcSprite.Animation;
\t\t\t\t\t_mobileSprite.Frame = srcSprite.Frame;
\t\t\t\t\t_mobileSprite.Scale = itemObject.spriteParentController.Scale;
\t\t\t\t\t_mobileSprite.Position = itemObject.spriteParentController.Position;
\t\t\t\t\t_mobileSprite.FlipH = srcSprite.FlipH;
\t\t\t\t\t_mobileSprite.FlipV = srcSprite.FlipV;
\t\t\t\t}
\t\t\t}
\t\t}"""

new_process_sync = """\t\tif (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
\t\t{
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.Visible = IsActiveForMobile;
\t\t\tif (_mobileSprite != null && GodotObject.IsInstanceValid(_mobileSprite))
\t\t\t{
\t\t\t\tif (itemObject != null && itemObject.spriteParentController != null && itemObject.spriteParentController.GetChildCount() > 0 && itemObject.spriteParentController.GetChild(0) is AnimatedSprite2D srcSprite)
\t\t\t\t{
\t\t\t\t\tif (_mobileSprite.SpriteFrames != srcSprite.SpriteFrames)
\t\t\t\t\t\t_mobileSprite.SpriteFrames = srcSprite.SpriteFrames;
\t\t\t\t\t_mobileSprite.Animation = srcSprite.Animation;
\t\t\t\t\t_mobileSprite.Frame = srcSprite.Frame;
\t\t\t\t\t_mobileSprite.Scale = itemObject.spriteParentController.Scale;
\t\t\t\t\t_mobileSprite.Position = itemObject.spriteParentController.Position;
\t\t\t\t\t_mobileSprite.FlipH = srcSprite.FlipH;
\t\t\t\t\t_mobileSprite.FlipV = srcSprite.FlipV;
\t\t\t\t}
\t\t\t}
\t\t}"""

if old_process_sync in item:
    item = item.replace(old_process_sync, new_process_sync)
else:
    # try to replace any similar
    item = item.replace("_mobileSpriteRoot.Visible = base.Visible && !CurrentlyPickedUp;", "_mobileSpriteRoot.Visible = IsActiveForMobile;")

# Fix SetupItemWindow
old_setup_mobile = """\t\t// Mobile renderer: create sprite at scene root to avoid Window flickering
\t\tif (Main._isMobile)
\t\t{
\t\t\tbase.Visible = true;
\t\t\tif (itemObject != null && itemObject.spriteParentController != null)
\t\t\t{
\t\t\t\titemObject.spriteParentController.Visible = false;
\t\t\t}
\t\t\t_mobileSprite = new AnimatedSprite2D();
\t\t\tif (itemObject != null && itemObject.spriteParentController != null && itemObject.spriteParentController.GetChildCount() > 0 && itemObject.spriteParentController.GetChild(0) is AnimatedSprite2D srcSprite)
\t\t\t{
\t\t\t\t_mobileSprite.SpriteFrames = srcSprite.SpriteFrames;
\t\t\t\t_mobileSprite.Scale = itemObject.spriteParentController.Scale;
\t\t\t\t_mobileSprite.Position = itemObject.spriteParentController.Position;
\t\t\t\t_mobileSprite.Play(srcSprite.Animation);
\t\t\t\t_mobileSprite.Frame = srcSprite.Frame;
\t\t\t\t_mobileSprite.FlipH = srcSprite.FlipH;
\t\t\t\t_mobileSprite.FlipV = srcSprite.FlipV;
\t\t\t}
\t\t\t
\t\t\t_mobileSpriteRoot = new Node2D();
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.AddChild(_mobileSprite);
\t\t\tGetTree().Root.AddChild(_mobileSpriteRoot);
\t\t}"""

new_setup_mobile = """\t\t// Mobile renderer: create sprite at scene root to avoid Window flickering (V30 fix: Visible=false to prevent black screen overlay)
\t\tif (Main._isMobile)
\t\t{
\t\t\tbase.Transparent = true;
\t\t\tbase.TransparentBg = true;
\t\t\tbase.Visible = false;
\t\t\tif (itemObject != null && itemObject.spriteParentController != null)
\t\t\t{
\t\t\t\titemObject.spriteParentController.Visible = false;
\t\t\t}
\t\t\t_mobileSprite = new AnimatedSprite2D();
\t\t\tif (itemObject != null && itemObject.spriteParentController != null && itemObject.spriteParentController.GetChildCount() > 0 && itemObject.spriteParentController.GetChild(0) is AnimatedSprite2D srcSprite)
\t\t\t{
\t\t\t\t_mobileSprite.SpriteFrames = srcSprite.SpriteFrames;
\t\t\t\t_mobileSprite.Scale = itemObject.spriteParentController.Scale;
\t\t\t\t_mobileSprite.Position = itemObject.spriteParentController.Position;
\t\t\t\t_mobileSprite.Play(srcSprite.Animation);
\t\t\t\t_mobileSprite.Frame = srcSprite.Frame;
\t\t\t\t_mobileSprite.FlipH = srcSprite.FlipH;
\t\t\t\t_mobileSprite.FlipV = srcSprite.FlipV;
\t\t\t}
\t\t\t
\t\t\t_mobileSpriteRoot = new Node2D();
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.Visible = true;
\t\t\t_mobileSpriteRoot.AddChild(_mobileSprite);
\t\t\ttry { GetTree().Root.AddChild(_mobileSpriteRoot); } catch { }
\t\t}"""

if old_setup_mobile in item:
    item = item.replace(old_setup_mobile, new_setup_mobile)
else:
    print("Item setup old not found, trying to patch Visible line")
    item = item.replace("base.Visible = true;", "base.Transparent = true;\n\t\t\tbase.TransparentBg = true;\n\t\t\tbase.Visible = false;")

# Also fix early _Process CurrentlyPickedUp handling to hide mobile sprite
old_picked = """\t\tif (CurrentlyPickedUp)
\t\t{
\t\t\tbase.Visible = false;
\t\t\tbase.MousePassthrough = true;
\t\t\treturn;
\t\t}"""

new_picked = """\t\tif (CurrentlyPickedUp)
\t\t{
\t\t\tbase.Visible = false;
\t\t\tbase.MousePassthrough = true;
\t\t\tif (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
\t\t\t\t_mobileSpriteRoot.Visible = false;
\t\t\treturn;
\t\t}"""

item = item.replace(old_picked, new_picked)

with open(ITEM_PATH, 'w', encoding='utf-8') as f:
    f.write(item)

print("ItemWindow.cs fixed")

# --- Fix ActorWindow.cs ---
with open(ACTOR_PATH, 'r', encoding='utf-8') as f:
    actor = f.read()

if "public bool IsActiveForMobile" not in actor:
    actor = actor.replace(
        "    // Mobile renderer: render NPC as sprite in scene root viewport to avoid Window flickering\n\tprivate Node2D _mobileSpriteRoot;",
        "    private bool isSetup = false;\n    public bool IsSetup => isSetup;\n    public bool IsActiveForMobile => isSetup;\n\n    // Mobile renderer: render NPC as sprite in scene root viewport to avoid Window flickering\n\tprivate Node2D _mobileSpriteRoot;"
    )
else:
    # ensure isSetup exists
    if "private bool isSetup" not in actor:
        actor = actor.replace("public bool IsSetup => isSetup;", "private bool isSetup = false;\n    public bool IsSetup => isSetup;")

# Ensure isSetup set in SetupActorWindow
old_setup_actor_transparent = """\t\tbase.MinSize = characterActor.trueSize;
\t\tbase.Size = base.MinSize;
\t\tif (overridePos == Vector2I.Zero)"""

# We'll insert isSetup = true near there and Transparent handling
# Replace the whole mobile block

old_actor_mobile = """\t\t// Mobile renderer: create sprite at scene root to avoid Window flickering
\t\tif (Main._isMobile)
\t\t{
\t\t\tbase.Visible = true;
\t\t\tif (characterActor != null && characterActor.MainBody != null)
\t\t\t{
\t\t\t\tcharacterActor.MainBody.Visible = false;
\t\t\t}
\t\t\t_mobileSprite = new AnimatedSprite2D();
\t\t\tif (characterActor != null && characterActor.MainBody != null)
\t\t\t{
\t\t\t\t_mobileSprite.SpriteFrames = characterActor.MainBody.SpriteFrames;
\t\t\t\t_mobileSprite.Scale = characterActor.MainBody.Scale;
\t\t\t\t_mobileSprite.Position = characterActor.MainBody.Position;
\t\t\t\t_mobileSprite.Play(characterActor.MainBody.Animation);
\t\t\t\t_mobileSprite.Frame = characterActor.MainBody.Frame;
\t\t\t\t_mobileSprite.FlipH = characterActor.MainBody.FlipH;
\t\t\t\t_mobileSprite.FlipV = characterActor.MainBody.FlipV;
\t\t\t}
\t\t\t
\t\t\t_mobileSpriteRoot = new Node2D();
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.AddChild(_mobileSprite);
\t\t\t
\t\t\t// Add to scene tree root (NOT Main.Instance) so it doesn't move with Byte
\t\t\tGetTree().Root.AddChild(_mobileSpriteRoot);
\t\t}
\t\telse
\t\t{
\t\t\tbase.Visible = true;
\t\t}"""

new_actor_mobile = """\t\tisSetup = true;
\t\t// Mobile renderer: create sprite at scene root to avoid Window flickering (V30 fix: Visible=false)
\t\tif (Main._isMobile)
\t\t{
\t\t\tbase.Transparent = true;
\t\t\tbase.TransparentBg = true;
\t\t\tbase.Visible = false;
\t\t\tif (characterActor != null && characterActor.MainBody != null)
\t\t\t{
\t\t\t\tcharacterActor.MainBody.Visible = false;
\t\t\t}
\t\t\t_mobileSprite = new AnimatedSprite2D();
\t\t\tif (characterActor != null && characterActor.MainBody != null)
\t\t\t{
\t\t\t\t_mobileSprite.SpriteFrames = characterActor.MainBody.SpriteFrames;
\t\t\t\t_mobileSprite.Scale = characterActor.MainBody.Scale;
\t\t\t\t_mobileSprite.Position = characterActor.MainBody.Position;
\t\t\t\t_mobileSprite.Play(characterActor.MainBody.Animation);
\t\t\t\t_mobileSprite.Frame = characterActor.MainBody.Frame;
\t\t\t\t_mobileSprite.FlipH = characterActor.MainBody.FlipH;
\t\t\t\t_mobileSprite.FlipV = characterActor.MainBody.FlipV;
\t\t\t}
\t\t\t
\t\t\t_mobileSpriteRoot = new Node2D();
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.Visible = true;
\t\t\t_mobileSpriteRoot.AddChild(_mobileSprite);
\t\t\t
\t\t\t// Add to scene tree root (NOT Main.Instance) so it doesn't move with Byte
\t\t\ttry { GetTree().Root.AddChild(_mobileSpriteRoot); } catch {}
\t\t}
\t\telse
\t\t{
\t\t\tbase.Visible = true;
\t\t}"""

if old_actor_mobile in actor:
    actor = actor.replace(old_actor_mobile, new_actor_mobile)
else:
    print("Actor mobile old not found")
    actor = actor.replace("base.Visible = true;\n\t\t\tif (characterActor != null && characterActor.MainBody != null)", "base.Transparent = true;\n\t\t\tbase.TransparentBg = true;\n\t\t\tbase.Visible = false;\n\t\t\tif (characterActor != null && characterActor.MainBody != null)")
    actor = actor.replace("base.MinSize = characterActor.trueSize;", "isSetup = true;\n\t\tbase.MinSize = characterActor.trueSize;")

# Fix _Process visibility sync
old_actor_process = """\t\tif (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
\t\t{
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.Visible = base.Visible && characterActor != null && characterActor.Visible;
\t\t\tif (_mobileSprite != null && GodotObject.IsInstanceValid(_mobileSprite) && characterActor != null && characterActor.MainBody != null)
\t\t\t{
\t\t\t\tif (_mobileSprite.SpriteFrames != characterActor.MainBody.SpriteFrames)
\t\t\t\t\t_mobileSprite.SpriteFrames = characterActor.MainBody.SpriteFrames;
\t\t\t\t_mobileSprite.Animation = characterActor.MainBody.Animation;
\t\t\t\t_mobileSprite.Frame = characterActor.MainBody.Frame;
\t\t\t\t_mobileSprite.Scale = characterActor.MainBody.Scale;
\t\t\t\t_mobileSprite.Position = characterActor.MainBody.Position;
\t\t\t\t_mobileSprite.FlipH = characterActor.MainBody.FlipH;
\t\t\t\t_mobileSprite.FlipV = characterActor.MainBody.FlipV;
\t\t\t}
\t\t}"""

new_actor_process = """\t\tif (Main._isMobile && _mobileSpriteRoot != null && GodotObject.IsInstanceValid(_mobileSpriteRoot))
\t\t{
\t\t\t_mobileSpriteRoot.Position = (Vector2)base.Position;
\t\t\t_mobileSpriteRoot.Visible = IsActiveForMobile && characterActor != null && characterActor.Visible;
\t\t\tif (_mobileSprite != null && GodotObject.IsInstanceValid(_mobileSprite) && characterActor != null && characterActor.MainBody != null)
\t\t\t{
\t\t\t\tif (_mobileSprite.SpriteFrames != characterActor.MainBody.SpriteFrames)
\t\t\t\t\t_mobileSprite.SpriteFrames = characterActor.MainBody.SpriteFrames;
\t\t\t\t_mobileSprite.Animation = characterActor.MainBody.Animation;
\t\t\t\t_mobileSprite.Frame = characterActor.MainBody.Frame;
\t\t\t\t_mobileSprite.Scale = characterActor.MainBody.Scale;
\t\t\t\t_mobileSprite.Position = characterActor.MainBody.Position;
\t\t\t\t_mobileSprite.FlipH = characterActor.MainBody.FlipH;
\t\t\t\t_mobileSprite.FlipV = characterActor.MainBody.FlipV;
\t\t\t}
\t\t}"""

if old_actor_process in actor:
    actor = actor.replace(old_actor_process, new_actor_process)
else:
    actor = actor.replace("_mobileSpriteRoot.Visible = base.Visible &&", "_mobileSpriteRoot.Visible = IsActiveForMobile &&")

with open(ACTOR_PATH, 'w', encoding='utf-8') as f:
    f.write(actor)

print("ActorWindow.cs fixed")

# --- Fix AttachObjWindow ---
with open(ATTACH_PATH, 'r', encoding='utf-8') as f:
    attach = f.read()

# Ensure transparent on mobile
old_attach_setup = """    private void DelayedSetupFlag()
    {
        base.Visible = true;
        _isSetup = true;
    }"""

new_attach_setup = """    private void DelayedSetupFlag()
    {
        if (Main._isMobile)
        {
            base.Transparent = true;
            base.TransparentBg = true;
        }
        base.Visible = true;
        _isSetup = true;
    }"""

if old_attach_setup in attach:
    attach = attach.replace(old_attach_setup, new_attach_setup)

# Also fix TEXT mouse passthrough already done, but add transparent for all
with open(ATTACH_PATH, 'w', encoding='utf-8') as f:
    f.write(attach)

print("AttachObjWindow fixed")
