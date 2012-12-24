/*globals describe,beforeEach,it,expect*/

define(
  ['jasmine', '../../app','../../noteviewmodel','../../dataservice', 'moment'],
  function (jasmine, app, nvm, service,  momemt) {

      'use strict';

      var notes= [];
      notes.push(new nvm.NoteViewModel( { 'title1 27 okt':'text123', 'id':'notes/99', 'projectId':'projects/99', 'created': new Date(2012, 9, 27, 16)  }));
      notes.push(new nvm.NoteViewModel({ 'title2 28 okt': 'text456', 'id': 'notes/98', 'projectId': 'projects/99', 'created': new Date(2012, 9, 28, 16) }));
      var data = { 'title': "Title", 'owner': 'user/1', 'id': 1, 'notes': notes };


      describe("ProjectViewModel", function () {
          it("it should find the last note", function () {

              var pvm = new app.ProjectViewModel(data);
              expect(pvm.getLastNote().id()).toBe('notes/98');
          });

          it("should load the notes on activate", function () {

              var pvm = new app.ProjectViewModel(data);

              var loadNotesCalled=false;
              pvm.loadNotes = function () { loadNotesCalled = true; }

              expect(pvm.loaded()).toBe(false);

              pvm.activate();
              expect(loadNotesCalled).toBe(true);
          });

      });


  });
