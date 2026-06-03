import matplotlib.pyplot as plt
from utils import save_chart

def analyze(delays_with_stations):
    delays_by_type_of_delay = delays_with_stations.groupby("TypeOfDelay")["DelayMinutes"].agg(
        TotalDelay = "sum",
        AverageDelay = "mean",
        TotalCount = "count"
    ).round(2).reset_index()

    return delays_by_type_of_delay

def create_chart(delays_by_type_of_delay):
    colors = ["#e74c3c", "#e67e22", "#3498db", "#2ecc71", "#9b59b6", "#1abc9c", "#f39c12"]
    n = len(delays_by_type_of_delay)

    fig, axes = plt.subplots(1, 2, figsize=(12, 5))

    axes[0].bar(delays_by_type_of_delay["TypeOfDelay"], delays_by_type_of_delay["TotalDelay"], color=colors[:n])
    axes[0].set_title("Total Delay in Minutes by Type of Delay")
    axes[0].set_xlabel("Type of Delay")
    axes[0].tick_params(axis="x", rotation=15)
    axes[0].set_ylabel("Minutes")

    axes[1].bar(delays_by_type_of_delay["TypeOfDelay"], delays_by_type_of_delay["AverageDelay"], color=colors[:n])
    axes[1].set_title("Average Delay in Minutes by Type of Delay")
    axes[1].set_xlabel("Type of Delay")
    axes[1].tick_params(axis="x", rotation=15)
    axes[1].set_ylabel("Minutes")
    
    plt.tight_layout()
    chart = save_chart(fig)
    plt.close(fig)
    return chart