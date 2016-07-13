$app.factory('peekViewFactory', function() {
    return construct;

    function construct(attrs, model) {
        var isDeadLetter = typeof (attrs.dead) != "undefined" || false;
        var isScheduled = typeof (attrs.scheduled) != "undefined" || false;
        this.isDeadLetter = isDeadLetter;
        this.isScheduled = isScheduled;
        this.title = getTitle();
        this.count = getCount(model);

        function getTitle() {
            if (isDeadLetter) {
                return "Dead Letters";
            }
            else if (isScheduled) {
                return "Scheduled Messages";
            }
            else {
                return "Active Messages";
            }
        };

        function getCount(model) {
            if (isDeadLetter) {
                return model.DeadLetterCount;
            }
            else if (isScheduled) {
                return model.ScheduledMessageCount;
            }
            else {
                return model.ActiveMessageCount;
            }
        };
    }
});