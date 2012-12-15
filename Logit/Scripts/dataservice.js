define(["jquery"], function ($) {

    var Service = function () {

        this.ajaxRequest = function (type, url, data) {
            var options = {
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: type,
                data: data
            };
            return $.ajax(url, options);
        };

    }


    var service = new Service();

    return service;



});