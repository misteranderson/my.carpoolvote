using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Npgsql;


namespace mycpv.Controllers
{
    public class HomeController : Controller
    {
	IConfiguration Configuration {get; set; }
	CPVSettings ConfigSettings {get; set; }

	public HomeController(IOptions<CPVSettings> settings, IConfiguration configuration)
	{
		ConfigSettings = settings.Value;
		Configuration = configuration;
	}

        public IActionResult Index()
        {

		string cxnString = ConfigurationExtensions.GetConnectionString(Configuration,"CPV");
		var sbld = new StringBuilder();
		
		using (var conn = new NpgsqlConnection(cxnString)){
			conn.Open();
	
			using (var cmd = new NpgsqlCommand())
			{
				cmd.Connection = conn;

				// Insert some data
				//cmd.CommandText = "INSERT into data (some_field) VALUES ('Hello Word')";
				//cmd.ExecuteNonQuery();	

				// Get the rows
				cmd.CommandText = "SELECT \"DriverCollectionZIP\",COUNT(*) as DriverCount FROM \"stage\".\"websubmission_driver\" GROUP BY \"DriverCollectionZIP\" ORDER BY COUNT(*) DESC LIMIT 5";
				using (var reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						sbld.AppendFormat("ZIP: {0} - {1} Drivers<br/>\n",reader.GetString(0), reader.GetString(1));
					}
				}
			}
		}

		ViewData["SB"] = sbld.ToString();
		ViewData["SiteName"] = ConfigSettings.SiteName;
           	ViewData["CPVCXN"] = cxnString; 
	    	return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
