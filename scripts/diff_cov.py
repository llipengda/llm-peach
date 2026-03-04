#! /usr/bin/env python3

import sys
import argparse
from collections import defaultdict

def parse_lcov_branches(file_path):
    covered_branches = set()
    current_file = None
    try:
        with open(file_path, 'r') as f:
            for line in f:
                line = line.strip()
                if line.startswith('SF:'):
                    current_file = line[3:]
                elif line.startswith('BRDA:'):
                    parts = line[5:].split(',')
                    if len(parts) == 4:
                        line_num, block, branch, taken = parts
                        if taken != '-' and int(taken) > 0:
                            covered_branches.add(
                                (current_file, line_num, block, branch))
                elif line == 'end_of_record':
                    current_file = None
    except FileNotFoundError:
        print(f"Error: File {file_path} not found.")
        sys.exit(1)
    return covered_branches

def print_diff_summary(title, branches, limit=5):
    print(f"\n{'='*15} {title} (Total: {len(branches)}) {'='*15}")
    if not branches:
        return

    file_stats = defaultdict(int)
    for b in branches:
        file_stats[b[0]] += 1

    top_files = sorted(file_stats.items(),
                       key=lambda x: x[1], reverse=True)[:limit]
    for filename, count in top_files:
        print(f" - {filename}: {count} branches")

def generate_info_file(output_path, branches):
    grouped_data = defaultdict(list)
    for b in branches:
        grouped_data[b[0]].append(b[1:])

    try:
        with open(output_path, 'w') as f:
            f.write("TN:\n")
            for file_path in sorted(grouped_data.keys()):
                f.write(f"SF:{file_path}\n")
                
                line_hits = set()
                
                sorted_branches = sorted(grouped_data[file_path], key=lambda x: (int(x[0]), int(x[1]), int(x[2])))
                
                for line_num, block, branch in sorted_branches:
                    f.write(f"BRDA:{line_num},{block},{branch},1\n")
                    line_hits.add(line_num)
                
                for line_num in sorted(line_hits, key=int):
                    f.write(f"DA:{line_num},1\n")
                
                f.write("end_of_record\n")
        print(f"Successfully generated: {output_path}")
    except Exception as e:
        print(f"Error writing to {output_path}: {e}")

def diff(a, b):
    set_a = parse_lcov_branches(a)
    set_b = parse_lcov_branches(b)

    a_only = set_a - set_b
    b_only = set_b - set_a
    intersection = set_a & set_b
    union = set_a | set_b

    return {
        'set_a': set_a,
        'set_b': set_b,
        'a_only': a_only,
        'b_only': b_only,
        'intersection': intersection,
        'union': union
    }

def main(args):
    parser = argparse.ArgumentParser(
        description="Diff and Merge two lcov info files.")
    parser.add_argument("-f", "--file", action="append",
                        required=True, help="Coverage info file")
    parser.add_argument("-n", "--name", action="append",
                        required=True, help="Name of the coverage")
    parser.add_argument("--gen-info", action="store_true", 
                        help="Generate .info files for unique branches")
    
    args = parser.parse_args(args)

    if len(args.file) < 2 or len(args.name) < 2:
        print("Error: Please provide at least two files and names.")
        return

    n1, n2 = args.name[0], args.name[1]
    
    diff_result = diff(args.file[0], args.file[1])
    a_only = diff_result['a_only']
    b_only = diff_result['b_only']

    print_diff_summary(f"Top 5 Files: {n1} Only", a_only)
    print_diff_summary(f"Top 5 Files: {n2} Only", b_only)

    if args.gen_info:
        generate_info_file(f"{n1}_only.info", a_only)
        generate_info_file(f"{n2}_only.info", b_only)

    labels = [n1, n2, f"{n1} Only", f"{n2} Only",
              "Intersection (A ∩ B)", "Union (A ∪ B)"]
    max_label_len = max(len(l) for l in labels) + 2

    print("\n" + "-" * (max_label_len + 15))
    print(f"{'Summary Report':^{max_label_len + 15}}")
    print("-" * (max_label_len + 15))

    fmt = f"  {{:<{max_label_len}}} : {{}}"
    print(fmt.format(n1, len(diff_result['set_a'])))
    print(fmt.format(n2, len(diff_result['set_b'])))
    print("-" * (max_label_len + 15))
    print(fmt.format(f"{n1} Only", len(a_only)))
    print(fmt.format(f"{n2} Only", len(b_only)))
    print(fmt.format("Intersection (∩)", len(diff_result['intersection'])))
    print(fmt.format("Union (∪)", len(diff_result['union'])))
    print("-" * (max_label_len + 15))

if __name__ == "__main__":
    main(sys.argv[1:])