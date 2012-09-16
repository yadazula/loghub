var loghub = loghub || {};

loghub.App = function () {
    var self = this;

    self.dashboard = new loghub.viewmodels.Page('#dashboard', 'icon-home', 'Dashboard', 'RecentLogList-template', function (params) {
        if (self.currentPage == self.dashboard) return;

        self.viewModel = new loghub.viewmodels.RecentLogList();
        self.viewModel.load(function () {
            self.setCurrentPage(self.dashboard, self.viewModel);
        });
    });
    
    self.searches = new loghub.viewmodels.Page('#search', 'icon-search', 'Search', 'SearchLogList-template', function (params) {
        if (self.currentPage != self.searches)
            self.viewModel = new loghub.viewmodels.SearchLogList();

        self.viewModel.currentFilter.page = params['p'] || 1;
        self.viewModel.load(function () {
            if (self.currentPage != self.searches)
                self.setCurrentPage(self.searches, self.viewModel);
        });
    });

    self.alerts = new loghub.viewmodels.Page('#alerts', 'icon-bell', 'Alerts', '', function (params) {
    });
    
    self.users = new loghub.viewmodels.Page('#users', 'icon-user', 'Users', 'UserList-template', function (params) {
        if (self.currentPage == self.users) return;

        self.viewModel = new loghub.viewmodels.UserList();
        self.viewModel.load(function () {
            self.setCurrentPage(self.users, self.viewModel);
        });
    });
    
    self.retention = new loghub.viewmodels.Page('#retention', 'icon-trash', 'Retention Settings', '', function (params) {
    });
    
    self.health = new loghub.viewmodels.Page('#health', 'icon-plus-sign', 'Server Health', '', function (params) {
    });
    
    self.pages = [self.dashboard, self.searches, self.alerts, self.users, self.retention, self.health];

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

    loghub.Routes.register(self.pages);
    loghub.Routes.run(self.dashboard);
};

$(function () {
    ko.applyBindings(new loghub.App(), $('pages')[0]);
});