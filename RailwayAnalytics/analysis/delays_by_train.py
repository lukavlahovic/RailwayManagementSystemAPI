import matplotlib.pyplot as plt
from utils import save_chart

def analyze(completed_trips):
    delays_by_train = completed_trips.groupby("Train")["DelayMinutes"].agg(
        TotalDelay = "sum",
        AverageDelay = "mean",
        TripCount="count",
        LateTrips = lambda x: (x > 0).sum()
    ).round(2).reset_index().sort_values("AverageDelay", ascending=False)

    return delays_by_train

def create_chart(delays_by_train):
    fig, axes = plt.subplots(1, 2, figsize=(12,5))

    axes[0].bar(delays_by_train["Train"], delays_by_train["TotalDelay"], color=["#e74c3c", "#e67e22", "#3498db"])
    axes[0].set_title("Total Delay in Minutes by Train")
    axes[0].set_xlabel("Train")
    axes[0].set_ylabel("Minutes")

    axes[1].bar(delays_by_train["Train"], delays_by_train["AverageDelay"], color=["#e74c3c", "#e67e22", "#3498db"])
    axes[1].set_title("Average Delay in Minutes by Train")
    axes[1].set_xlabel("Train")
    axes[1].set_ylabel("Minutes")
    
    plt.tight_layout()
    chart = save_chart(fig)
    plt.close(fig)
    return chart