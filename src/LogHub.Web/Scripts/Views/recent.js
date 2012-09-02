var loghub = loghub || {};

(function () {
    'use strict';

    loghub.RecentLogListView = Backbone.View.extend({
        template: _.template($('#recent-log-list-template').html()),

        initialize: function () {
            var itemTemplate = $('#log-item-template').html();
            var elManagerFactory = new Backbone.CollectionBinder.ElManagerFactory(itemTemplate, "data-name");
            this.collectionBinder = new Backbone.CollectionBinder(elManagerFactory);
            this.filterBinder = new Backbone.ModelBinder();
        },

        events: {
            'click .recent-log-filter-header': 'toggleFilter',
            'click #recent-log-filter-button': 'applyFilter'
        },

        toggleFilter: function () {
            $('#recent-log-filter').toggle();
        },

        applyFilter: function () {
            this.collection.stream({
                interval: this.collection.interval,
                filterModel: this.filterModel.clone()
            });
            this.toggleFilter();
        },

        stream: function (options) {
            this.collection.stream(options);
            this.filterModel = this.collection.filterModel.clone();
            this.filterBinder.bind(this.filterModel, this.$('#recent-log-filter'));
        },

        render: function (eventName) {
            $(this.el).html(this.template());
            this.collectionBinder.bind(this.collection, this.$('#log-item-container'));

            return this;
        },

        close: function () {
            this.collection.unstream();
            this.collectionBinder.unbind();
            this.filterBinder.unbind();
        }
    });
}());