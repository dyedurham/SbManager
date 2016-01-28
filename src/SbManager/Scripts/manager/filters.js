///Usage <any nguid-bind-html="value | unsafe"></any>
$app.filter('unsafe', function ($sce) { return $sce.trustAsHtml; });
