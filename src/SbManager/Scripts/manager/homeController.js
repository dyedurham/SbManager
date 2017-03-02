$app.controller('homeController', ['$scope', 'messageTypeConstants', function ($scope, messageTypeConstants) {
    $scope.messageTypes = messageTypeConstants;
    $scope.refresh = function () {
        $scope.model = null;
        $scope.deadletterFilterEnabled = false;
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

    $scope.toggleDeadletterFilter = function () {
        $scope.deadletterFilterEnabled = !$scope.deadletterFilterEnabled;
    }
}]);