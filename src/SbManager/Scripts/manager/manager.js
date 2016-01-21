var $app = angular.module('manager', ['ngRoute', 'ui.bootstrap','flash']);
window.applicationBasePath = "";

$app.config(function ($routeProvider) {
    $routeProvider

		.when('/', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/home.html',
		    controller: 'homeController'
		})

		.when('/help', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/help.html',
		    controller: 'helpController'
		})

		.when('/queue/:queue', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/queue.html',
		    controller: 'queueController'
		})

		.when('/topic/:topic', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/topic.html',
		    controller: 'topicController'
		})

		.when('/topic/:topic/:subscription', {
		    templateUrl: window.applicationBasePath + '/Content/tmpl/manager/subscription.html',
		    controller: 'subscriptionController'
		})

		.otherwise({ redirectTo: "/" });
    ;
})