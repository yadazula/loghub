var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.SearchLogList = function () {
    var self = this;
    
    self.url = function () {
        return '/api/search?' + $.param(self.currentFilter);
    };

    self.filterModel = {
        host: ko.observable(),
        source: ko.observable(),
        level: ko.observable('All'),
        message: ko.observable(),
        messageCount: ko.observable(50),
        page: ko.observable(1)
    };

    self.prev = ko.observable(0);
    self.next = ko.observable(0);
    self.pages = ko.observable(0);

    self.filterVisible = ko.observable(false);

    self.currentFilter = ko.toJS(self.filterModel);

    self.toggleFilter = function () {
        var filterVisible = self.filterVisible();
        self.filterVisible(!filterVisible);
    };

    self.applyFilter = function () {
        self.toggleFilter();
        self.currentFilter = ko.toJS(self.filterModel);
        self.load();
    };

    self.load = function (callback) {
        loghub.restClient.read(self.url(), function (data, textStatus, jqXHR) {
            if (self.logItems)
                ko.mapping.fromJS(data.Models, self.logItems);
            else
                self.logItems = ko.mapping.fromJS(data.Models);
            
            self.page = data.Page;
            self.total = data.Total;
            self.pages(Math.ceil(self.total / self.currentFilter.messageCount));
            self.prev((self.page > 1) ? (self.page - 1) : 0);
            self.next((self.page < self.pages()) ? (self.page + 1) : 0);
            if (callback) callback();
        });
    };
};