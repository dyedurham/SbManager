$app.controller('topicController', ['$scope', '$routeParams', function ($scope, $routeParams) {
    $scope.name = $routeParams.topic;
    
    $scope.refresh = function () {
        $scope.model = null;
        $.getJSON(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic, {}, function (d) {
            $scope.model = d;
            $scope.$digest();
        });
    };

    $scope.delete = function () {
        if (!window.confirm("You sure? This can't be undone and your app might explode.")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/delete", function (d) {
            window.location = "#/";
        });
    };

    $scope.refresh();
}]);