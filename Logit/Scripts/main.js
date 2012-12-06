require.config({
    paths:
    {
        'jquery': 'jquery-1.8.3',
        'knockout': 'knockout-2.2.0.debug',
        'knockout.mapping': 'knockout.mapping-latest.debug'
    },
    urlArgs: "bust=v2"+ (new Date()).getTime()

});


require(['jquery', 'knockout', 'app', 'knockout.mapping'], function ($, ko, app) {


    var vm = new app.ViewModel();

    vm.loadProjects();

    ko.applyBindings(vm);


});

