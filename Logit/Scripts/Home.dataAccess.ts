/// <reference path="./jquery-1.8.d.ts" />
/// <reference path="./knockout-2.2.d.ts" />

module Jalla {

    export class ProjectDataAccess {

        // Private: Routes
        projectUrl(id:any) : string {
            return "/api/project/" + (id || "")
        }
        noteItemUrl(id:any) : string {
            return "/api/note/" + (id || "")
        }

        ajaxRequest(type: string, url:string, data:any) {
            var options = { dataType: "json", contentType: "application/json", cache: false, type: type, data: ko.toJSON(data) }
            return $.ajax(url, options);
        }

         getLists() {
            return this.ajaxRequest("get", this.projectUrl(null), null);
        }

        saveList (todoList): any {
            if (todoList.TodoListId) {
                // Update
                return this.ajaxRequest("put", this.projectUrl(todoList.TodoListId), todoList);
            } else {
                // Create
                return this.ajaxRequest("post", this.projectUrl(null), todoList)
                    .done(function (result) {
                        todoList.TodoListId = result.TodoListId;
                        todoList.UserId = result.UserId;
                    });
            }
        }

        deleteList(todoListId) {
            return this.ajaxRequest("delete", this.projectUrl(todoListId),null);
        }

        saveItem (todoItem) : any {
            if (todoItem.TodoItemId) {
                // Update
                return this.ajaxRequest("put", this.noteItemUrl(todoItem.TodoItemId), todoItem);
            } else {
                // Create
                return this.ajaxRequest("post", this.noteItemUrl(null), todoItem)
                    .done(function (result) {
                        todoItem.TodoItemId = result.TodoItemId;
                    });
            }
        }

        deleteItem (todoItemId) {
            return this.ajaxRequest("delete", this.noteItemUrl(todoItemId), null);
        }



    }
}