$app.controller('statusController', ['$scope', function ($scope) {
    $scope.refresh = function () {
        $scope.model = null;
        $.ajax({
            url: window.applicationBasePath + "/api/v1/busmanager/",
            dataType: 'json',
            success: function (d) {
                $scope.model = d;
                $scope.$digest();
            },
            error: function (jqXHR) {
                var err = $.parseJSON(jqXHR.responseText);
                alert("ERROR: " + err.Title + err.Summary);
            }
        });
    };
    $scope.refresh();
}]);