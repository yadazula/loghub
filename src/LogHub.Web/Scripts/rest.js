var loghub = loghub || {};

loghub.restClient = new function () {
    var self = this;
    self.read = function (url, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            statusCode: {
                200: function (data, textStatus, jqXHR) {
                    if (callback) callback(data, textStatus, jqXHR);
                },
                304: function () {
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log("Error while getting the resource at " + url + " .");
            }
        });
    };
};