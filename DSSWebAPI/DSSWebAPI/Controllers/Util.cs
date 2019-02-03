using DSSWebAPI.Models;
using System.Configuration;
using System;
using System.Collections.Generic;
using LinqStatistics;

namespace DSSWebAPI.Controllers {
	public static class Util {
		public static Pair<string,string> handleConnection(string c, string f, string dataDirectory) {
			string sdb = ConfigurationManager.AppSettings["dbServer"];
			string connString = c;
			string factory = f;
			switch(sdb) {
				case "SQLiteConn":
					connString =
					ConfigurationManager.ConnectionStrings["SQLiteConn"].ConnectionString;
					factory =
					ConfigurationManager.ConnectionStrings["SQLiteConn"].ProviderName;
					string dbpath = dataDirectory + "\\db.sqlite";
					connString = connString.Replace("DBFILE",dbpath);
					return new Pair<string, string>(connString,factory);
				case "LocalDbConn":
					connString =
					ConfigurationManager.ConnectionStrings["LocalDbConn"].ConnectionString;
					factory =
					ConfigurationManager.ConnectionStrings["LocalDbConn"].ProviderName;
					return new Pair<string,string>(connString,factory);
				case "RemoteSqlServConn":
					connString =
					ConfigurationManager.ConnectionStrings["RemoteSqlServConn"].ConnectionString;
					factory =
					ConfigurationManager.ConnectionStrings["RemoteSqlServConn"].ProviderName;
					return new Pair<string,string>(connString,factory);
				default:
					connString =
					ConfigurationManager.ConnectionStrings["LocalDbConn"].ConnectionString;
					factory =
					ConfigurationManager.ConnectionStrings["LocalDbConn"].ProviderName;
					return new Pair<string,string>(connString,factory);
			}
		}
	}

	public static class PearsonSeasonality {

		private static Model M = new Model();
		private static int seasonalityRate;

		private static string connString, factory;
		private static string dataDirectory = (string) AppDomain.CurrentDomain.GetData("DataDirectory");

		static PearsonSeasonality() {
			Pair<string,string> p = Util.handleConnection(connString,factory,dataDirectory);
			connString = p.getFirstElem();
			factory = p.getSecondElem();
		}

		private static double Pearson(this IEnumerable<double> source,IEnumerable<double> other) {
			return source.Covariance(other) / (source.StandardDeviationP() * other.StandardDeviationP());
		}

		public static int findSeasonality(string selection) {
			double[] arrSource;
			string queryText = "select " + selection + " from serie";
			arrSource = M.getListFromSerie(connString, queryText, factory).ToArray();
			double pearson;
			double max = -1;
			for(int shifts = 1; shifts <= 13; shifts++) {
				double[] arr = new double[arrSource.Length];
				Array.Copy(arrSource, 0, arr, shifts, arr.Length - shifts);
				pearson = Pearson(arrSource, arr);
				if(pearson > max) {
					seasonalityRate = shifts;
					max = pearson;
				}
			}
			return seasonalityRate;
		}
	}
}