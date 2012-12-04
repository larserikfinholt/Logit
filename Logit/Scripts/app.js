/// <reference path="knockout.mapping-latest.debug.js" />
var NoteViewModel = function (data) {
    var self = this;
    this.text = ko.observable(data.text);
    this.createdDate = ko.observable(data.created);
}

var ProjectViewModel = function (data) {
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


    this.loadNotes = function () {
        url = "api/" + this.projectId() + "/notes";
        $.getJSON(url, {}, function (data, textStatus, jqXHR) {
            ko.mapping.fromJS(data, noteDataMappingOptions, me.notes);
            me.loaded(true);
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
    var self = this;
    this.projects = ko.observableArray([]);
    this.newProjectTitle = ko.observable("Untiled");
    this.selected = ko.observable(new ProjectViewModel({}));

    this.loadProjects = function () {
        $.getJSON("api/projects", {}, function (data, textStatus, jqXHR) {
            ko.mapping.fromJS(data, projectDataMappingOptions, self.projects);
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
                    var t = ko.mapping.fromJS(created);
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