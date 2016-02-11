(function(angular) {
    "use strict";

    angular
        .module("manager")
        .constant("messageTypeConstants", messageTypeConstants());

    function messageTypeConstants() {
        var constants = {};

        constants.active = "active";
        constants.dead = "dead";
        constants.scheduled = "scheduled";

        return constants;

    }
}(angular));