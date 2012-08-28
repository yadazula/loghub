var loghub = loghub || {};

(function () {
    'use strict';

    loghub.SearchLogItemView = Backbone.View.extend({
        el: "#search-log-item-container",

        template: _.template($('#search-log-item-template').html()),

        render: function (eventName) {
            $(this.el).append(this.template(this.model.toJSON()));
            return this;
        }
    });

    loghub.SearchLogListView = Backbone.View.extend({
        template: _.template($('#search-log-list-template').html()),

        initialize: function () {
            this.model.bind("reset", this.render, this);
        },
        
        render: function (eventName) {
            var pageInfo = this.model.pageInfo();
            $(this.el).html(this.template(pageInfo));
            _.each(this.model.models, function (log) {
                var itemView = new window.loghub.SearchLogItemView({ model: log });
                itemView.render();
            }, this);
            return this;
        }
    });
}());