import os
from jinja2 import Template
from datetime import datetime
from weasyprint import HTML

def generate(completed_trips, delays, delay_routes, delays_by_station, 
             delays_by_train, delays_by_type_of_delay, chart_route, chart_station, 
             chart_trains, chart_route_on_time, chart_delay_type, format="both", date_of_report=None):

    os.makedirs("output", exist_ok=True)

    template_str = """
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset="UTF-8">
        <title>Railway Analytics Report</title>
        <style>
            body { font-family: Arial, sans-serif; margin: 40px; color: #333; }
            h1 { color: #2c3e50; border-bottom: 2px solid #e74c3c; padding-bottom: 10px; }
            h2 { color: #e74c3c; margin-top: 40px; }
            .chart { margin: 20px 0; }
            .chart img { width: 100%; max-width: 900px; }
            .summary { background: #f8f9fa; padding: 20px; border-radius: 8px; margin: 20px 0; }
            .summary-grid { display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; }
            .stat { text-align: center; }
            .stat-number { font-size: 2em; font-weight: bold; color: #e74c3c; }
            .stat-label { color: #666; font-size: 0.9em; }
            table { width: 100%; border-collapse: collapse; margin: 20px 0; }
            th { background: #2c3e50; color: white; padding: 10px; text-align: left; }
            td { padding: 8px 10px; border-bottom: 1px solid #ddd; }
            tr:nth-child(even) { background: #f8f9fa; }
            .generated { color: #999; font-size: 0.8em; margin-top: 40px; }
        </style>
    </head>
    <body>
        <h1>🚆 Railway Management System — Analytics Report</h1>
        <p class="generated">Generated on {{ generated_at }}</p>

        <div class="summary">
            <h2>Summary</h2>
            <div class="summary-grid">
                <div class="stat">
                    <div class="stat-number">{{ total_trips }}</div>
                    <div class="stat-label">Total Completed Trips</div>
                </div>
                <div class="stat">
                    <div class="stat-number">{{ total_delays }}</div>
                    <div class="stat-label">Total Delay Incidents</div>
                </div>
                <div class="stat">
                    <div class="stat-number">{{ avg_delay }} min</div>
                    <div class="stat-label">Average Delay per Trip</div>
                </div>
            </div>
        </div>

        <h2>Delays by Route</h2>
        <div class="chart"><img src="data:image/png;base64,{{ chart_route }}"></div>
        <table>
            <tr><th>Route</th><th>Total Delay (min)</th><th>Avg Delay (min)</th><th>Trips</th><th>Late Trips</th></tr>
            {% for _, row in delays_by_route.iterrows() %}
            <tr>
                <td>{{ row.RouteName }}</td>
                <td>{{ row.TotalDelayMinutes }}</td>
                <td>{{ row.AverageDelayMinutes }}</td>
                <td>{{ row.TripCount }}</td>
                <td>{{ row.LateTrips }}</td>
            </tr>
            {% endfor %}
        </table>

        <h2>Delays by Station</h2>
        <div class="chart"><img src="data:image/png;base64,{{ chart_station }}"></div>
        <table>
            <tr><th>Station</th><th>Total Delay (min)</th><th>Avg Delay (min)</th><th>Incidents</th></tr>
            {% for _, row in delays_by_station.iterrows() %}
            <tr>
                <td>{{ row.StationName }}</td>
                <td>{{ row.TotalDelayMinutes }}</td>
                <td>{{ row.AverageDelayMinutes }}</td>
                <td>{{ row.DelayCount }}</td>
            </tr>
            {% endfor %}
        </table>

        <h2>Delays by Train</h2>
        <div class="chart"><img src="data:image/png;base64,{{ chart_train }}"></div>
        <table>
            <tr><th>Train</th><th>Total Delay (min)</th><th>Avg Delay (min)</th><th>Trips</th><th>Late Trips</th></tr>
            {% for _, row in delays_by_train.iterrows() %}
            <tr>
                <td>{{ row.Train }}</td>
                <td>{{ row.TotalDelay }}</td>
                <td>{{ row.AverageDelay }}</td>
                <td>{{ row.TripCount }}</td>
                <td>{{ row.LateTrips }}</td>
            </tr>
            {% endfor %}
        </table>

        <h2>On-Time Performance by Route</h2>
        <div class="chart"><img src="data:image/png;base64,{{ chart_ontime }}"></div>

        <h2>Delays by Type</h2>
        <div class="chart"><img src="data:image/png;base64,{{ chart_delay_type }}"></div>
        <table>
            <tr><th>Type</th><th>Total Delay (min)</th><th>Avg Delay (min)</th><th>Incidents</th></tr>
            {% for _, row in delays_by_type_of_delay.iterrows() %}
            <tr>
                <td>{{ row.TypeOfDelay }}</td>
                <td>{{ row.TotalDelay }}</td>
                <td>{{ row.AverageDelay }}</td>
                <td>{{ row.TotalCount }}</td>
            </tr>
            {% endfor %}
        </table>

    </body>
    </html>
    """

    template = Template(template_str)

    html = template.render(
        generated_at=datetime.now().strftime("%Y-%m-%d %H:%M"),
        total_trips=len(completed_trips),
        total_delays=len(delays),
        avg_delay=round(completed_trips["DelayMinutes"].mean(), 1),
        chart_route=chart_route,
        chart_station=chart_station,
        chart_train=chart_trains,
        chart_ontime=chart_route_on_time,
        chart_delay_type=chart_delay_type,
        delays_by_route=delay_routes,
        delays_by_station=delays_by_station,
        delays_by_train=delays_by_train,
        delays_by_type_of_delay=delays_by_type_of_delay
    )

    filename = f"report_{date_of_report}" if date_of_report else "report"

    with open(f"output/{filename}.html", "w", encoding="utf-8") as f:
        f.write(html)

    print("Report saved to output/report.html")

    if(format in ("pdf", "both")):
        HTML(filename=f"output/{filename}.html").write_pdf(f"output/{filename}.pdf")
        print("PDF saved to output/report.pdf")