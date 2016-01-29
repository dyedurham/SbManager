$app.controller('homeController', ['$scope', '$routeParams', function ($scope, $routeParams) {
    $scope.refresh = function () {
        $scope.model = null;
        $.ajax({
            url: window.applicationBasePath + "/api/v1/busmanager/",
            dataType: 'json',
            success: function(d) {
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

    $scope.deleteAll = function () {
        if (!window.confirm("You sure? This can't be undone and your world might explode.")) return;
        $scope.model = null;
        $.post(window.applicationBasePath + "/api/v1/busmanager/deleteall", function (d) {
            window.location = "#/";
        });
    };
}]);