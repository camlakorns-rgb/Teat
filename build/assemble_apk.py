#!/usr/bin/env python3
"""Assemble the final v9 APK: launcher base + merged PCK + patched project.binary + slim libs."""
import zipfile, os, shutil, sys

V9 = '/home/user/.cache/v9'
BASE = f'{V9}/proj/out/Byte-Launcher.apk'
PCK = f'{V9}/out/Byte-v9.pck'
PATCHED_PB = '/tmp/v8pb/assets/project.binary'  # 12787B known-good patched project.binary
WORK = f'{V9}/apk_work'
FINAL = f'{V9}/out/Byte-Launcher-v9-fullmove2.apk'

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
print('replaced assets/assets.sparsepck + assets/project.binary')

# ---- 4. rezip with correct compression per entry (match v8 layout)
DEFLATE_LIBS = {'lib/arm64-v8a/libc++_shared.so', 'lib/arm64-v8a/libgodot_android.so'}
tmp = FINAL + '.tmp'
with zipfile.ZipFile(tmp, 'w', allowZip64=True) as z:
    for root, dirs, files in os.walk(WORK):
        for fn in sorted(files):
            full = os.path.join(root, fn)
            arc = os.path.relpath(full, WORK).replace(os.sep, '/')
            if arc.startswith('lib/') and arc not in DEFLATE_LIBS:
                store = True
            elif arc.startswith('lib/'):
                store = False
            elif arc.startswith(('resources.arsc', 'assets/assets.sparsepck')):
                store = True
            else:
                store = False
            z.write(full, arc, compress_type=zipfile.ZIP_STORED if store else zipfile.ZIP_DEFLATED)
print('rezip done')

# ---- 5. zipalign
os.system(f'/home/user/.cache/fakesdk/build-tools/35.0.0/zipalign -f 4 {tmp} {FINAL}')
os.remove(tmp)
print('zipalign done ->', FINAL)
print('size:', os.path.getsize(FINAL))
