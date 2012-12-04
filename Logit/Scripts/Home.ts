/// <reference path="./jquery-1.8.d.ts" />
/// <reference path="./knockout-2.2.d.ts" />
/// <reference path="./Home.dataAccess.ts" />

//interface Window { DAC: any; }

window.DAC = window.DAC || {};

module Jalla {

   export class LogEntry {

       inEdit: KnockoutObservableBool;
       id: number;
        date: Date;
        text: KnockoutObservableString;
        by: string;
        projectId: number;
        chars: KnockoutComputed;


        constructor (date: Date, text: string, by: string, projectId: number, id: number) {
            this.date = date;
            this.text = ko.observable(text);
            this.by = by;
            this.projectId = projectId;
            this.id = id;
            this.inEdit = ko.observable(false);
            this.chars = ko.computed(() =>{
                return this.text().length;
            });
        }

        toggleEdit() {
            this.inEdit(!this.inEdit());
        }
    }

   export class Project {
        id: number;
        name: string;
        selected: KnockoutObservableBool;
        entries: KnockoutObservableArray;
        url: KnockoutComputed;
        hurl: KnockoutComputed;
        textToAdd: KnockoutObservableString;

        constructor (id: number, name: string) {
            this.entries = ko.observableArray([]);
            this.id = id;
            this.name = name;
            this.selected = ko.observable(false);
            this.textToAdd = ko.observable("");
            this.url = ko.computed(() =>{
                return "proj" + this.id;
            });
            this.hurl = ko.computed(() =>{
                return "#proj" + this.id;
            });
        }

        select() {
            this.selected(true);
        }

        addLogEntry(project: Project) {
            this.entries.push(new LogEntry(new Date, this.textToAdd(), "by", project.id , 99));
        }


    }

    export class ViewModel {

        user: string;
        customer: string;
        selectedProjectId: number;
        projects: KnockoutObservableArray;
        selected: KnockoutObservableAny;

        constructor (projects: Project[]) {
            this.projects = ko.observableArray(projects);
            this.selected = ko.observable();
            this.customer = "";
        };

        addProject(projectId: number, name: string) {
            this.projects.push(new Project(projectId, name));
        }


    }
}
$(() =>{

    window.DAC = new Jalla.ProjectDataAccess();
    var projects: Jalla.Project[] = [];
    var today = new Date();
    for (var p = 0; p<5; p++) {
        var project = new Jalla.Project(p, "Project" + p);
        if (p == 2) {
            project.select();
        }
        
        for (var l = 0; l<3; l++) {
            var d = new Date(today.getTime() + l* (24 * 60 * 60 * 1000));
            project.entries.push(
                new Jalla.LogEntry(
                    d,
                    "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut ac mattis felis. Sed sit amet sem nunc, accumsan pulvinar nibh. In hendrerit sagittis neque eget bibendum. Vivamus quis lectus nibh, a ultrices ipsum. ", 
                    "Lars Erik", 
                    p,
                    p+l));
        }
        projects.push(project);
    }

    var vm = new Jalla.ViewModel(projects);
    vm.user = "Lars Erik";

    ko.applyBindings(vm)
    vm.selected(vm.projects()[1]);

});

