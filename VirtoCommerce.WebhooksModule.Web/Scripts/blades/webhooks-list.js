angular.module('virtoCommerce.webhooksModule')
    .controller('virtoCommerce.webhooksModule.webhooksListController', ['$scope', 'virtoCommerce.webhooksModule.webApi', 'platformWebApp.bladeNavigationService', 'platformWebApp.dialogService', 'platformWebApp.authService', 'platformWebApp.uiGridHelper', 'platformWebApp.bladeUtils', function ($scope, api, bladeNavigationService, dialogService, authService, uiGridHelper, bladeUtils) {
        $scope.uiGridConstants = uiGridHelper.uiGridConstants;
        var blade = $scope.blade;
        var selectedNode = null;

        blade.title = 'Webhoks list';
        blade.currentEntities = [{ name: 'AzureLogicApp1', active: 'active', calls: '10', errors: '3' }];

        blade.refresh = function () {
            blade.isLoading = false;
            $scope.pageSettings.totalItems = blade.currentEntities.length;
        };

        //blade.refresh();

        $scope.editWebhook = function(item) {

        };

        $scope.deleteWebhook = function(item) {

        };


        // simple and advanced filtering
        var filter = blade.filter = { keyword: null };

        filter.criteriaChanged = function () {

        };

        // ui-grid
        $scope.setGridOptions = function (gridOptions) {
            uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                //update gridApi for current grid
                $scope.gridApi = gridApi;

                uiGridHelper.bindRefreshOnSortChanged($scope);
            });
            bladeUtils.initializePagination($scope);
        };


    }]);