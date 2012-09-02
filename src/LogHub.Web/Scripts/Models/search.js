var loghub = loghub || {};
(function () {
    'use strict';

    loghub.SearchLog = Backbone.Model.extend();

    loghub.SearchLogList = Backbone.Collection.extend({
        model: loghub.SearchLog,
        baseUrl: '/api/search',
        initialize: function(model, options) {
            _.bindAll(this, 'parse', 'url', 'pageInfo', 'nextPage', 'previousPage');
            this.page = options.page || 1;
            this.perPage = options.perPage || 30;
        },
        parse: function(resp) {
            this.page = resp.Page;
            this.perPage = resp.PerPage;
            this.total = resp.Total;
            return resp.Models;
        },
        url: function() {
            return this.baseUrl + '?' + $.param({page: this.page, perPage: this.perPage});
        },
        pageInfo: function() {
            var info = {
                total: this.total,
                page: this.page,
                perPage: this.perPage,
                pages: Math.ceil(this.total / this.perPage),
                prev: false,
                next: false
            };

            var max = Math.min(this.total, this.page * this.perPage);

            if (this.total == this.pages * this.perPage) {
                max = this.total;
            }

            info.range = [(this.page - 1) * this.perPage + 1, max];

            if (this.page > 1) {
                info.prev = this.page - 1;
            }

            if (this.page < info.pages) {
                info.next = this.page + 1;
            }

            return info;
        },
        toPage: function(page) {
            if (page > this.total) {
                return false;
            }
            this.page = page;
            return this.fetch();
        },
        nextPage: function() {
            if (!this.pageInfo().next) {
                return false;
            }
            this.page = this.page + 1;
            return this.fetch();
        },
        previousPage: function() {
            if (!this.pageInfo().prev) {
                return false;
            }
            this.page = this.page - 1;
            return this.fetch();
        }

    });
}());