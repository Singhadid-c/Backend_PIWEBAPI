using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly DatabaseContext _databaseContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _logger = logger;
        }
        #region MSSQL database
        // CRUD
        // Create
        // https://localhost:5001/api/postdata
        [HttpPost("postdata")]
        public IActionResult PostData(TagValue result)
        {
            try
            {
                var results = _databaseContext.TagValue.Add(result);
                _databaseContext.SaveChanges();
                return Ok( new{result=result, message="sucess"});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        // Read
        // https://localhost:5001/api/getdata
        [HttpGet("getdata")]
        public IActionResult GetData()
        {
            try
            {
                var tagValueData = _databaseContext.TagValue.ToList();
                return Ok( new{result=tagValueData, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        // Update
        // https://localhost:5001/api/updatedata
        [HttpPut("updatedata")]
        public IActionResult UpdateData(TagValue result)
        {
            try
            {
                var results = _databaseContext.TagValue.SingleOrDefault(o => o.Tagname == result.Tagname);
                if(result != null)
                {
                    // update value
                    results.Value = result.Value;

                    _databaseContext.Update(results);
                    _databaseContext.SaveChanges();
                }
                return Ok( new{result=result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }

        // Delete
        // https://localhost:5001/api/deletedata
        [HttpDelete("deletedata/{tagname}")]
        public IActionResult DeleteData(string tagname)
        {
            try
            {
                var result = _databaseContext.TagValue.SingleOrDefault(o => o.Tagname == tagname);
                if(result != null)
                {
                    _databaseContext.Remove(result);
                    _databaseContext.SaveChanges();
                }
                return Ok( new{result=result, message="sucess"});
            }
            catch (Exception ex)
            {
                return NotFound( new{result=ex, message="fail"});
            }
        }
        
        #endregion
        
        #region PIWEBAPI
        [HttpGet("getTT01Value")]
        public async Task<IActionResult> getTT01Value()
        {
            try
            {
                HttpClientHandler clientHandler = ( new HttpClientHandler() { UseDefaultCredentials = true }); 
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                string TT01url = @"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwo9yQNHh16hG-S7Rr_C3XWQakkySLwDCEmqkqLidoGBaAMjAyLjQ0LjEyLjE0NlxHUk9VUDFfOFxGQUNUT1JZXEFSTTF8VFQtMDE/recorded?starttime=*-23d&endtime=*";

                HttpResponseMessage response = await client.GetAsync(TT01url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp", item["Timestamp"].Value<string>());
                        dataPair.Add("Value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok( new{ result = result, message = "success" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }

        [HttpGet("getTT02Value")]
        public async Task<IActionResult> getTT02Value()
        {
            try
            {
                HttpClientHandler clientHandler = ( new HttpClientHandler() { UseDefaultCredentials = true }); 
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }; // acess to https
                HttpClient client = new HttpClient(clientHandler);

                string TT01url = @"https://202.44.12.146/piwebapi/streams/F1AbEP9i6VrUz70i0bz0vbTQKhwo9yQNHh16hG-S7Rr_C3XWQDfo-Qn2I3EmEf1oNNLGiGAMjAyLjQ0LjEyLjE0NlxHUk9VUDFfOFxGQUNUT1JZXEFSTTF8VFQtMDI/recorded?starttime=*-24d&endtime=*";

                HttpResponseMessage response = await client.GetAsync(TT01url);

                string content = await response.Content.ReadAsStringAsync();

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();

                foreach(var item in data)
                {
                    if(item["Good"].Value<bool>() == true)
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp", item["Timestamp"].Value<string>());
                        dataPair.Add("Value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return Ok( new{ result = result, message = "success" });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new{result=ex, message="fail"});
            }
        }
        #endregion
    }
}
