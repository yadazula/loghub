var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.status = function () {
	var self = this;

	self.url = './api/status';

	self.isLoading = ko.observable(false);

	self.initLoader = function () {
		var script = document.createElement("script");
		script.src = "https://www.google.com/jsapi?callback=app.viewModel.loadCharts";
		script.type = "text/javascript";
		document.getElementsByTagName("head")[0].appendChild(script);
	};

	self.loadCharts = function () {
		google.load("visualization", "1", { packages: ["corechart"], "callback": self.drawChart });
	};

	self.drawChart = function () {
		var data = google.visualization.arrayToDataTable(self.messageCounts);

		var options = {
			vAxis: { maxValue: 1000, },
			series: [{ color: '#5391EF', visibleInLegend: false }]
		};

		self.chart = new google.visualization.AreaChart(document.getElementById('chart_div'));
		self.chart.draw(data, options);
	};

	self.load = function (callback) {
		loghub.restClient.read(self.url, {
			success: function (data) {
				if (self.item) {
					ko.mapping.fromJS(data, self.item, { 'ignore': ["messageCounts"] });
				}
				else {
					self.item = ko.mapping.fromJS(data, { 'ignore': ["messageCounts"] });
				}

				self.messageCounts = data.messageCounts;
			},
			complete: function () {
				if (callback)
					callback();

				self.initLoader();
			}
		});
	};

	self.dispose = function () {
		if (self.chart) self.chart.clearChart();
	};
}