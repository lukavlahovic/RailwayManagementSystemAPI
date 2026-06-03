import matplotlib.pyplot as plt
from utils import save_chart

def analyze(delay_routes):  
    delay_routes = delay_routes.copy()  
    delay_routes["OnTimePercentage"] = (delay_routes["TripCount"] - delay_routes["LateTrips"]) / delay_routes["TripCount"] * 100
    delay_routes = delay_routes.round(2)

    return delay_routes

def create_chart(delay_routes):
    fig, axis = plt.subplots(figsize=(12,5))

    axis.bar(delay_routes["RouteName"], delay_routes["OnTimePercentage"], color=["#e74c3c", "#e67e22", "#3498db"])
    axis.set_title("Percentage of on time arrivals")
    axis.set_xlabel("Route")
    axis.set_ylabel("On time arrivals(%)")
    axis.axhline(y=100, color="green", linestyle="--", alpha=0.5, label="100% on time")
    axis.legend()
    axis.set_ylim(0, 110)
    
    plt.tight_layout()
    chart = save_chart(fig)
    plt.close(fig)
    return chart