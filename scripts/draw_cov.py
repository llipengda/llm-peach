#! /usr/bin/env python3

import sys
import argparse
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
import warnings
from collections import defaultdict

col_names = ["timestamp", "lines_hit", "lines_total", "functions_hit",
             "functions_total", "branches_hit", "branches_total"]

parser = argparse.ArgumentParser()

parser.add_argument("-f", "--file", action="append",
                    required=True, help="CSV file to read")
parser.add_argument("-n", "--name", action="append",
                    required=True, help="Name of the coverage")
parser.add_argument("-o", "--output", required=True, help="Output file name")
parser.add_argument("--title", default="Coverage Analysis",
                    help="Title of the plot")


def load_data(file):
    try:
        data = pd.read_csv(file, header=0, names=col_names)
        data = data.iloc[:-1]

        data["timestamp"] = data["timestamp"].str.replace("_", " ")
        data["timestamp"] = pd.to_datetime(data["timestamp"])

        start_time = data["timestamp"].iloc[0]
        data["time_minutes"] = (
            data["timestamp"] - start_time) / pd.Timedelta(minutes=1)

        return data[["time_minutes", "branches_hit"]]
    except Exception as e:
        print(f"Error loading {file}: {e}")
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

    fig, ax = plt.subplots(figsize=(12, 6), dpi=300)
    ax.set_title(args.title)
    ax.set_xlabel("Time (minutes)")
    ax.set_ylabel("Branches Hit (Mean)")

    for name, dfs in groups.items():
        interpolated_y = []

        for df in dfs:
            y_interp = np.interp(global_common_time,
                                 df["time_minutes"],
                                 df["branches_hit"],
                                 right=np.nan)
            interpolated_y.append(y_interp)

        y_matrix = np.array(interpolated_y)

        with warnings.catch_warnings():
            warnings.simplefilter("ignore", category=RuntimeWarning)
            y_mean = np.nanmean(y_matrix, axis=0)
            y_std = np.nanstd(y_matrix, axis=0)

        if len(dfs) == 1:
            line, = ax.plot(
                global_common_time,
                y_mean,
                label=name,
                marker='o',
                markersize=4,
                markevery=2
            )
        else:
            line, = ax.plot(
                global_common_time,
                y_mean,
                label=name
            )

        valid_indices = ~np.isnan(y_mean)

        if not np.any(valid_indices):
            continue

        last_valid_idx = np.where(valid_indices)[0][-1]

        final_x = global_common_time[last_valid_idx]
        final_y = y_mean[last_valid_idx]

        ax.annotate(
            f"{final_y:.1f}",
            xy=(final_x, final_y),
            xytext=(5, 0),
            textcoords='offset points',
            color=line.get_color(),
            fontweight='bold',
            fontsize=9,
            verticalalignment='center'
        )

        ax.plot(final_x, final_y, marker='o',
                color=line.get_color(), markersize=5)

        if len(dfs) > 1:
            ax.fill_between(global_common_time,
                            y_mean - y_std,
                            y_mean + y_std,
                            color=line.get_color(),
                            alpha=0.2)

    ax.legend()
    plt.tight_layout()
    plt.savefig(output)
    plt.close()
    print(f"Plot saved to {output}")


if __name__ == "__main__":
    main()
