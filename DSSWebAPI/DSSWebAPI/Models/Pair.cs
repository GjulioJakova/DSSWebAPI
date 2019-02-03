using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSSWebAPI.Models {
	public class Pair<X, Y> {
		X x;
		Y y;

		public Pair(X elem1,Y elem2) {
			this.x = elem1;
			this.y = elem2;
		}

		public X getFirstElem() {
			return this.x;
		}

		public Y getSecondElem() {
			return this.y;
		}
	}
}