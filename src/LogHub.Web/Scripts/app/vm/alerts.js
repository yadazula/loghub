var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.alertList = function () {
    var self = this;
    self.currentItem = ko.observable();

    self.url = '/api/alert';

    self.create = function () {
        self.currentItem({
            name: ko.observable(),
            host: ko.observable(),
            source: ko.observable(),
            level: ko.observable('None'),
            message: ko.observable(),
            messageCount: ko.observable(),
            minutes: ko.observable(),
            isNew: ko.observable(true),
            validationErrors: ko.observable([]),
            isLoading: ko.observable(false)
        });

        $('#alertModal').modal('show');
    };

    self.edit = function (itemVM) {
        var item = ko.mapping.toJS(itemVM);
        item.isNew = false;
        item.isLoading = false;
        item.validationErrors = [];

        var clonedItem = ko.mapping.fromJS(item);
        self.currentItem(clonedItem);
        $('#alertModal').modal('show');
    };

    self.delete = function (item) {
        loghub.restClient.delete(self.url + '?id=' + item.id(), {
            success: function () {
                self.refresh();
            }
        });
    };

    self.validate = function (item) {
        var validationErrors = [];
        
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

        item.validationErrors(validationErrors);
        return validationErrors.length == 0;
    };

    self.save = function (item) {
        if (!self.validate(item))
            return;

        var data = ko.mapping.toJSON(item, { ignore: ['isNew', 'validationErrors', 'isLoading'] });
        item.isLoading(true);

        loghub.restClient.post(self.url, data, {
            success: function () {
                self.refresh();
            },
            error: function () {
                item.isLoading(false);
            }
        });
    };

    self.refresh = function () {
        $('#alertModal').modal('hide');
        self.currentItem(null);
        self.load();
    };

    self.load = function (callback) {
        loghub.restClient.read(self.url, {
            success: function (data) {
                if (self.items)
                    ko.mapping.fromJS(data, self.items);
                else
                    self.items = ko.mapping.fromJS(data);
            },
            complete: function () {
                if (callback) callback();
            }
        });
    };
}