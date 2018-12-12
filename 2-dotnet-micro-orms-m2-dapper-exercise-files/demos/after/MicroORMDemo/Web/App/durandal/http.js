define(function() {
    return {
        defaultJSONPCallbackParam:'callback',
        get:function(url, query) {
            return $.ajax(url, { data: query });
        },
        jsonp: function (url, query, callbackParam) {
            if (url.indexOf('=?') == -1) {
                callbackParam = callbackParam || this.defaultJSONPCallbackParam;

                if (url.indexOf('?') == -1) {
                    url += '?';
                } else {
                    url += '&';
                }

                url += callbackParam + '=?';
            }

            return $.ajax({
                url: url,
                dataType:'jsonp',
                data:query
            });
        },
        post:function(url, data) {
            return $.ajax({
                url: url,
                data: ko.toJSON(data),
                type: 'POST',
                contentType: 'application/json',
                dataType: 'json'
            });
        },
        put: function (url, data) {
            return $.ajax({
                url: url,
                data: ko.toJSON(data),
                type: 'PUT',
                contentType: 'application/json',
                dataType: 'json'
            });
        },
        delete: function (url) {
            return $.ajax({
                url: url,
                type: 'DELETE',
                contentType: 'application/json',
                dataType: 'json'
            });
        }
    };
});