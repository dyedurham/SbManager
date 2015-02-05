$app.controller('queueController', ['$scope', '$routeParams', function ($scope, $routeParams) {
    $scope.name = $routeParams.queue;

    $scope.refresh = function () {
        $scope.model = null;
        $.getJSON(window.applicationBasePath + "/api/v1/busmanager/queue/" + $routeParams.queue, {}, function(d) {
            $scope.model = d;
            $scope.$digest();
        });
    };
    $scope.refresh();

    $scope.requeue = function () {
        if (!window.confirm("Are you sure you want to requeue all these messages?")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/queue/" + $routeParams.queue + "/requeue/all", function (d) {
            $scope.model = d;
            $scope.$digest();
        });
    };

    $scope.removeall = function (deadletter) {
        if (!window.confirm("You sure? These messages will be entirely deleted!")) return;
        $scope.model = null;
        var dead = deadletter ? "_$DeadLetterQueue" : "";
        $.post(window.applicationBasePath + "/api/v1/busmanager/queue/" + $routeParams.queue + dead + "/remove/all", function (d) {
            $scope.model = d;
            $scope.$digest();
        });
    };

    $scope.deadletterall = function () {
        if (!window.confirm("Are you sure you want to send all these active messages to the deadletter queue?")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/queue/" + $routeParams.queue + "/dead", function (d) {
            $scope.model = d;
            $scope.$digest();
        });
    };

    $scope.delete = function() {
        if (!window.confirm("You sure? This can't be undone and your app might explode.")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/queue/" + $routeParams.queue + "/delete", function(d) {
            window.location = "#/";
        });
    };
}]);