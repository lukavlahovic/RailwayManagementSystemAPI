import matplotlib.pyplot as plt
from utils import save_chart

def analyze(delays_with_stations):
    delays_by_station = delays_with_stations.groupby("StationName")["DelayMinutes"].agg(
        TotalDelayMinutes="sum",
        AverageDelayMinutes="mean",
        DelayCount="count"
    ).round(2).reset_index().sort_values("TotalDelayMinutes", ascending=False)

    return delays_by_station

def create_chart(delays_by_station):
    fig, axes = plt.subplots(1, 2, figsize=(12, 5))

    axes[0].bar(delays_by_station["StationName"], delays_by_station["TotalDelayMinutes"], color=["#e74c3c", "#3498db", "#e67e22"])
    axes[0].set_title("Total Delay in Minutes by Station")
    axes[0].set_xlabel("Station")
    axes[0].set_ylabel("Minutes")

    axes[1].bar(delays_by_station["StationName"], delays_by_station["AverageDelayMinutes"], color=["#e74c3c", "#3498db", "#e67e22"])
    axes[1].set_title("Average Delay in Minutes by Station")
    axes[1].set_xlabel("Station")
    axes[1].set_ylabel("Minutes")

    plt.tight_layout()
    chart = save_chart(fig)
    plt.close(fig)
    return chart