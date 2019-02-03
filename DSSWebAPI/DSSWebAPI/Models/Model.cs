using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Newtonsoft.Json;
using System.IO;
using RDotNet;
using System.Text;

namespace DSSWebAPI.Models {
	public class Model {

		GAPInstance GAP;
		BasicHeu bh;
		string dataDirectory = (string) AppDomain.CurrentDomain.GetData("DataDirectory");


		public string solveInstance(string selection, string param) {
			string dataDirectory = (string) AppDomain.CurrentDomain.GetData("DataDirectory");
			string path = dataDirectory + "\\" + selection + ".json";

			try {
				GAP = JsonConvert.DeserializeObject<GAPInstance>(File.ReadAllText(path));
			} catch(Exception e) {
				return e.Message;
			}
			bh = new BasicHeu(GAP);
			try {
				switch(param) {
					case "Opt10":
						int solGAP = bh.GAP10();
						return "" + solGAP;
					case "SA":
						int solSA = bh.simulatedAnnealing();
						return "" + solSA;
					case "TS":
						int solTS = bh.tabuSearch();
						return "" + solTS;
					default:
						int solFind = bh.constructiveEuFirstSol();
						return "" + solFind;
				}
			} catch(Exception e) {
				return e.Message;
			}
		}

		public string getSerie(string connString, string queryText, string factory) {
			int i, numcol;
			string res = "[";
			List<string> columns = new List<string>();
			DbProviderFactory dbFactory = null;

			dbFactory = DbProviderFactories.GetFactory(factory);

			using(DbConnection conn = dbFactory.CreateConnection()) {
				try {
					conn.ConnectionString = connString;
					conn.Open();
					IDbCommand com = conn.CreateCommand();
					com.CommandText = queryText;
					IDataReader reader = com.ExecuteReader();

					numcol = reader.FieldCount;
					for(i = 0;i < numcol;i++)
						columns.Add(reader.GetName(i));

					while(reader.Read()) {
						res += "{";
						for(i = 0;i < numcol;i++) {
							res += "\"" + columns[i] + "\":\"" + reader[i] + "\",";
						}
						res += "},";
						res = res.Replace(",}","}");
					}
					reader.Close();
					conn.Close();
				} catch(Exception ex) {
					res = "[ERROR] " + ex.Message;
					goto end;
				} finally {
					if(conn.State == ConnectionState.Open)
						conn.Close();
				}
			}
			res = (res + "]").Replace(",]","]");
		end:
			return res;
		}

		public List<Double> getListFromSerie(string connString, string queryText, string factory) {
			List<Double> list = new List<Double>();
			int i, numcol;
			DbProviderFactory dbFactory = null;

			dbFactory = DbProviderFactories.GetFactory(factory);

			using(DbConnection conn = dbFactory.CreateConnection()) {
				try {
					conn.ConnectionString = connString;
					conn.Open();
					IDbCommand com = conn.CreateCommand();
					com.CommandText = queryText;
					IDataReader reader = com.ExecuteReader();

					numcol = reader.FieldCount;

					while(reader.Read()) {
						for(i = 0;i < numcol;i++) {
							list.Add(Convert.ToDouble(reader[i]));
						}
					}
					reader.Close();
					conn.Close();
				} catch {
					goto end;
				} finally {
					if(conn.State == ConnectionState.Open)
						conn.Close();
				}
			}
		end:
			return list;
		}

		public void createCSVFile(string fileName, List<Double> valueList, int months, int startYear) {
			string filePath = dataDirectory + "\\" + fileName + ".csv"; ;
			string delimiter = ",";
			int y = startYear;
			int m = 1;

			string[][] output = new string[valueList.Count + 1][];
			output[0] = new string[] { "anno", "month", "sales" };
			for(int i = 0; i < valueList.Count; i++) {
				output[i + 1] = new string[] { "" + y, "" + m, "" + Convert.ToInt32(valueList[i]) };
				if(m == months) {
					m = 1;
					y++;
				} else {
					m++;
				}
			}
			int length = output.GetLength(0);
			StringBuilder sb = new StringBuilder();
			for(int index = 0; index < length; index++)
				sb.AppendLine(string.Join(delimiter, output[index]));

			File.WriteAllText(filePath, sb.ToString());
		}

        public NumericVector computerRScript(string file, string seasonality, string startingYear)
        {
            REngine.SetEnvironmentVariables();
            // There are several options to initialize the engine, but by default the following suffice:
            REngine en = REngine.GetInstance();
            string ddr = dataDirectory.Replace("\\", "/");
            //en.Evaluate("install.packages('tseries')");
            //en.Evaluate("install.packages('forecast')");
            en.Evaluate("library(tseries)");
            en.Evaluate("library(forecast)");
            en.Evaluate("data <- read.csv(\"" + ddr + "/" + file + ".csv" + "\")");
            en.Evaluate("myts <- ts(data[,3], frequency = " + seasonality + ")");
            en.Evaluate("ARIMAfit1 <- auto.arima(myts)");
            en.Evaluate("myfc <- forecast(ARIMAfit1, h = " + startingYear + ")");
            en.Evaluate("forecast <- myfc$mean");
            NumericVector forcastedValues = en.GetSymbol("forecast").AsNumeric();
            return forcastedValues;

        }

    }
}