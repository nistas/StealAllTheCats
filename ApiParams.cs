
namespace StealAllTheCats
{

    public class ApiParams
    {
        //public static IWebHostEnvironment WebEnviroment()
        //{
        //    var _accessor = new HttpContextAccessor();
        //    return _accessor.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        //}

        public static string CatApiKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ApiParams")["CatApiKey"]!;
        public static string CatApiBaseAddress = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ApiParams")["CatApiBaseAddress"]!;
        public static string SaveImageOn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ApiParams")["SaveImageOn"]!;
        public static string UseBase64Prefix = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ApiParams")["UseBase64Prefix"]!;
        public static string ImagesFolderPath = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ApiParams")["ImagesFolderPath"]!;
    }
}
