var Jalla;
(function (Jalla) {
    var ProjectDataAccess = (function () {
        function ProjectDataAccess() { }
        ProjectDataAccess.prototype.projectUrl = function (id) {
            return "/api/project/" + (id || "");
        };
        ProjectDataAccess.prototype.noteItemUrl = function (id) {
            return "/api/note/" + (id || "");
        };
        ProjectDataAccess.prototype.ajaxRequest = function (type, url, data) {
            var options = {
                dataType: "json",
                contentType: "application/json",
                cache: false,
                type: type,
                data: ko.toJSON(data)
            };
            return $.ajax(url, options);
        };
        ProjectDataAccess.prototype.getLists = function () {
            return this.ajaxRequest("get", this.projectUrl(null), null);
        };
        ProjectDataAccess.prototype.saveList = function (todoList) {
            if(todoList.TodoListId) {
                return this.ajaxRequest("put", this.projectUrl(todoList.TodoListId), todoList);
            } else {
                return this.ajaxRequest("post", this.projectUrl(null), todoList).done(function (result) {
                    todoList.TodoListId = result.TodoListId;
                    todoList.UserId = result.UserId;
                });
            }
        };
        ProjectDataAccess.prototype.deleteList = function (todoListId) {
            return this.ajaxRequest("delete", this.projectUrl(todoListId), null);
        };
        ProjectDataAccess.prototype.saveItem = function (todoItem) {
            if(todoItem.TodoItemId) {
                return this.ajaxRequest("put", this.noteItemUrl(todoItem.TodoItemId), todoItem);
            } else {
                return this.ajaxRequest("post", this.noteItemUrl(null), todoItem).done(function (result) {
                    todoItem.TodoItemId = result.TodoItemId;
                });
            }
        };
        ProjectDataAccess.prototype.deleteItem = function (todoItemId) {
            return this.ajaxRequest("delete", this.noteItemUrl(todoItemId), null);
        };
        return ProjectDataAccess;
    })();
    Jalla.ProjectDataAccess = ProjectDataAccess;    
})(Jalla || (Jalla = {}));

