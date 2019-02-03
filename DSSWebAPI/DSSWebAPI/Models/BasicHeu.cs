using System;

namespace DSSWebAPI.Models {
	class BasicHeu {
		int n, m;
		GAPInstance GAP;
		double EPSILON = 1000;
		public int[] sol;
		public int[] capLeft;

		public BasicHeu(GAPInstance gap) {
			GAP = gap;
			n = GAP.numcli;
			capLeft = (int[]) GAP.cap.Clone();
			m = GAP.numserv;
		}

		/* Restituisce il costo di GAP per ogni client */
		public int getCost(int[] sol) {
			if(!getSolValidity(sol)) return Int32.MaxValue;

			int z = 0;
			for(int j = 0;j < GAP.numcli;j++)
				z += GAP.cost[sol[j],j];

			return z;
		}

		/*
		 * Verifica la validità della soluzione
		 * controllando la presenza di GAP, sol e 
		 * che ci siano i corretti numeri di client
		 * e server
		 */
		private bool getSolValidity(int[] sol) {
			if(GAP == null || sol == null || GAP.numcli != sol.Length) return false;

			int[] capused = new int[GAP.numserv];
			for(int j = 0;j < n;j++) {
				if(sol[j] < 0 || sol[j] >= m)
					return false;

				capused[sol[j]] += GAP.req[sol[j],j];
				if(capused[sol[j]] > GAP.cap[sol[j]])
					return false;
			}
			return true;
		}

		/*
		 * Funzione che controlla il costo della soluzione
		 * proposta.
		 * 
		 */
		public int checkSolCost(int[] sol) {
			int z = 0, j;
			int[] capused = new int[m];
			for(int i = 0;i < m;i++) capused[i] = 0;
			for(j = 0;j < n;j++)
				if(sol[j] < 0 || sol[j] >= m) {
					return int.MaxValue;
				} else
					z += GAP.cost[sol[j],j];
			for(j = 0;j < n;j++) {
				capused[sol[j]] += GAP.req[sol[j],j];
				if(capused[sol[j]] > GAP.cap[sol[j]]) {
					return int.MaxValue;
				}
			}
			return z;
		}

		/* Soluzione euristica costruttiva */
		public int constructiveEuFirstSol() {
			sol = new int[n];
			int[] keys = new int[m];
			int[] index = new int[m];
			int[] capleft = capLeft;
			int ii;

			for(int j = 0;j < n;j++) {
				for(int i = 0;i < m;i++) {
					keys[i] = GAP.req[i,j];
					index[i] = i;
				}
				Array.Sort(keys,index);
				for(ii = 0;ii < m;ii++) {
					int i = index[ii];
					if(capleft[i] >= GAP.req[i,j]) {
						capleft[i] -= GAP.req[i,j];
						sol[j] = i;
						break;
					}
				}
				if(ii == m) {
					return -1;
				}
			}
			int z = checkSolCost(sol);
			GAP.zub = z;
			return z;
		}

		/* Soluzione GAP10 */
		public int GAP10() {
			int isol = 0;
			int[] capleft = this.capLeft;
			int[,] cost = GAP.cost;
			int[,] req = GAP.req;
			int z = 0;
			bool isImproved = true;

			int k = this.constructiveEuFirstSol();
			for(int j = 0;j < n;j++) {
				z += cost[sol[j],j];
			}
			while(isImproved) {
				isImproved = false;
				for(int j = 0;j < n;j++) {
					for(int i = 0;i < m;i++) {
						isol = sol[j];
						if(i != isol && cost[i,j] < cost[isol,j] && capleft[i] >= req[i,j]) {
							isImproved = true;
							sol[j] = i;
							capleft[i] -= req[i,j];
							capleft[isol] += req[isol,j];
							z -= (cost[isol,j] - cost[i,j]);
							if(z < GAP.zub) {
								GAP.zub = z;
							}
						}
					}
				}
			}
			double zCheck = 0;
			for(int j = 0;j < n;j++) {
				zCheck += cost[sol[j],j];
			}
			if(Math.Abs(z - zCheck) > EPSILON) {
				return -1;
			}
			return z;
		}

        public int simulatedAnnealing()
        {
            this.constructiveEuFirstSol();
            sol = computeSimulatedAnnealing(sol);
            return checkSolCost(sol);
        }

        /* Soluzione Simulated Annealing */
        public int[] computeSimulatedAnnealing(int[] sol) {
			int[] capLeft = this.capLeft;
			int[] optSol = (int[]) sol.Clone();
			int optCost = getCost(sol);

			int[] currentSol = (int[]) optSol.Clone();
			int currentCost = optCost;

			Random r = new Random();
			double p = 0.6;
			double? T = null;

			int totalSteps = 0, step = 0;

			while(step <= 50 || p > 0.0001) {
				int[] newSol = (int[]) currentSol.Clone();
				int j = r.Next(n);
				int i = r.Next(m);
				newSol[j] = i;
				if(capLeft[i] >= GAP.req[i,j]) {
					int newCost = currentCost - GAP.cost[currentSol[j],j] + GAP.cost[i,j];

					if(newCost <= currentCost) {
						capLeft[currentSol[j]] += GAP.req[currentSol[j],j];
						capLeft[i] -= GAP.req[i,j];

						currentSol = newSol;
						currentCost = newCost;

						if(newCost < optCost) {
							optSol = newSol;
							optCost = newCost;
							step = 0;
						}
					} else {
						if(!T.HasValue) T = -(newCost - currentCost) / Math.Log(p);
						else p = Math.Exp(-(newCost - currentCost) / T.Value);

						if(r.Next() / (float) Int32.MaxValue < p) {
							capLeft[currentSol[j]] += GAP.req[currentSol[j],j];
							capLeft[i] -= GAP.req[i,j];

							currentSol = newSol;
							currentCost = newCost;
						}
					}
				}

				step++;
				totalSteps++;
				if(totalSteps % (n * (n - 1)) == 0) T *= 0.98;
			}
			return optSol;
		}

        public int tabuSearch()
        {
            this.constructiveEuFirstSol();
            sol = computeTabuSearch(sol);
            return checkSolCost(sol);
        }

        /* Soluzione Tabu Search */
        private int[] computeTabuSearch(int[] solution,int tabuTenure = 10,int maxIteration = 1000) {

			int bestAbsoluteSolution = checkSolCost(solution); //Miglior soluzione possibile, usato per accettare valori Tabu.
			int bestLocalSolution = Int32.MaxValue; //Miglior soluzione locale, usato dentro l'algoritmo
			int stepsToDo = maxIteration;
			Queue<Pair<int,int>> tabuQueue = new Queue<Pair<int,int>>(tabuTenure); //Coda per le mosse Tabu
			int[] tmpsol = (int[]) solution.Clone();
			int[] nextsol = (int[]) solution.Clone();
			Pair<int,int> bestPosition = new Pair<int,int>(-1,-1);
			int step = 0;

			int z = bestAbsoluteSolution;
			int isol = 0;
			int[] capleft = new int[m];
			int[] nextCapleft = new int[m];
			int[,] cost = GAP.cost;
			int[,] req = GAP.req;

			while(step < stepsToDo) {

				for(int j = 0;j < n;j++) {
					for(int i = 0;i < m;i++) {
						capleft = (int[]) capLeft.Clone();
						tmpsol = (int[]) solution.Clone();
						isol = tmpsol[j];
						Pair<int,int> currentPosition = new Pair<int,int>(i,j);
						int tempCost = z;
						if(i != isol && capleft[i] >= req[i,j]) {
							tmpsol[j] = i;
							capleft[i] -= req[i,j];
							capleft[isol] += req[isol,j];
							tempCost -= (cost[isol,j] - cost[i,j]);

							if(!tabuQueue.contains(currentPosition) && tempCost < bestLocalSolution) {
								bestLocalSolution = tempCost;
								nextsol = (int[]) tmpsol.Clone();
								bestPosition = currentPosition;
								nextCapleft = (int[]) capleft.Clone();

							}
							if(tabuQueue.contains(currentPosition) && tempCost < bestLocalSolution
								&& tempCost < bestAbsoluteSolution) {
								bestLocalSolution = tempCost;
								bestAbsoluteSolution = tempCost;
								nextsol = (int[]) tmpsol.Clone();
								bestPosition = currentPosition;
								nextCapleft = (int[]) capleft.Clone();
							}
						}
					}
				}

				solution = (int[]) nextsol.Clone();
				tabuQueue.add(bestPosition);
				if(bestLocalSolution < bestAbsoluteSolution) {
					bestAbsoluteSolution = bestLocalSolution;
				}
				capLeft = (int[]) nextCapleft.Clone();
				z = bestLocalSolution;
				step++;
			}
			return solution;
		}

	}
}