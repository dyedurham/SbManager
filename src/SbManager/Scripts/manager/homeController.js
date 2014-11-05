$app.controller('homeController', ['$scope', '$routeParams', function ($scope, $routeParams) {
    $scope.refresh = function () {
        $scope.model = null;
        $.getJSON(window.applicationBasePath + "/api/v1/busmanager/", {}, function (d) {
            $scope.model = d;
            $scope.$digest();
        });
    };
    $scope.refresh();

    $scope.deleteAll = function () {
        if (!window.confirm("You sure? This can't be undone and your world might explode.")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/deleteall", function (d) {
            window.location = "#/";
        });
    };
}]);