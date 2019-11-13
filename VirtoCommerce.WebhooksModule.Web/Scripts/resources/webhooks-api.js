angular.module('virtoCommerce.webhooksModule')
    .factory('virtoCommerce.webhooksModule.webApi', ['$resource', function ($resource) {
        return $resource('api/webhooks/:id',
            { id: '@id' },
            {
                search: { method: 'POST', url: 'api/webhooks/search' },
                searchFeed: { method: 'POST', url: 'api/webhooks/feed/search' },
                save: { method: 'post', url: 'api/webhooks' },
                send: { method: 'POST', url: 'api/webhooks' }
            });
    }
    ]);