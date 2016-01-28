//To subscribe:
//PubSubService.subscribe(function() {
// event callback code
//});

//to emit
//PubSubService.publish

(function () {
    'use strict';
    angular.module('pubsub',[])
    .factory('pubSubService', pubSubService);

    //http://becausejavascript.com/angularjs-pubsub-implementation-with-a-service/
    function pubSubService($rootScope) {
        return {
            subscribe: function (event,fn) {
                //$rootScope.$on('notifying-service-event', function (e, data) {
                $rootScope.$on(event, function (e, data) {
                    fn(data);
                });
                //var handler = $rootScope.$on('notifying-service-event', callback);
                //scope.$on('$destroy', handler);
            },
            publish: function(event,data) {
                //$rootScope.$emit('notifying-service-event',data);
                $rootScope.$emit(event, data);
            }
        }
    }
})();