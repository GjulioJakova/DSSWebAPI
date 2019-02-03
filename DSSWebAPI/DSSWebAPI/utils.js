function createLabel(length, num, startYear, startMonth) {
	var items = [];
	var year = startYear;
	var n = startMonth;
	for (var i = 0; i < length; i++) {
		items.push("" + n + "-" + "" + year);
		if (n == num) {
			n = 1;
			year++;
		} else {
			n++;
		}
	}
	return items;
}

function createGraphSerie(result, length, num, startYear) {
	Chart.defaults.global.elements.point.radius = 0
	globalx = result;
	var items = createLabel(length, num, startYear, 1);
	var ctx = document.getElementById("myChart");
	$("#loading").remove();
	var _ = new Chart(ctx, {
		type: 'line',
		data: {
			labels: items,
			datasets: [{
				borderWidth: 1,
				label: 'Serie Values',
				data: result,
				fill: false,
				borderColor: ['rgba(29,13,255,1)']
			}]
		},
		options: {
			responsive: true,
			tooltips: {
				mode: 'index',
				intersect: false,
			},
			hover: {
				mode: 'nearest',
				intersect: true
			},
			scales: {
				yAxes: [{
					ticks: {
						beginAtZero: true
					}
				}]
			}
		}
	});
}

function createGraph(result, length, num, startYear, startYearSerie, startMonth) {
	var items = createLabel(globalx.length, num, startYearSerie, 1);
	var lastSerieLabel = items[items.length - 1];
	var itemsToAdd = createLabel(length, num, startYear, startMonth);
	for (var j = 0; j < itemsToAdd.length; j++) {
		items.push(itemsToAdd[j]);
	}
	var data = [{ x: lastSerieLabel, y: globalx[globalx.length - 1] }];

	for (var g = 0; g < itemsToAdd.length; g++) {
		data.push({ x: itemsToAdd[g], y: "" + result[g] });
	}
	var ctx = document.getElementById("myChart");
	$("#loading").remove();
	Chart.defaults.global.elements.point.radius = 0
	var _ = new Chart(ctx, {
		type: 'line',
		data: {
			labels: items,
			datasets: [
				{
					borderWidth: 1,
					label: 'Serie',
					data: globalx,
					fill: false,
					borderColor: 'rgba(29,13,255,1)'
				},
				{
					borderWidth: 1,
					label: 'Forecast',
					data: data,
					fill: false,
					borderColor: 'rgba(200, 0, 0, 1)'
				}
			]
		},
		options: {
			responsive: true,
			tooltips: {
				mode: 'index',
				intersect: false,
			},
			hover: {
				mode: 'nearest',
				intersect: true
			},
			scales: {
				yAxes: [{
					ticks: {
						beginAtZero: true
					}
				}]
			},
		}
	});

}
function createArray(myObj, result, typeSerie) {
	var x = [];
	for (i in myObj) {
		switch (typeSerie) {
			case 'esempio':
				if (JSON.parse(result)[i].esempio != "") {
					x.push(JSON.parse(result)[i].esempio);
				}
				break;
			case 'esempio2':
				if (JSON.parse(result)[i].esempio2 != "") {
					x.push(parseFloat(JSON.parse(result)[i].esempio2));
				}
				break;
			case 'Passengers':
				if (JSON.parse(result)[i].Passengers != "") {
					x.push(JSON.parse(result)[i].Passengers);
				}
				break;
			default:
				if (JSON.parse(result)[i].jewelry != "") {
					x.push(JSON.parse(result)[i].jewelry);
				}
		}
	}
	return x;
}