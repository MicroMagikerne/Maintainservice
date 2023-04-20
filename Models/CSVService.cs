using System;
namespace MaintainService.Models
{
    public class CSVService
    {

        public List<Plan> ReadCSV(string path)
        {
            List<Plan> fulllist = new List<Plan>();
            var lines = File.ReadAllLines(path);
            foreach (var line in lines)
            {
                var values = line.Split(',');

                var plan = new Plan(int.Parse(values[0]), int.Parse(values[1]), values[2], values[3], DateTime.Parse(values[4]), values[5]);
                fulllist.Add(plan);
            }
            return fulllist;
        }

        public void AppendCSV(string path, Plan newplan)
        {
            File.AppendAllText(path, $"{newplan.vehicleID},{newplan.ProductionYear},{newplan.Model},{newplan.RepairOrService},{newplan.TurnInDate},{newplan.Description}" + Environment.NewLine);
        }
    }
}

