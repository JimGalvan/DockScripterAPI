# DockScripter

DockScripter is a robust and scalable platform designed to manage and execute scripts within Docker containers. It leverages AWS S3 for secure file storage and provides a seamless interface for script management and execution.

## Execution Process

DockScripter allows users to execute scripts in isolated Docker containers. Here’s an overview of the execution workflow:

1. **Script Upload**: Users upload their scripts to AWS S3, ensuring scalable and secure storage. Each script file is associated with a unique identifier.

2. **Temporary Storage**: When a script execution is requested, the relevant script files are downloaded from S3 to a secure temporary directory on the server.

3. **Docker Execution**: The temporary directory is mounted as a volume in a Docker container, isolating execution. The main script file is then executed within this environment.

4. **Result Capture**: Output and status of the script execution are captured and stored in the database, enabling users to review the results of past executions.

---

## Features

- **Script Management**: Create, update, delete, and retrieve scripts.
- **File Management**: Upload and manage script files stored in AWS S3.
- **Execution Service**: Execute scripts within Docker containers with detailed result tracking.
- **Authentication**: Secure user authentication using JWT.

## Technologies Used

- **Languages**: C#
- **Frameworks**: .NET Core, ASP.NET Core, Entity Framework Core
- **Database**: SQLite (configured for easy setup, can be switched to other databases in production)
- **Cloud Services**: AWS S3 for secure file storage
- **Containerization**: Docker for isolated and scalable script execution

---

## Installation and Setup
(Will provide)
## Project Structure

- **Controllers**: Define API endpoints for managing scripts, files, and executions.
- **Services**: Contains business logic for script, file, and execution management.
- **Repositories**: Data access layer for interacting with the database.
- **Domain**: Contains entities, DTOs, and enums used across the application.

---

## API Endpoints

### Authentication
- **Register**: `/api/v1/auth/register`
- **Login**: `/api/v1/auth/login`

### Scripts
- **Create Script**: `/api/v1/script`
- **Upload File**: `/api/v1/script/{scriptId}/upload`
- **Execute Script**: `/api/v1/script/execute/{scriptId}`
- **Get Execution Results**: `/api/v1/script/results/{scriptId}`

---

## Future Improvements

- **Support for Multiple Script Types**: Expand execution to support additional languages (e.g., Node.js, Ruby).
- **Enhanced Error Handling**: Improve error handling to provide more detailed feedback on execution errors.
- **Scalability Enhancements**: Use a distributed task queue (e.g., AWS SQS) to handle script execution requests at scale.
- **UI Interface**: Develop a front-end interface for easier script and file management.

> **Note to Recruiters**: This project demonstrates proficiency in C#, .NET Core, cloud services (AWS), and containerization (Docker). It showcases the ability to build scalable, secure, and cloud-integrated applications with modern technologies.

