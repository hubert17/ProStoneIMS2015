//Sink
var urlSink = ROOT + 'api/sink/';

app.factory('sinkFactory', function ($http) {
    return {
        getModel: function (Id) {
            return $http.get(urlSink + "?QuoteId=" + Id);
        },
        addModel: function (sink) {
            return $http.post(urlSink, sink);
        },
        deleteModel: function (sink) {
            return $http.delete(urlSink + sink.Id);
        },
        updateModel: function (sink) {
            return $http.put(urlSink + sink.Id, sink);
        },

    };
});

app.controller('sinkController', function myController($scope, sinkFactory) {
    $scope.sinkModel = [];
    $scope.sinkLookup = [];
    $scope.loading = true;
    $scope.adding = false;
    $scope.saving = false;
    $scope.deleting = false;
    $scope.addMode = true;
    $scope.isNumber = angular.isNumber;

    $scope.edit = function () {
        $scope.originalModel = angular.copy(this.sink);
        this.sink.editMode = true;
    };

    $scope.discard = function () {
        this.sink = angular.copy($scope.originalModel);
        this.sink.editMode = false;
    }

    $scope.toggleAdd = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.result = null;
    $scope.findPrice = function (enteredValue) {
        angular.forEach($scope.sinkLookup, function (sinkLookup, key) {
            if (sinkLookup.Id == enteredValue) {
                $scope.newSinkModel.Price = sinkLookup.Price;
                $scope.newSinkModel.CatalogID = sinkLookup.CatalogID;
            }

        });
    };

    $scope.save = function () {
        this.saving = true;
        var thisModel = this.sink;
        sinkFactory.updateModel(thisModel).success(function (data) {
            $.each($scope.sinkModel, function (i) {
                if ($scope.sinkModel[i].Id === thisModel.Id) {
                    $scope.sinkModel[i] = data;
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
        this.newSinkModel.QuoteId = Id;
        $scope.adding = true;
        sinkFactory.addModel(this.newSinkModel).success(function (data) {
            //$scope.addAlert('success', 'Added Successfully!' + data, 5000);
            $scope.sinkModel.push(data);
            $scope.adding = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Adding data! ' + data.ExceptionMessage);
            $scope.adding = false;
        });
        this.newSinkModel = null;
        $scope.newSinkModel = { Quantity: 1 };
    };

    // delete Data
    $scope.delete = function () {
        var result = confirm("Are you sure?");
        if (result == true) {
            this.deleting = true;
            var currentModel = this.sink;
            //alert(currentModel.Id);
            sinkFactory.deleteModel(currentModel).success(function (data) {
                $scope.addAlert('warning', 'Deleted Successfully!', 5000);
                $.each($scope.sinkModel, function (i) {
                    if ($scope.sinkModel[i].Id === currentModel.Id) {
                        $scope.sinkModel.splice(i, 1);
                        return false;
                    }
                });
                this.deleting = false;
            }).error(function (data) {
                $scope.addAlert('danger', 'An Error has occured while Saving data! ' + data.ExceptionMessage);
                this.deleting = false;
            });
        }
    };

    //get all Data
    $scope.loadData = function (Id) {
        sinkFactory.getModel(Id).success(function (data) {
            $scope.sinkModel = data.sinkData;
            $scope.sinkLookup = data.sinkLookup;
            $scope.newSinkModel = { Quantity: 1 };
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Loading data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

    $scope.reset = function () {
        $scope.$broadcast('show-errors-reset');
        $scope.newSinkModel = null;
        $scope.newSinkModel = { Quantity: 1 };
    }

    $scope.total = function () {
        var total = 0;
        angular.forEach($scope.sinkModel, function (sink) {
            total += sink.Price * sink.Quantity;
        })
        return total;
    }

});
