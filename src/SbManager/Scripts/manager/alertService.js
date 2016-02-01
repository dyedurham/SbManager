(function () {
    'use strict';
    angular.module('alerts',[])
    .factory('alertService', alertService);

    function alertService() {
        var service = {
            add: add,
            closeAlert: closeAlert,
            closeAlertIdx: closeAlertIdx,
            clear: clear,
            get: get
        },
        alerts = [];

        return service;

        function add(type, msg) {
            return alerts.push({
                type: type,
                msg: msg,
                close: function () {
                    return closeAlert(this);
                }
            });
        }

        function closeAlert(alert) {
            return closeAlertIdx(alerts.indexOf(alert));
        }

        function closeAlertIdx(index) {
            return alerts.splice(index, 1);
        }

        function clear() {
            alerts = [];
        }

        function get() {
            return alerts;
        }
    }
})();