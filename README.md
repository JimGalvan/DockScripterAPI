# DockScripter

DockScripter is a robust and scalable platform designed to manage and execute scripts within Docker containers. It leverages AWS S3 for storage and provides a seamless interface for script management and execution.

## Execution Process

DockScripter allows users to execute scripts in isolated Docker containers. Here is how we solved the problem:

1. **Script Upload**: Users upload their scripts to AWS S3. Each script is associated with a unique identifier and stored securely in the cloud.

2. **Temporary Storage**: When a script execution is requested, the script files are downloaded from S3 to a temporary directory on the server.

3. **Docker Execution**: The temporary directory is mounted as a volume in a Docker container. The main script file is executed within this isolated environment.

4. **Result Capture**: The output and status of the script execution are captured and stored in the database. This ensures that users can retrieve and review the results of their script executions.

---

## Features

- **Script Management**: Create, update, delete, and retrieve scripts.
- **File Management**: Upload and manage script files stored in AWS S3.
- **Execution Service**: Execute scripts within Docker containers and capture execution results.
- **Authentication**: Secure user authentication using JWT.

## Technologies Used

- **Languages**: C#
- **Frameworks**: .NET Core, ASP.NET Core, Entity Framework
- **Database**: SQLite
- **Cloud Services**: AWS S3
- **Containerization**: Docker

## Project Structure

- **Services**: Contains business logic for script and execution management.
- **Repositories**: Data access layer for interacting with the database.
- **Domain**: Contains entities, DTOs, and enums used across the application.

> **Note to Recruiters**: This project demonstrates proficiency in C#, .NET Core, cloud services (AWS), and containerization (Docker). It showcases the ability to build scalable and secure applications with modern technologies.

