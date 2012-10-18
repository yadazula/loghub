var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.searchLogList = function () {
	var self = this;

	self.currentItem = ko.observable();

	self.url = function () {
		return './api/search?' + $.param(self.currentFilter);
	};

	self.filterModel = {
		host: ko.observable(),
		source: ko.observable(),
		level: ko.observable('None'),
		message: ko.observable(),
		dateFrom: ko.observable(),
		dateTo: ko.observable(),
		timeFrom: ko.observable(),
		timeTo: ko.observable(),
		messageCount: ko.observable(25),
		page: ko.observable(1)
	};

	self.prev = ko.observable(0);
	self.next = ko.observable(0);
	self.pages = ko.observable(0);

	self.filterVisible = ko.observable(false);

	self.currentFilter = ko.toJS(self.filterModel);

	self.toggleFilter = function () {
		var filterVisible = self.filterVisible();
		self.filterVisible(!filterVisible);
	};

	self.applyFilter = function () {
		self.toggleFilter();
		self.currentFilter = ko.toJS(self.filterModel);
		self.load();
	};
	
	self.showDetail = function (model) {
		loghub.restClient.read('./api/logdetail?id=' + model.id(), {
			success: function (data) {
				var detailVM = ko.mapping.fromJS(data);
				self.currentItem(detailVM);
				$('#logDetailModal').modal('show');
			}
		});
	};

	self.load = function (callback) {
		loghub.restClient.read(self.url(), {
			success: function (data) {
				if (self.logItems)
					ko.mapping.fromJS(data.models, self.logItems);
				else
					self.logItems = ko.mapping.fromJS(data.models);

				self.page = data.page;
				self.total = data.total;
				self.pages(Math.ceil(self.total / self.currentFilter.messageCount));
				self.prev((self.page > 1) ? (self.page - 1) : 0);
				self.next((self.page < self.pages()) ? (self.page + 1) : 0);
			},
			complete: function () {
				if (callback) {
					callback();
					$('#logDetailModal').on('hidden', function () {
						self.currentItem(null);
					});
				}
			}
		});
	};
	
	self.dispose = function () {
		$('#logDetailModal').off('hidden');
	};
};