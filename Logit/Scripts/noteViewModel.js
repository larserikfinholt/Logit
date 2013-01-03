/// <reference path="require.js" />


define(['knockout', 'jquery', 'dataservice', 'knockout.mapping', 'moment'], function (ko, $, service, mapping, moment) {

    ko.mapping = mapping;

    var NoteViewModel = function (data) {
        console.log("Created NoteViewModel " + data.text);
        var self = this;
        this.text = ko.observable(data.text);
        this.date = "idag";
        this.today = moment();
        this.inEdit = ko.observable(false);
        this.id = ko.observable(data.id);
        this.project = data.project;
        this.autoUpdateEnabled = false;
        this.projectId = data.projectId;
        this.created = ko.observable(moment(data.created));
        this.lastUpdated = ko.observable(moment(data.lastUpdated));
        this.orignalText = ko.observable(data.text);
        this.textChanged = ko.computed(function () {
            if (self.orignalText() != self.text()) {
                return true;
            } else {
                return false;
            }

        });

        this.isCreatedToday = function () {
            var diff = this.today.diff(this.created(), 'hours');
            console.log("Diff:" + diff);
            if (diff < 3) {
                return true;
            }
            return false;
        }
        this.setEditMode = function (setEdit) {
            console.log("set edit mode: " + setEdit);
            self.orignalText(self.text());
            self.inEdit(setEdit);
            self.startAutoUpdate();
        }
        this.startAutoUpdate = function () {
            if (self.autoUpdateEnabled) {
                setTimeout(function () { self.update(); }, 10000);
                console.log("started auto update");
            }
        }

        this.setToday = function (dag) {
            self.today = moment(dag);
        }

        this.update = function () {
            if (self.inEdit() && self.textChanged()) {
                self.created(self.created().toDate()); // Hack... pag moment ikke deserialiserer 
                service.ajaxRequest('POST', 'api/notes', ko.mapping.toJSON(self)).success(function (d) {
                    self.id(d.id);
                    self.created(moment(d.created));
                    self.text(d.text);
                    self.setEditMode(self.isCreatedToday());
                });
            } else {
                self.setEditMode(true);
            }
        }
    }



    return {
        NoteViewModel: NoteViewModel
    };
});