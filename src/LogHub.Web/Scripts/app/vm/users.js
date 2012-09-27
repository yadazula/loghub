var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.userList = function () {
    var self = this;
    self.currentUser = ko.observable();

    self.create = function () {
        self.currentUser({
            username: ko.observable(),
            email: ko.observable(),
            name: ko.observable(),
            role: ko.observable('1'),
            password: ko.observable(),
            passwordAgain: ko.observable(),
            isNew: ko.observable(true),
            validationErrors: ko.observable([]),
            isLoading: ko.observable(false)
        });

        $('#userModal').modal('show');
    };

    self.edit = function (userVM) {
        var user = ko.mapping.toJS(userVM);
        user.password = null;
        user.passwordAgain = null;
        user.isNew = false;
        user.isLoading = false;
        user.validationErrors = [];
        
        var clonedUser = ko.mapping.fromJS(user);
        self.currentUser(clonedUser);
        $('#userModal').modal('show');
    };

    self.delete = function (user) {
        loghub.restClient.delete('/api/users?username=' + user.username(), {
            success: function () {
                self.refresh();
            }
        });
    };

    self.validate = function (user) {
        var validationErrors = [];
        if (!user.username()) {
            validationErrors.push('Please enter a username.');
        }

        if (!user.email()) {
            validationErrors.push('Please enter a email.');
        }
        else if (!loghub.utils.isEmail(user.email())) {
            validationErrors.push('Please enter a valid email.');
        }

        if (!user.name()) {
            validationErrors.push('Please enter a name.');
        }
        
        if (user.isNew() && (!user.password() || !user.passwordAgain())) {
            validationErrors.push('Please enter a password.');
        }
        else if (user.password() != user.passwordAgain()) {
            validationErrors.push('Passwords are not match.');
        }

        user.validationErrors(validationErrors);
        return validationErrors.length == 0;
    };

    self.save = function (user) {
        if (!self.validate(user))
            return;

        var data = ko.mapping.toJSON(user, { ignore: ['isNew', 'validationErrors', 'passwordAgain', 'isLoading'] });
        user.isLoading(true);

        if (user.isNew()) {
            loghub.restClient.post('/api/users', data, {
                success: function () {
                    self.refresh();
                },
                error: function () {
                    user.isLoading(false);
                }
            });

            return;
        }

        loghub.restClient.put('/api/users', data, {
            success: function () {
                self.refresh();
            },
            error: function () {
                user.isLoading(false);
            }
        });
    };

    self.refresh = function () {
        $('#userModal').modal('hide');
        self.currentUser(null);
        self.load();
    };

    self.load = function (callback) {
        loghub.restClient.read('/api/users', {
            success: function (data) {
                if (self.users)
                    ko.mapping.fromJS(data, self.users);
                else
                    self.users = ko.mapping.fromJS(data);
            },
            complete: function () {
                if (callback) callback();
            }
        });
    };
}