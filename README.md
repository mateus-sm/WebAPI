[🇧🇷 Português](README.pt-BR.md)
# WebAPI - Cities and Students Management

This is an API developed in C# with ASP.NET Core for managing cities and students. The system allows bulk importing of all Brazilian cities (2026) from a CSV file into a MySQL database, CRUD operations for cities, and student management with support for photo upload and Base64 conversion. The entire infrastructure is orchestrated via Docker.

Developed to fulfill the assignment proposed by the Programming Languages course - 1st Term, 2026.

## 🚀 Technologies Used

* **C# / ASP.NET Core:** Main Web API framework.
* **MySQL:** Relational database for persisting cities and students.
* **Docker:** Containerization and orchestration of the API and the Database.
* **Swagger/OpenAPI:** Interactive documentation and endpoint testing.

## ⚙️ Features

The project was divided into three main parts:

### Part 1: City Management and Import
* Import city data via `.csv` file upload directly into the database.
* Batch processing support (`usarLote` parameter) for large files.
* CRUD operations (Create, Read, Update, Delete) for cities.
* Filters and aggregations: total city count, state listing, and city filtering by specific state.

### Part 2: Student Management (Challenge)
* Registration and querying of student entities linked to cities.
* Upload endpoint to receive image files (student photo) via `multipart/form-data`.
* Viewing endpoint that returns the stored student photo encoded as a Base64 string.

### Part 3: Documentation
* The API implements the OpenAPI standard, providing the Swagger interface for querying and testing all contracts and routes.

## 🛣️ API Endpoints

Below is a summary of the routes exposed by the application.

### 🏙️ Cities

| Method | Route | Description |
| :--- | :--- | :--- |
| `POST` | `/importar` | Imports cities from a `.csv` file (supports `usarLote=true`). |
| `GET` | `/api/cidades` | Returns all registered cities. |
| `POST` | `/api/cidades` | Manually inserts a new city. |
| `GET` | `/api/cidades/{id}` | Returns details of a specific city by ID. |
| `PUT` | `/api/cidades/{id}` | Updates data of an existing city. |
| `DELETE` | `/api/cidades/{id}` | Deletes a city by ID. |
| `GET` | `/total` | Returns the total count of registered cities. |
| `GET` | `/estados` | Returns the list of all registered states (UFs). |
| `GET` | `/estado/{uf}` | Returns a list of cities filtered by state abbreviation. |

### 🧑‍🎓 Students

| Method | Route | Description |
| :--- | :--- | :--- |
| `POST` | `/Alunos` | Creates a new student record. |
| `GET` | `/Alunos/{id}` | Retrieves a student record by ID. |
| `POST` | `/Alunos/{id}/foto` | Receives and stores the student's photo via form-data. |
| `GET` | `/Alunos/{id}/foto` | Returns the student's photo encoded in Base64 format. |

## 📦 Expected CSV File Structure

**Note:** A sample file (`cidade.csv`) is located at the root of this project for import testing.

For the `/importar` endpoint, the `.csv` file must contain the following column structure (with header row):

1.  **CidadeId**: Unique city identifier (Int).
2.  **Nome**: City name (String).
3.  **Sigla**: State abbreviation / UF (String).
4.  **IBGEMunicipio**: IBGE city code (Int).
5.  **Latitude**: City latitude (Double/Decimal).
6.  **Longitude**: City longitude (Double/Decimal).

## 🛠️ How to Run the Project

1. **Clone the repository:**
   ```bash
   git clone https://github.com/mateus-sm/WebAPI.git
   ```

The entire project infrastructure, including MySQL table initialization and creation, is automated with Docker.

2. **Start the containers:**  
   Make sure you have Docker Desktop (or Docker Engine) running on your machine.  
   In the terminal, at the project root (where the `docker-compose.yml` file is located), run:
   ```bash
   docker-compose up -d --build
   ```

   Docker will download the MySQL image, create the `cidades_db` database via `init.sql`, and start the services.

3. **Access the API:**  
   With the containers running, start the C# project via Visual Studio or your preferred method.  
   Open the browser and navigate to the URL mapped to the `/doc` route.  
   Terminal output when running the project:
   ```bash
   info: Microsoft.Hosting.Lifetime[14]
        Now listening on: http://localhost:5182
   info: Microsoft.Hosting.Lifetime[0]
        Application started. Press Ctrl+C to shut down.
   info: Microsoft.Hosting.Lifetime[0]
        Hosting environment: Development
   info: Microsoft.Hosting.Lifetime[0]
        Content root path: C:\WebAPI
   ```

   In the example above, access would be at: http://localhost:5182/doc
