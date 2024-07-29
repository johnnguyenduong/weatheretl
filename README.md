# Weather Data Gatherer

This application gathers weather forecast data from the Hong Kong Observatory Open Data API, processes it, and uploads it to Azure Blob Storage.

## Prerequisites

- .NET 8.0 SDK
- Docker
- Azure CLI
- Azure subscription
- Azure Kubernetes Service (AKS) cluster
- Azure Container Registry (ACR)
- Azure Storage Account

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/johnnguyenduong/weatheretl.git
   cd weatheretl
   cd WeatherETL
   ```

2. Edit the `appsettings.json` file to set:

   - Logging configuration
   - HK Observatory API URL
   - Azure Blob Storage connection string and container name

   Environment variables:
     - `AzureStorageConnection`: Connection string for Azure Blob Storage
     - `AzureEnabled`: set false if you dont want to upload csv file Azure Blob Storage

3. Build the Docker image:
   ```
   docker build -t weatheretl:latest .
   ```

4. Run the Docker image in local:
   ```
   docker run -t weatheretl:latest .
   ```

5. Push the image to your Azure Container Registry:
   ```
   az acr login --name your-acr-name
   docker tag weatheretl:latest your-acr-name.azurecr.io/weatheretl:latest
   docker push your-acr-name.azurecr.io/weatheretl:latest
   ```

## Running the Application

Run the following command in the project directory:

```
dotnet run
```

## Testing

Run unit tests with:

```
dotnet test
```

### CI/CD (not done yet)

A GitHub Actions workflow is provided in `.github/workflows/main.yml` for automated build, test, and deployment to AKS.


## Operation

The application runs as a scheduled job in AKS. It will automatically fetch weather data, process it, and upload it to Azure Blob Storage according to the schedule defined in the Kubernetes CronJob.

```
kubectl apply -f cronjob.yaml
```

## Monitoring

Use Azure Monitor and Azure Log Analytics to monitor the application's performance and logs.

## Troubleshooting

Find the CronJob's associated Job:
```
kubectl get jobs
```

Identify the most recent Job for your CronJob.

Find the Pod created by that Job:
```
kubectl get pods --selector=job-name=<job-name>
```

View the Pod's logs
```
kubectl logs <pod-name>
```

For more detailed logging, adjust the log level in `appsettings.json`.


## Logging

Logs are written to both console and file. The file log is configured in `appsettings.json`.

## Cron Job

A Kubernetes CronJob is defined in `cronjob.yaml` to run the application periodically.

### Further improve the application
 - Finish CI/CD
 - Adding more comprehensive unit tests and integration tests.
 - Implementing a caching mechanism to reduce API calls.
 - Adding a simple web interface to display the weather forecast.
 - Add validation when call api