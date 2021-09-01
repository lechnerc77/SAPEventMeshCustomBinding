# Azure Function Custom Output Binding - SAP Event Mesh

## Content

This project contains an implementation of a _custom output binding_ for Azure Functions. Target for the binding is the [SAP EVent Mesh](https://help.sap.com/viewer/product/SAP_EM/Cloud/)

## How to build

Build the project from scratch via

```powershell
dotnet clean
dotnet build
```

## How to run

In order to make the binding available to your Azure Function, you must execute the following steps:

1. Create an Azure Functions project e. g. via VSCode Azure Function extension with an HTTP triggered Azure Function project in it
2. Add a folder `extension` to your Azure Functions project.
3. Copy the `dll` file of the built extension into the folder.
4. Remove the `extensionbundle` section from the `host.json` file.
5. Initialize the Azure Functions project with a `extensions.csproj` file via `func extensions sync`
6. Add the dependencies to the `extensions.csproj` file namely:

  ```XML
  <PackageReference Include="Microsoft.Azure.WebJobs" Version="3.0.27" />
  <PackageReference Include="RestSharp" Version="106.12.0" />
  ```

7. Execute `func extensions install`. You can check if this was successful by checking the file `extension.json` in the `bin` directory. You should now have an entry for the Event Mesh binding in there.
8. Add the output binding to the function in the `function.json`

  ```json
  {
      "type": "EventMesh",
      "direction": "out",
      "name": "EventMeshMessage",
      "EventMeshTokenEndpoint": "%EventMeshTokenEndpoint%",
      "EventMeshClientId": "%EventMeshClientId%",
      "EventMeshClientSecret": "%EventMeshClientSecret%",
      "EventMeshGrantType": "%EventMeshGrantType%",
      "EventMeshRestBaseEndpoint": "%EventMeshRestBaseEndpoint%",
      "EventMeshQueueName": "%EventMeshQueueName%"
    }
  ```

9. Add the connection values for the event mesh into the `local settings.json` like:

  ```json
  {
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "node",
    "EventMeshTokenEndpoint": "<YOUR EVENT MESH TOKEN ENDPOINT>",
    "EventMeshClientId": "<YOUR EVENT MESH CLIENT ID>",
    "EventMeshClientSecret": "<YOUR EVENT MESH CLIENT SECRET>",
    "EventMeshGrantType": "client_credentials",
    "EventMeshRestBaseEndpoint": "<YOUR EVENT MESH REST ENDPOINT>",
    "EventMeshQueueName": "<YOUR EVENT MESH QUEUE NAME>"
    }
  }
  ```

Now you are set to run the function and put messages into a queue in the SAP Event Mesh.

You find a sample function in this GitHub repository: [link](https://github.com/lechnerc77/SAPEventMeshCustomBindingSampleFunction)
