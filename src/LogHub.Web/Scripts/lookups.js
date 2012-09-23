var loghub = loghub || {};
loghub.lookups = loghub.lookups || {};

loghub.lookups.getSources = function (query, callback) {
    loghub.restClient.read('/api/lookup?type=source&query=' + query, {
        success: function(data) {
            callback(data);
        }
    });
}

loghub.lookups.getHosts = function (query, callback) {
    loghub.restClient.read('/api/lookup?type=host&query=' + query, {
        success: function (data) {
            callback(data);
        }
    });
}