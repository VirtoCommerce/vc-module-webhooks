angular.module('virtoCommerce.webhooksModule')
    .controller('virtoCommerce.webhooksModule.webhookDetailController', ['$rootScope', '$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'virtoCommerce.webhooksModule.webApi', 'platformWebApp.metaFormsService', function ($rootScope, $scope, dialogService, bladeNavigationService, webhookApi, metaFormsService) {
        var blade = $scope.blade;
        blade.availableContentTypes = [{ value: 'application/json', title: 'application/json' }];
        blade.availableEvents = [];
        blade.availablePayloadProperties = [];

        blade.metaFields = metaFormsService.getMetaFields("webhookDetail");

        blade.refresh = function(parentRefresh) {
            blade.isLoading = true;
            if (blade.isNew) {
                initializeBlade({});
            } else {
                webhookApi.get({ id: blade.currentEntityId },
                    function(data) {
                        initializeBlade(data);
                        if (parentRefresh) {
                            blade.parentBlade.refresh(parentRefresh);
                        }
                    });
            }
        };

        function initializeBlade(data) {
            data.contentType = blade.availableContentTypes[0].value;
            data.events = _.map(data.events, function (event) { return { eventId: event.eventId }; });

            blade.item = angular.copy(data);

            blade.currentEntity = blade.item || {};
            blade.currentEntity.payloads = blade.currentEntity.payloads || [];
            blade.subscribedEvent = angular.copy(blade.currentEntity.events[0]);

            // If the webhook is subscribed to multiple events we considering is as a bad practice and recommend to remove these type webhooks
            blade.isBadData = (blade.currentEntity.events && blade.currentEntity.events.length > 1) || blade.currentEntity.isAllEvents;
            blade.origEntity = data;

            webhookApi.getEvents(function (response) {
                blade.availableEvents = _.map(response, function (value) { return { eventId: value.id }; });
                blade.isLoading = false;
            });

            blade.title = blade.isNew ? 'webhooks.blades.webhook-detail.title' : data.name;
            blade.subtitle = 'webhooks.blades.webhook-detail.subtitle';
        }

        function saveChanges() {
            blade.isLoading = true;
            var promise = saveOrUpdate();
            promise.catch(function (error) {
                bladeNavigationService.setError('Error ' + error.status, blade);
            }).finally(function () {
                blade.isLoading = false;
            });
        }

        function saveOrUpdate() {
            return webhookApi.save([blade.currentEntity], function (data) {
                blade.isNew = false;
                blade.currentEntityId = data[0].id;
                blade.refresh(true);
            }).$promise;
        }

        function deleteEntry() {
            var dialog = {
                id: "confirmDelete",
                title: "webhooks.dialogs.setting-delete.title",
                message: "webhooks.dialogs.setting-delete.message",
                callback: function (remove) {
                    if (remove) {
                        blade.isLoading = true;
                        webhookApi.remove({ ids: [blade.currentEntityId] }, function () {
                            bladeNavigationService.closeBlade(blade, function () {
                                blade.parentBlade.refresh(true);
                            });
                        });
                    }
                }
            };
            dialogService.showConfirmationDialog(dialog);
        }

        var detailForm;

        $scope.setForm = function(form) {
            detailForm = form;
        };

        function isDirty() {
            return !angular.equals(blade.currentEntity, blade.origEntity) && blade.hasUpdatePermission();
        }

        function canSave() {
            return isDirty() && detailForm && detailForm.$valid && !blade.isBadData;
        }

        function loadProperties(eventType) {
            blade.isLoading = true;
            webhookApi.getProperties({ objectType: eventType }, (data) => {
                
                blade.availablePayloadProperties = _.map(data.properties, function (property) { return { eventPropertyName: property }; });
                $scope.isNoChoices = !data.discovered;

                blade.isLoading = false;
            });
        }

        $scope.reducePayload = () => {
            if (blade.currentEntity && blade.currentEntity.payloads.length > 10) {
                blade.currentEntity.payloads.pop();
            }
        };

        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "webhooks.dialogs.setting-save.title", "webhooks.dialogs.setting-save.message");
        };

        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
                permission: blade.updatePermission,
                executeMethod: function () {
                    saveChanges();
                },
                canExecuteMethod: function () {
                    return canSave();
                }
            },
            {
                name: "platform.commands.reset", icon: 'fa fa-undo',
                executeMethod: function () {
                    angular.copy(blade.origEntity, blade.currentEntity);
                    blade.subscribedEvent = angular.copy(blade.currentEntity.events[0]);
                },
                canExecuteMethod: isDirty
            },
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                permission: 'webhooks:delete',
                executeMethod: deleteEntry,
                canExecuteMethod: function () {
                    return !blade.isNew;
                }
            }
        ];

        $scope.$watch('blade.subscribedEvent', function(newValue, oldValue) {

            // User has changed event, so we need to clean previous payloads 
            if (oldValue && blade.currentEntity.events && blade.currentEntity.events[0].eventId !== newValue.eventId) {
                blade.currentEntity.payloads = [];
            }

            if (newValue !== oldValue) {

                loadProperties(newValue.eventId);

                blade.currentEntity.events = [newValue];
            }
            
        });

        blade.refresh();

    }]);
