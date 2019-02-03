using System;
using System.Web.Http;
using DSSWebAPI.Models;
using System.Threading;
using RDotNet;

namespace DSSWebAPI.Controllers {
	public class SolverController : ApiController {

		Model M = new Model();
		public string connString, factory;
		string dataDirectory = (string) AppDomain.CurrentDomain.GetData("DataDirectory");

		public SolverController() {
			Pair<string,string> p = Util.handleConnection(connString,factory,dataDirectory);
			connString = p.getFirstElem();
			factory = p.getSecondElem();
		}

		[HttpGet]
		[ActionName("SolveInstance")]
		public string SolveInstance(string selection, string param) {
			//GAPInstance GAP = new GAPInstance();
			return M.solveInstance(selection, param);
		}

		[HttpGet]
		[ActionName("GetSeasonality")]
		public int GetSeasonality(string selection) {
			return selection == ("esempio2") ? 12 : PearsonSeasonality.findSeasonality(selection);
		}

		[HttpGet]
		[ActionName("GetSerie")]
		public IHttpActionResult GetSerie(string selection, string param) {
			if(param.Equals("serie")) {
				string queryText = "select " + selection + " from serie";
				string s = M.getSerie(connString, queryText, factory);
				switch(selection) {
					case "esempio":
						M.createCSVFile(selection, M.getListFromSerie(connString, queryText, factory), 4, 2004);
						break;
					case "esempio2":
						M.createCSVFile(selection, M.getListFromSerie(connString, queryText, factory), 12, 1992);
						break;
					case "jewelry":
						M.createCSVFile(selection, M.getListFromSerie(connString, queryText, factory), 12, 1997);
						break;
					default:
						M.createCSVFile(selection, M.getListFromSerie(connString, queryText, factory), 12, 1992);
						break;
				}
				if(s == null)
					return NotFound();
				return Ok(s);
			} else {
				return Ok(computeInR(selection, GetSeasonality(selection) + "", GetSeasonality(selection) * 2 + ""));
			}
		}

		private NumericVector computeInR(string file, string seasonality, string startingYear) {
			object ret = null;
			Thread t = new Thread(() => { ret = M.computerRScript(file, seasonality, startingYear); },2500000);
			t.Start();
			t.Join();
			return (NumericVector) ret;
		}
	}
}








