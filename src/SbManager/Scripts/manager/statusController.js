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
            if (jqXHR.data == "") {
                alert("Undefined Error");
            } else {
                var err = jqXHR.data;
                alert("ERROR " + err.StatusCode + ": " + err.Message);
            } 
        });
    };
    $scope.refresh();
}]);
