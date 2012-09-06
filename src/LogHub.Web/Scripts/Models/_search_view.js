//var loghub = loghub || {};

//(function () {
//    'use strict';

//    loghub.SearchLogListView = Backbone.View.extend({
//        template: _.template($('#search-log-list-template').html()),

//        pagerTemplate: _.template($('#search-log-pager-template').html()),

//        initialize: function () {
//            var itemTemplate = $('#log-item-template').html();
//            var elManagerFactory = new Backbone.CollectionBinder.ElManagerFactory(itemTemplate, "data-name");
//            this.collectionBinder = new Backbone.CollectionBinder(elManagerFactory);
//            this.collection.bind('reset', this.setPageInfo, this);
//        },
        
//        render: function (eventName) {
//            $(this.el).html(this.template());
//            this.collectionBinder.bind(this.collection, this.$('#log-item-container'));
//            return this;
//        },
        
//        setPageInfo: function () {
//            var pageInfo = this.collection.pageInfo();
//            this.$('#page-container').html(this.pagerTemplate(pageInfo));
//        },
        
//        close: function () {
//            this.collection.off('reset', this.setPageInfo);
//            this.collectionBinder.unbind();
//        }
//    });
//}());