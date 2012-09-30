var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.settings = function () {
    var self = this;
    
    self.url = '/api/settings';
    
    self.isLoading = ko.observable(false);

    self.validationErrors = ko.observable([]);

    self.validate = function (item) {
        var validationErrors = [];
        
        if (!item.notification.smtpServer()) {
            validationErrors.push('Please fill server field.');
        }
        
        if (!item.notification.smtpPort()) {
            validationErrors.push('Please fill port field.');
        }
        
        if (!item.notification.smtpUsername()) {
            validationErrors.push('Please fill username field.');
        }
        
        if (!item.notification.smtpPassword()) {
            validationErrors.push('Please fill password field.');
        }
        
        if (!item.notification.fromAddress()) {
            validationErrors.push('Please fill from email adress field.');
        } else if (!loghub.utils.isEmail(item.notification.fromAddress())) {
            validationErrors.push('Please enter a valid email address for sender field.');
        }

        self.validationErrors(validationErrors);
        return validationErrors.length == 0;
    };

    self.saveNotification = function () {
        if (!self.validate(self.item))
            return;
        
        self.save();
    };

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