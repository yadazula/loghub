var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.status = function () {
	var self = this;

	self.url = './api/status';

	self.isLoading = ko.observable(false);

	self.load = function (callback) {
		loghub.restClient.read(self.url, {
			success: function (data) {
				if (self.item)
					ko.mapping.fromJS(data, self.item);
				else
					self.item = ko.mapping.fromJS(data);
			},
			complete: function () {
				if (callback) callback();
			}
		});
	};
}