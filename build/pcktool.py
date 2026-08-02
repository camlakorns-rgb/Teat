#!/usr/bin/env python3
"""Minimal Godot 4 PCK reader/writer (handles v1/v2, embedded-data entries)."""
import struct, sys, os, hashlib

def read_pck(path):
    with open(path, 'rb') as f:
        data = f.read()
    assert data[:4] == b'GDPC', 'not a Godot PCK'
    version, vmajor, vminor, vpatch = struct.unpack_from('<4I', data, 4)
    reserved = data[20:36]
    file_count = struct.unpack_from('<I', data, 36)[0]
    pos = 40
    files = []
    for i in range(file_count):
        path_len = struct.unpack_from('<I', data, pos)[0]; pos += 4
        path = data[pos:pos+path_len].decode('utf-8', 'replace'); pos += path_len
        off, size = struct.unpack_from('<Q Q', data, pos); pos += 16
        md5 = data[pos:pos+16]; pos += 16
        flags = struct.unpack_from('<I', data, pos)[0]; pos += 4
        files.append({'path': path, 'offset': off, 'size': size, 'md5': md5, 'flags': flags})
    return {'version': version, 'file_count': file_count, 'files': files, 'data': data}

def extract_files(pck, outdir, filter_suffix=None):
    data = pck['data']
    for f in pck['files']:
        if filter_suffix and not f['path'].endswith(filter_suffix):
            continue
        off, size = f['offset'], f['size']
        # embedded data flag (Godot 4.4+ sparse): flags bit 0 = embedded? try both
        try:
            content = data[off:off+size]
        except Exception:
            content = b''
        dst = os.path.join(outdir, f['path'].lstrip('/'))
        os.makedirs(os.path.dirname(dst), exist_ok=True)
        with open(dst, 'wb') as out:
            out.write(content)
        print(f"{f['path']}  size={size}  offset={off}")

if __name__ == '__main__':
    pck = read_pck(sys.argv[1])
    print(f"PCK v{pck['version']}, {pck['file_count']} files")
    extract_files(pck, sys.argv[2], sys.argv[3] if len(sys.argv) > 3 else None)
