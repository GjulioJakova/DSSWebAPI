using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebAPI.Models {
	public class GAPInstance {

		public string name;
		public int numcli;
		public int numserv;
		public int[,] cost;
		public int[,] req;
		public int[] cap;
		public int[] sol;
		public int zub;
	}
}