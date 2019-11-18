angular.module('virtoCommerce.webhooksModule')
	.factory('virtoCommerce.webhooksModule.webApi', ['$resource', function ($resource) {
		return $resource('api/webhooks/:id',
			{ id: '@id' },
			{
				search: { method: 'POST', url: 'api/webhooks/search' },
				searchFeed: { method: 'POST', url: 'api/webhooks/feed/search' },
				save: { method: 'POST', url: 'api/webhooks', isArray: true },
                send: { method: 'POST', url: 'api/webhooks/send' },
                getEvents: { method: 'GET', url: 'api/webhooks/events', isArray: true}
			});
	}
	]);