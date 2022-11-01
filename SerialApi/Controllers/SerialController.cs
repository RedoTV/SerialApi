using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SerialApi.DatabaseContext;
using SerialApi.Models;
using System.Net;
using SerialApi.Services;
using Microsoft.AspNetCore.Authorization;
using SerialApi.FormModels;

namespace SerialApi.Controllers
{
    
    [ApiController]
    [Route("title")]
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
        
        [HttpGet("search/{id?}")]
        public IActionResult GetSerials(int? id)
        {
            if(id == null)
            {
                List<Serial> serials = SerialDbContext.Serials.ToList();
                return Ok(serials);
            }
            else
            {
                Serial serial = SerialDbContext.Serials.Find(id)!;
                if (serial == null) return NotFound();
                else
                {
                    return Ok(serial);
                }
            }
        }

        /// <summary>
        /// Добавить сериал
        /// </summary>
        /// <param name="formInfo">Информация о тайтле</param>
        [Authorize(Roles = "Admin")]
        [HttpPost("addtitle")]
        public async Task<IResult> AddSerial([FromForm]AddSerialForm formInfo)
        {
            SerialService serialService = new SerialService();
            await serialService.AddSerialTask(WebHostEnvironment , SerialDbContext , formInfo);
            return Results.Ok();
        }

        /// <summary>
        /// Изменить информацию о сериале
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpPut("changetitle")]
        public async Task<IResult> ChangeSerial([FromForm]ChangeSerialForm serialInfo)
        {
            IResult result = Results.NotFound();
            SerialService serialService = new SerialService();
            if (serialInfo.Id != 0)
            {
                await serialService.ChangeSerialTask(WebHostEnvironment, SerialDbContext, serialInfo);
                result = Results.Ok();
            }
            return result;
        }

        /// <summary>
        /// удалить сериал по id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("deletetitle/{id?}")]
        public async Task<IResult> DeleteSerial(int? id)
        {
            if(id != null)
            {
                Serial serial = SerialDbContext.Serials.Find(id)!;
                if (serial != null)
                {
                    string pathToPrieview = Path.Combine(WebHostEnvironment.WebRootPath, "posters", serial.Name + ".png");
                    if (System.IO.File.Exists(pathToPrieview))
                    {
                        System.IO.File.Delete(pathToPrieview);
                    }
                    SerialDbContext.Serials.Remove(serial);
                    await SerialDbContext.SaveChangesAsync();
                    return Results.Ok();
                }
                else
                {
                    return Results.NotFound();
                }
            }
            else
            {
                return Results.NotFound();
            }
        }
    }
}