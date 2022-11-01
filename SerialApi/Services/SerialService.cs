using Newtonsoft.Json.Linq;
using SerialApi.DatabaseContext;
using SerialApi.FormModels;
using SerialApi.Models;
using System.Net;

namespace SerialApi.Services
{
    public class SerialService
    {
        
        public async Task AddSerialTask(IWebHostEnvironment WebHostEnvironment , SerialContext SerialDbContext , AddSerialForm formInfo)
        {
            string pathToWebRoot = WebHostEnvironment.WebRootPath;
            JObject jsonResp = new JObject();
            if (formInfo.Description == null)
            {
                jsonResp = await GetShikiInfoAsync(formInfo.ShikiId);
                formInfo.Description = jsonResp["description"]!.ToString();
            }
            if (formInfo.Poster == null)
            {
                jsonResp = await GetShikiInfoAsync(formInfo.ShikiId);
                string image = jsonResp["image"]!["original"]!.ToString();
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile($"https://shikimori.one{image}", Path.Combine(pathToWebRoot, "posters", $"{formInfo.Name}.png"));
                }
            }
            else if(formInfo.Poster!=null)
            {
                string fullPath = Path.Combine(pathToWebRoot, "posters", formInfo.Name + ".png");
                using (FileStream imgStream = new FileStream(fullPath, FileMode.Create))
                {
                    await formInfo.Poster!.CopyToAsync(imgStream);
                }
            }
            if (formInfo.Lenght == null)
            {
                jsonResp = await GetShikiInfoAsync(formInfo.ShikiId);
                formInfo.Lenght = Convert.ToInt32(jsonResp["episodes"]);
            }
            if (formInfo.Age == null)
            {
                jsonResp = await GetShikiInfoAsync(formInfo.ShikiId);
                string rating = jsonResp["rating"]!.ToString();
                int resultRating = 0;
                Dictionary<string, int> ratings = new Dictionary<string, int>()
                {
                    {"r_plus" , 18},
                    {"r" , 16 },
                    {"pg_13" , 13 },
                    {"pg", 6},
                    {"g", 0}
                };
                foreach (var item in ratings)
                {
                    if (item.Key == rating)
                    {
                        resultRating = item.Value;
                        break;
                    }
                }
                formInfo.Age = resultRating;

                bool resultStrict = false;
                Dictionary<string, bool> stricts = new Dictionary<string, bool>()
                {
                    {"r_plus" , true},
                    {"r" , false },
                    {"pg_13" , true },
                    {"pg", false},
                    {"g", false}
                };
                foreach (var item in stricts)
                {
                    if (item.Key == rating)
                    {
                        resultStrict = item.Value;
                        break;
                    }
                }
                formInfo.Strict = resultStrict;
            }

            Dictionary<string, string> kinds = new Dictionary<string, string>()
            {
                {"tv" , "serial"},
                {"movie" , "film" },
                {"ova" , "special" },
                {"ona", "serial"},
                {"special", "special"}
            };
            if (formInfo.Kind == null)
            {
                jsonResp = await GetShikiInfoAsync(formInfo.ShikiId);
                string kind = jsonResp["kind"]!.ToString();
                foreach (var item in kinds)
                {
                    if (item.Key == kind)
                    {
                        formInfo.Kind = item.Value;
                        break;
                    }
                }
            }
            string imgInRoot = String.Format("posters" + "/" + formInfo.Name + ".png");

            Serial serial = new Serial(formInfo.Name, formInfo.Description,formInfo.ShikiId,formInfo.Franchise, imgInRoot, formInfo.Lenght, formInfo.Age, formInfo.Strict ,formInfo.Kind);
            SerialDbContext!.Add(serial);
            SerialDbContext?.SaveChangesAsync();
        }


        private static async Task<JObject> GetShikiInfoAsync(int shikiId)
        {
            JObject jsonResp = new JObject();
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync($"https://shikimori.one/api/animes/{shikiId}");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            jsonResp = JObject.Parse(responseBody);
            return jsonResp;
        }
        public async Task ChangeSerialTask(IWebHostEnvironment WebHostEnvironment, SerialContext SerialDbContext, ChangeSerialForm serialInfo)
        {
            Serial serial = SerialDbContext.Serials.Find(serialInfo.Id)!;
            string oldPoster = serial.Poster;
            if (serialInfo.Name != null) serial.Name = serialInfo.Name;
            if (serialInfo.Franchise != null) serial.Franchise = serialInfo.Franchise;
            if (serialInfo.Description != null) serial.Description = serialInfo.Description;
            if (serialInfo.Poster != null)
            {
                if (System.IO.File.Exists(oldPoster))
                {
                    System.IO.File.Delete(oldPoster);
                }
                string fileName = serial.Name;
                string pathToWebRoot = WebHostEnvironment.WebRootPath;
                string fullPath = Path.Combine(pathToWebRoot, "posters", fileName + ".png");
                using (FileStream imgStream = new FileStream(fullPath, FileMode.Create))
                {
                    await serialInfo.Poster!.CopyToAsync(imgStream);
                }
                string imgInRoot = String.Format("posters" + "/" + fileName + ".png");
                serial.Poster = imgInRoot;
            }
            if (serialInfo.Age != null) serial.Age = serialInfo.Age;
            if (serialInfo.Strict != null) serial.Strict = serialInfo.Strict;
            if (serialInfo.Lenght != null) serial.Lenght = serialInfo.Lenght;
            if (serialInfo.Kind != null) serial.Kind = serialInfo.Kind;
            await SerialDbContext.SaveChangesAsync();
        }
    }
}
