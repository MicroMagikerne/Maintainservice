using System;
namespace MaintainService.Models
{
	public class Plan
	{
		public int vehicleID { get; set; }
		public int ProductionYear { get; set; }
		public string Model { get; set; }
		public string RepairOrService { get; set; }
		public DateTime TurnInDate { get; set; }
		public string Description { get; set; }

		public Plan(int vehicleid, int productionyear, string model, string repairorservice, DateTime turnindate, string description)
		{
            this.vehicleID = vehicleid;
			this.ProductionYear = productionyear;
			this.Model = model;
			this.RepairOrService = repairorservice;
			this.TurnInDate = turnindate;
            this.Description = description;
        
        }
	}
}

