using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MaintainService.Models;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Connections;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;



// Namespace for hele projektet
namespace MaintainService
{
    // Angiv ruten til controlleren og gør den til en API-controller
    [Route("api/")]
    [ApiController]
    public class TaxiBookingController : ControllerBase
    {

        private readonly ILogger<TaxiBookingController> _logger;
        private readonly IModel _channel;
        private string CSVPath = string.Empty;
        private string RHQHN = string.Empty;


        public TaxiBookingController(ILogger<TaxiBookingController> logger, IConfiguration configuration)
        {
            CSVPath = configuration["CSVPath"] ?? string.Empty;
            RHQHN = configuration["RHQHN"] ?? string.Empty;
            _logger = logger;
            _logger.LogInformation($"rabbitmq hostname sat til {RHQHN}(maintain)");
           
            var factory = new ConnectionFactory() { HostName = RHQHN };
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();


   
            var hostName = System.Net.Dns.GetHostName();
            var ips = System.Net.Dns.GetHostAddresses(hostName);

            var _ipaddr = ips.First().MapToIPv4().ToString();
           
            _logger.LogInformation(1, $"MaintainService svarer fra {_ipaddr}");

        }



        // Angiver HTTP, og ruten til handlingen, der tilføjer en booking
        [HttpPost("plan")]
        public IActionResult AddBooking(Plan newplan, ILogger<TaxiBookingController> logger)
        {
            var tempplan = newplan;
            string plantype = tempplan.RepairOrService;
            try
            {

                {
                    _channel.ExchangeDeclare(exchange: "topic_logs", type: ExchangeType.Topic);
                    logger.LogInformation("exchance declare virker måske" + Environment.NewLine + Environment.NewLine);
                    var routingKey = "public." + newplan.RepairOrService;

                    var body = JsonSerializer.SerializeToUtf8Bytes(tempplan);

                    _channel.BasicPublish(exchange: "topic_logs",
                                         routingKey: routingKey,
                                         basicProperties: null,
                                         body: body);

                    logger.LogInformation("sendt til rabbit");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while adding booking");
                return StatusCode(500);
            }
        }

        // Angiv HTTP og ruten til handlingen, der genererer en plan over alle bookinger
        [HttpGet("repair")]
        public IActionResult GetRepairPlans()
        {
            try
            {
               
                var plan = GetPlans("repair");
                plan = SortPlans(plan);
                return Ok(plan);
            }
            catch (Exception ex)
            {
                // Hvis der opstår en fejl, returner en HTTP-statuskode 500 (Internal Server Error) og fejlmeddelelsen
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("service")]
        public IActionResult GetServicePlans()
        {
            try
            {

                var plan = GetPlans("service");
                plan = SortPlans(plan);
                return Ok(plan);
            }
            catch (Exception ex)
            {
                // Hvis der opstår en fejl, returner en HTTP-statuskode 500 (Internal Server Error) og fejlmeddelelsen
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }







        // Metode til at generere en liste af DTO'er, der repræsenterer planen
        private List<Plan> SortPlans(List<Plan> plans)
        {
            List<Plan> OrderedPlanList = plans.OrderBy(p => p.TurnInDate).ToList();

            return OrderedPlanList;
        }

        // Metode til at hente alle bookinger som DTO'er
        private List<Plan> GetPlans(string plantype)
        {
            // Kode til at hente bookinger fra CSV
            CSVService csvservice = new CSVService();
            List<Plan> fulllist = new List<Plan>();
            fulllist = csvservice.ReadCSV(CSVPath + plantype + ".csv");

            //tjekker om der blev hentet noget, hvis ikke returnere den noget seeddata
            if (fulllist.Count < 1)
            {
                return new List<Plan>
                {
                    new Plan(1,1999,"ford lort","repair",DateTime.Now,"fyld med bræk og virker ikke")

                };
            }
            else
            {
                return fulllist;
            }
        }


        // Endepunkt som læser det interne metadata indhold fra jeres .NETassembly og sender det til en REST-klient.
        [HttpGet("version")]
        public IEnumerable<string> Get()
        {
            _logger.LogInformation("Metoden er blevet kaldt WUHUHU tjek git");
            var properties = new List<string>();
            var assembly = typeof(Program).Assembly;
            foreach (var attribute in assembly.GetCustomAttributesData())
            {
                properties.Add($"{attribute.AttributeType.Name} - {attribute.ToString()}");
            }
            return properties;
        }

    }

}