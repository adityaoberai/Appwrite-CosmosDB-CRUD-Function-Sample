using Newtonsoft.Json;

public class FunctionRequest
{
    [JsonProperty(PropertyName = "function")]
    public string Function { get; set; }

    [JsonProperty(PropertyName = "product")]
    public Product Product { get; set; }
}

public class Product
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    
    [JsonProperty(PropertyName = "productCategory")]
    public string ProductCategory { get; set; }

    [JsonProperty(PropertyName = "productName")]
    public string ProductName { get; set; }
}