# About the Cache Server Client

Use the Cache Server Client to upload and download files to any Unity Cache Server. The Cache Server Client is used to integrate the Unity Cache Server into processes that extend outside of the normal asset import pipeline - for example, to store and retrieve incremental artifacts of a build process.

# Installation

To install this package, follow the instructions in the [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html). 

# Usage
## API Examples
### Upload a file
```csharp
const string guidStr = "f7950ee725f9d47c7b90b02224b4534f";
const string  hashStr = "5082668810f105d565e2da3f8bf394ee";
var fileId = FileId.From(guidStr, hashStr);

var client = new Client("localhost", 8126);
client.Connect();

using(var stream = new FileStream())
{
    client.BeginTransaction(fileId);
    client.Upload(FileType.Asset, stream);
    client.EndTransaction();
}

client.Close();
```
### Download a file
```csharp
const string guidStr = "f7950ee725f9d47c7b90b02224b4534f";
const string  hashStr = "5082668810f105d565e2da3f8bf394ee";
var fileId = FileId.From(guidStr, hashStr);
var filePath = "/target/filename";

var client = new Client("localhost", 8126);
client.Connect();

// FileDownloadItem implements IDownloadItem
var downloadItem = new FileDownloadItem(fileId, FileType.Asset, filePath);
client.QueueDownload(downloadItem);

client.DownloadFinished += (object sender, DownloadFinishedEventArgs args) =>
{
    DownloadResult result = args.Result;
    long size = args.Size;
    long queueLength = args.DownloadQueueLength;
};

client.ResetDownloadFinishedEventHandler(); // cleanup
client.Close();
```
## Advanced

### IDownloadItem

Implement `IDownloadItem` to download vai WriteStream to a custom location.
## Utilities
### Upload All Assets
Quickly seed a local or remote cache server with the current project's imported assets.

1) From the Unity Editor toolbar, select `Assets -> Cache Server -> Upload All Assets`
2) Input the destination Cache Server. The currently configured global Unity Editor setting will be used by default.
3) Press Upload - for large projects, a progress dialog will display during the upload.

Or frome the Command Line:

`Unity -projectPath [projectPath] -ExecuteMethod Unity.CacheServer.CacheServerUploader.UploadAllFilesToCacheServer -batchmode -quit`

# Technical details
## Requirements

This version of the Cache Server Client is compatible with the following versions of the Unity Editor:

* 2017.1 and later (recommended)
* 5.6 and earlier may work but are untested

This Cache Server Client is compatible with the following versions of the Unity Cache Server:
* [v5.x](https://github.com/Unity-Technologies/unity-cache-server) and later (recommended)
* Other Cache Server versions shipped with Unity 5.x and later

## Document revision history
|Date|Reason|
|---|---|
|May 17, 2018|Added utility documentation.|
|May 15, 2018|Initial revision.|