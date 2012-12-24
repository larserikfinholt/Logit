/*globals describe,beforeEach,it,expect*/

define(
  ['jasmine', '../../noteViewModel', '../../dataservice','moment'],
  function (jasmine, nvm, service, momemt) {

      'use strict';


          describe("NoteViewModel", function () {
              it("it should be in edit mode if it is created for less than 3 hours ago", function () {

                  var lastNote = new Date(2012, 9, 27, 16,0,0,0);
                  console.log(lastNote);

                  var nv = new nvm.NoteViewModel({
                      'created': lastNote,
                  });

                  nv.setToday(new Date(2012, 9, 27, 18));
                  expect(nv.isCreatedToday()).toBe(true);

                  nv.setToday(new Date(2012, 9, 27, 19));
                  expect(nv.isCreatedToday()).toBe(false);

              });
          });


  });
