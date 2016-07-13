$app.directive('queuelength', function () {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/queuelength.html',
        link: function ($scope, $element, $attrs) {
            $scope.active = parseInt($attrs.active,10) || 0;
            $scope.dead = parseInt($attrs.dead,10) || 0;
            $scope.activeHighlight = !!$scope.active;
            $scope.deadHighlight = !!$scope.dead;
        }
    };
});
$app.directive('loading', function () {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/loading.html'
    };
});
$app.directive('messagingentity', function () {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/messagingEntity.html'
    };
});
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
$app.directive('peek', ['$modal', '_', 'peekViewFactory', function ($modal, _, peekViewFactory) {
    return {
        restrict: 'EA',
        templateUrl: window.applicationBasePath + '/Content/tmpl/directives/peek.html',
        scope: {model: "=model"},
        link: function ($scope, $element, $attrs) {
            var peek = new peekViewFactory($attrs, $scope.model);
            var topic = $scope.model.TopicName || null;
            $scope.messages = [];
            $scope.viewing = null;
            $scope.searched = false;
            $scope.title = peek.title;
            $scope.peekCount = $scope.messageCount = peek.count;

            var actionUrl = window.applicationBasePath + "/api/v1/busmanager/";
            if (peek.isDeadLetter && !topic) actionUrl += "queue/" + $scope.model.Name + "_$DeadLetterQueue";
            if (peek.isDeadLetter && topic) actionUrl += "topic/" + $scope.model.TopicName + "/" + $scope.model.Name + "_$DeadLetterQueue";
            if (!peek.isDeadLetter && !topic) actionUrl += "queue/" + $scope.model.Name;
            if (!peek.isDeadLetter && topic) actionUrl += "topic/" + $scope.model.TopicName + "/" + $scope.model.Name;

            $scope.peek = function () {
                $scope.peeking = true;
                $scope.messages = [];
                var calculatedPeekCount = peek.isScheduled ? addActiveMessagesCountToPeekCount($scope.peekCount) : $scope.peekCount; //needed to extract scheduled messages
                $.getJSON(actionUrl + "/messages/" + calculatedPeekCount, function (d) {
                    $scope.peeking = false;
                    $scope.messages = peek.isScheduled ? filterScheduledMessages(d.Messages) : d.Messages;
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

            function addActiveMessagesCountToPeekCount(count) {
                return count + $scope.model.ActiveMessageCount;
            }

            function filterScheduledMessages(messages) {
                return _.filter(messages, "IsScheduled");
            }
        }
    };
}]);