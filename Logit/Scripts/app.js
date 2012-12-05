/// <reference path="knockout.mapping-latest.debug.js" />
var NoteViewModel = function (data) {
    console.log("Created NoteViewModel " + data.text);
    var self = this;
    this.text = ko.observable(data.text);
    this.date = "idag";
    this.inEdit = ko.observable(data.isEdit);
    this.noteId = ko.observable(data.noteId);
    this.projectId = data.projectId;
    this.createdDate = ko.observable(data.created);
    this.update = function () {
        if (self.inEdit()) {
            $.ajax({
                url: "api/notes/",
                type: 'PUT',
                dataType: 'json',
                data: ko.mapping.toJSON(self),
                success: function (data, textStatus, jqXHR) {
                    self.inEdit(false);
                },
                statusCode: {
                    201: function (created) {
                        var t = ko.mapping.fromJS(created, projectDataMappingOptions);
                        self.projects.push(t);
                    }
                }
            });
        } else {
            self.inEdit(true);
        }
    }
}

var ProjectViewModel = function (data) {
    console.log("Created ProjectViewModel " + data.title);
    var me = this;
    this.title = ko.observable(data.title);
    this.projectId = ko.observable(data.projectId);
    this.text = ko.observable(data.text);
    this.notes = ko.observableArray([]);
    this.loaded = ko.observable( false);
    this.activate = function () {
        if (!this.loaded()) {
            this.loadNotes();
        }
    }
    this.noteToAdd = ko.observable('...');
    this.addNote = function () {


    }
    this.updateProject = function () {

        $.ajax({
            url: "api/"+ me.projectId(),
            type: 'PUT',
            dataType: 'json',
            data: ko.mapping.toJSON(me),
            success: function (data, textStatus, jqXHR) {
                //alert(jqXHR.getResponseHeader('Location'));
            },
            statusCode: {
                200: function (created) {
                    var t = ko.mapping.fromJS(created, projectDataMappingOptions);
                }
            }
        });


    }

    this.loadNotes = function () {
        url = "api/" + this.projectId() + "/notes";
        $.getJSON(url, {}, function (data, textStatus, jqXHR) {
            ko.mapping.fromJS(data, noteDataMappingOptions, me.notes);
            me.loaded(true);
            me.notes.push(new NoteViewModel({ text: "...new note...", projectId: me.projectId, noteId: "notes/0", isEdit:true }));
        });
    };
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
        return new NoteViewModel(options.data);
    }
};

var ViewModel = function () {
    console.log("Created ViewModel ");

    var self = this;
    this.projects = ko.observableArray([]);
    this.newProjectTitle = ko.observable("Untiled");
    this.selected = ko.observable();//new ProjectViewModel({ projectId:"projects/0", title:'tmp' }));

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

$(function () {
    var vm = new ViewModel();

    vm.loadProjects();

    ko.applyBindings(vm);
});