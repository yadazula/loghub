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
            validationErrors: ko.observable()
        });

        $('#userModal').modal('show');
    };

    self.edit = function (userVM) {
        var user = ko.mapping.toJS(userVM);
        user.password = null;
        user.passwordAgain = null;
        user.validationErrors = null;
        user.isNew = false;

        var clonedUser = ko.mapping.fromJS(user);
        self.currentUser(clonedUser);
        $('#userModal').modal('show');
    };

    self.delete = function (user) {
        var confirmed = confirm("Really delete this user ?");

        if (!confirmed)
            return;

        loghub.restClient.delete('/api/users?username=' + user.username(), function () {
            self.refresh();
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

        var json = ko.mapping.toJSON(user, { ignore: ['isNew', 'validationErrors', 'passwordAgain'] });
        if (user.IsNew()) {
            loghub.restClient.post('/api/users', json, function () {
                self.refresh();
            });

            return;
        }

        loghub.restClient.put('/api/users', json, function () {
            self.refresh();
        });
    };

    self.refresh = function () {
        $('#userModal').modal('hide');
        self.currentUser(null);
        self.load();
    };

    self.load = function (callback) {
        loghub.restClient.read('/api/users', function (users) {
            if (self.users)
                ko.mapping.fromJS(users, self.users);
            else
                self.users = ko.mapping.fromJS(users);

            if (callback) callback();
        });
    };
}