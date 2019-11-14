angular.module('virtoCommerce.webhooksModule')
    .controller('virtoCommerce.webhooksModule.webhookDetailController', ['$rootScope', '$scope', 'platformWebApp.dialogService', 'platformWebApp.bladeNavigationService', 'virtoCommerce.webhooksModule.webApi', 'platformWebApp.metaFormsService', function ($rootScope, $scope, dialogService, bladeNavigationService, webhookApi, metaFormsService) {
        var blade = $scope.blade;
        blade.availableContentTypes = [{ value: 'application/json', title: 'application/json' }];
        blade.availableEvents = ['Order:create', 'Order:edit'];

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
            blade.item = angular.copy(data);
            blade.currentEntity = blade.item;
            blade.currentEntity.contentType = blade.availableContentTypes[0].value;
            blade.currentEntity.events = [];
            blade.origEntity = data;
            blade.isLoading = false;

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
                        webhookApi.delete([blade.currentEntityId], function () {
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
            return isDirty() && detailForm && detailForm.$valid;
        }


        blade.onClose = function (closeCallback) {
            bladeNavigationService.showConfirmationIfNeeded(isDirty(), canSave(), blade, $scope.saveChanges, closeCallback, "webhooks.dialogs.setting-save.title", "webhooks.dialogs.setting-save.message");
        };

        blade.toolbarCommands = [
            {
                name: "platform.commands.save", icon: 'fa fa-save',
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
                },
                canExecuteMethod: isDirty
            },
            {
                name: "platform.commands.delete", icon: 'fa fa-trash-o',
                executeMethod: deleteEntry,
                canExecuteMethod: function () {
                    return !blade.isNew;
                }
            }
        ];

        blade.refresh();

    }]);
