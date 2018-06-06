$app.controller('homeController', ['$scope', '$http', '$route', '$log', function ($scope, $http, $route, $log) {
    $scope.deadletterFilterEnabled = $route.current.$$route.deadletterFilter;
    $scope.refresh = function () {
        $scope.model = null;
        $http.get(window.applicationBasePath + "/api/v1/busmanager/")
        .then(function (d) {
            $scope.model = d.data;
            $scope.queuesWithDeadlettersCount = d.data.Queues.reduce((c, q) => c + (q.DeadLetterCount ? 1 : 0), 0);
            $scope.topicsWithDeadlettersCount = d.data.Topics.reduce((c, t) => c + (t.DeadLetterCount ? 1 : 0), 0);
        })
        .catch(function (jqXHR) {
            $scope.model = {Error: jqXHR.data};
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