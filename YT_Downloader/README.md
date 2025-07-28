# YouTube Downloader

A Blazor application for downloading videos from YouTube.

## Features
- Download videos in various formats
- Manage cookies for authentication
- User-friendly interface

## Prerequisites
- .NET 8.0 SDK or later
- Docker (for containerized deployment)

## Setup and Installation

1. Clone the repository:
   ```
   git clone https://github.com/fan92rus/YTDownloader.git
   ```

2. Navigate to the project directory:
   ```
   cd YT_Downloader
   ```

3. Restore dependencies:
   ```
   dotnet restore
   ```

4. Build the application:
   ```
   dotnet build
   ```

## Running the Application

### Local Development
1. Run the application:
   ```
   dotnet run --project YT_Downloader.csproj
   ```

2. Open your browser and navigate to `https://localhost:5001`

### Docker Deployment

#### Using Dockerfile
1. Build the Docker image:
   ```
   docker build -t ytdownloader .
   ```

2. Run the container with cookie folder mounted:
   ```
   docker run -d -p 8080:80 --name ytdownloader_container -v ./data:/app/data ytdownloader
   ```

3. Open your browser and navigate to `http://localhost:8080`

#### Using Docker Compose
1. Create a docker-compose.yml file with the following content:
   ```yaml
   version: '3.8'
   services:
     ytdownloader:
       build: .
       ports:
         - "8080:80"
       volumes:
         - ./data:/app/data
   ```

2. Run the application with docker-compose:
   ```
   docker-compose up -d
   ```

3. Open your browser and navigate to `http://localhost:8080`

## Usage

1. Navigate to the home page
2. Enter a YouTube video URL in the input field
3. Select your preferred download format
4. Click "Download" button
5. The video will be downloaded to your browser

## Cookie Management

The application provides a dedicated page for cookie management at `/cookie`. This page allows you to:

1. View currently stored cookies
2. Add new cookies manually
3. Delete existing cookies
4. Save your current session cookies (useful for YouTube authentication)

To access the cookie manager:
1. Navigate to `https://localhost:8080/cookie` in your browser
2. Use the provided interface to manage your cookies

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a new branch (`git checkout -b feature-branch`)
3. Make your changes
4. Commit your changes (`git commit -am 'Add some feature'`)
5. Push to the branch (`git push origin feature-branch`)
6. Create a new Pull Request

## License

This project is licensed under the MIT License.
