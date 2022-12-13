using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;

public async Task<RuntimeResponse> Main(RuntimeRequest req, RuntimeResponse res)
{
  // Cosmos DB Client Authentication Details

  var cosmosDBEndpoint = req.Variables["COSMOSDB_ENDPOINT"];
  var cosmosDBKey = req.Variables["COSMOSDB_KEY"];

  if(String.IsNullOrEmpty(cosmosDBEndpoint) || String.IsNullOrEmpty(cosmosDBKey))
  {
    return res.Json(
      data: new()
      {
        { "response", "Cosmos DB credentials are missing" },
        { "data", null }
      }, 
      statusCode: 400
    );
  }

  // Database Details

  var databaseId = "ProductsDB";
  var containerId = "Products";
  var partitionKeyPath = "/productCategory";

  CosmosClient client = new CosmosClient(
    accountEndpoint: cosmosDBEndpoint, 
    authKeyOrResourceToken: cosmosDBKey
  );

  // Check If Database and Container Exist
  
  Container container;
  
  try
  {
    DatabaseResponse databaseResponse = await client.CreateDatabaseIfNotExistsAsync(databaseId);
    Database database = databaseResponse.Database;
    ContainerResponse containerResponse = await database.CreateContainerIfNotExistsAsync(
      id: containerId,
      partitionKeyPath: partitionKeyPath,
      throughput: 400
    );
    container = containerResponse.Container;
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.StackTrace);
    return res.Json(
      data: new()
      {
        { "response", ex.Message },
        { "data", "Check logs for stack trace" }
      }, 
      statusCode: 400
    );
  }

  // Deserialize Payload
  FunctionRequest functionRequest;
  try
  {
    functionRequest = JsonConvert.DeserializeObject<FunctionRequest>(req.Payload);
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.StackTrace);
    return res.Json(
      data: new()
      {
        { "response", ex.Message },
        { "data", "Check logs for stack trace" }
      }, 
      statusCode: 400
    );
  }
  var function = functionRequest.Function;
  var product = functionRequest.Product;

  // Cosmos DB Functions

  try
  {
    if(function.Equals("create"))
    {
      if(String.IsNullOrEmpty(product.Id))
      {
        product.Id = Guid.NewGuid().ToString();
      }
      var createdProduct = await CreateProduct(container, product);
      return res.Json(new()
      {
        { "response", "Created Product" },
        { "data", createdProduct }
      });
    }

    else if(function.Equals("read"))
    {
      var readProduct = await ReadProduct(container, product);
      return res.Json(new()
      {
        { "response", "Read Product" },
        { "data", readProduct }
      });
    }
    
    else if(function.Equals("readall"))
    {
      var readProducts = await ReadAllProducts(container);
      return res.Json(new()
      {
        { "response", "Read All Products" },
        { "data", readProducts }
      });
    }

    else if(function.Equals("update"))
    {
      var updatedProduct = await UpdateProduct(container, product);
      return res.Json(new()
      {
        { "response", "Updated Product" },
        { "data", updatedProduct }
      });
    }
    
    else if(function.Equals("delete"))
    {
      var deletedProduct = await DeleteProduct(container, product);
      return res.Json(new()
      {
        { "response", "Deleted Product" },
        { "data", deletedProduct }
      });
    }

    // Closing return statement

    else
    {
      return res.Json(
        data: new()
        {
          { "response", "Function value is invalid" },
          { "data", null }
        }, 
        statusCode: 400
      );
    }
  }
  catch(Exception ex)
  {
    Console.WriteLine(ex.StackTrace);
    return res.Json(
      data: new()
      {
        { "response", ex.Message },
        { "data", "Check logs for stack trace" }
      }, 
      statusCode: 500
    );
  }
}

public async Task<Product> CreateProduct(Container container, Product product)
{
  var createdProduct = await container.CreateItemAsync<Product>(
    item: product,
    partitionKey: new PartitionKey(product.ProductCategory)
  );

  return createdProduct.Resource;
}

public async Task<Product> ReadProduct(Container container, Product product)
{
  var readProduct = await container.ReadItemAsync<Product>(
    id: product.Id,
    partitionKey: new PartitionKey(product.ProductCategory)
  );

  return readProduct.Resource;
}

public async Task<List<Product>> ReadAllProducts(Container container)
{
  List<Product> products = new List<Product>();

  FeedIterator<Product> feed = container.GetItemQueryIterator<Product>(
    queryText: "SELECT * FROM Products"
  );

  while (feed.HasMoreResults)
  {
    FeedResponse<Product> response = await feed.ReadNextAsync();

    foreach (Product product in response)
    {
      products.Add(product);
    }
  }

  return products;
}

public async Task<Product> UpdateProduct(Container container, Product product)
{
  var updatedProduct = await container.UpsertItemAsync<Product>(
    item: product,
    partitionKey: new PartitionKey(product.ProductCategory)
  );

  return updatedProduct.Resource;
}

public async Task<Product> DeleteProduct(Container container, Product product)
{
  product = await ReadProduct(container, product);

  await container.DeleteItemAsync<Product>(
    id: product.Id,
    partitionKey: new PartitionKey(product.ProductCategory)
  );

  return product;
}
