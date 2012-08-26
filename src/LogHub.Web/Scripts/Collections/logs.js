var loghub = loghub || {};
(function () {
    'use strict';

    loghub.LogList = Backbone.Collection.extend({
        model: loghub.Log,
        url: '/api/logs'
    });
}());