angular.module('virtoCommerce.webhooksModule')
    .factory('virtoCommerce.webhooksModule.webApi', ['$resource', function ($resource) {
        return $resource('api/VirtoCommerceWebhooksModule');
}]);
