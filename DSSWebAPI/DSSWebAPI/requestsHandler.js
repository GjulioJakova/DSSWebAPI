var globalx = [];
var seasonality = 0;

function setSeasonality(val) {
	seasonality = val;
}

function getSeasonalitySerieAndForecast(param) {
	var sel = document.getElementById('sel');
	function getSelectedOption(sel) {
		var opt;
		for (var i = 0, len = sel.options.length; i < len; i++) {
			opt = sel.options[i];
			if (opt.selected === true) {
				break;
			}
		}
		return opt;
	}
	var opt = getSelectedOption(sel);
	var selection = String(opt.value);
	$.ajax({
		url: "api/Solver/GetSeasonality/" + param + "/" + selection,
		type: "GET",
		contentType: "application/json",
		success: function (result) {
			setSeasonality(result);
			getSerieAndForecast(param);
		},
		error: function (xhr, status, p3) {
			var err = "Error " + " " + status + " " + p3;
			if (xhr.responseText && xhr.responseText[0] == "{")
				err = JSON.parse(xhr.responseText).message;
			alert(err);
		}
	});
}

function solveInstance(param) {
	var sel = document.getElementById('selOpt');
	function getSelectedOption(sel) {
		var opt;
		for (var i = 0, len = sel.options.length; i < len; i++) {
			opt = sel.options[i];
			if (opt.selected === true) {
				break;
			}
		}
		return opt;
	}
	var opt = getSelectedOption(sel);
	var selection = String(opt.value);
	$.ajax({
		url: "api/Solver/SolveInstance/" + param + "/" + selection,
		type: "GET",
		contentType: "application/json",
		success: function (result) {
			readResult(param + " Cost: " + JSON.parse(result));
		},
		error: function (xhr, status, p3) {
			var err = "Error " + " " + status + " " + p3;
			if (xhr.responseText && xhr.responseText[0] == "{")
				err = JSON.parse(xhr.responseText).message;
			alert(err);
		}
	});
}

function getSerieAndForecast(param) {
	$("#myChart").remove();
	$("<h3 id=\"loading\" class=\"text-center\">Loading...</h3>").appendTo("#graph");
	$("<canvas id=\"myChart\"></canvas>").appendTo("#graph");
	var sel = document.getElementById('sel');
	function getSelectedOption(sel) {
		var opt;
		for (var i = 0, len = sel.options.length; i < len; i++) {
			opt = sel.options[i];
			if (opt.selected === true) {
				break;
			}
		}
		return opt;
	}
	var opt = getSelectedOption(sel);
	var selection = String(opt.value);
	$.ajax(
		{
			url: "api/Solver/GetSerie/" + param + "/" + selection,
			type: "GET",
			contentType: "application/json",
			data: "",
			success: function (result) {

				if (param == 'serie') {
					var myObj = JSON.parse(result);
					var x = [];
					switch (selection) {
						case 'esempio':
							x = createArray(myObj, result, 'esempio');
							readResultJSON(x, seasonality, 2004);
							createGraphSerie(x, x.length, seasonality, 2004);
							break;
						case 'esempio2':
							x = createArray(myObj, result, 'esempio2');
							readResultJSON(x, seasonality, 1992);
							createGraphSerie(x, x.length, seasonality, 1992);
							break;
						case 'Passengers':
							x = createArray(myObj, result, 'Passengers');
							readResultJSON(x, seasonality, 1997);
							createGraphSerie(x, x.length, seasonality, 1997);
							break;
						default:
							x = createArray(myObj, result, 'jewelry');
							readResultJSON(x, seasonality, 1992);
							createGraphSerie(x, x.length, seasonality, 1992);
					}
				} else {
					switch (selection) {
						case 'esempio':
							createGraph(result, (seasonality * 2), seasonality, 2009, 2004, 1);
							var itemsToAdd = createLabel((seasonality * 2), seasonality, 2009, 1);

							var el = "Forecasted values:\n";
							for (var i = 0; i < result.length; i++) {
								el += itemsToAdd[i] + " => " + result[i] + "\n";
							}
							readResult(el);
							break;
						case 'esempio2':
							createGraph(result, seasonality * 2, seasonality, 2009, 1992, 8);
							var itemsToAdd = createLabel(seasonality * 2, seasonality, 2008, 8);

							var el = "Forecasted values:\n";
							for (var i = 0; i < result.length; i++) {
								el += itemsToAdd[i] + " => " + result[i] + "\n";
							}
							readResult(el);
							break;
						case 'Passengers':
							createGraph(result, seasonality * 2, seasonality, 2009, 1997, 1);
							var itemsToAdd = createLabel(seasonality * 2, seasonality, 2009, 1);

							var el = "Forecasted values:\n";
							for (var i = 0; i < result.length; i++) {
								el += itemsToAdd[i] + " => " + result[i] + "\n";
							}
							readResult(el);
							break;
						default:
							createGraph(result, seasonality * 2, seasonality, 2016, 1992, 6);
							var itemsToAdd = createLabel(seasonality * 2, seasonality, 2016, 6);

							var el = "Forecasted values:\n";
							for (var i = 0; i < result.length; i++) {
								el += itemsToAdd[i] + " => " + parseFloat(result[i]).toFixed(2) + "\n";
							}
							readResult(el);
					}
				}
			},
			error: function (xhr, status, p3) {
				var err = "Error " + " " + status + " " + p3;
				if (xhr.responseText && xhr.responseText[0] == "{")
					err = JSON.parse(xhr.responseText).message;
				alert(err);
			}
		});
}

function readResultJSON(a, num, startYearSerie) {
	var items = createLabel(a.length, num, startYearSerie, 1);
	var elem = "Serie values: \n";
	for (var i = 0; i < a.length; i++) {
		elem += "" + items[i] + " => " + a[i] + "\n";
	}
	$('#output').val(elem);
}

function readResult(elem) {
	$('#output').val(elem);
}