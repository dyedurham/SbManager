(function (window, angular, undefined) {
    'use strict';

    angular.module('sbManager.filters', [])
    // To send html tags through via a bind
    // Usage: <any nguid-bind-html="value | unsafe"></any>
    .filter('unsafe', function ($sce) { return $sce.trustAsHtml; });

})(window, window.angular);