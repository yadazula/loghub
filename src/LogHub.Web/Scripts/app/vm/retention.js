var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.retentionList = loghub.viewmodels.baseEditableList.extend({

	url: './api/retention',

	getNewModel: function () {
		return {
			source: ko.observable(),
			days: ko.observable(),
			archiveToDisk: ko.observable(false),
			archiveToGlacier: ko.observable(false),
			archiveToS3: ko.observable(false)
		};
	},

	validate: function (item, validationErrors) {
		if (!item.days()) {
			validationErrors.push('Please fill days field.');
		} else if (!loghub.utils.isNumber(item.days())) {
			validationErrors.push('Days must be a number bigger than 0.');
		}
	}
});