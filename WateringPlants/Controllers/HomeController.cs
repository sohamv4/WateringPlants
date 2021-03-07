using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WateringPlants.Models;

namespace WateringPlants.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

      private static  ConcurrentDictionary<string, WaterDTO> plantsActivityDict;
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
            //check in dict if plantActiveDict contains task and activityTodo is start or stop 
            //if task exists and is new process then create new task with new cancellation token and add into concurrentBag
            //if task exists and is not new process then cancel task
            double waterlevel = 0;
            //if (plantDict.ContainsKey(plantNumber))
            //{
            if (plantsActivityDict == null)
            {
                plantsActivityDict = new ConcurrentDictionary<string, WaterDTO>();
            }
           if (activityToDo == 1 && !plantsActivityDict.ContainsKey(plantNumber))
            {
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                //start water=1 && stop Water=2
                WaterDTO waterDTO = new WaterDTO();
                // ViewData["planActivitySession"] = plantsActivityDict;
                waterDTO.tokenSource = tokenSource;
                plantsActivityDict.TryAdd(plantNumber, waterDTO);

               
                Task t;
                var tasks = new ConcurrentBag<Task>();

                t = Task.Run(() => DoRunTask(activityToDo, plantNumber, plantDict, waterDTO,token, waterLevel), token);
                tasks.Add(t);

            }
            //  }
            else if (activityToDo == 2)
            {
               // WaterDTO waterDTO = new WaterDTO();
                //waterDTO.stopwatch.Stop();
                //waterlevel = waterDTO.stopwatch.Elapsed.TotalSeconds;
                //if(waterlevel>10)
                var waterObj = plantsActivityDict[plantNumber];
                waterObj.tokenSource.Cancel();
                waterObj.stopwatch.Stop();
                var timeElapsed=waterObj.stopwatch.Elapsed;
                waterlevel = timeElapsed.TotalSeconds;
                plantsActivityDict.Remove(plantNumber, out waterObj);
                ViewData["planActivitySession"] = plantsActivityDict;
              
            }

           

           // plantDict.TryGetValue(plantNumber, out waterlevel);

            //Stopwatch stopwatch = new Stopwatch();


            //if (plantDict.ContainsKey(plantNumber))
            //{
            //    if (activityToDo == 1 && !plantsActivityDict.ContainsKey(plantNumber))
            //    {

            //        plantsActivityDict.TryAdd(plantNumber, waterDTO);


            //        // Begin timing
            //        waterDTO.stopwatch = new Stopwatch();
            //        waterDTO.stopwatch.Start();
            //        //  var timer = new System.Timers.Timer(10);
            //        // timer.Elapsed+=new ElapsedEventHandler()


            //        while (waterDTO.stopwatch.Elapsed < TimeSpan.FromSeconds(100))
            //        {
            //            if (!plantsActivityDict.ContainsKey(plantNumber))
            //            {
            //                var waterObj = plantsActivityDict[plantNumber];
            //                waterObj.tokenSource.Cancel();
            //                waterDTO.stopwatch.Stop();
            //            }
            //        }

            //        // Stop.s
            //        waterDTO.stopwatch.Stop();
            //        waterlevel = waterDTO.stopwatch.Elapsed.TotalSeconds;

            //    }
                //else if (activityToDo == 2)
                //{
                //    //waterDTO.stopwatch.Stop();
                //    //waterlevel = waterDTO.stopwatch.Elapsed.TotalSeconds;
                //    //if(waterlevel>10)
                //    var waterObj = plantsActivityDict[plantNumber];
                //    waterObj.tokenSource.Cancel();
                //    plantsActivityDict.TryRemove(plantNumber, out waterDTO);
                //    ViewData["planActivitySession"] = plantsActivityDict;


                //}
                // waterDTO.WaterLevel = waterDTO.stopwatch.Elapsed.TotalSeconds;


                plantDict[plantNumber] = waterlevel;
            
            var jsonResult = Json(plantDict);
            return jsonResult;
        }
        public async void CancelTask(CancellationTokenSource ct)
        {
            if (ct.IsCancellationRequested)
            {
                ct.Cancel();
            }
        }
        public async Task DoRunTask(int activityToDo, string plantNumber, Dictionary<string, double> plantDict, WaterDTO waterDTO, CancellationToken ct,int waterLevel = 0)
        {
             
            // Begin timing
            waterDTO.stopwatch = new Stopwatch();
            waterDTO.stopwatch.Start();
            //  var timer = new System.Timers.Timer(10); 
            // timer.Elapsed+=new ElapsedEventHandler()

            //  for (var i = TimeSpan.Zero; i < TimeSpan.FromSeconds(100); i++)
            while (waterDTO.stopwatch.Elapsed < TimeSpan.FromSeconds(100))
            {
                if (plantsActivityDict.ContainsKey(plantNumber))
                {
                    var waterObj = plantsActivityDict[plantNumber];
                    waterObj.tokenSource.Cancel();
                    waterDTO.stopwatch.Stop();
                }
                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                    waterDTO.stopwatch.Stop();
                }
                if (TimeSpan.FromSeconds(100).TotalSeconds==100)
                {
                    waterDTO.stopwatch.Stop();
                }
            }

            //while (waterDTO.stopwatch.Elapsed < TimeSpan.FromSeconds(100))
            //{
            //    if (!plantsActivityDict.ContainsKey(plantNumber))
            //    {
            //        var waterObj = plantsActivityDict[plantNumber];
            //        waterObj.tokenSource.Cancel();
            //        waterDTO.stopwatch.Stop();
            //    }
            //}

            // Stop.s
            //if (ct.IsCancellationRequested)
            //{
            //    waterDTO.stopwatch.Stop();
            //}
            //waterDTO.stopwatch.Stop();
            waterDTO.WaterLevel = waterDTO.stopwatch.Elapsed.TotalSeconds;
          
            plantsActivityDict.Remove(plantNumber, out waterDTO);
            ViewData["planActivitySession"] = plantsActivityDict;
        }
    }
}
