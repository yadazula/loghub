var loghub = loghub || {};

loghub.app = new function () {
    var self = this;

    self.dashboard = new loghub.viewmodels.page('#dashboard', 'icon-home', 'Dashboard', 'RecentLogs', function () {
        self.setCurrentPage(self.dashboard, loghub.viewmodels.recentLogList);
    });
    
    self.searches = new loghub.viewmodels.page('#search', 'icon-search', 'Search', 'Searches', function (params) {
        if (self.currentPage != self.searches)
            self.viewModel = new loghub.viewmodels.searchLogList();

        self.viewModel.currentFilter.page = params['p'] || 1;
        self.viewModel.load(function () {
            if (self.currentPage != self.searches)
                self.renderPage(self.searches, self.viewModel);
        });
    });

    self.alerts = new loghub.viewmodels.page('#alerts', 'icon-bell', 'Alerts', 'Alerts', function () {
        self.setCurrentPage(self.alerts, loghub.viewmodels.alertList);
    });
    
    self.users = new loghub.viewmodels.page('#users', 'icon-user', 'Users', 'Users', function () {
        self.setCurrentPage(self.users, loghub.viewmodels.userList);
    });
    
    self.retention = new loghub.viewmodels.page('#retention', 'icon-trash', 'Retention & Archiving', 'Retention', function () {
        self.setCurrentPage(self.retention, loghub.viewmodels.retentionList);
    });
    
    self.settings = new loghub.viewmodels.page('#settings', 'icon-wrench', 'Settings', 'Settings', function () {
        self.setCurrentPage(self.settings, loghub.viewmodels.settings);
    });

    self.health = new loghub.viewmodels.page('#health', 'icon-plus-sign', 'Server Health', '', function () {
    });
    
    self.pages = [self.dashboard, self.searches, self.alerts, self.users, self.retention, self.settings, self.health];

    self.setCurrentPage = function (page, viewModel) {
        if (self.currentPage == page) return;

        self.viewModel = new viewModel();
        self.viewModel.load(function() {
            self.renderPage(page, self.viewModel);
        });
    };

    self.renderPage = function (page, viewModel) {
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
    
    loghub.routes.register(self.pages);
    loghub.routes.run(self.dashboard);
};