var loghub = loghub || {};

loghub.restClient = new function () {
    var self = this;
    
    self.callbackProxy = function(callback) {
        this.success = function(data, textStatus, jqXHR) {
            if (callback && callback.success)
                callback.success(data, textStatus, jqXHR);
        };

        this.error = function (jqXHR, textStatus, errorThrown) {
            if (callback && callback.error)
                callback.error(jqXHR, textStatus, errorThrown);

            var message = 'Status : {0} ({1}) {2} Response : {3} {2}'.format(jqXHR.status, jqXHR.statusText,'<br / >', jqXHR.responseText);
            loghub.errorHandler.show(message);
        };

        this.complete = function(jqXHR, textStatus) {
            if (callback && callback.complete)
                callback.complete(jqXHR, textStatus);
        };
    };

    self.read = function (url, callback) {
        var proxy = new self.callbackProxy(callback);
        
        $.ajax({
            type: 'GET',
            url: url,
            success: proxy.success,
            error: proxy.error,
            complete: proxy.complete
        });
    };

    self.post = function (url, data, callback) {
        var proxy = new self.callbackProxy(callback);
        
        $.ajax({
            type: "POST",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: data,
            success: proxy.success,
            error: proxy.error,
            complete: proxy.complete
        });
    };

    self.put = function (url, data, callback) {
        var proxy = new self.callbackProxy(callback);
        
        $.ajax({
            type: "PUT",
            url: url,
            contentType: "application/json",
            dataType: "json",
            data: data,
            success: proxy.success,
            error: proxy.error,
            complete: proxy.complete
        });
    };

    self.delete = function (url, callback) {
        var proxy = new self.callbackProxy(callback);
        
        $.ajax({
            type: "DELETE",
            url: url,
            contentType: "application/json",
            dataType: "json",
            success: proxy.success,
            error: proxy.error,
            complete: proxy.complete
        });
    };

};