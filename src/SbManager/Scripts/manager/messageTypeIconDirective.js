(function (angular) {

    angular
        .module('manager')
        .directive('messageTypeIcon', messageTypeIcon);

    messageTypeIcon.$inject = ['messageTypeConstants'];
    function messageTypeIcon(messageTypeConstants) {
        return {
            restrict: 'EA',
            templateUrl:  window.applicationBasePath + '/Content/tmpl/directives/messageTypeIcon.html',
            scope: {
                messageType: '='
            },
            link: function ($scope) {
                $scope.messageTypes = messageTypeConstants;
            }
        };
    }

})(angular);