$app.controller('topicController', [
    '$scope', '$routeParams', 'dialogs', function($scope, $routeParams, dialogs) {
        $scope.name = $routeParams.topic;

        $scope.refresh = function() {
            $scope.model = null;
            $.getJSON(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic, {}, function(d) {
                $scope.model = d;
                $scope.$digest();
            });
        };

        $scope.delete = function() {
            var dlg = dialogs.confirm("Delete Topic", "Confirm you want to delete the topic?");
            dlg.result.then(function ok() {
                $scope.model = null;
                $.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/delete", function(d) {
                    window.location = "#/";
                });
            }, function cancel() { return; });
        };
        $scope.refresh();
    }
]);