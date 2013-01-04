/// <reference path="moment.js" />
/// <reference path="knockout.mapping-latest.debug.js" />
/// <reference path="require.js" />

define(['jquery', 'noteViewModel', 'knockout', 'dataservice', 'knockout.mapping', 'moment'], function ($, note, ko, service, mapping, moment) {

    ko.mapping = mapping;

    var ProjectViewModel = function (data) {
        //console.log("Created ProjectViewModel " + data.title);
        var me = this;

        this.title = ko.observable(data.title);
        this.maxNoteIntervalInDaysBeforeReminder = ko.observable(data.maxNoteIntervalInDaysBeforeReminder);
        this.id = ko.observable(data.id);
        this.owner = ko.observable(data.owner);
        this.text = ko.observable(data.text);
        this.notes = ko.observableArray(data.notes); // []
        this.loaded = ko.observable(false);
        this.activate = function () {
            if (!this.loaded()) {
                this.loadNotes();
            }
        }
        this.noteToAdd = ko.observable('...');
        this.addNote = function () {


        }
        this.getLastNote = function () {
            return me.notes()[me.notes().length-1];
        }
        this.updateProject = function () {

            service.ajaxRequest('POST', 'api/projects', ko.mapping.toJSON(me)).statusCode({
                200: function () { alert(200); },
                201: function () { alert(201); },
            });

        }

        this.loadNotes = function () {
            url = "api/notes";
            service.ajaxRequest('GET', 'api/notes', { projectId: me.id() })
                .success(me.notesLoadedCallback);
        };
        this.notesLoadedCallback = function (data, textStatus, jqXHR) {
            ko.mapping.fromJS(data, noteDataMappingOptions, me.notes);
            me.loaded(true);
            me.addEmptyNoteOrActiveLatest();
        }

        this.addEmptyNoteOrActiveLatest = function () {

            console.log("addEmptyNoteOrActiveLatest");
            if (me.notes().length > 0) {
                var last = me.getLastNote();
                if (last.isCreatedToday()) {
                    last.autoUpdateEnabled = true;
                    last.setEditMode(true);
                    console.log("Last note created today - reusing " + last.created());
                    return
                }
            }
            console.log("Creating new empty note");
            me.notes.push(new note.NoteViewModel({ text: "", projectId: me.id() }));
            me.getLastNote().autoUpdateEnabled = true;
            me.getLastNote().setEditMode(true);
        }
    };


    var projectDataMappingOptions = {
        //key: function (data) {
        //    return data.id;
        //},
        create: function (options) {
            return new ProjectViewModel(options.data);
        }
    };
    var noteDataMappingOptions = {
        create: function (options) {
            return new note.NoteViewModel(options.data);
        }
    };

    var ViewModel = function () {
        console.log("Created ViewModel ");

        var self = this;
        this.projects = ko.observableArray([]);
        this.newProjectTitle = ko.observable("Untiled");
        this.selected = ko.observable();//new ProjectViewModel({ id:"projects/0", title:'tmp' }));

        this.loadProjects = function () {
            $.getJSON("api/projects?random=" + (new Date()).getTime(), {}, function (data, textStatus, jqXHR) {
                ko.mapping.fromJS(data, projectDataMappingOptions, self.projects);
                self.select(self.projects()[0]);
            });
        };
        this.select = function (d) {
            if (d) {
                d.activate();
                self.selected(d);
            }
        };

        this.addProject = function () {
            $.ajax({
                url: "api/projects/",
                type: 'POST',
                dataType: 'json',
                data: 'title=' + self.newProjectTitle(),
                success: function (data, textStatus, jqXHR) {
                    //alert(jqXHR.getResponseHeader('Location'));
                },
                statusCode: {
                    201: function (created) {
                        var t = ko.mapping.fromJS(created, projectDataMappingOptions);
                        self.projects.push(t);
                    }
                }
            });


        };

    };

    //////////////
    ko.bindingHandlers.datepicker = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            //initialize datepicker with some optional options
            var options = allBindingsAccessor().datepickerOptions || {};
            $(element).datepicker(options);

            //when a user changes the date, update the view model
            ko.utils.registerEventHandler(element, "changeDate", function (event) {
                var value = valueAccessor();
                if (ko.isObservable(value)) {
                    value(event.date);
                }
            });

            ko.utils.registerEventHandler(element, "change", function () {
                var value = valueAccessor();
                if (ko.isObservable(value)) {
                    var day =moment(element.value, "DD.MM.YYYY").add('h',12);
                    value(day.toDate());

//                    value(new Date(element.value));
                }
            });
        },
        update: function (element, valueAccessor) {
            var widget = $(element).data("datepicker");
            //when the view model is updated, update the widget
            if (widget) {
                widget.date = ko.utils.unwrapObservable(valueAccessor());

                if (!widget.date) {
                    return;
                }

                if (_.isString(widget.date)) {
                    widget.date = new Date(widget.date);
                }

                widget.setValue();
            }
        }
    };

    // Using http://vitalets.github.com/bootstrap-datepicker/ works with error
    // same https://raw.github.com/vitalets/bootstrap-datepicker/master/js/bootstrap-datepicker.js


    return {
        ViewModel: ViewModel,
        ProjectViewModel: ProjectViewModel
    }

});


