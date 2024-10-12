# Financial Management API

This project contains the source code for the Financial Management API, which is built using **C#** and the **.NET framework**. It provides secure and scalable services for managing financial transactions, budgets, accounts, and user data.

## Getting Started

To get started with the project, ensure that you have the following tools installed:

- **.NET SDK**
- **Visual Studio** or **Visual Studio Code**

### Installation

1. **Clone the repository**:
    ```bash
    git clone https://github.com/your-repository/financial-management-api.git
    ```
2. **Navigate to the project directory**:
    ```bash
    cd financial-management-api
    ```
3. **Install dependencies**:
    ```
    dotnet restore
    ```


## Building the Project

1. Open the solution (`FinancialManagementSystem.sln`) in **Visual Studio** or **Visual Studio Code**.
2. **Build the project** using the terminal or IDE:
    ```bash
    dotnet build FinancialManagementSystem.sln
    ```
   This command compiles the code and outputs the binary to the `/bin` directory.

## Running the Project

1. To **run the API**, use the following command:
    ```bash
    dotnet run --project src/FinancialManagementSystem.API
    ```
   This will start the API at `https://localhost:5001`.

2. Open the browser and navigate to the **Swagger UI** at:
https://localhost:5001/swagger

Here, you can interact with and test the API.

## Services and API Endpoints

### 1. **Authentication**
- **POST** `/api/Auth/login`: Authenticate with username and password to retrieve a JWT token.
- **POST** `/api/Auth/register`: Register a new user.

### 2. **Transaction Service**
- **GET** `/api/Transaction`: Get transactions for an authenticated user.
- **POST** `/api/Transaction`: Create a new transaction.

### 3. **Budget Service**
- **GET** `/api/Budget`: Retrieve budgets for an authenticated user.
- **POST** `/api/Budget`: Create a new budget.

### 4. **Income Service**
- **GET** `/api/Income`: Get income records for an authenticated user.
- **POST** `/api/Income`: Add new income records.

## Authentication with JWT
The API uses JWT-based authentication. All API requests (except login/register) require an authorization header with a valid JWT token:
```
Authorization: Bearer <your-token-here>
```
You can obtain this token by logging in using the /api/Auth/login endpoint and passing the token in the headers of subsequent requests.

## Middleware
The API uses the following middleware:

Authentication Middleware: Ensures that only authenticated users can access protected resources.
Global Exception Handling Middleware: Handles any unhandled exceptions and returns a consistent error response format.
CORS
The API is configured to allow CORS for frontend access with the following policy:


``
"AllowAll": {
  "AllowAnyOrigin": true,
  "AllowAnyMethod": true,
  "AllowAnyHeader": true
}
``

## Running Unit and Integration Tests
Navigate to the tests folder:
``cd tests``
Run the tests:
```dotnet test``
This will run all unit and integration tests in the solution.
API Documentation
The API documentation is automatically generated using Swagger. You can access the documentation at:

``https://localhost:5001/swagger``


## Contributing
We welcome contributions! Please fork this repository and submit pull requests to contribute.

Before submitting a pull request, please ensure:

Your code follows the existing coding conventions.
All unit and integration tests pass.


## License
This project is licensed under the MIT License. For more details, see the LICENSE.md file in the repository.

`` Nilufer Basgul :) ``
