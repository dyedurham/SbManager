$app.controller('homeController', ['$scope', '$http', function ($scope, $http) {
    $scope.refresh = function () {
        $scope.model = null;
        $http.get(window.applicationBasePath + "/api/v1/busmanager/")
        .then(function (d) {
            $scope.model = d.data;
        })
        .catch(function (jqXHR) {
            var err = $.parseJSON(jqXHR.responseText);
            alert("ERROR: " + err.Title + err.Summary);
        });
    };
    $scope.refresh();

    $scope.deleteAll = function () {
        if (!window.confirm("Are you sure you want to delete all topics and queues? This can't be undone and your world might explode.")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/deleteall", function (d) {
            $scope.refresh();
        });
    };

    $scope.deleteAllDeadLetters = function () {
        if (!window.confirm("Are you sure you want to delete all dead letters? This can't be undone and your world might explode.")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/deletealldeadletters", function () {
            $scope.refresh();
        });
    };
}]);