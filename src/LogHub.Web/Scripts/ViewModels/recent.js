var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.RecentLogList = function () {
    var self = this;

    self.url = function () {
        return '/api/recent?' + $.param(self.currentFilter);
    };

    self.filterModel = {
        host: ko.observable(),
        source: ko.observable(),
        level: ko.observable('None'),
        message: ko.observable(),
        messageCount: ko.observable('25')
    };

    self.filterVisible = ko.observable(false);

    self.currentFilter = ko.toJS(self.filterModel);
    
    self.lastUpdate = ko.observable();

    self.toggleFilter = function () {
        var filterVisible = self.filterVisible();
        self.filterVisible(!filterVisible);
    };

    self.applyFilter = function () {
        self.toggleFilter();
        self.currentFilter = ko.toJS(self.filterModel);
        self.stream();
    };

    self.load = function (callback) {
        loghub.restClient.read(self.url(), function (data, textStatus, jqXHR) {
            self.logItems = ko.mapping.fromJS(data);
            self.lastUpdate(new Date());
            if (callback) callback();
            self.stream();
        });
    };

    self.stream = function () {
        self.unstream();
        self.isStreaming = true;

        var startTimer = function () {
            loghub.restClient.read(self.url(), function (data, textStatus, jqXHR) {
                ko.mapping.fromJS(data, self.logItems);
                self.lastUpdate(new Date());
                if(self.isStreaming) {
                    self.streamTimer = window.setTimeout(startTimer, 5000);
                }
            });
        };

        startTimer();
    };

    self.unstream = function() {
        self.isStreaming = false;
        if (self.streamTimer) {
            window.clearTimeout(self.streamTimer);
        }
    };

    self.dispose = function () {
        self.unstream();
    };
}