var loghub = loghub || {};
$(function () {
    'use strict';

    var AppRouter = Backbone.Router.extend({
        routes: {
            '': 'listLogs',
            'logs?page=:page': 'listLogs'
        },

        listLogs: function (page) {
            if (this.logList) {
                this.logList.toPage(page);
                return;
            }

            this.logList = new window.loghub.LogList(null, { page: page });
            this.logListView = new window.loghub.LogListView({ model: this.logList });
            this.logList.fetch();
            $('#main').html(this.logListView.render().el);
        }
    });

    loghub.Router = new AppRouter();
    Backbone.history.start();
});