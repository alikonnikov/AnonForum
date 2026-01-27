# Anonymous Forum

A simple anonymous forum application built with .NET 8, Blazor, gRPC, and MongoDB.

## Features

- **Anonymous Interaction:** No registration or login required.
- **Topics:** Create new discussion topics with a title and description.
- **Search:** Search for topics by title.
- **Messages:** Post and read messages within specific topics.
- **Real-time communication:** Uses gRPC for efficient communication between the client and server.
- **Persistence:** All data is stored in a MongoDB database.

## Technologies Used

- **Frontend:** Blazor Interactive Server (C#)
- **Communication:** gRPC
- **Backend:** ASP.NET Core gRPC Service
- **Database:** MongoDB
- **Styling:** Bootstrap

## Project Structure

- **Contracts:** Contains the `.proto` files defining the gRPC service and message structures.
- **Server:** The ASP.NET Core backend that implements the gRPC service and interacts with MongoDB.
- **Client:** The Blazor Server application providing the user interface.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [MongoDB](https://www.mongodb.com/try/download/community) running locally on port `27017`.

## Getting Started

1. **Clone the repository.**
2. **Start MongoDB:** Ensure your MongoDB instance is running at `localhost:27017`.
3. **Run the Server:**
   ```bash
   cd Server
   dotnet run
   ```
   The server runs on `http://localhost:5114`.
4. **Run the Client:**
   ```bash
   cd Client
   dotnet run
   ```
   The client will be available at the URL provided in the console (usually `http://localhost:5000` or similar).

## Data Models

- **Topic:** `Id`, `Title`, `Description`, `CreatedAt`
- **Message:** `Id`, `TopicId`, `Content`, `CreatedAt`

## API (gRPC)

The `Forum` service provides the following methods:
- `GetTopics`: Retrieve a list of topics (with optional search filter).
- `GetTopic`: Get details of a single topic by ID.
- `CreateTopic`: Create a new topic.
- `GetMessages`: Retrieve all messages for a specific topic.
- `SendMessage`: Post a new message to a topic.
