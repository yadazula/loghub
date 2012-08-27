var loghub = loghub || {};

(function () {
    'use strict';

    loghub.LogListItemView = Backbone.View.extend({
        el: "#log-item-container",

        template: _.template($('#log-item-template').html()),

        render: function (eventName) {
            $(this.el).append(this.template(this.model.toJSON()));
            return this;
        }
    });

    loghub.LogListView = Backbone.View.extend({
        template: _.template($('#log-list-template').html()),

        initialize: function () {
            this.model.bind("reset", this.render, this);
        },
        
        render: function (eventName) {
            var pageInfo = this.model.pageInfo();
            $(this.el).html(this.template(pageInfo));
            _.each(this.model.models, function (log) {
                var itemView = new window.loghub.LogListItemView({ model: log });
                itemView.render();
            }, this);
            return this;
        }
    });
}());