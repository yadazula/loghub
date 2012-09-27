var loghub = loghub || {};

loghub.app = new function () {
    var self = this;

    self.dashboard = new loghub.viewmodels.page('#dashboard', 'icon-home', 'Dashboard', 'RecentLogList-template', function (params) {
        if (self.currentPage == self.dashboard) return;

        self.viewModel = new loghub.viewmodels.recentLogList();
        self.viewModel.load(function () {
            self.setCurrentPage(self.dashboard, self.viewModel);
        });
    });
    
    self.searches = new loghub.viewmodels.page('#search', 'icon-search', 'Search', 'SearchLogList-template', function (params) {
        if (self.currentPage != self.searches)
            self.viewModel = new loghub.viewmodels.searchLogList();

        self.viewModel.currentFilter.page = params['p'] || 1;
        self.viewModel.load(function () {
            if (self.currentPage != self.searches)
                self.setCurrentPage(self.searches, self.viewModel);
        });
    });

    self.alerts = new loghub.viewmodels.page('#alerts', 'icon-bell', 'Alerts', '', function (params) {
    });
    
    self.users = new loghub.viewmodels.page('#users', 'icon-user', 'Users', 'UserList-template', function (params) {
        if (self.currentPage == self.users) return;

        self.viewModel = new loghub.viewmodels.userList();
        self.viewModel.load(function () {
            self.setCurrentPage(self.users, self.viewModel);
        });
    });
    
    self.retention = new loghub.viewmodels.page('#retention', 'icon-trash', 'Retention & Archiving', 'RetentionList-template', function (params) {
        if (self.currentPage == self.retention) return;

        self.viewModel = new loghub.viewmodels.retentionList();
        self.viewModel.load(function () {
            self.setCurrentPage(self.retention, self.viewModel);
        });
    });
    
    self.settings = new loghub.viewmodels.page('#settings', 'icon-wrench', 'Settings', '', function (params) {
    });

    self.health = new loghub.viewmodels.page('#health', 'icon-plus-sign', 'Server Health', '', function (params) {
    });
    
    self.pages = [self.dashboard, self.searches, self.alerts, self.users, self.retention, self.settings, self.health];

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
    
    loghub.routes.register(self.pages);
    loghub.routes.run(self.dashboard);
};