var loghub = loghub || {};
$(function () {
    'use strict';

    var AppRouter = Backbone.Router.extend({
        routes: {
            '': 'listLogs'
        },

        listLogs: function () {
            var logList = new window.loghub.LogList();
            var logListView = new window.loghub.LogListView({ model: logList });
            logList.fetch();
            $('#main').html(logListView.render().el);
        }
    });

    loghub.Router = new AppRouter();
    Backbone.history.start();
});