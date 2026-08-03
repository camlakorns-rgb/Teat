#!/usr/bin/env python3
"""Assemble the final v12 APK: launcher base + merged PCK + patched project.binary + slim libs."""
import zipfile, os, shutil, sys

V9 = '/home/user/.cache/v9'
BASE = f'{V9}/proj/out/Byte-Launcher.apk'
PCK = f'{V9}/out/Byte-v12.pck'
PATCHED_PB = '/tmp/pb/assets/project.binary'
LOOSE_DLLS = f'{V9}/v11_extracted/assets/.godot/mono/publish/arm64'
WORK = f'{V9}/apk_work'
FINAL = f'{V9}/out/Byte-Launcher-v12-handoff7fix.apk'

# ---- 1. extract base apk
if os.path.exists(WORK):
    shutil.rmtree(WORK)
os.makedirs(WORK)
with zipfile.ZipFile(BASE) as z:
    z.extractall(WORK)
print('extracted base apk')

# ---- 2. remove *.a static libs
removed = 0
for root, dirs, files in os.walk(WORK):
    for fn in files:
        if fn.endswith('.a'):
            os.remove(os.path.join(root, fn))
            removed += 1
print(f'removed {removed} .a libs')

# ---- 3. replace sparsepck + project.binary
shutil.copy(PCK, f'{WORK}/assets/assets.sparsepck')
shutil.copy(PATCHED_PB, f'{WORK}/assets/project.binary')

# ---- 4. Add/replace loose .NET assemblies (replace DesktopPets.dll with new one)
mono_dir = f'{WORK}/assets/.godot/mono/publish/arm64'
os.makedirs(mono_dir, exist_ok=True)
# Copy all DLLs from v11 extraction
for fn in os.listdir(LOOSE_DLLS):
    src = os.path.join(LOOSE_DLLS, fn)
    dst = os.path.join(mono_dir, fn)
    if fn == 'DesktopPets.dll':
        # Use our newly built DLL
        shutil.copy(f'{V9}/proj/.godot/mono/temp/bin/Debug/DesktopPets.dll', dst)
        print(f'replaced DesktopPets.dll (v12 build)')
    else:
        shutil.copy2(src, dst)
print(f'loose .NET assemblies: {len(os.listdir(mono_dir))} files')

# ---- 5. rezip with correct compression per entry
DEFLATE_LIBS = {'lib/arm64-v8a/libc++_shared.so', 'lib/arm64-v8a/libgodot_android.so'}
STORE_EXTENSIONS = {'.arsc', '.sparsepck'}
STORE_PREFIXES = {'assets/.godot/mono/publish/arm64/'}

tmp = FINAL + '.tmp'
with zipfile.ZipFile(tmp, 'w', allowZip64=True) as z:
    for root, dirs, files in os.walk(WORK):
        for fn in sorted(files):
            full = os.path.join(root, fn)
            arc = os.path.relpath(full, WORK).replace(os.sep, '/')
            
            # Determine compression
            store = False
            if arc == 'resources.arsc':
                store = True
            elif arc == 'assets/assets.sparsepck':
                store = True
            elif arc.startswith('assets/.godot/mono/publish/arm64/'):
                store = True
            elif arc.startswith('lib/'):
                store = (arc not in DEFLATE_LIBS)
            z.write(full, arc, compress_type=zipfile.ZIP_STORED if store else zipfile.ZIP_DEFLATED)
print('rezip done')
os.remove(tmp + '.bak') if os.path.exists(tmp + '.bak') else None

# ---- 6. zipalign
os.system(f'/home/user/.cache/fakesdk/build-tools/35.0.0/zipalign -f 4 {tmp} {FINAL}')
os.remove(tmp)
print(f'zipalign done -> {FINAL}')
print(f'size: {os.path.getsize(FINAL) / 1024 / 1024:.1f} MB')
