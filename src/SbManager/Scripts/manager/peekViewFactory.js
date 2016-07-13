$app.factory('peekViewFactory', function() {
    return construct;

    function construct(attrs, model) {
        var isDeadLetter = typeof (attrs.dead) != "undefined" || false;
        this.isDeadLetter = isDeadLetter;
        this.title = getTitle();
        this.count = getCount(model);

        function getTitle() {
            if (isDeadLetter) {
                return "Dead Letters";
            } else {
                return "Active Messages";
            }
        };

        function getCount(model) {
            if (isDeadLetter) {
                return model.DeadLetterCount;
            } else {
                return model.ActiveMessageCount;
            }
        };
    }
});