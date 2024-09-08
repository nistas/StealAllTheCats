using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StealAllTheCats.Data;
using StealAllTheCats.Entities;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Web;
using static System.Net.Mime.MediaTypeNames;

namespace StealAllTheCats.Controllers
{
    

    [Route("api/[controller]")]
    [ApiController]
    public class CatsController : ControllerBase
    {

        //private readonly IConfiguration _configuration;
        //public CatsController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}

        private readonly DataContext _context;
        public CatsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Cat>>> GetAllCats()
        {
            var miaou =  _context.Cats.ToList();
            if (miaou is null)
            {
                return Ok("No Cats Found in the Database. Please try to the Fetch endpoint to fill some.");
            }


            return Ok(miaou);
        }

        [HttpGet]
        [Route("tags")]
        public async Task<ActionResult<List<Tag>>> GetAllTags()
        {
            var miaou =  _context.Tags.ToList();
            if (miaou is null)
            {
                return Ok("No Tags Found in the Database. Please try to the Fetch new Cats and maybe some of them are available.");
            }


            return Ok(miaou);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<Cat>>> GetSpecificCat([FromRoute]int id)
        {
            var miaou = await _context.Cats.FindAsync(id);
            if(miaou is null)
                return NotFound("Cat with Id:" + id + " not found. Make sure that there are stored Cats in the Database first, and then try another Id."); // status 404




            //var mycats = (from c in _context.Cats
            //              join t in _context.Tags
            //              on c.Id == ct1.Ca
            //                        join t in _context.Tags
            //                        on t
            //                        equals albumList.ArtistId
            //              select new
            //              {
            //                  Album = albumList,
            //                  Artist = artistList
            //              })
            //           .ToList();

            return Ok(miaou);
        }

        [HttpGet]
        //[Route("{page:int}/{pageSize:int}")]
        [Route("page={page:int}&pageSize={pageSize:int}")] // because the assigment asking this style
        public async Task<ActionResult<List<Cat>>> GetCats([FromRoute] int page, [FromRoute] int pageSize)
        {
            if (page == 0 || pageSize== 0)
            {
                return BadRequest("Page and/or pageSize parameters must be greater than zero");
            }


            int SkipRecs = 0;
            if (page > 1)
            {
                SkipRecs = (page * pageSize)- pageSize;
            }
            
            var miaou = _context.Cats.Skip(SkipRecs).Take(pageSize);
            if (miaou is null || miaou.Count()==0)
            {
                return Ok("No Cats Found in the Database. Please try to the Fetch endpoint to fill some.");
            }
            
            return Ok(miaou);
        }

        [HttpGet]
        //[Route("{tag}/{page:int}/{pageSize:int}")]
        [Route("tag={tag}&page={page:int}&pageSize={pageSize:int}")] // because the assigment asking this style
        public async Task<ActionResult<List<Cat>>> GetCatsOverTag([FromRoute] string tag, [FromRoute] int page, [FromRoute] int pageSize)
        {
            if (string.IsNullOrEmpty(tag) || page == 0 || pageSize == 0)
            {
                return BadRequest("Tag must have a value and Page and/or pageSize parameters must be greater than zero");
            }


            int SkipRecs = 0;
            if (page > 1)
            {
                SkipRecs = (page * pageSize) - pageSize;
            }
            var miaou = _context.Cats.Skip(SkipRecs).Take(pageSize);
            if (miaou is null || miaou.Count() == 0)
            {
                return Ok("No Cats Found in the Database. Please try to the Fetch endpoint to fill some.");
            }
            return Ok(miaou);
        }

        [HttpPost]
        [Route("fetch")]
        public async Task<ActionResult> FetchNewCats()
        {
            try
            {
                int NumOCatsToRequest = 25;
                dynamic newCats = FetchFromTheCatsApi(25);
                if (newCats.ApiMessage.ToString().ToLower() != "#ok#")
                {
                    return Ok(newCats.ApiMessage);
                }
                else
                {
                    List<Cat> LCats = ClearDuplicateCats((List<Cat>)newCats.ListOfCats);
                    if (LCats.Count > 0)
                    {
                        _context.Cats.AddRange(LCats); // performance to make one call
                        await _context.SaveChangesAsync();
                    }

                    string msg = string.Empty; 

                    if (NumOCatsToRequest != LCats.Count)
                    {
                        msg = (NumOCatsToRequest- LCats.Count) +" of them already exist and not stored.";
                    }
                    
                    return Ok("You have requested to fetch " + NumOCatsToRequest  + " cats from TheCatApi. Finally " + LCats.Count + " Cat(s) stored in the Database. " + msg);

                }
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            finally
            { }


        }

        [HttpPost]
        [Route("fetch/{NumOfCats:int}")]
        public async Task<ActionResult> FetchNewCats([FromRoute] int NumOfCats)
        {
            try
            {
                if (NumOfCats <= 0)
                {
                    return Ok("Please define the number of cats you want to fetch. Min: 1");
                }
                
                    dynamic newCats = FetchFromTheCatsApi(NumOfCats);
                    if (newCats.ApiMessage.ToString().ToLower() != "#ok#")
                    {
                        return Ok(newCats.ApiMessage);
                    }
                    else
                    {
                        List<Cat> LCats = ClearDuplicateCats((List<Cat>)newCats.ListOfCats);
                    if (LCats.Count > 0)
                    {
                       
                        foreach (Cat cat in LCats)
                        {
                            string catimage = SaveImage(cat.CaaSCatId, cat.Image);
                            if (!String.IsNullOrEmpty(catimage)) // in any other case i hold the CaaSApi url
                            {
                                cat.Image = catimage; 
                            }
                        }

                        _context.Cats.AddRange(LCats); // performance to make one call
                        await _context.SaveChangesAsync();

                    }
                    string msg = string.Empty;

                    if (NumOfCats != LCats.Count)
                    {
                        msg = (NumOfCats - LCats.Count) + " of them already exist and not stored.";
                    }

                    return Ok("You have requested to fetch " + NumOfCats + " cats from TheCatApi. Finally " + LCats.Count + " Cat(s) stored in the Database. " + msg);


                    }
                
                
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            finally
            {

                
            }


        }

        private object GetUriDetails(int NumOfCats)
        {
            dynamic o = new ExpandoObject();
            string BaseAddress = string.Empty;
            string Error = string.Empty;
            //builder.Configuration.GetConnectionString("DbConnection")
            if (String.IsNullOrEmpty(ApiParams.CatApiBaseAddress))
            {
                Error += "CatApiBaseAddress not found. Please check configuration";
            }
            else
            {
                BaseAddress += String.Format(ApiParams.CatApiBaseAddress, NumOfCats);
            }

            if (String.IsNullOrEmpty(ApiParams.CatApiKey))
            {
                Error += "CatApiKey not found. Please check configuration";
            }
            else
            {
                BaseAddress +="&api_key="+ ApiParams.CatApiKey;
            }
            o.Error = Error;
            o.BaseAddress = BaseAddress;
            return o;
            
        }

        private object FetchFromTheCatsApi(int NumOfcats)
        {
            dynamic Result = new ExpandoObject();
            List<Cat> LCats = new List<Cat>();
            string ApiMessage = string.Empty;
            dynamic o = GetUriDetails(NumOfcats);
            if (!String.IsNullOrEmpty(o.Error))
            {
                ApiMessage = o.Error;
            }
            else
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(o.BaseAddress);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(client.BaseAddress).Result;
                if (response.IsSuccessStatusCode == true)
                {
                    string ApiResponse = response.Content.ReadAsStringAsync().Result;
                    List<dynamic> catO = JsonConvert.DeserializeObject<List<dynamic>>(ApiResponse)!;

                    foreach (dynamic item in catO)
                    {
                        Cat a = new Cat();
                        a.CaaSCatId = item.id;
                        a.Width = item.width;
                        a.Height = item.height;
                        a.Image = item.url;
                        a.Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
                        string JsonBreeds = item.breeds.ToString();
                        List<dynamic> catBreeds = JsonConvert.DeserializeObject<List<dynamic>>(JsonBreeds)!;
                        foreach (dynamic itemBread in catBreeds)
                        {
                            string[] catTags = itemBread.temperament.ToString().Split(',');
                            dynamic examinetags = ClearTags(catTags);
                            //Note:
                            //Scenario 1. we have tags that are totally new and must be store to the tags table and make the connection with the cat table
                            //Scenario 2. we have tags that are stored and must be asigned without re-store them in tha table tags and we must only apply connection with the cat table

                            foreach (string TagName in examinetags.newTags)
                            {
                                a.Tags.Add(new Tag
                                {
                                    Name = TagName,
                                    Created = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
                                });
                            }

                            
                        }

                            LCats.Add(a);
                    }     
                    ApiMessage = "#ok#";
                }
                else
                {
                    ApiMessage = "Error. Please check the Uri";
                }
            }

            Result.ListOfCats = LCats;
            Result.ApiMessage = ApiMessage;
            return Result;
        }

        private  List<Cat> ClearDuplicateCats(List<Cat> NewFechedCats)
        {
            List<Cat> UniqueCats = new List<Cat>();
            foreach (Cat c in NewFechedCats)
            {
                var miaou = _context.Cats.Where(p => p.CaaSCatId == c.CaaSCatId).ToList();
                if (miaou.Count==0)//means does not exist in the database
                {
                    //List<Tag> tags = ClearDuplicateTags(c.Tags);
                    UniqueCats.Add(c);

                }
            }

            return UniqueCats; // only cats must be added
        }

        private object ClearTags(string[] catTags)
        {
    
            List<string> newTags = new List<string>();
            List<string> existingTags = new List<string>();
            foreach (string t in catTags) // check if exists in the db
            {
                string TagName = FixTagName(t);
                var existintTag = _context.Tags.Where(p => p.Name == TagName).ToList();
                if (existintTag.Count == 0)//means does not exist in the database
                {
                    newTags.Add(TagName);
                }
                else {
                    existingTags.Add(TagName);
                }
            }
            dynamic o = new ExpandoObject();
            o.newTags = newTags;
            o.existingTags = existingTags;
            return o;
        }


        private string FixTagName(string name)
        {
            string term = " ";
            string replace = "";
            if (name.Length > 0)
            {
                if (name.StartsWith(term))
                {
                    int position = name.IndexOf(term);
                    name = name.Substring(0, position) + replace + name.Substring(position + term.Length);
                    return name;
                }
                else {
                    return name;
                }
            }
            return name;
        }


        private string SaveImage(string CaaSCatId, string url)
        {
            string ImageText = string.Empty;
            
            if (!string.IsNullOrEmpty(url))
            {
                var fileExtension = System.IO.Path.GetExtension(url);
                switch (ApiParams.SaveImageOn.ToLower())
                {
                    case "filesystem":
                        using (var webClient = new WebClient())
                        {
                            
                            Uri imageurl = new Uri(url);
                            string myFilename = CaaSCatId + fileExtension;
                            string savepath = Path.Combine(ApiParams.ImagesFolderPath, myFilename);

                            webClient.DownloadFile(imageurl, savepath);
                            ImageText = savepath;
                        }
                        break;
                    case "db":
                        using (var webClient = new WebClient())
                        {
                            byte[] imageBytes = webClient.DownloadData(url);
                            
                            if (!String.IsNullOrEmpty(ApiParams.UseBase64Prefix))
                            {
                                try
                                {
                                    if (Convert.ToBoolean(ApiParams.UseBase64Prefix))
                                    {
                                        ImageText = Base64PrefixForUrl(fileExtension) + Convert.ToBase64String(imageBytes);
                                    }
                                    else
                                    {
                                        ImageText = Convert.ToBase64String(imageBytes);
                                    }
                                }
                                catch (Exception)
                                {

                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return ImageText;
        }


        private string Base64PrefixForUrl(string FileExtention)
        {
            string checkVal = FileExtention.Replace(".", "").ToLower();
            string a = "data:";
            if ((checkVal.ToLower() == "jpg") ||
                (checkVal.ToLower() == "jpeg") ||
                (checkVal.ToLower() == "png") ||
                (checkVal.ToLower() == "gif") ||
                (checkVal.ToLower() == "svg") ||
                (checkVal.ToLower() == "webp") ||
                (checkVal.ToLower() == "eps"))
            {
                return a + "image/" + FileExtention + ";base64,";
            }
            else
            {
                return string.Empty;
          
            }
        }

    }
}

