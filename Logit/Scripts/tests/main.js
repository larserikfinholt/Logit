/*globals require, jasmine*/

require(
  [
    'jasmine-html',
    'specs/noteviewmodel.spec',
    'specs/projectviewmodel.spec'
  ],
function () {
    'use strict';

    jasmine.getEnv().addReporter(new jasmine.HtmlReporter());
    jasmine.getEnv().execute();
});
