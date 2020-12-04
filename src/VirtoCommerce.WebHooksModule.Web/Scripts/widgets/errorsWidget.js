angular.module('virtoCommerce.webhooksModule')
    .controller('virtoCommerce.webhooksModule.errorsWebhookController', ['$state', '$scope', 'platformWebApp.bladeNavigationService', function ($state, $scope, bladeNavigationService) {
        var blade = $scope.blade;
        $scope.openBlade = function () {
            var newBlade = {
                id: "webhookErrors",
                webHookId: blade.currentEntity.id,
                title: 'webhooks.blades.webhook-error-list.title',
                subtitle: 'webhooks.blades.webhook-error-list.subtitle',
                controller: 'virtoCommerce.webhooksModule.feedListController',
                template: 'Modules/$(virtoCommerce.webhooks)/Scripts/blades/webhook-error-list.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        
        };
    }]);
