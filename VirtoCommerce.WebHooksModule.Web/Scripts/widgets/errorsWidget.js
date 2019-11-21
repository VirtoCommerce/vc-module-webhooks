angular.module('virtoCommerce.webhooksModule')
    .controller('virtoCommerce.webhooksModule.errorsWebhookController', ['$state', '$scope', 'platformWebApp.bladeNavigationService', function ($state, $scope, bladeNavigationService) {
        //var blade = $scope.widget.blade;
        $scope.openBlade = function () {
            var newBlade = {
                id: "webhookErrors",
                webHookId: listItem.id,
                title: 'webhooks.blades.webhook-detail.title',
                subtitle: 'webhooks.blades.webhook-detail.subtitle',
                controller: 'virtoCommerce.webhooksModule.webhookDetailController',
                template: 'Modules/$(virtoCommerce.webhooksModule)/Scripts/blades/webhook-detail.tpl.html'
            };
            bladeNavigationService.showBlade(newBlade, blade);
        
        };
    }]);
