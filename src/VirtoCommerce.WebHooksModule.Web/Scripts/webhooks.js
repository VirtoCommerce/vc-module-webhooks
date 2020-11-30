// Call this to register your module to main application
var moduleName = "virtoCommerce.webhooksModule";

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $stateProvider
                .state('workspace.virtoCommerceWebhooksModuleState', {
                    url: '/virtoCommerce.webhooksModule',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        '$scope', 'platformWebApp.bladeNavigationService', function ($scope, bladeNavigationService) {
                            var newBlade = {
                                id: 'webhooks-list',
                                title: 'webhooks.blades.webhooks-list.title',
                                subtitle:'webhooks.blades.webhooks-list.subtitle',
                                controller: 'virtoCommerce.webhooksModule.webhooksListController',
                                template: 'Modules/$(virtoCommerce.webhooksModule)/Scripts/blades/webhooks-list.tpl.html',
                                isClosingDisabled: true
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run(['$rootScope', 'platformWebApp.mainMenuService', 'platformWebApp.widgetService', '$state',
        function ($rootScope, mainMenuService, widgetService, $state) {
            //Register module in main menu
            var menuItem = {
                path: 'browse/virtoCommerce.webhooksModule',
                icon: 'fa fa-rocket',
                title: 'webhooks.main-menu-title',
                priority: 100,
                action: function () { $state.go('workspace.virtoCommerceWebhooksModuleState'); },
                permission: 'webhooks:access'
            };
            mainMenuService.addMenuItem(menuItem);


            //Register widgets to webhookDetail

            var webhookErrorsWidget = {
                controller: 'virtoCommerce.webhooksModule.errorsWebhookController',
                template: 'Modules/$(virtoCommerce.webhooksModule)/Scripts/widgets/errorsWidget.tpl.html'
            };
            widgetService.registerWidget(webhookErrorsWidget, 'webhookDetail');
        }
    ]);
