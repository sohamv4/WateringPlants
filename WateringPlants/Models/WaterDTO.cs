using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WateringPlants.Models
{
    public class WaterDTO
    {
        public double WaterLevel { get; set; }
        public int p1activityToDo { get; set; }
        public int p2activityToDo { get; set; }
        public Stopwatch stopwatch { get; set; }

    }
}
