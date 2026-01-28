# Running AnonForum in Docker

This project supports running via Docker Compose.

## Requirements

* Docker
* Docker Compose

## Running the Application

To start all services (database, gRPC server, and Blazor client), run the following command in the project's root directory:

```bash
docker-compose up -d --build
```

After a successful startup:
- **Client (Web UI)** will be available at: [http://localhost:5000](http://localhost:5000)
- **Server (gRPC)** runs inside the Docker network on port 8080 (forwarded as 5114 for external access)
- **MongoDB** is available on the standard port 27017

## Stopping

To stop and remove containers:

```bash
docker-compose down
```

## Configuration via Environment Variables

You can change settings via environment variables in `docker-compose.yml`:

- `ConnectionStrings__MongoDb` — MongoDB connection string.
- `ForumServer__Url` — gRPC server URL for the client.
