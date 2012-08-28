var loghub = loghub || {};
$(function () {
    'use strict';

    var AppRouter = Backbone.Router.extend({
        routes: {
            '': 'showRecentLogs',
            'search?p=:page': 'showSearches'
        },

        showSearches: function (page) {
            if (this.SearchLogList) {
                this.SearchLogList.toPage(page);
                return;
            }

            this.SearchLogList = new window.loghub.SearchLogList(null, { page: page });
            this.SearchLogListView = new window.loghub.SearchLogListView({ model: this.SearchLogList });
            this.SearchLogList.fetch();
            $('#main').html(this.SearchLogListView.render().el);
        },
        
        showRecentLogs: function () {
        }
        
    });

    loghub.Router = new AppRouter();
    Backbone.history.start();
});