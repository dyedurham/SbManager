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

        function getMessageType(attrs) { //see how the directive is used to obtain these values
            if (_.has(attrs,'dead')) {
                return messageTypeConstants.dead;
            } else if (_.has(attrs, 'scheduled')) {
                return messageTypeConstants.scheduled;
            }
            else {
                return messageTypeConstants.active;
            }
        }

        function getDescription() {
            switch(messageType) {
                case messageTypeConstants.dead:
                    return "Dead Letters";
                case messageTypeConstants.scheduled:
                    return "Scheduled Messages";
                default:
                    return "Active Messages";
            }
        };

        function getCount(model) {
            switch (messageType) {
                case messageTypeConstants.dead:
                    return model.DeadLetterCount;
                case messageTypeConstants.scheduled:
                    return model.ScheduledMessageCount;
                default:
                    return model.ActiveMessageCount;
            }
        };
    }
})(angular);