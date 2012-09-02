var loghub = loghub || {};
$(function () {
    'use strict';

    var AppRouter = Backbone.Router.extend({
        routes: {
            '': 'showRecentLogs',
            'dashboard': 'showRecentLogs',
            'search': 'showSearches',
            'search?p=:page': 'showSearches'
        },
        
        showSearches: function (page) {
            var searchLogList = new window.loghub.SearchLogList(null, { page: page });
            var searchLogListView = VM.createView("searchLogListView", function () {
                return new window.loghub.SearchLogListView({ collection: searchLogList });
            });
            
            $('#main').html(searchLogListView.render().el);
            searchLogList.fetch();
            this.highlightItem("#navSearch");
        },

        showRecentLogs: function () {
            var recentLogList = new window.loghub.RecentLogList;
            var recentLogListView = VM.createView("recentLogListView", function () {
                return new window.loghub.RecentLogListView({ collection: recentLogList });
            });
            
            $('#main').html(recentLogListView.render().el);
            recentLogListView.stream({ interval: 5000 });
            this.highlightItem("#navDashboard");
        },
        
        highlightItem: function (el) {
            $('#navList > li').removeClass('active');
            $('#navList > li > a > i').removeClass('icon-white');
            $(el).parent().addClass('active');
            $(el).children().addClass('icon-white');
        }

    });

    loghub.Router = new AppRouter();
    Backbone.history.start();
});