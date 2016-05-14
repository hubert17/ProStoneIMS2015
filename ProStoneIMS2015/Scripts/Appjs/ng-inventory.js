//Inventory
var urlStone = ROOT + 'api/stone/';

app.factory('inventoryFactory', function ($http) {
    return {
        getModel: function (Id) {
            return $http.get(urlStone + "?stoneId=" + Id);
        },
        updateModel: function (Id, QuoteId) {
            return $http.put(urlStone + "?Id=" + Id + "&QuoteId=" + QuoteId);
        }
    };
});

app.controller('inventoryController', function myController($scope, inventoryFactory) {
    $scope.loading = true;
    this.saving = false;
    $scope.inventoryModel = [];

    $scope.holdInventory = function (QuoteId) {
        this.saving = true;
        var invId = this.inventory.Id;
        inventoryFactory.updateModel(invId, QuoteId).success(function (data) {
            $.each($scope.inventoryModel, function (i) {
                if ($scope.inventoryModel[i].Id === invId) {
                    $scope.inventoryModel[i].QuoteId = data.QuoteId;
                }
            });
            this.saving = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Saving data! ' + data.ExceptionMessage);
            this.saving = false;
        });
    };

    //get all Data
    $scope.loadInventory = function (Id) {
        inventoryFactory.getModel(Id).success(function (data) {
            //alert('heo ' + data)
            $scope.inventoryModel = data;
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while loading Inventory data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };
});


