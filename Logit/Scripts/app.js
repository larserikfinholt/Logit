/// <reference path="moment.js" />
/// <reference path="knockout.mapping-latest.debug.js" />
/// <reference path="require.js" />

define(['jquery', 'noteViewModel', 'knockout', 'dataservice', 'knockout.mapping'], function ($, note, ko, service, mapping) {

    ko.mapping = mapping;

    var ProjectViewModel = function (data) {
        //console.log("Created ProjectViewModel " + data.title);
        var me = this;

        this.title = ko.observable(data.title);
        this.id = ko.observable(data.id);
        this.owner = ko.observable(data.owner);
        this.text = ko.observable(data.text);
        this.notes = ko.observableArray([]);
        this.loaded = ko.observable(false);
        this.activate = function () {
            if (!this.loaded()) {
                this.loadNotes();
            }
        }
        this.noteToAdd = ko.observable('...');
        this.addNote = function () {


        }
        this.updateProject = function () {

            service.ajaxRequest( 'POST', 'api/projects', ko.mapping.toJSON(me)).statusCode({
                200: function(){ alert(200);},
                201: function(){ alert(201);},
            });

        }

        this.loadNotes = function () {
            url = "api/notes";
            $.getJSON(url, {projectId:me.id()}, function (data, textStatus, jqXHR) {
                ko.mapping.fromJS(data, noteDataMappingOptions, me.notes);
                me.loaded(true);
                me.addEmptyNoteOrActiveLatest();
            });
        };

        this.addEmptyNoteOrActiveLatest = function () {

            // Todo: if last note date is today, then activate latest note, else create a new empty note and activate it
            me.notes.push(new note.NoteViewModel({ text: "...new note...", projectId: me.id(), isEdit: true }));

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
            $.getJSON("api/projects", {}, function (data, textStatus, jqXHR) {
                ko.mapping.fromJS(data, projectDataMappingOptions, self.projects);
                self.select(self.projects()[0]);
            }); 
        };
        this.select = function (d) {
            d.activate();
            self.selected(d);
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


    return {
        ViewModel: ViewModel,
    }

});


