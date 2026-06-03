# 🚆 Railway Management System API

A backend REST API for managing railway operations including train scheduling, station management, route planning, and delay tracking. Built with ASP.NET Core following a layered architecture with separation of concerns and deployed to Azure.

---

## ⚙️ Tech Stack

### Backend
- **.NET 8 / ASP.NET Core Web API**
- **C#**
- **SQL Server**
- **Entity Framework Core**
- **AutoMapper**
- **FluentValidation**
- **JWT Authentication**
- **Swagger / OpenAPI**

### Testing
- **xUnit**
- **SQLite In-Memory**

### DevOps
- **Docker + Docker Compose**
- **GitHub Actions CI/CD**
- **Azure App Service**
- **Azure Container Registry**
- **Azure SQL Database**

### Analytics
- **Python 3.13**
- **pandas**
- **matplotlib**
- **SQLAlchemy**
- **jinja2**
- **weasyprint**

---

## 🏗️ Architecture

The project follows a layered architecture with clear separation of concerns:

```
Request → Middleware → Controller → Service → DbContext → Database
```

### Layers
- **Controllers** — handle HTTP concerns only (routing, request parsing, response codes)
- **Services** — contain all business logic, accessed via interfaces
- **Data (DbContext)** — EF Core database access layer
- **DTOs** — separate models for input and output, never exposing raw entities
- **Validators** — FluentValidation classes, one per DTO
- **Mapping** — AutoMapper profiles for entity ↔ DTO conversion
- **Middleware** — global exception handling returning consistent ProblemDetails responses
- **Exceptions** — custom exception types (NotFoundException, BadRequestException)

### Project Structure
```
RailwayManagementSystemAPI/
├── .github/
│   └── workflows/
│       └── ci.yml                    ← GitHub Actions CI/CD pipeline
├── Controllers/
│   ├── AuthController.cs
│   ├── StationController.cs
│   ├── TrainController.cs
│   ├── TrainTypeController.cs
│   ├── RouteController.cs
│   ├── TripController.cs
│   └── DelayController.cs
├── Services/
│   ├── IAuthService.cs / AuthService.cs
│   ├── IStationService.cs / StationService.cs
│   ├── ITrainService.cs / TrainService.cs
│   ├── ITrainTypeService.cs / TrainTypeService.cs
│   ├── IRouteService.cs / RouteService.cs
│   ├── ITripService.cs / TripService.cs
│   └── IDelayService.cs / DelayService.cs
├── Models/
│   ├── User.cs
│   ├── Station.cs
│   ├── Train.cs
│   ├── TrainType.cs
│   ├── Route.cs
│   ├── RouteStation.cs
│   ├── Trip.cs
│   └── Delay.cs
├── Dtos/
│   ├── StationDto.cs / StationResponseDto.cs
│   ├── CreateTrainDto.cs / TrainResponseDto.cs
│   ├── CreateTrainTypeDto.cs / TrainTypeResponseDto.cs
│   ├── CreateRouteDto.cs / RouteResponseDto.cs
│   ├── RouteStationDto.cs / RouteStationResponseDto.cs
│   ├── CreateTripDto.cs / TripResponseDto.cs
│   ├── TripScheduleDto.cs / TripSearchResponseDto.cs
│   ├── TripSearchQuery.cs
│   ├── TripPositionDto.cs
│   ├── CompleteTripDto.cs
│   ├── CreateDelayDto.cs / DelayResponseDto.cs
│   ├── StationScheduleDto.cs
│   ├── PaginationQuery.cs
│   └── PagedResult.cs
├── Validators/
│   ├── StationDtoValidator.cs
│   ├── CreateTrainDtoValidator.cs
│   ├── CreateTrainTypeDtoValidator.cs
│   ├── CreateRouteDtoValidator.cs
│   ├── RouteStationDtoValidator.cs
│   ├── CreateTripDtoValidator.cs
│   ├── CreateDelayDtoValidator.cs
│   ├── RegisterDtoValidator.cs
│   ├── LoginDtoValidator.cs
│   └── CompleteTripDtoValidator.cs
├── Mapping/
│   └── MappingProfile.cs
├── Middleware/
│   └── ExceptionHandlingMiddleware.cs
├── Exceptions/
│   ├── NotFoundException.cs
│   └── BadRequestException.cs
├── Configuration/
│   └── JwtSettings.cs
├── Seeding/
│   └── DatabaseSeeder.cs
├── Data/
│   └── RailwayContext.cs
├── RailwayManagementSystemAPI.Tests/
│   ├── Services/
│   │   ├── StationServiceTests.cs
│   │   ├── TrainServiceTests.cs
│   │   ├── TrainTypeServiceTests.cs
│   │   ├── RouteServiceTests.cs
│   │   ├── TripServiceTests.cs
│   │   ├── TripPositionServiceTests.cs
│   │   └── DelayServiceTests.cs
│   ├── Validators/
│   │   ├── StationDtoValidatorTests.cs
│   │   ├── CreateTrainDtoValidatorTests.cs
│   │   ├── CreateTrainTypeDtoValidatorTests.cs
│   │   ├── CreateRouteDtoValidatorTests.cs
│   │   ├── RouteStationDtoValidatorTests.cs
│   │   ├── CreateTripDtoValidatorTests.cs
│   │   └── CreateDelayDtoValidatorTests.cs
│   └── TestBase.cs
├── RailwayAnalytics/
│   ├── main.py
│   ├── config.py
│   ├── requirements.txt
│   └── analysis/
├── Dockerfile
├── docker-compose.yml
└── .dockerignore
```

---

## 🗄️ Data Models

### User
Represents an API user with role-based access.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Username | string | Unique username |
| Email | string | Unique email |
| PasswordHash | string | BCrypt hashed password |
| Role | UserRole | Admin or Operator |
| CreatedAt | DateTime | Account creation time |

### Station
Represents a railway station.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Station name |
| City | string | City |
| Country | string | Country |
| NumberOfPlatforms | int | Number of platforms (0-20) |
| Latitude | double? | Optional geographic coordinate |
| Longitude | double? | Optional geographic coordinate |

### TrainType
Defines a category of train with shared characteristics.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Type name |
| MaxSpeed | int | Max speed in km/h (1-500) |
| Capacity | int | Passenger capacity (1-2000) |
| Manufacturer | string | Manufacturer name |
| Type | TypeOfTrain | Enum: Passenger, Freight, HighSpeed, Commuter |

### Train
A physical train assigned to a type.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| SerialNumber | string | Unique identifier (max 50 chars) |
| TrainTypeId | int | FK → TrainType |

### Route
A named sequence of stations.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| Name | string | Route name |
| RouteStations | collection | Ordered list of stations on this route |

### RouteStation
Junction table linking routes to stations with timing data.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| RouteId | int | FK → Route |
| StationId | int | FK → Station |
| Order | int | Position in route (unique per route) |
| ArrivalOffsetMinutes | int | Minutes from trip departure to arrival at this station |
| StopDuration | int | Minutes the train stops at this station |

### Trip
A scheduled journey of a train along a route.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| TrainId | int | FK → Train |
| RouteId | int | FK → Route |
| DepartureTime | DateTime | Scheduled departure |
| ArrivalTime | DateTime | Scheduled arrival |
| ActualArrivalTime | DateTime? | Actual arrival time, set when trip is completed |

### Delay
Records a delay incident at a specific station during a trip.
| Field | Type | Description |
|-------|------|-------------|
| Id | int | Primary key |
| TripId | int | FK → Trip |
| StationId | int | FK → Station |
| DelayMinutes | int | Duration of delay in minutes |
| TypeOfDelay | enum | Weather, Technical, StationCongestion, TrackMaintenance, PassengerIncident, ExternalFactor, Other |
| CreatedAt | DateTime | When the delay was recorded |
| Note | string | Optional free-text description (max 250 chars) |

---

## 🌐 API Endpoints

### Authentication — `/api/auth`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register/admin` | None (first admin) / Admin token | Register an admin user |
| POST | `/api/auth/register/operator` | None | Register an operator user |
| POST | `/api/auth/login` | None | Login and receive JWT token |

### Stations — `/api/stations`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/stations` | None | Get all stations (paginated) |
| GET | `/api/stations/{id}` | None | Get station by ID |
| POST | `/api/stations` | Admin | Create a station |
| PUT | `/api/stations/{id}` | Admin | Update a station |
| DELETE | `/api/stations/{id}` | Admin | Delete a station |

### Train Types — `/api/train-types`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/train-types` | None | Get all train types |
| GET | `/api/train-types/{id}` | None | Get train type by ID |
| POST | `/api/train-types` | Admin | Create a train type |

### Trains — `/api/trains`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/trains` | None | Get all trains (paginated) |
| GET | `/api/trains/{id}` | None | Get train by ID |
| POST | `/api/trains` | Admin | Create a train |
| PUT | `/api/trains/{id}` | Admin | Update a train |
| DELETE | `/api/trains/{id}` | Admin | Delete a train |

### Routes — `/api/routes`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/routes` | None | Get all routes (paginated) |
| GET | `/api/routes/{id}` | None | Get route by ID |
| POST | `/api/routes` | Admin | Create a route with stations |
| PUT | `/api/routes/{id}` | Admin | Update a route |
| DELETE | `/api/routes/{id}` | Admin | Delete a route |

### Trips — `/api/trips`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/trips` | Admin / Operator | Create a trip |
| GET | `/api/trips/{id}` | None | Get trip by ID |
| GET | `/api/trips/{id}/position` | None | Get real-time position of a trip |
| PUT | `/api/trips/{id}/complete` | Admin / Operator | Mark trip as completed with actual arrival time |
| GET | `/api/trips/station/{stationId}` | None | Get all trips passing through a station |
| GET | `/api/trips/date?date={date}` | None | Get all trips on a specific date |
| GET | `/api/trips/station/{stationId}/schedule` | None | Get live arrival schedule for a station (delay-aware) |
| GET | `/api/trips/search` | None | Search trips with filters |

#### Trip Search Query Parameters
| Parameter | Type | Description |
|-----------|------|-------------|
| FromStationId | int? | Departure station |
| ToStationId | int? | Arrival station |
| Date | DateTime? | Filter by date (cannot be in the past) |
| MinDepartureTime | TimeSpan? | Earliest departure time |
| MaxDepartureTime | TimeSpan? | Latest departure time |
| Page | int | Page number (default: 1) |
| PageSize | int | Results per page (default: 10, max: 50) |

#### Trip Position Status Lifecycle
```
NotDeparted → AtStation / InTransit → WaitingForCompletion → Completed
```

### Delays — `/api/delays`
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/delays` | Admin / Operator | Record a new delay |
| GET | `/api/delays/{id}` | None | Get delay by ID |
| GET | `/api/delays/trip/{tripId}` | None | Get all delays for a trip |

---

## 🔐 Authentication

JWT Bearer token authentication with two roles:

- **Admin** — full access to all endpoints including create, update, delete
- **Operator** — can create trips and record delays

**First admin registration** is open (no token required) if no admins exist in the database. After the first admin is created, subsequent admin registrations require a valid Admin JWT token.

**Using the API:**
1. Register via `POST /api/auth/register/operator` or `POST /api/auth/register/admin`
2. Login via `POST /api/auth/login` to receive a JWT token
3. Include the token in requests: `Authorization: Bearer <token>`

---

## ⚠️ Error Handling

All errors return a consistent `ProblemDetails` response:

```json
{
  "title": "Request failed",
  "status": 400,
  "detail": "Train with id 5 does not exist",
  "instance": "/api/trains/5",
  "traceId": "80000006-0000-fd00-b63f-84710c7967bb"
}
```

| Status Code | Cause |
|-------------|-------|
| 400 | Validation failure or bad input |
| 401 | Missing or invalid JWT token |
| 403 | Valid token but insufficient role |
| 404 | Resource not found |
| 500 | Unexpected server error |

---

## ✅ Validation

FluentValidation is used for all input DTOs. Key rules include:

- **Station** — name/city/country required, max 100 chars; platforms 0-20
- **TrainType** — name required; max speed 1-500; capacity 1-2000; valid enum value
- **Train** — positive TrainTypeId; serial number required, max 50 chars
- **Route** — name required; at least one station; no duplicate orders or duplicate stations; offsets must increase; first station offset must be 0
- **Trip** — departure must be in the future; arrival must be after departure
- **Delay** — delay must be at least 1 minute; valid enum; note max 250 chars
- **Auth** — password minimum 8 chars with uppercase letter and number

---

## 🗺️ Station Schedule (Delay-Aware)

The `GET /api/trips/station/{stationId}/schedule` endpoint calculates real arrival times by:

1. Fetching all upcoming trips that pass through the station
2. Loading all delays recorded for those trips
3. Summing delays only from stations **at or before** the requested station in the route order
4. Adding total delay to the planned arrival time
5. Showing trains that departed up to 5 minutes ago

---

## 🚂 Real-Time Train Position

The `GET /api/trips/{id}/position` endpoint calculates where a train currently is based on:

- Time elapsed since departure
- `ArrivalOffsetMinutes` and `StopDuration` of each station on the route
- Total delay minutes recorded for the trip

Returns one of five statuses: `NotDeparted`, `AtStation`, `InTransit`, `WaitingForCompletion`, `Completed`

---

## 🧪 Testing

118 unit tests covering all service methods and validators using xUnit and SQLite in-memory database.

```
dotnet test
```

Test coverage includes:
- All CRUD operations for every service
- Exception scenarios (NotFoundException, BadRequestException)
- All validator rules including boundary conditions
- Trip position calculation for all status types

---

## 🐳 Docker

Run the full stack locally with Docker Compose:

```bash
# create .env file with your password
echo SA_PASSWORD=YourPassword123! > .env

# start API + SQL Server
docker-compose up --build

# stop
docker-compose down
```

API available at `http://localhost:8080`
SQL Server available at `localhost:1434` (connect with SSMS using SQL Server Authentication, login: sa)

---

## 🚀 Getting Started (Local Development)

### Prerequisites
- .NET 8 SDK
- SQL Server
- Visual Studio 2022 or VS Code

### Setup
1. Clone the repository
2. Update the connection string in `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=RailwayDB;Trusted_Connection=True;"
}
```
3. Apply migrations:
```
dotnet ef database update
```
4. Run the project:
```
dotnet run
```

The database seeder runs automatically on startup and populates the database with sample data including stations, trains, routes, trips and delays.

---

## ☁️ Deployment

The API is deployed to **Azure App Service** with **Azure SQL Database**.

Live URL: `https://railway-api-app.azurewebsites.net`

### CI/CD Pipeline (GitHub Actions)
Every push to `master`:
1. Builds the project
2. Runs all 118 tests
3. If tests pass — builds Docker image and pushes to Azure Container Registry
4. Restarts Azure App Service with the new image

---

## 📊 Python Analytics Module

A standalone Python module that connects directly to the SQL Server database and generates an HTML and PDF report analyzing:

- Delays by route — total and average delay minutes per route
- Delays by station — most problematic stations
- Delays by train — which trains are most delayed
- On-time percentage by route
- Most common delay reasons

### Setup
```bash
cd RailwayAnalytics
python -m venv venv
venv\Scripts\activate
pip install -r requirements.txt
python main.py
```

Report is generated in `RailwayAnalytics/output/`.
