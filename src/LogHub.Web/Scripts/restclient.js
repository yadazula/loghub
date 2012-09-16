var loghub = loghub || {};

loghub.restClient = new function () {
    var self = this;
    self.read = function (url, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            success: function (value, textStatus, jqXHR) {
                if (callback) callback(value, textStatus, jqXHR);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error while getting the resource at " + url + " .");
            }
        });
    };

    self.post = function (url, data, callback) {
        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: data,
            success: function (value, textStatus, jqXHR) {
                if (callback) callback(value, textStatus, jqXHR);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error while posting the resource at " + url + " .");
            }
        });
    };

    self.put = function (url, data, callback) {
        $.ajax({
            type: "PUT",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: data,
            success: function (value, textStatus, jqXHR) {
                if (callback) callback(value, textStatus, jqXHR);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error while putting the resource at " + url + " .");
            }
        });
    };

    self.delete = function (url, callback) {
        $.ajax({
            type: "DELETE",
            url: url,
            contentType: "application/json",
            dataType: "json",
            success: function (value, textStatus, jqXHR) {
                if (callback) callback(value, textStatus, jqXHR);
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert("Error while deleting the resource at " + url + " .");
            }
        });
    };

};