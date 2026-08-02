#!/usr/bin/env python3
"""Strip Godot source-generator glue from ILSpy-decompiled C# so the Godot.NET.Sdk
source generators can regenerate it at compile time."""
import re
import sys
import os

def skip_block(lines, i):
    """lines[i] is the header of a block; consume through its matching closing brace
    (brace may open on a later line). Returns index AFTER the closing line."""
    depth = 0
    while i < len(lines):
        depth += lines[i].count('{') - lines[i].count('}')
        if depth > 0:
            break
        i += 1
    i += 1  # move past the opening-brace line (already counted)
    while i < len(lines) and depth > 0:
        depth += lines[i].count('{') - lines[i].count('}')
        i += 1
    return i

def strip_source(src: str) -> str:
    lines = src.split('\n')

    # 1. Remove nested glue classes: public new class MethodName/PropertyName/SignalName
    out, i = [], 0
    while i < len(lines):
        line = lines[i]
        if re.match(r'\s*public new class (MethodName|PropertyName|SignalName)\b', line):
            i = skip_block(lines, i)
            continue
        out.append(line)
        i += 1
    lines = out

    # 2. Remove [EditorBrowsable(EditorBrowsableState.Never)] members (generated glue)
    out, i = [], 0
    while i < len(lines):
        line = lines[i]
        if '[EditorBrowsable(EditorBrowsableState.Never)]' in line:
            i += 1
            # skip additional attribute lines and modifiers until we hit the member header
            while i < len(lines) and not re.match(r'\s*[\w<>.\[\], ]+\s*\(', lines[i]) and '{' not in lines[i]:
                i += 1
            if i < len(lines):
                i = skip_block(lines, i)
            continue
        out.append(line)
        i += 1
    lines = out

    # 3. Remove backing_ field declarations
    lines = [l for l in lines if not re.match(r'^\s*private\s+[\w.<>\[\], ]+\s+backing_\w+;\s*$', l)]

    # 4. Remove event accessors: public [new] event X Y { add {..} remove {..} }
    out, i = [], 0
    while i < len(lines):
        line = lines[i]
        if re.match(r'\s*public (new )?event\b', line):
            i = skip_block(lines, i)
            continue
        out.append(line)
        i += 1
    lines = out

    # 5. Remove generated EmitSignalX() methods (any parameter list)
    out, i = [], 0
    while i < len(lines):
        line = lines[i]
        if re.match(r'\s*(protected|public|private|internal)?\s*void EmitSignal\w*\(', line):
            i = skip_block(lines, i)
            continue
        out.append(line)
        i += 1
    lines = out

    # 6. Add 'partial' to class declarations
    out = []
    for line in lines:
        m = re.match(r'^(\s*)(public|internal|private)?\s*(sealed\s+)?class\s+(\w+)', line)
        if m and 'partial' not in line and 'static' not in line:
            prefix = m.group(1)
            vis = m.group(2) or 'public'
            sealed = m.group(3) or ''
            name = m.group(4)
            line = f'{prefix}{vis} {sealed}partial class {name}' + line[m.end():]
        out.append(line)
    src = '\n'.join(out)

    src = re.sub(r'\n{3,}', '\n\n', src)
    return src

def main():
    src_dir, out_dir = sys.argv[1], sys.argv[2]
    os.makedirs(out_dir, exist_ok=True)
    for fn in sorted(os.listdir(src_dir)):
        if not fn.endswith('.cs'):
            continue
        with open(os.path.join(src_dir, fn), encoding='utf-8', errors='replace') as f:
            src = f.read()
        stripped = strip_source(src)
        with open(os.path.join(out_dir, fn), 'w', encoding='utf-8') as f:
            f.write(stripped)
        print(f'{fn}: {len(src.splitlines())} -> {len(stripped.splitlines())} lines')

if __name__ == '__main__':
    main()
