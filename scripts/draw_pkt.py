#! /usr/bin/env python3

import sys
import argparse
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
from collections import defaultdict
from statsmodels.nonparametric.smoothers_lowess import lowess

col_names = ["time", "valid_packets", "packets", "total_packets"]

parser = argparse.ArgumentParser()

parser.add_argument("-f", "--file", action="append", required=True, help="CSV file to read")
parser.add_argument("-n", "--name", action="append", required=True, help="Name of the coverage")
parser.add_argument("-o", "--output", required=True, help="Output file name")
parser.add_argument("--title", default="Valid Packet Analysis", help="Title of the plot")

def load_data(file):
    try:
        data = pd.read_csv(file, header=0, names=col_names)
        
        data["time"] = pd.to_datetime(data["time"])
        
        start_time = data["time"].iloc[0]
        data["time_minutes"] = (data["time"] - start_time) / pd.Timedelta(minutes=1)
        
        data["valid_ratio"] = data["valid_packets"] / data["packets"]
        
        return data[["time_minutes", "valid_ratio"]]
    except Exception as e:
        return None

def main():
    args = parser.parse_args()
    files = args.file
    names = args.name
    output = args.output
    
    if len(files) != len(names):
        parser.error("Number of files and names must be the same")
        
    groups = defaultdict(list)
    max_total_time = 0.0
    
    for file, name in zip(files, names):
        df = load_data(file)
        if df is not None and not df.empty:
            groups[name].append(df)
            max_total_time = max(max_total_time, df["time_minutes"].max())

    if not groups:
        print("No valid data files were loaded. Exiting.")
        sys.exit(1)
        
    global_common_time = np.linspace(0, max_total_time, 500)
    analysis_results = {}

    fig, ax = plt.subplots(figsize=(10, 5), dpi=300) 
    ax.set_title(args.title)
    ax.set_xlabel("Time (minutes)")
    ax.set_ylabel("Valid Packet Ratio")
    
    for name, dfs in groups.items():
        all_time = np.concatenate([df["time_minutes"].values for df in dfs])
        all_ratio = np.concatenate([df["valid_ratio"].values for df in dfs])

        order = np.argsort(all_time)
        all_time = all_time[order]
        all_ratio = all_ratio[order]

        ax.scatter(
            all_time,
            all_ratio,
            s=10,
            alpha=0.2,
            label=name
        )

        smooth = lowess(
            all_ratio,
            all_time,
            frac=0.05,
            it=0
        )

        ax.plot(
            smooth[:, 0],
            smooth[:, 1],
            label=name,
            linewidth=2
        )
        
    ax.legend()
    plt.savefig(output)
    plt.close()
    print(f"Plot saved to {output}")

if __name__ == "__main__":
    main()