var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.userList = loghub.viewmodels.baseEditableList.extend({

	url: './api/user',
	
	getNewModel: function () {
		return {
			username: ko.observable(),
			email: ko.observable(),
			name: ko.observable(),
			role: ko.observable('1'),
			password: ko.observable(),
			passwordAgain: ko.observable()
		};
	},

	getEditModel: function (user) {
		user.password = null;
		user.passwordAgain = null;
		return user;
	},

	getIgnoredFields: function () {
		var ignored = this._super();
		ignored.push('passwordAgain');
		return ignored;
	},

	buildDeleteUrl: function (user) {
		return this.url + '?username=' + user.username();
	},

	validate: function (user, validationErrors) {
		if (!user.username()) {
			validationErrors.push('Please enter a username.');
		}

		if (!user.email()) {
			validationErrors.push('Please enter a email.');
		} else if (!loghub.utils.isEmail(user.email())) {
			validationErrors.push('Please enter a valid email.');
		}

		if (!user.name()) {
			validationErrors.push('Please enter a name.');
		}

		if (user.isNew() && (!user.password() || !user.passwordAgain())) {
			validationErrors.push('Please enter a password.');
		} else if (user.password() != user.passwordAgain()) {
			validationErrors.push('Passwords are not match.');
		}
	}
});