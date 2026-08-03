#!/usr/bin/env python3
import zipfile, os, shutil, sys, glob

V9 = '/home/user/.cache/v9'
BASE_APK = '/home/user/Teat/Byte-Launcher-v10-mobileui.apk'
PCK = f'{V9}/out/Byte-v30.pck'
PATCHED_PB = '/tmp/v10_extract/assets/project.binary'
LOOSE_DLLS_SRC = '/home/user/.cache/pck_v10/.godot/mono/publish/arm64'
NEW_DLL = f'{V9}/proj/.godot/mono/temp/bin/Debug/DesktopPets.dll'
WORK = f'{V9}/apk_work_v30'
FINAL = f'{V9}/out/Byte-Launcher-v30-blackscreen-fix.apk'

if os.path.exists(WORK):
    shutil.rmtree(WORK)
os.makedirs(WORK)

print("extracting base")
with zipfile.ZipFile(BASE_APK) as z:
    z.extractall(WORK)

removed = 0
for root, dirs, files in os.walk(WORK):
    for fn in files:
        if fn.endswith('.a'):
            os.remove(os.path.join(root, fn))
            removed += 1
print(f"removed {removed} .a")

shutil.copy(PCK, f'{WORK}/assets/assets.sparsepck')
shutil.copy(PATCHED_PB, f'{WORK}/assets/project.binary')
print("replaced pck and pb")

mono_dir = f'{WORK}/assets/.godot/mono/publish/arm64'
os.makedirs(mono_dir, exist_ok=True)

for fn in os.listdir(LOOSE_DLLS_SRC):
    src = os.path.join(LOOSE_DLLS_SRC, fn)
    dst = os.path.join(mono_dir, fn)
    if fn == 'DesktopPets.dll':
        shutil.copy(NEW_DLL, dst)
        print("replaced DesktopPets.dll")
    else:
        if os.path.isfile(src):
            shutil.copy2(src, dst)

print(f"mono dir count {len(os.listdir(mono_dir))}")

tmp = FINAL + '.tmp.zip'
DEFLATE_LIBS = {'lib/arm64-v8a/libc++_shared.so', 'lib/arm64-v8a/libgodot_android.so'}

with zipfile.ZipFile(tmp, 'w', allowZip64=True) as z:
    for root, dirs, files in os.walk(WORK):
        for fn in sorted(files):
            full = os.path.join(root, fn)
            arc = os.path.relpath(full, WORK).replace(os.sep, '/')
            if arc == 'resources.arsc' or arc == 'assets/assets.sparsepck' or arc.startswith('assets/.godot/mono/publish/arm64/'):
                comp = zipfile.ZIP_STORED
            elif arc.startswith('lib/'):
                comp = zipfile.ZIP_DEFLATED if arc in DEFLATE_LIBS else zipfile.ZIP_STORED
            else:
                comp = zipfile.ZIP_DEFLATED
            z.write(full, arc, compress_type=comp)

print(f"rezip size {os.path.getsize(tmp)/1024/1024:.1f} MB")

# find zipalign
zipalign_candidates = glob.glob('/home/user/.cache/**/zipalign', recursive=True)
zipalign = zipalign_candidates[0] if zipalign_candidates else 'zipalign'
print(f"using zipalign {zipalign}")
os.system(f'{zipalign} -f 4 {tmp} {FINAL}')
os.remove(tmp)
print(f"final {FINAL} {os.path.getsize(FINAL)/1024/1024:.1f} MB")
