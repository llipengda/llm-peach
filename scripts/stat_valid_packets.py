#! /usr/bin/env python3

import datetime
import sys

if len(sys.argv) != 2:
    print(f"Usage: {sys.argv[0]} <fuzz_status_file>")
    sys.exit(1)

file = sys.argv[1]

content = []

with open(file, 'rb') as f:
    while True:
        line = f.read(8+8+5000+1)  # time(8) + total(8) + packets(5000) + end(1)
        if not line:
            break
        content.append(line)

file_dir = file.rsplit('/', 1)[0] if '/' in file else '.'
csv_file = f"{file_dir}/valid_packets.csv"
    
total_packets = 0
total_valid = 0
    
with open(csv_file, 'wb') as f:
    f.write("time,valid_packets,packets,total_packets\n".encode('utf-8'))
    for line in content:
        time = int.from_bytes(line[0:8], 'little')
        try:
            time = datetime.datetime.fromtimestamp(time, datetime.timezone.utc).strftime('%Y-%m-%d %H:%M:%S')
        except:
            time = f'invalid({time})'
        
        total = int.from_bytes(line[8:16], 'little')
        local = 0
        valid = 0
        for i in range(16, len(line) - 1):
            if line[i] == 0x00:
                valid += 1
                total_valid += 1
            total_packets += 1
            local += 1
        if local == 0:
            continue
        print(f"time={time}, total_packets={total}, packets={local}, valid_packets={valid}, ratio={valid/local:.2f}")
        f.write(f"{time},{valid},{local},{total}\n".encode('utf-8'))

print(f"total_packets={total_packets}, valid_packets={total_valid}, ratio={total_valid/total_packets:.2f}")