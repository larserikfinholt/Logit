/// <reference path="require.js" />
define(['knockout', 'jquery', 'dataservice', 'knockout.mapping'], function (ko, $, service, mapping) {

    ko.mapping = mapping;

    var NoteViewModel = function (data) {
        //console.log("Created NoteViewModel " + data.text);
        var self = this;
        this.text = ko.observable(data.text);
        this.date = "idag";
        this.inEdit = ko.observable(data.isEdit);
        this.id = ko.observable(data.id);
        this.projectId = data.projectId;
        this.createdDate = ko.observable(data.created);
        this.update = function () {
            if (self.inEdit()) {
                service.ajaxRequest('POST', 'api/notes', ko.mapping.toJSON(self)).success( function (d) {
                    self.id(d.id);
                    self.createdDate(d.created);
                    self.text(d.text);
                    self.inEdit(false);
                });
            } else {
                self.inEdit(true);
            }
        }
    }



    return {
        NoteViewModel: NoteViewModel
    };
});