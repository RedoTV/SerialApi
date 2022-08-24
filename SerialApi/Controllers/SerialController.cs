using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SerialApi.DatabaseContext;
using SerialApi.Models;
using System.Net;

namespace SerialApi.Controllers
{
    [ApiController]
    [Route("api/serial")]
    public class SerialController : ControllerBase
    {
        SerialContext SerialDbContext;
        IWebHostEnvironment WebHostEnvironment;
        public SerialController(SerialContext serialContext, IWebHostEnvironment webHostEnvironment)
        {
            SerialDbContext = serialContext;
            WebHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Получить все сериалы
        /// </summary>
        [HttpGet("getall")]
        public IEnumerable<Serial> GetSerials()
        {
            List<Serial> serials = SerialDbContext.Serials.ToList();
            return serials;
        }

        /// <summary>
        /// Добавить сериал
        /// </summary>
        /// <param name="name">название сериала</param>
        /// <param name="shikiId">Id аниме на шикимори</param>
        /// <param name="description">описание сериала</param>
        /// <param name="poster">постер сериала</param>
        [HttpPost("addserial")]
        public async Task AddSerial(string name, string shikiId, string? description, IFormFile? poster)
        {
            string pathToWebRoot = WebHostEnvironment.WebRootPath;
            JObject jsonResp = new JObject();
            if (description == null || poster == null)
            {
                
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync($"https://shikimori.one/api/animes/{shikiId}");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                jsonResp = JObject.Parse(responseBody);
            }
            if (poster == null && description == null)
            {
                description = jsonResp["description"]!.ToString();
                string image = jsonResp["image"]!["original"]!.ToString();
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile($"https://shikimori.one{image}", Path.Combine(pathToWebRoot,"poster_img",$"{name}.jpg"));
                }
            }
            else if (poster == null)
            {
                description = jsonResp["description"]!.ToString();
                string image = jsonResp["image"]!["original"]!.ToString();
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile($"https://shikimori.one{image}", Path.Combine(pathToWebRoot, "poster_img", $"{name}.jpg"));
                }
            }
            else if (description == null)
            {
                description = jsonResp["description"]!.ToString();
            }
            else
            {
                string fullPath = Path.Combine(pathToWebRoot, "poster_img", name + ".png");
                using (FileStream imgStream = new FileStream(fullPath, FileMode.Create))
                {
                    await poster!.CopyToAsync(imgStream);
                }
            }
            string imgInRoot = String.Format("poster_img" + "/" + name + ".png");

            Serial serial = new Serial(name, description!, imgInRoot, shikiId);
            SerialDbContext.Add(serial);
            SerialDbContext?.SaveChangesAsync();
        }

        /// <summary>
        /// Изменить информацию о сериале
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="name">название сериала</param>
        /// <param name="description">описание сериала</param>
        /// <param name="poster">постер к сериалу</param>
        /// <returns></returns>
        [HttpPut("changeserial")]
        public async Task<IResult> ChangeSerial(int id, string? name, string? description, IFormFile? poster)
        {
            IResult result = Results.NotFound();
            if (SerialDbContext.Serials.Find(id) != null)
            {
                if (name != null || description != null || poster != null)
                {
                    Serial serial = SerialDbContext.Serials.Find(id)!;
                    if (name != null) serial.Name = name;
                    if (description != null) serial.Description = description;
                    if (poster != null && name == null)
                    {
                        string fileName = name == null ? fileName = serial.Name : fileName = name;
                        string pathToWebRoot = WebHostEnvironment.WebRootPath;
                        string fullPath = Path.Combine(pathToWebRoot, "poster_img", fileName + ".png");
                        using (FileStream imgStream = new FileStream(fullPath, FileMode.Create))
                        {
                            await poster!.CopyToAsync(imgStream);
                        }
                        string imgInRoot = String.Format("poster_img" + "/" + fileName + ".png");
                        serial.PathToPoster = imgInRoot;
                    }
                    await SerialDbContext.SaveChangesAsync();
                    result = Results.Accepted();
                }
            }
            return result;
        }

        /// <summary>
        /// удалить сериал по id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpDelete("deleteserial")]
        public async Task<IResult> DeleteSerial(int id)
        {
            if(SerialDbContext.Serials.Find(id) != null)
            {
                SerialDbContext.Serials.Remove(SerialDbContext.Serials.Find(id)!);
                await SerialDbContext.SaveChangesAsync();
                return Results.Accepted();
            }
            else
            {
                return Results.NotFound();
            }
        }
    }
}