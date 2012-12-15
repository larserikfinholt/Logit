require.config({
    paths:
    {
        'jquery': 'jquery-1.8.3',
        'jquery': 'jquery-1.8.3',
        'knockout': 'knockout-2.2.0.debug',
        'knockout.mapping': 'knockout.mapping-latest.debug'
    },
    shim: {
        "bootstrap"  : ["jquery"]
    },
    urlArgs: "bust=v2"+ (new Date()).getTime()

});


require(['jquery', 'knockout', 'app', 'bootstrap'], function ($, ko, app, bo) {


    var vm = new app.ViewModel();

    vm.loadProjects();

    ko.applyBindings(vm);


});

