import matplotlib.pyplot as plt
from utils import save_chart

def analyze(completed_trips):
    delay_routes = completed_trips.groupby("RouteName")["DelayMinutes"].agg(
        TotalDelayMinutes="sum",
        AverageDelayMinutes="mean",
        TripCount="count",
        LateTrips=lambda x: (x > 0).sum()
    ).round(2).reset_index()
    
    return delay_routes

def create_chart(delay_routes):
    fig, axes = plt.subplots(1, 2, figsize=(12, 5))

    axes[0].bar(delay_routes["RouteName"], delay_routes["AverageDelayMinutes"], color=["#e74c3c", "#3498db", "#e67e22"])
    axes[0].set_title("Average Delay by Route (minutes)")
    axes[0].set_xlabel("Route")
    axes[0].set_ylabel("Minutes")
    axes[0].tick_params(axis="x", rotation=15)

    axes[1].bar(delay_routes["RouteName"], delay_routes["TripCount"], label="Total Trips", color="#95a5a6")
    axes[1].bar(delay_routes["RouteName"], delay_routes["LateTrips"], label="Late Trips", color="#e74c3c")
    axes[1].set_title("Late Trips vs Total Trips by Route")
    axes[1].set_xlabel("Route")
    axes[1].set_ylabel("Count")
    axes[1].tick_params(axis="x", rotation=15)
    axes[1].legend()

    plt.tight_layout()
    chart = save_chart(fig)
    plt.close(fig)
    return chart