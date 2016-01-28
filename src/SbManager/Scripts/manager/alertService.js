(function() {
    'use strict';
    angular.module('alerts', [])
        .service('alertService', alertService);

    function alertService() {
        var that = this;
        this.alerts = [];

        this.add = function (type, msg) {
            return this.alerts.push({
                type: type,
                msg: msg,
                close: function() {
                    return that.closeAlert(this);
                }
            });
        };

        this.closeAlert = function (alert) {
            return this.closeAlertIdx(this.alerts.indexOf(alert));
        };
        this.closeAlertIdx = function(index) {
            return this.alerts.splice(index, 1);
        };
        this.clear = function() {
            this.alerts = [];
        };
        this.get = function() {
            return this.alerts;
        };
    }
})();