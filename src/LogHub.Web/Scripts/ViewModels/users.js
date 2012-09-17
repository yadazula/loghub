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
            validationErrors: ko.observable(),
            isLoading: ko.observable(false)
        });

        $('#userModal').modal('show');
    };

    self.edit = function (userVM) {
        var user = ko.mapping.toJS(userVM);
        user.password = null;
        user.passwordAgain = null;
        user.validationErrors = null;
        user.isNew = false;
        user.isLoading = false;

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

    self.save = function (user) {
        if (!user.username()) {
            user.validationErrors('Username can not be empty !');
            return;
        }

        if (!user.name()) {
            user.validationErrors('Name can not be empty !');
            return;
        }

        if (user.isNew() && (!user.password() || !user.passwordAgain())) {
            user.validationErrors('Password can not be empty !');
            return;
        }

        if (user.password() != user.passwordAgain()) {
            user.validationErrors('Passwords are not match !');
            return;
        }

        var data = ko.mapping.toJSON(user, { ignore: ['isNew', 'validationErrors', 'passwordAgain'] });
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