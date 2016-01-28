// To subscribe:
// PubSubService.subscribe(function() {
// event callback code
// });

// to emit
// PubSubService.publish
(function() {
    'use strict';
    angular.module('pubsub', [])
        .service('pubSubService', pubSubService);

    function pubSubService($rootScope) {
        this.subscribe = function(event, fn) {
            $rootScope.$on(event, function(e, data) {
                fn(data);
            });
        };
        this.publish = function(event, data) {
            $rootScope.$emit(event, data);
        };
    };
})
();