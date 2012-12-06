/// <reference path="require.js" />
define(['knockout', 'jquery', 'knockout.mapping'], function (ko, $, mapping) {

    ko.mapping = mapping;

    var NoteViewModel = function (data) {
        //console.log("Created NoteViewModel " + data.text);
        var self = this;
        this.text = ko.observable(data.text);
        this.date = "idag";
        this.inEdit = ko.observable(data.isEdit);
        this.noteId = ko.observable(data.noteId);
        this.projectId = data.projectId;
        this.createdDate = ko.observable(data.created);
        this.update = function () {
            if (self.inEdit()) {
                if (self.noteId() == "notes/0") {
                    $.ajax({
                        url: "api/" + self.projectId() + "/notes",
                        type: 'POST',
                        dataType: 'json',
                        data: ko.mapping.toJSON(self),
                        success: function (data, textStatus, jqXHR) {
                            self.inEdit(false);
                        },
                        statusCode: {
                            201: function (created) {
                                self.noteId(created.noteId);
                                self.createdDate = created.created;
                                self.text(created.text);
                            }
                        }
                    });

                } else {
                    $.ajax({
                        url: "api/notes/",
                        type: 'PUT',
                        dataType: 'json',
                        data: ko.mapping.toJSON(self),
                        success: function (data, textStatus, jqXHR) {
                            self.inEdit(false);
                        },
                        statusCode: {
                            200: function (ok) {
                                self.text(ok.text);
                            }
                        }
                    });
                }
            } else {
                self.inEdit(true);
            }
        }
    }



    return {
        NoteViewModel: NoteViewModel
    };
});