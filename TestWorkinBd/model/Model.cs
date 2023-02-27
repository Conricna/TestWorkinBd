using Newtonsoft.Json;

public class JsonObject
{
    [JsonProperty("report")]
    public Media[] Media { get; set; }
}

public class Media
{
    [JsonProperty("Firstname")]
    public string Firstname { get; set; }

    [JsonProperty("Lastname")]
    public string Lastname { get; set; }

    [JsonProperty("Email")]
    public string Email { get; set; }

    [JsonProperty("Phone")]
    public string Phone { get; set; }

    [JsonProperty("BDay")]
    public string BDay { get; set; }

}

public class Json
{
    //static void Main(string[] args)
    public  string GreateJson(String firstname, String lastname, String email, String phone, DateTime dbirth)
    {
        var obj = new JsonObject
        {
            Media = new Media[]
            {
                new Media
                {
                    Firstname = firstname,
                    Lastname = lastname,
                    Email = email,
                    Phone =phone,
                    BDay= Convert.ToString(dbirth)
                }          

            }
            
        };

        var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        File.WriteAllText(@"..\..\..\myJson.json", json);
        
        return json;
    }
}
