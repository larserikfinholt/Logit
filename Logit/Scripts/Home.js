window.DAC = window.DAC || {
};
var Jalla;
(function (Jalla) {
    var LogEntry = (function () {
        function LogEntry(date, text, by, projectId, id) {
            var _this = this;
            this.date = date;
            this.text = ko.observable(text);
            this.by = by;
            this.projectId = projectId;
            this.id = id;
            this.inEdit = ko.observable(false);
            this.chars = ko.computed(function () {
                return _this.text().length;
            });
        }
        LogEntry.prototype.toggleEdit = function () {
            this.inEdit(!this.inEdit());
        };
        return LogEntry;
    })();
    Jalla.LogEntry = LogEntry;    
    var Project = (function () {
        function Project(id, name) {
            var _this = this;
            this.entries = ko.observableArray([]);
            this.id = id;
            this.name = name;
            this.selected = ko.observable(false);
            this.textToAdd = ko.observable("");
            this.url = ko.computed(function () {
                return "proj" + _this.id;
            });
            this.hurl = ko.computed(function () {
                return "#proj" + _this.id;
            });
        }
        Project.prototype.select = function () {
            this.selected(true);
        };
        Project.prototype.addLogEntry = function (project) {
            this.entries.push(new LogEntry(new Date(), this.textToAdd(), "by", project.id, 99));
        };
        return Project;
    })();
    Jalla.Project = Project;    
    var ViewModel = (function () {
        function ViewModel(projects) {
            this.projects = ko.observableArray(projects);
            this.selected = ko.observable();
            this.customer = "";
        }
        ViewModel.prototype.addProject = function (projectId, name) {
            this.projects.push(new Project(projectId, name));
        };
        return ViewModel;
    })();
    Jalla.ViewModel = ViewModel;    
})(Jalla || (Jalla = {}));

$(function () {
    window.DAC = new Jalla.ProjectDataAccess();
    var projects = [];
    var today = new Date();
    for(var p = 0; p < 5; p++) {
        var project = new Jalla.Project(p, "Project" + p);
        if(p == 2) {
            project.select();
        }
        for(var l = 0; l < 3; l++) {
            var d = new Date(today.getTime() + l * (24 * 60 * 60 * 1000));
            project.entries.push(new Jalla.LogEntry(d, "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut ac mattis felis. Sed sit amet sem nunc, accumsan pulvinar nibh. In hendrerit sagittis neque eget bibendum. Vivamus quis lectus nibh, a ultrices ipsum. ", "Lars Erik", p, p + l));
        }
        projects.push(project);
    }
    var vm = new Jalla.ViewModel(projects);
    vm.user = "Lars Erik";
    ko.applyBindings(vm);
    vm.selected(vm.projects()[1]);
});
