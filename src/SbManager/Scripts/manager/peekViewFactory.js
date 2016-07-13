(function (angular) {
    'use strict';

    angular
        .module('manager')
        .factory('peekViewFactory', peekViewFactory);

    peekViewFactory.$inject = ['_', 'messageTypeConstants'];

    function peekViewFactory(_, messageTypeConstants) {
        var messageType;

        return construct;

        function construct(attrs, model) {
            messageType = getMessageType(attrs);
            this.messageType = messageType;
            this.description = getDescription();
            this.messageCount = getCount(model);
        };

        function getMessageType(attrs) {
            if (_.has(attrs,'dead')) {
                return messageTypeConstants.dead;
            }
            else {
                return messageTypeConstants.active;
            }
        }

        function getDescription() {
            switch(messageType) {
                case messageTypeConstants.dead:
                    return "Dead Letters";
                default:
                    return "Active Messages";
            }
        };

        function getCount(model) {
            switch (messageType) {
                case messageTypeConstants.dead:
                    return model.DeadLetterCount;
                default:
                    return model.ActiveMessageCount;
            }
        };
    }
})(angular);