import zipfile, os, glob, shutil
V9='/home/user/.cache/v9'
BASE='/home/user/Teat/Byte-Launcher-v10-mobileui.apk'
PCK=f'{V9}/out/Byte-v45.pck'
PB='/tmp/v10_extract/assets/project.binary'
SRC='/home/user/.cache/pck_v10/.godot/mono/publish/arm64'
NEW=f'{V9}/proj/.godot/mono/temp/bin/Debug/DesktopPets.dll'
WORK=f'{V9}/apk_work_v45'
FINAL=f'{V9}/out/Byte-Launcher-v45-bit-shorter-spawn-menu.apk'
if os.path.exists(WORK):
    shutil.rmtree(WORK)
os.makedirs(WORK)
with zipfile.ZipFile(BASE) as z:
    z.extractall(WORK)
shutil.copy(PCK, f'{WORK}/assets/assets.sparsepck')
shutil.copy(PB, f'{WORK}/assets/project.binary')
mono_dir=f'{WORK}/assets/.godot/mono/publish/arm64'
os.makedirs(mono_dir, exist_ok=True)
for fn in os.listdir(SRC):
    src=os.path.join(SRC,fn)
    dst=os.path.join(mono_dir,fn)
    if fn=='DesktopPets.dll':
        shutil.copy(NEW,dst)
    else:
        if os.path.isfile(src):
            shutil.copy2(src,dst)
tmp=FINAL+'.tmp.zip'
DEFLATE={'lib/arm64-v8a/libc++_shared.so','lib/arm64-v8a/libgodot_android.so'}
with zipfile.ZipFile(tmp,'w',allowZip64=True) as z:
    for root,dirs,files in os.walk(WORK):
        for fn in sorted(files):
            full=os.path.join(root,fn)
            arc=os.path.relpath(full,WORK).replace(os.sep,'/')
            if arc=='resources.arsc' or arc=='assets/assets.sparsepck' or arc.startswith('assets/.godot/mono/publish/arm64/'):
                comp=zipfile.ZIP_STORED
            elif arc.startswith('lib/'):
                comp=zipfile.ZIP_DEFLATED if arc in DEFLATE else zipfile.ZIP_STORED
            else:
                comp=zipfile.ZIP_DEFLATED
            z.write(full,arc,compress_type=comp)
cands=glob.glob('/home/user/.cache/**/zipalign',recursive=True)
za=cands[0] if cands else 'zipalign'
os.system(f'{za} -f 4 {tmp} {FINAL}')
os.remove(tmp)
print(f"final {FINAL} {os.path.getsize(FINAL)/1024/1024:.1f} MB")
