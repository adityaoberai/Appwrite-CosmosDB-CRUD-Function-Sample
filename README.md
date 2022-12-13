# Appwrite CosmosDB CRUD Function Sample

## 🤖 Documentation

Appwrite Function Sample to showcase CRUD functions for an Azure CosmosDB NoSQL database.

### Input Schema

```json
{
	"function": "string",
	"product": {
		"id": "string",
		"productCategory": "string",
		"productName": "string"
	}
}
```

#### Types of Inputs

The input can vary a little based on which CRUD function you would like to consume. The following table should help you out:

| `function` value | Mandatory `product` fields             | Nullable `product` fields |
|------------------|----------------------------------------|---------------------------|
| `create`         | `productCategory`, `productName`       | `id`                      |
| `read`           | `id`, `productCategory`                | `productName`             |
| `readall`        |                                        |                           |
| `update`         | `id`, `productCategory`, `productName` |                           |
| `delete`         | `id`, `productCategory`                | `productName`             |


### Output Schema

```json
{
	"response": "Read Product",
	"data": {
		"Id": "1a1d54bf-9523-460e-a78d-1d127af7be67",
		"ProductCategory": "Laptop",
		"ProductName": "ROG Zephyrus Duo 16"
	}
}
```

## 📝 Environment Variables

This cloud function needs the following environment variables

- `COSMOSDB_ENDPOINT`: Your Azure CosmosDB endpoint URI
- `COSMOSDB_KEY`: Your Azure CosmosDB primary key

ℹ️ _Note: In order to get your Azure CosmosDB endpoint and key, go ahead and avail a [free Microsoft Azure trial account](https://azure.microsoft.com/en-us/free/)._

## 🚀 Deployment

There are two ways of deploying the Appwrite function, both having the same results, but each using a different process. We highly recommend using CLI deployment to achieve the best experience.

### Using CLI

Make sure you have [Appwrite CLI](https://appwrite.io/docs/command-line#installation) installed, and you have successfully logged into your Appwrite server. To make sure Appwrite CLI is ready, you can use the command `appwrite client --debug` and it should respond with green text `✓ Success`.

Make sure you are in the same folder as your `appwrite.json` file and run `appwrite deploy function` to deploy your function. You will be prompted to select which functions you want to deploy.

### Manual using tar.gz

Manual deployment has no requirements and uses Appwrite Console to deploy the tag. First, enter the folder of your function. Then, create a tarball of the whole folder and gzip it. After creating `.tar.gz` file, visit Appwrite Console, click on the `Deploy Tag` button and switch to the `Manual` tab. There, set the `entrypoint` to `src/Index.cs`, and upload the file we just generated.
