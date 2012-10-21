var loghub = loghub || {};

loghub.app = function (showAdminPages) {
	var self = this;

	self.dashboard = new loghub.viewmodels.page('#dashboard', 'icon-home', 'Dashboard', 'RecentLogs', function () {
		self.setCurrentPage(self.dashboard, loghub.viewmodels.recentLogList);
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

	self.searches = new loghub.viewmodels.page('#search', 'icon-search', 'Search', 'Searches', function (params) {
		if (self.currentPage != self.searches)
			self.viewModel = new loghub.viewmodels.searchLogList();

		self.viewModel.currentFilter.page = params['p'] || 1;
		self.viewModel.load(function () {
			if (self.currentPage != self.searches)
				self.renderPage(self.searches, self.viewModel);
		});
	});

	self.status = new loghub.viewmodels.page('#status', 'icon-signal', 'System Status', 'Status', function () {
		self.setCurrentPage(self.status, loghub.viewmodels.status);
	});

	if (showAdminPages)
		self.pages = [self.dashboard, self.searches, self.alerts, self.users, self.retention, self.settings, self.status];
	else
		self.pages = [self.dashboard, self.searches, self.alerts, self.status];

	self.setCurrentPage = function (page, viewModel) {
		if (self.currentPage == page) return;

		self.viewModel = new viewModel();
		self.viewModel.load(function () {
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