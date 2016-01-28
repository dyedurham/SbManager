$app.controller('subscriptionController', [
    '$routeParams', 'dialogs', '$http', 'pubSubService', function ($routeParams, dialogs, $http, pubSubService) {
        var vm = this;
        vm.name = $routeParams.subscription;
        vm.topic = $routeParams.topic;
        vm.refresh = function() {
            vm.model = null;
            $http.get(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription).then(function success(response) {
                vm.model = response.data;
            });
        };
        vm.refresh();

        vm.requeue = function() {
            var dlg = dialogs.confirm("Confirm Requeue", "Are you sure you want to requeue all these messages?");
            dlg.result.then(function ok() {
                vm.model = null;
                $http.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + "/requeue/all").
                    then(function success(response) {
                        vm.model = response.data;
                    });
            }, function cancel() { return; });
        };

        pubSubService.subscribe('refresh',function (args) {
                vm.refresh();
        });

        vm.removeall = function(deadletter) {
            var dlg = dialogs.confirm("Delete Topic Messages", "These messages will be entirely deleted?");
            dlg.result.then(function ok() {
                vm.model = null;
                var dead = deadletter ? "_$DeadLetterQueue" : "";

                $http.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + dead + "/remove/all").
                    then(function success(response) {
                        vm.model = response.data;
                    });
            }, function cancel() { return; });
        };

        vm.deadletterall = function() {
            var dlg = dialogs.confirm("Send to dead letter", "Are you sure you want to send all these active messages to the deadletter queue?");
            dlg.result.then(function ok() {
                vm.model = null;

                $http.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + "/dead").
                    then(function success(response) {
                        vm.model = response.data;
                    });
            }, function cancel() { return; });
        };

        vm.delete = function() {
            var dlg = dialogs.confirm("Delete topic", "This can't be undone and your app might explode?");
            dlg.result.then(function ok() {
                vm.model = null;

                $http.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/" + $routeParams.subscription + "/delete").
                    then(function success(response) {
                        window.location = "#/topic/" + $routeParams.topic;
                    });
            });
        };
    }
]);