$app.controller('homeController', ['$scope', '$routeParams', 'alertService','$http','dialogs', function ($scope, $routeParams, alertService,$http,dialogs) {

    $scope.refresh = function () {
        $scope.model = null;
        $http({
            method: 'GET',
            url: window.applicationBasePath + "/api/v1/busmanager/"
        }).success(function (data, status, headers, config) {
            $scope.model = data;
        }).error(function (data, status, headers, config) {
            alertService.add('danger', '<strong>' + data.Title + '</strong> ' + data.Summary);
        });
    };

    $scope.alerts = alertService.get();
    $scope.refresh();

    $scope.deleteAll = function () {
        var dlg = dialogs.confirm("Delete everything?", "You are about to delete all the queues and topics on this bus. Whats done cannot be undone.");
        dlg.result.then(function resolved(btn) {
            $scope.model = null;
            $http({
                method: 'POST',
                url: window.applicationBasePath + "/api/v1/busmanager/deleteall"
            }).success(function (data) {
                $scope.refresh();
            }).error(function (data, status, headers, config) {
                alertService.add('danger', '<strong>' + data.Title + '</strong> ' + data.Summary);
            });
        },
        function declined(btn) {
        });
        //if (!window.confirm("You sure? This can't be undone and your world might explode.")) return;
//        $scope.model = null;
//        $http({
//            method: 'POST',
//            url: window.applicationBasePath + "/api/v1/busmanager/deleteall"
//        }).success(function (data) {
//            $scope.refresh();
//        }).error(function (data, status, headers, config) {
//            alertService.add('danger', '<strong>' + data.Title + '</strong> ' + data.Summary);
//        });
    };
}]);