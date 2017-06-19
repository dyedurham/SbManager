$app.controller('homeController', ['$scope', '$route', '_', function ($scope, $route, _) {

    $scope.deadletterFilterEnabled = $route.current.$$route.deadletterFilter;

    $scope.refresh = function () {
        $scope.model = null;

        $.ajax({
            url: window.applicationBasePath + "/api/v1/busmanager/",
            dataType: 'json',
            success: function(d) {
                $scope.model = d;
                $scope.queuesWithDeadlettersCount = d.Queues.reduce((c, q) => c + (q.DeadLetterCount ? 1 : 0), 0);
                $scope.topicsWithDeadlettersCount = d.Topics.reduce((c, t) => c + (t.DeadLetterCount ? 1 : 0), 0);
                $scope.$digest();
            },
            error: function (jqXHR) {
                var err = $.parseJSON(jqXHR.responseText);
                alert("ERROR: " + err.Title + err.Summary);
            }
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