require.config({
    paths:
    {
        'jquery': 'jquery-1.8.3',
        'jquery': 'jquery-1.8.3',
        'knockout': 'knockout-2.2.0.debug',
        'knockout.mapping': 'knockout.mapping-latest.debug'
        //'bootstrap-datepicker':'https://raw.github.com/vitalets/bootstrap-datepicker/master/js/bootstrap-datepicker'
        //'bootstrap-datepicker':'https://raw.github.com/eternicode/bootstrap-datepicker/master/js/bootstrap-datepicker'
    },
    shim: {
        "bootstrap": ["jquery"],
        "bootstrap-datepicker": ["bootstrap", "underscore"]
    },
    urlArgs: "bust=v2"+ (new Date()).getTime()

});


require(['jquery', 'knockout', 'app', 'bootstrap','bootstrap-datepicker', 'underscore'], function ($, ko, app, bo) {


    var vm = new app.ViewModel();

    vm.loadProjects();

    ko.applyBindings(vm);


});

