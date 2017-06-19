$app.controller('statusController', ['$scope', '$http', function ($scope, $http) {
    $scope.refresh = function () {
        $scope.model = null;
        $http.get(window.applicationBasePath + "/api/v1/busmanager/")
        .then(function (d) {
            $scope.model = d.data;
            var currentTime = new Date();
            $scope.time = currentTime.getHours() + ":" + ('0' + currentTime.getMinutes()).slice(-2);
        })
        .catch(function (jqXHR) {
            var err = $.parseJSON(jqXHR.responseText);
            alert("ERROR: " + err.Title + err.Summary);
        });
    };
    $scope.refresh();
}]);
