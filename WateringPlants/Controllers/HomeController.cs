using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WateringPlants.Models;

namespace WateringPlants.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        ConcurrentDictionary<string, WaterDTO> plantsActivityDict = new ConcurrentDictionary<string, WaterDTO>();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("StartWatering")]
        public async Task<JsonResult> StartWatering(int activityToDo, string plantNumber, Dictionary<string, double> plantDict, int waterLevel = 0)
        {
            //start water=1 && stop Water=2
            WaterDTO waterDTO = new WaterDTO();
           // session["planActivitySession"] = new ConcurrentDictionary<string, WaterDTO>();

            double waterlevel = 0;

            plantDict.TryGetValue(plantNumber, out waterlevel);
            
            //Stopwatch stopwatch = new Stopwatch();

            if (plantDict.ContainsKey(plantNumber))
            {
                if (activityToDo == 1 && !plantsActivityDict.ContainsKey(plantNumber))
                {
                    plantsActivityDict.TryAdd(plantNumber, waterDTO);
                    // Begin timing
                    waterDTO.stopwatch = new Stopwatch();
                    waterDTO.stopwatch.Start();
                    
                    while (waterDTO.stopwatch.Elapsed < TimeSpan.FromSeconds(100))
                    {
                        if(!plantsActivityDict.ContainsKey(plantNumber))
                            waterDTO.stopwatch.Stop();
                    }

                    // Stop.s
                    waterDTO.stopwatch.Stop();
                    waterlevel = waterDTO.stopwatch.Elapsed.TotalSeconds;
                    plantsActivityDict.TryRemove(plantNumber,out waterDTO);
                }
                else if (activityToDo == 2)
                {
                    waterDTO.stopwatch.Stop();
                    waterlevel = waterDTO.stopwatch.Elapsed.TotalSeconds;
                    //if(waterlevel>10)
                    plantsActivityDict.TryRemove(plantNumber,out waterDTO);
                    
                }
               // waterDTO.WaterLevel = waterDTO.stopwatch.Elapsed.TotalSeconds;
               

                plantDict[plantNumber] = waterlevel;
            }
            var jsonResult =Json(plantDict);
            return  jsonResult;
        }
    }
}
