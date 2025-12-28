#! /usr/bin/env python3

import pandas as pd
import sys

if len(sys.argv) != 3:
    print(f"Usage: {sys.argv[0]} <main_valid_packets.csv> <peach_valid_packets.csv>")
    sys.exit(1)

df_main = pd.read_csv(sys.argv[1], parse_dates=["time"])
df_peach = pd.read_csv(sys.argv[2], parse_dates=["time"])

df_peach = df_peach.rename(columns={
    "packets": "total_packets_peach"
})

df_main = df_main.sort_values("time")
df_peach = df_peach.sort_values("time")

print("Merging data...")

df_merged = pd.merge_asof(
    df_main,
    df_peach,
    on="time",
    direction="nearest",
    tolerance=pd.Timedelta("10s")
)

df_merged = df_merged.sort_values("time").reset_index(drop=True)

rows_to_drop = set()
i = 0

while i < len(df_merged):
    if pd.isna(df_merged.at[i, "total_packets_peach"]):
        j = i
        valid_sum = df_merged.at[i, "valid_packets"] or 0
        packet_sum = df_merged.at[i, "packets"] or 0

        j += 1
        while j < len(df_merged) and pd.isna(df_merged.at[j, "total_packets_peach"]):
            valid_sum += df_merged.at[j, "valid_packets"] or 0
            packet_sum += df_merged.at[j, "packets"] or 0
            rows_to_drop.add(j)
            j += 1

        if j < len(df_merged):
            valid_sum += df_merged.at[j, "valid_packets"] or 0
            packet_sum += df_merged.at[j, "packets"] or 0

            df_merged.at[i, "time"] = df_merged.at[j, "time"]
            df_merged.at[i, "total_packets_peach"] = df_merged.at[j, "total_packets_peach"]
            df_merged.at[i, "total_packets"] = df_merged.at[j, "total_packets"]

            df_merged.at[i, "valid_packets"] = valid_sum
            df_merged.at[i, "packets"] = packet_sum

            rows_to_drop.add(j)
        else:
            break

    i += 1
    
df_merged = df_merged.drop(index=rows_to_drop).reset_index(drop=True)
    
i = 0
rows_to_drop = set()

while i < len(df_merged):
    if i + 1 < len(df_merged) and df_merged.at[i, "total_packets_peach"] == df_merged.at[i + 1, "total_packets_peach"]:
        j = i
        valid_sum= df_merged.at[i, "valid_packets"] or 0
        packet_sum = df_merged.at[i, "packets"] or 0

        j += 1
        while j < len(df_merged) and df_merged.at[j, "total_packets_peach"] == df_merged.at[i, "total_packets_peach"]:
            valid_sum += df_merged.at[j, "valid_packets"] or 0
            packet_sum += df_merged.at[j, "packets"] or 0
            rows_to_drop.add(j)
            j += 1

        if j < len(df_merged):
            valid_sum += df_merged.at[j, "valid_packets"] or 0
            packet_sum += df_merged.at[j, "packets"] or 0

            df_merged.at[i, "time"] = df_merged.at[j, "time"]
            df_merged.at[i, "total_packets_peach"] = df_merged.at[j, "total_packets_peach"]
            df_merged.at[i, "total_packets"] = df_merged.at[j, "total_packets"]

            df_merged.at[i, "valid_packets"] = valid_sum
            df_merged.at[i, "packets"] = packet_sum

            rows_to_drop.add(j)
        else:
            break

    i += 1

df_merged = df_merged.drop(index=rows_to_drop).reset_index(drop=True)

if pd.isna(df_merged.at[len(df_merged) - 1, "total_packets_peach"]) and len(df_merged) > 1:
    i = len(df_merged) - 2
    valid_sum = df_merged.at[i, "valid_packets"] or 0
    packet_sum = df_merged.at[i, "packets"] or 0

    valid_sum += df_merged.at[len(df_merged) - 1, "valid_packets"] or 0
    packet_sum += df_merged.at[len(df_merged) - 1, "packets"] or 0

    df_merged.at[i, "time"] = df_merged.at[len(df_merged) - 1, "time"]
    df_merged.at[i, "total_packets_peach"] = df_merged.at[i, "total_packets_peach"]
    df_merged.at[i, "total_packets"] = df_merged.at[len(df_merged) - 1, "total_packets"]

    df_merged.at[i, "valid_packets"] = valid_sum
    df_merged.at[i, "packets"] = packet_sum

    df_merged = df_merged.drop(index=len(df_merged) - 1).reset_index(drop=True)
    
df_merged["packets_peach"] = df_merged["total_packets_peach"].diff().fillna(df_merged["total_packets_peach"])

df_merged["total_packets_peach"] = df_merged["total_packets_peach"].astype(int)
df_merged["packets_peach"] = df_merged["packets_peach"].astype(int)

cols = df_merged.columns.tolist()
cols.insert(cols.index("total_packets_peach"), cols.pop(cols.index("packets_peach")))
df_merged = df_merged[cols]

out_csv = sys.argv[1].rsplit('/', 1)[0] + "/merged_valid_packets.csv"

df_merged.to_csv(out_csv, index=False)
print(f"Merged data written to {out_csv}")