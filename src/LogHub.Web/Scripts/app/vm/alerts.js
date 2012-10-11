var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.alertList = loghub.viewmodels.baseEditableList.extend({

	url: '/api/alert',

	getNewModel: function () {
		var alert = {
			name: ko.observable(),
			host: ko.observable(),
			source: ko.observable(),
			level: ko.observable('None'),
			message: ko.observable(),
			messageCount: ko.observable(),
			minutes: ko.observable(),
			emailTo: ko.observable(),
			emailToMe: ko.observable(true)
		};

		alert.emailToOther = ko.computed({
			read: function() { return !alert.emailToMe(); },
			write: function(value) { return alert.emailToMe(!value); },
			owner: this
		});

		return alert;
	},
	
	getIgnoredFields: function () {
		var ignored = this._super();
		ignored.push('emailToMe');
		ignored.push('emailToOther');
		return ignored;
	},
	
	mapToViewModel: function (model) {
		var alert = this._super(model);
		alert.emailToMe = ko.observable(!alert.emailTo());
		alert.emailToOther = ko.computed({
			read: function () { return !alert.emailToMe(); },
			write: function (value) {
				 return alert.emailToMe(!value);
			},
			owner: this
		});
		return alert;
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
		
		if (item.emailToOther() && !item.emailTo()) {
			validationErrors.push('Please enter a email.');
		} else if (item.emailToOther() && !loghub.utils.isEmail(item.emailTo())) {
			validationErrors.push('Please enter a valid email.');
		}
	},
	
	save: function (alert) {
		if(alert.emailToMe()) {
			alert.emailTo(null);
		}

		this._super(alert);
	}
});