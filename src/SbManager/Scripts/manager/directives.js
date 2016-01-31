$app.directive('queuelength', ['messageTypeConstants', function (messageTypeConstants) {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/queuelength.html',
        link: function ($scope, $element, $attrs) {
            $scope.active = parseInt($attrs.active,10) || 0;
            $scope.dead = parseInt($attrs.dead, 10) || 0;
            $scope.scheduled = parseInt($attrs.scheduled, 10) || 0;
            $scope.activeHighlight = !!$scope.active;
            $scope.deadHighlight = !!$scope.dead;
            $scope.scheduledHighlight = !!$scope.scheduled;
            $scope.messageTypes = messageTypeConstants;
        }
    };
}]);
$app.directive('loading', function () {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/loading.html'
    };
});
$app.directive('messagingentity',['messageTypeConstants', function (messageTypeConstants) {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/messagingEntity.html',
        link: function($scope) {
            $scope.messageTypes = messageTypeConstants;
        }
    };
}]);
$app.directive('messagedetails', function () {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/messagedetails.html'
    };
});
$app.directive('messageproperty', function () {
    return {
        restrict: 'EA',
        template: [
                '<span class="badge"><input type="text" readonly="readonly" value="{{val}}" class="prop-view" /> <i class="clickable glyphicon glyphicon-new-window" ng-click="breakout(title,val)"></i></span>',
                '{{title}}'
        ].join(''),
        scope: { viewing: '=model' },
        link: function ($scope, $element, $attrs) {
            $scope.title = $attrs.title;
            $scope.val = $attrs.val;
            $scope.breakout = $scope.$parent.breakout;
            $attrs.$observe('val', function (val) {
                $scope.val = val;
            });
        }
    };
});
$app.directive('peek', ['$modal', 'messageTypeConstants', function ($modal, messageTypeConstants) {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/peek.html',
        scope: {model: "=model"},
        link: function ($scope, $element, $attrs) {
            var dead = $scope.isDeadLetter = typeof ($attrs.dead) != "undefined" || false;
            var topic = $scope.model.TopicName || null;
            $scope.messages = [];
            $scope.viewing = null;
            $scope.searched = false;
            $scope.peekCount = 10;
            $scope.messageTypeDescription = dead ? "Dead Letters" : "Active Messages";
            $scope.messageType = dead ? messageTypeConstants.dead : messageTypeConstants.active;
            $scope.messageCount = dead ? $scope.model.DeadLetterCount : $scope.model.ActiveMessageCount;
            $scope.messageTypes = messageTypeConstants;

            var actionUrl = window.applicationBasePath + "/api/v1/busmanager/";
            if (dead && !topic) actionUrl += "queue/" + $scope.model.Name + "_$DeadLetterQueue";
            if (dead && topic) actionUrl += "topic/" + $scope.model.TopicName + "/" + $scope.model.Name + "_$DeadLetterQueue";
            if (!dead && !topic) actionUrl += "queue/" + $scope.model.Name;
            if (!dead && topic) actionUrl += "topic/" + $scope.model.TopicName + "/" + $scope.model.Name;

            $scope.peek = function () {
                $scope.peeking = true;
                $scope.messages = [];

                $.getJSON(actionUrl + "/messages/" + $scope.peekCount, function (d) {
                    $scope.peeking = false;
                    $scope.messages = d.Messages;
                    $scope.$digest();
                });
            };
            $scope.view = function (model) {
                $scope.viewing = model;
            };
            $scope.breakout = function (label, value) {
                value = value || "";
                var cls = value.length > 100 ? 'lg' : 'sm';
                $modal.open({
                    template: [
                        '<div class="modal-header"><h3 class="modal-title">',label,'</h3></div>',
                        '<div class="modal-body"><textarea class="breakout-text-'+cls+'">',value,'</textarea></div>',
                        '<div class="modal-footer"><button class="btn btn-primary" ng-click="$close()">Done</button>'
                    ].join('')
                });
            };
            $scope.removeMessage = function (msg) {
                $scope.peeking = true;
                $scope.messages = [];
                $.post(actionUrl + "/remove", { messageId: msg.MessageId }, function (d) {
                    $scope.viewing = null;
                    $scope.peek();
                    setTimeout($scope.$parent.refresh, 1);
                });
            };
            $scope.requeueMessage = function (msg) {
                $scope.peeking = true;
                $scope.messages = [];

                $.post(actionUrl + "/requeue", { messageId: msg.MessageId }, function (d) {
                    $scope.viewing = null;
                    $scope.peek();
                    setTimeout($scope.$parent.refresh,1);
                });
            };
            $scope.requeueModifiedMessage = function (msg) {
                $scope.peeking = true;
                $scope.messages = [];

                $.post(actionUrl + "/requeueModified", { messageId: msg.MessageId, body: msg.Body }, function (d) {
                    $scope.viewing = null;
                    $scope.peek();
                    setTimeout($scope.$parent.refresh,1);
                });
            };
            $scope.deadLetter = function (msg) {
                $scope.peeking = true;
                $scope.messages = [];
                $.post(actionUrl + "/dead/" + msg.MessageId, function (d) {
                    $scope.viewing = null;
                    $scope.peek();
                    setTimeout($scope.$parent.refresh, 1);
                });
            };
            $scope.forwardMessage = function (msg) {
                alert('not implemented');
            };
        }
    };
}]);