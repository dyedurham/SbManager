var $app = angular.module('manager', ['ngRoute', 'ui.bootstrap','alerts','dialogs.main','dialogs.default-translations','pubsub']);
window.applicationBasePath = "";

$app.config(['$routeProvider','$translateProvider', function ($routeProvider, $translateProvider) {

    $translateProvider.useSanitizeValueStrategy('sanitize');
    $translateProvider.preferredLanguage('en-US');

    $routeProvider
		.when('/', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/home.html',
		    controller: 'homeController',
            controllerAs: 'vm'
		})

		.when('/help', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/help.html',
		    controller: 'helpController',
		    controllerAs: 'vm'
		})

		.when('/queue/:queue', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/queue.html',
		    controller: 'queueController',
		    controllerAs: 'vm'
		})

		.when('/topic/:topic', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/topic.html',
		    controller: 'topicController',
		    controllerAs: 'vm'
		})

		.when('/topic/:topic/:subscription', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/subscription.html',
		    controller: 'subscriptionController',
		    controllerAs: 'vm'
		})

		.otherwise({ redirectTo: "/" });
}])