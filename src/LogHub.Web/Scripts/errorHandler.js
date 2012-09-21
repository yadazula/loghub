var loghub = loghub || {};

loghub.errorHandler = new function () {
    var self = this;

    self.message = ko.observable();

    self.show = function (error) {
        self.message(error);
        $('#errorModal').modal('show');
    };

    self.hide = function() {
        self.message();
        $('#errorModal').modal('hide');
    };
};