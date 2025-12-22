#! /usr/bin/env python3

import sys
import argparse
import matplotlib.pyplot as plt
import pandas as pd
import numpy as np
from collections import defaultdict
from statsmodels.nonparametric.smoothers_lowess import lowess

parser = argparse.ArgumentParser()

parser.add_argument("-f", "--file", action="append",
                    required=True, help="CSV file to read")
parser.add_argument("-n", "--name", action="append",
                    required=True, help="Name of the coverage")
parser.add_argument("-o", "--output", required=True, help="Output file name")
parser.add_argument(
    "--title", default="Valid Packet Analysis", help="Title of the plot")


def load_data(file):
    try:
        data = pd.read_csv(file, header=0)

        data["time"] = pd.to_datetime(data["time"])

        start_time = data["time"].iloc[0]
        data["time_minutes"] = (
            data["time"] - start_time) / pd.Timedelta(minutes=1)

        data["valid_ratio"] = data["valid_packets"] / data["packets"]

        cols_to_return = ["time_minutes",
                          "valid_ratio", "valid_packets", "packets"]

        if "total_packets_peach" in data.columns:

            denom_col = "packets_peach" if "packets_peach" in data.columns else "total_packets_peach"

            data = data.iloc[:-1]
            data["valid_ratio_peach"] = data["valid_packets"] / data[denom_col]

            cols_to_return.append("valid_ratio_peach")
            cols_to_return.append(denom_col)

            return data[cols_to_return]

        return data[cols_to_return]
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

    fig, ax = plt.subplots(figsize=(12, 6), dpi=300)
    ax.set_title(args.title)
    ax.set_xlabel("Time (minutes)")
    ax.set_ylabel("Valid Packet Ratio")

    for name, dfs in groups.items():

        all_time = np.concatenate([df["time_minutes"].values for df in dfs])
        all_ratio = np.concatenate([df["valid_ratio"].values for df in dfs])

        total_valid_sum = np.sum([df["valid_packets"].sum() for df in dfs])
        total_packets_sum = np.sum([df["packets"].sum() for df in dfs])
        avg_ratio_global = total_valid_sum / \
            total_packets_sum if total_packets_sum > 0 else 0

        order = np.argsort(all_time)
        all_time = all_time[order]
        all_ratio = all_ratio[order]

        if "valid_ratio_peach" in dfs[0].columns:
            all_ratio_peach = np.concatenate(
                [df["valid_ratio_peach"].values for df in dfs])
            all_ratio_peach = all_ratio_peach[order]

            peach_denom_col = [
                c for c in dfs[0].columns if "packets_peach" in c][0]
            total_peach_sum = np.sum([df[peach_denom_col].sum() for df in dfs])
            avg_ratio_peach_global = total_valid_sum / \
                total_peach_sum if total_peach_sum > 0 else 0

            ax.scatter(
                all_time,
                all_ratio_peach,
                s=10,
                alpha=0.1,

            )

            smooth_peach = lowess(
                all_ratio_peach,
                all_time,
                frac=0.2,
                it=0
            )

            p_peach = ax.plot(
                smooth_peach[:, 0],
                smooth_peach[:, 1],
                label=f"{name} (Peach)",
                linewidth=2,
                linestyle=':'
            )
            color_peach = p_peach[0].get_color()

            ax.axhline(y=avg_ratio_peach_global, color=color_peach,
                       linestyle='--', alpha=0.5, linewidth=1)
            ax.text(
                x=all_time.max(),
                y=avg_ratio_peach_global,
                s=f" Avg: {avg_ratio_peach_global:.2%}",
                color=color_peach,
                verticalalignment='bottom',
                fontsize=8,
                fontweight='bold'
            )

        ax.scatter(
            all_time,
            all_ratio,
            s=10,
            alpha=0.1,

        )

        smooth = lowess(
            all_ratio,
            all_time,
            frac=0.05,
            it=0
        )

        p = ax.plot(
            smooth[:, 0],
            smooth[:, 1],
            label=name,
            linewidth=2
        )
        color = p[0].get_color()

        ax.axhline(y=avg_ratio_global, color=color,
                   linestyle='--', alpha=0.5, linewidth=1)
        ax.text(
            x=all_time.max(),
            y=avg_ratio_global,
            s=f" Avg: {avg_ratio_global:.2%}",
            color=color,
            verticalalignment='bottom',
            fontsize=8,
            fontweight='bold'
        )

    ax.legend(loc='best')
    plt.tight_layout()
    plt.savefig(output)
    plt.close()
    print(f"Plot saved to {output}")


if __name__ == "__main__":
    main()
