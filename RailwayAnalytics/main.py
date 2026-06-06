import pandas as pd
import sqlalchemy as sa

from config import CONNECTION_STRING
from analysis import delays_by_route, delays_by_station, delays_by_train, on_time_performance, delays_by_type
from report import generator

import argparse

parser = argparse.ArgumentParser(description="Railway Analytics Report Generator")
parser.add_argument("--date", type=str, help="Date to generate report for (YYYY-MM-DD)", default=None)
parser.add_argument("--format", type=str, choices=["html", "pdf", "both"], default="both")
args = parser.parse_args()

engine = sa.create_engine(CONNECTION_STRING)

trips = pd.read_sql("SELECT * FROM Trip", engine)
delays = pd.read_sql("SELECT * FROM Delays", engine)

if args.date:
    trips = trips[trips["ArrivalTime"].dt.date == pd.to_datetime(args.date).date()]
    delays = delays[delays["TripId"].isin(trips["Id"])]

if len(trips) == 0:
    print(f"No trips found for {args.date}")
    exit(1)
    
routes = pd.read_sql("SELECT * FROM Routes", engine)
stations = pd.read_sql("SELECT * FROM Stations", engine)
trains = pd.read_sql("SELECT * FROM Trains", engine)

# ANALYSIS ON COMPLETED TRIPS
trips_with_routes = trips.merge(routes, left_on="RouteId", right_on="Id", suffixes=("_trip", "_route"))
trips_with_routes = trips_with_routes[["Id_trip", "TrainId", "RouteId", "Name", "DepartureTime", "ArrivalTime", "ActualArrivalTime"]]
trips_with_routes = trips_with_routes.rename(columns={"Id_trip": "TripId", "Name": "RouteName"})

trips_with_routes_trains = trips_with_routes.merge(trains, left_on="TrainId", right_on="Id", suffixes=("_trip", "_train"))
trips_with_routes_trains = trips_with_routes_trains[["TripId", "TrainId", "TrainTypeId", "SerialNumber", "RouteId", "RouteName", "DepartureTime"
                                                     , "ArrivalTime", "ActualArrivalTime"]]
trips_with_routes_trains = trips_with_routes_trains.rename(columns={"SerialNumber": "Train"})

completed_trips = trips_with_routes_trains[trips_with_routes_trains["ActualArrivalTime"].notna()].copy()
completed_trips["DelayMinutes"] = (
    completed_trips["ActualArrivalTime"] - completed_trips["ArrivalTime"]
).dt.total_seconds() / 60

delay_routes = delays_by_route.analyze(completed_trips)
chart_route = delays_by_route.create_chart(delay_routes)

# ANALYSIS OF STATIONS
delays_with_stations = delays.merge(stations, left_on="StationId", right_on="Id", suffixes=("_delay","_station"))
delays_with_stations = delays_with_stations[["Id_delay", "TripId", "StationId", "Name", "DelayMinutes", "TypeOfDelay"]]
delays_with_stations = delays_with_stations.rename(columns={"Id_delay": "DelayId", "Name": "StationName"})

delay_type_map = {
    0 : "Weather", 
    1 :"Technical", 
    2 :"StationCongestion", 
    3 :"TrackMaintenance", 
    4 :"PassengerIncident", 
    5 :"ExternalFactor", 
    6 :"Other"
}

delays_with_stations["TypeOfDelay"] = delays_with_stations["TypeOfDelay"].map(delay_type_map)

delays_station = delays_by_station.analyze(delays_with_stations)
chart_station = delays_by_station.create_chart(delays_station)

# ANALYSIS OF TRAINS
delays_train = delays_by_train.analyze(completed_trips)
chart_trains = delays_by_train.create_chart(delays_train)

# PERCENTAGE OF ON TIME TRIPS
on_time_routes = on_time_performance.analyze(delay_routes)
chart_route_on_time = on_time_performance.create_chart(on_time_routes)

# ANALYSIS OF TYPE OF DELAYS
delays_by_type_of_delay = delays_by_type.analyze(delays_with_stations)
chart_delay_type = delays_by_type.create_chart(delays_by_type_of_delay)

generator.generate(completed_trips, delays, delay_routes, delays_station,
                   delays_train, delays_by_type_of_delay, chart_route, chart_station, 
                   chart_trains, chart_route_on_time, chart_delay_type, args.format, args.date)