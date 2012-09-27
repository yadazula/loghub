var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.Settings = function () {
    var self = this;
    self.isLoading = ko.observable(false);
    self.url = '/api/settings';

    self.save = function () {
        var data = ko.mapping.toJSON(self.item);
        self.isLoading(true);

        loghub.restClient.post(self.url, data, {
            complete: function () {
                self.isLoading(false);
            }
        });
    };

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