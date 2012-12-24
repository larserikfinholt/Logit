/*globals require*/
'use strict';

// Set the require.js configuration file for your application
require.config({

    baseUrl: '.',

    //urlArgs: "ts="+new Date().getTime(),
    urlArgs: "ts=",

    deps: ['main'],

    paths: {
        'src': '..',
        'jasmine': '../jasmine/jasmine',
        'jasmine-html': '../jasmine/jasmine-html',
        'jquery': '../jquery-1.8.3.min',
        'knockout': '../knockout-2.2.0',
        'knockout.mapping': '../knockout.mapping-latest.debug',
        'dataservice': '../dataservice',
        'moment': '../moment',
        'noteViewModel': '../noteviewmodel',


    },

    shim: {
        jasmine: {
            deps: ['jquery'],
            exports: 'jasmine'
        },
        'jasmine-html': {
            deps: ['jasmine'],
            exports: 'jasmine-html'
        }
    }

});
