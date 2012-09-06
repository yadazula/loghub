//var loghub = loghub || {};
//(function () {
//    'use strict';

//    loghub.RecentLog = Backbone.Model.extend();

//    loghub.RecentLogFilter = Backbone.Model.extend({
//        defaults: {
//            level: 'All',
//            messageCount: 50
//        }
//    });

//    loghub.RecentLogList = Backbone.Collection.extend({
//        model: loghub.RecentLog,

//        baseUrl: '/api/recent',

//        url: function () {
//            return this.baseUrl + '?' + $.param({
//                host: this.filterModel.get('host'),
//                source: this.filterModel.get('source'),
//                level: this.filterModel.get('level'),
//                message: this.filterModel.get('message'),
//                messageCount: this.filterModel.get('messageCount'),
//            });
//        },

//        stream: function (options) {
//            this.unstream();
//            this.filterModel = options.filterModel || new window.loghub.RecentLogFilter;
//            this.interval = options.interval || 1000;
            
//            var _update = _.bind(function () {
//                this.fetch();
//                this._intervalFetch = window.setTimeout(_update, this.interval);
//            }, this);

//            _update();
//        },

//        unstream: function () {
//            if (this.isStreaming) {
//                window.clearTimeout(this._intervalFetch);
//                delete this._intervalFetch;
//                delete this.filterModel;
//                delete this.interval;
//            }
//        },

//        isStreaming: function () {
//            return !(_.isUndefined(this._intervalFetch));
//        }
//    });
//}());