//Service
var urlService = ROOT + 'api/service/';

app.factory('serviceFactory', function ($http) {
    return {
        getModel: function (Id) {
            return $http.get(urlService + "?QuoteId=" + Id);
        },
        addModel: function (service) {
            return $http.post(urlService, service);
        },
        deleteModel: function (service) {
            return $http.delete(urlService + service.Id);
        },
        updateModel: function (service) {
            return $http.put(urlService + service.Id, service);
        },

    };
});

app.controller('serviceController', function myController($scope, serviceFactory) {
    $scope.serviceModel = [];
    $scope.serviceLookup = [];
    $scope.loading = true;
    $scope.saving = false;
    $scope.adding = false;
    $scope.deleting = false;
    $scope.addMode = true;
    $scope.isNumber = angular.isNumber;

    $scope.stringIsNumber = function(s) {
        var x = +s; // made cast obvious for demonstration
        return x.toString() === s;
    }

    $scope.edit = function () {
        $scope.originalModel = angular.copy(this.service);
        this.service.editMode = true;
    };

    $scope.discard = function () {
        this.service = angular.copy($scope.originalModel);
        this.service.editMode = false;
    }

    $scope.toggleAdd = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.result = null;
    $scope.findPrice = function (enteredValue) {
        var service;
        if (this.service) {
            service = this.service;
        }
        else {
            service = $scope.newServiceModel;
        }
        angular.forEach($scope.serviceLookup, function (serviceLookup, key) {
            if (serviceLookup.Id == enteredValue) {
                service.Price = serviceLookup.Price;
                service.CatalogID = serviceLookup.CatalogID;
                service.ServiceCode = serviceLookup.ServiceCode;
                if (service.ServiceCode == 'T') {
                    service.Quantity = angular.element($('#stone-tab')).scope().totalSF();
                }
                else if (service.ServiceCode == 'Q') {
                    service.Quantity = null;
                }
                else {
                    service.Quantity = 1;
                }
            }
        });
    };

    $scope.save = function () {
        this.saving = true;
        var thisModel = this.service;
        serviceFactory.updateModel(thisModel).success(function (data) {
            $.each($scope.serviceModel, function (i) {
                if ($scope.serviceModel[i].Id === thisModel.Id) {
                    $scope.serviceModel[i] = data;
                }
            });
            //$scope.addAlert('success', 'Saved Successfully!', 5000);
            thisModel.editMode = false;
            this.saving = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Saving data! ' + data.ExceptionMessage);
            this.saving = false;
        });
    };

    // add Data
    $scope.add = function (Id) {
        this.newServiceModel.QuoteId = Id;
        $scope.adding = true;
        serviceFactory.addModel(this.newServiceModel).success(function (data) {
            //$scope.addAlert('success', 'Added Successfully!' + data, 5000);
            $scope.serviceModel.push(data);
            $scope.adding = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Adding data! ' + data.ExceptionMessage);
            $scope.adding = false;
        });
        this.newServiceModel = null;
        $scope.newServiceModel = { Quantity: 1 };
    };

    // delete Data
    $scope.delete = function () {
        var result = confirm("Are you sure?");
        if (result == true) {
            this.deleting = true;
            var currentModel = this.service;
            serviceFactory.deleteModel(currentModel).success(function (data) {
                //$scope.addAlert('warning', 'Deleted Successfully!', 5000);
                $.each($scope.serviceModel, function (i) {
                    if ($scope.serviceModel[i].Id === currentModel.Id) {
                        $scope.serviceModel.splice(i, 1);
                        return false;
                    }
                });
                this.deleting = false;
            }).error(function (data) {
                $scope.addAlert('danger', 'An Error has occured while Deleting data! ' + data.ExceptionMessage);
                this.deleting = false;
            });
        }
    };

    //get all Data
    $scope.loadData = function (Id) {
        serviceFactory.getModel(Id).success(function (data) {
            $scope.serviceModel = data.serviceData;
            $scope.serviceEdgeModel = data.serviceEdge;
            $scope.serviceLookup = data.serviceLookup;
            $scope.newServiceModel = { Quantity: 1 };
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Loading data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

    $scope.reset = function () {
        $scope.$broadcast('show-errors-reset');
        $scope.newServiceModel = null;
        $scope.newServiceModel = { Quantity: 1 };
    }

    $scope.total = function () {
        var stotal = 0;
        var etotal = 0;
        var total = 0;
        angular.forEach($scope.serviceModel, function (service) {
            stotal += service.Price * service.Quantity;
        })
        angular.forEach($scope.serviceEdgeModel, function (service) {
            etotal += service.Price * service.Quantity;
        })
        return stotal + etotal;
    }

});
