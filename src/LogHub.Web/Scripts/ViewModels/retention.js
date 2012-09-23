var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.retentionList = function () {
    var self = this;
    self.currentItem = ko.observable();

    self.create = function () {
        self.currentItem({
            source: ko.observable(),
            days: ko.observable(),
            isNew: ko.observable(true),
            validationErrors: ko.observable([]),
            isLoading: ko.observable(false)
        });

        $('#retentionModal').modal('show');
    };

    self.edit = function (itemVM) {
        var item = ko.mapping.toJS(itemVM);
        item.isNew = false;
        item.isLoading = false;
        item.validationErrors = [];
        
        var clonedItem = ko.mapping.fromJS(item);
        self.currentItem(clonedItem);
        $('#retentionModal').modal('show');
    };

    self.delete = function (item) {
        loghub.restClient.delete('/api/retention?id=' + item.id(), {
            success: function () {
                self.refresh();
            }
        });
    };

    self.validate = function (item) {
        var validationErrors = [];
        if (!item.source()) {
            validationErrors.push('Please fill source field.');
        }

        if (!item.days()) {
            validationErrors.push('Please fill days field.');
        } else if (!loghub.utils.isNumber(item.days())) {
            validationErrors.push('Days must be a number bigger than 0.');
        }
           
        item.validationErrors(validationErrors);
        return validationErrors.length == 0;
    };

    self.save = function (item) {
        if (!self.validate(item))
            return;

        var data = ko.mapping.toJSON(item, { ignore: ['isNew', 'validationErrors', 'isLoading'] });
        item.isLoading(true);

        if (item.isNew()) {
            loghub.restClient.post('/api/retention', data, {
                success: function () {
                    self.refresh();
                },
                error: function () {
                    item.isLoading(false);
                }
            });

            return;
        }

        loghub.restClient.put('/api/retention', data, {
            success: function () {
                self.refresh();
            },
            error: function () {
                item.isLoading(false);
            }
        });
    };

    self.refresh = function () {
        $('#retentionModal').modal('hide');
        self.currentItem(null);
        self.load();
    };

    self.load = function (callback) {
        loghub.restClient.read('/api/retention', {
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