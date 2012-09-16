var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.UserList = function () {
    var self = this;
    self.currentUser = ko.observable();

    self.create = function () {
        self.currentUser({
            Username: ko.observable(),
            Email: ko.observable(),
            Name: ko.observable(),
            Role: ko.observable('1'),
            Password: ko.observable(),
            PasswordAgain: ko.observable(),
            IsNew: ko.observable(true),
            ValidationErrors: ko.observable()
        });

        $('#userModal').modal('show');
    };

    self.edit = function (userVM) {
        var user = ko.mapping.toJS(userVM);
        user.Password = null;
        user.PasswordAgain = null;
        user.ValidationErrors = null;
        user.IsNew = false;

        var clonedUser = ko.mapping.fromJS(user);
        self.currentUser(clonedUser);
        $('#userModal').modal('show');
    };

    self.delete = function (user) {
        var confirmed = confirm("Really delete this user ?");

        if (!confirmed)
            return;

        loghub.restClient.delete('/api/users?username=' + user.Username(), function () {
            self.refresh();
        });
    };

    self.save = function (user) {
        if (!user.Username()) {
            user.ValidationErrors('Username can not be empty !');
            return;
        }

        if (!user.Name()) {
            user.ValidationErrors('Name can not be empty !');
            return;
        }

        if (user.IsNew() && (!user.Password() || !user.PasswordAgain())) {
            user.ValidationErrors('Password can not be empty !');
            return;
        }

        if (user.Password() != user.PasswordAgain()) {
            user.ValidationErrors('Passwords are not match !');
            return;
        }

        var json = ko.mapping.toJSON(user, { ignore: ['IsNew', 'ValidationErrors', 'PasswordAgain'] });
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