$app.controller('homeController', ['$routeParams', 'alertService','$http','dialogs','pubSubService', function ($routeParams, alertService,$http,dialogs,pubSubService) {
    var vm = this;
    vm.refresh = function () {
        vm.model = null;
        $http({
            method: 'GET',
            url: window.applicationBasePath + "/api/v1/busmanager/"
        }).success(function (data, status, headers, config) {
            vm.model = data;
        }).error(function (data, status, headers, config) {
            alertService.add('danger', '<strong>' + data.Title + '</strong> ' + data.Summary);
        });
    };

    pubSubService.subscribe('refresh',function(data) {
        vm.refresh();
    });

    vm.alerts = alertService.get();
    vm.refresh();

    vm.deleteAll = function () {
        var dlg = dialogs.confirm("Delete everything?", "You are about to delete all the queues and topics on this bus. Whats done cannot be undone.");
        dlg.result.then(function ok(btn) {
            vm.model = null;
            $http({
                method: 'POST',
                url: window.applicationBasePath + "/api/v1/busmanager/deleteall"
            }).success(function (data) {
                vm.refresh();
            }).error(function (data, status, headers, config) {
                alertService.add('danger', '<strong>' + data.Title + '</strong> ' + data.Summary);
            });
        },
        function cancel(btn) {
        });
    };
}]);