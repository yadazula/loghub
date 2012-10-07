var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.alertList = loghub.viewmodels.baseEditableList.extend({

	url: '/api/alert',

	getNewModel: function () {
		return {
			name: ko.observable(),
			host: ko.observable(),
			source: ko.observable(),
			level: ko.observable('None'),
			message: ko.observable(),
			messageCount: ko.observable(),
			minutes: ko.observable()
		};
	},

	validate: function (item, validationErrors) {
		if (!item.name()) {
			validationErrors.push('Please fill name field.');
		}

		if (!item.messageCount()) {
			validationErrors.push('Please fill number of messages field.');
		} else if (!loghub.utils.isNumber(item.messageCount())) {
			validationErrors.push('Number of messages must be a number bigger than 0.');
		}

		if (!item.minutes()) {
			validationErrors.push('Please fill minutes field.');
		} else if (!loghub.utils.isNumber(item.minutes())) {
			validationErrors.push('Minutes must be a number bigger than 0.');
		}
	}
});