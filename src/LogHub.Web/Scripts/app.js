var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.Page = function (url, icon, name) {
    this.url = url;
    this.icon = icon;
    this.name = name;
    this.isActive = ko.observable(false);
};

loghub.App = function () {
    var self = this;
    var dashboard = new loghub.viewmodels.Page('#dashboard', 'icon-home', 'Dashboard');
    var searches = new loghub.viewmodels.Page('#searches', 'icon-search', 'Searches');
    var settings = new loghub.viewmodels.Page('#settings', 'icon-wrench', 'Settings');
    self.pages = [dashboard, searches, settings];

    self.setCurrentPage = function (page, templateName, data) {
        if (self.currentPage) {
            self.currentPage.isActive(false);
        }

        if (self.currentPageEl) {
            ko.removeNode(self.currentPageEl);
        }

        if (self.currentPageObservable) {
            self.currentPageObservable.dispose();
        }

        self.currentPage = page;
        self.currentPage.isActive(true);

        var el = $('#pageContainer')[0];
        self.currentPageObservable = ko.renderTemplate(templateName, data, {}, el);
        if (el.children.length === 1) {
            self.currentPageEl = el.children[0];
        }
    };

    Sammy(function () {
        this.get('#dashboard', function () {
            var log = { Date: 'Host', Host: 'Host', Source: 'Source', Logger: 'Logger', Level: 'Level', Message: 'Message' };
            var logs = { logItems: [log] }; //TODO : dispose logs
            self.setCurrentPage(dashboard, 'recent-log-list-template', logs);
        });

        this.get('#searches', function () {
            self.setCurrentPage(searches,'', {});
        });

        this.get('#settings', function () {
            self.setCurrentPage(settings, '', {});
        });

        this.get('', function () { this.app.runRoute('get', '#dashboard'); });
    }).run();
};

ko.applyBindings(new loghub.App());