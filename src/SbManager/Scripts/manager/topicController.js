$app.controller('topicController', [
    '$routeParams', 'dialogs', '$http', 'pubSubService', function ($routeParams, dialogs, $http, pubSubService) {
        var vm = this;
        vm.name = $routeParams.topic;

        vm.refresh = function() {
            vm.model = null;

            $http.get(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic).
                then(function success(response) {
                    vm.model = response.data;
                });
        };

        pubSubService.subscribe('refresh',function(args) {
            vm.refresh();
        });

        vm.delete = function() {
            var dlg = dialogs.confirm("Delete Topic", "Confirm you want to delete the topic?");
            dlg.result.then(function ok() {
                vm.model = null;
                $http.post(window.applicationBasePath + "/api/v1/busmanager/topic/" + $routeParams.topic + "/delete").
                    then(function success(response) {
                        window.location = "#/";
                    });
            });
        };

        vm.refresh();
    }
]);