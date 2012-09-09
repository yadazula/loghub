var loghub = loghub || {};

loghub.App = function () {
    var self = this;
    self.dashboard = new loghub.viewmodels.Page('#dashboard', 'icon-home', 'Dashboard', 'recent-log-list-template');
    self.searches = new loghub.viewmodels.Page('#searches', 'icon-search', 'Searches', '');
    self.settings = new loghub.viewmodels.Page('#settings', 'icon-wrench', 'Settings', '');
    self.pages = [self.dashboard, self.searches, self.settings];

    self.setCurrentPage = function (page, viewModel) {
        if (self.currentPage) {
            self.currentPage.isActive(false);
        }

        if (self.currentPageEl) {
            ko.removeNode(self.currentPageEl);
        }

        self.currentPage = page;
        self.currentPage.isActive(true);

        var el = $('#pageContainer')[0];
        var observable = ko.renderTemplate(page.template, viewModel, {}, el);

        self.currentPageEl = el.children[0];
        ko.utils.domNodeDisposal.addDisposeCallback(self.currentPageEl, function () {
            if (viewModel.dispose) {
                viewModel.dispose();
            }
        });

        observable.dispose();
    };

    Sammy(function () {
        this.get(self.dashboard.url, function () {
            if (self.currentPage == self.dashboard) return;

            var recentLogList = new loghub.viewmodels.RecentLogList();
            recentLogList.load(function () {
                self.setCurrentPage(self.dashboard, recentLogList);
            });
        });

        this.get(self.searches.url, function () {
            if (self.currentPage == self.searches) return;
            self.setCurrentPage(self.searches, {});
        });

        this.get(self.settings.url, function () {
            if (self.currentPage == self.settings) return;
            self.setCurrentPage(self.settings, {});
        });

        this.get('', function () { this.app.runRoute('get', '#dashboard'); });
    }).run();
};

$(function () {
    ko.applyBindings(new loghub.App(), $('pages')[0]);
});