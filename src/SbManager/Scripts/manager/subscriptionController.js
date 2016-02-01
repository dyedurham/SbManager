$app.controller('subscriptionController', [
    '$scope', '$routeParams', 'dialogs', function($scope, $routeParams, dialogs) {
        $scope.name = $routeParams.subscription;
        $scope.topic = $routeParams.topic;

        $scope.refresh = function() {
            $scope.model = null;
            $.getJSON(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription, function(d) {
                $scope.model = d;
                $scope.$digest();
            });
        };
        $scope.refresh();

        $scope.requeue = function() {
            var dlg = dialogs.confirm("Confirm Requeue", "Are you sure you want to requeue all these messages?");
            dlg.result.then(function ok() {
                $scope.model = null;
                $.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + "/requeue/all", function(d) {
                    $scope.model = d;
                    $scope.$digest();
                });
            }, function cancel() { return; });
        };

        $scope.removeall = function(deadletter) {
            var dlg = dialogs.confirm("Delete Topic Messages", "These messages will be entirely deleted?");
            dlg.result.then(function ok() {
                $scope.model = null;
                var dead = deadletter ? "_$DeadLetterQueue" : "";
                $.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + dead + "/remove/all", function(d) {
                    $scope.model = d;
                    $scope.$digest();
                });
            }, function cancel() { return; });
        };

        $scope.deadletterall = function() {
            var dlg = dialogs.confirm("Send to dead letter", "Are you sure you want to send all these active messages to the deadletter queue?");
            dlg.result.then(function ok() {
                $scope.model = null;
                $.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + "/dead", function(d) {
                    $scope.model = d;
                    $scope.$digest();
                });
            }, function cancel() { return; });
        };

        $scope.delete = function() {
            var dlg = dialogs.confirm("Delete topic", "This can't be undone and your app might explode?");
            dlg.result.then(function ok() {
                $scope.model = null;
                $.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + "/delete", function(d) {
                    window.location = "#/topic/" + $routeParams.topic;
                });
            }, function cancel() { return; });
        };
    }
]);