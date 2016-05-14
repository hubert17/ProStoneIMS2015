//Measure
var urlMeasure = ROOT + 'api/measure/';

gabsApp.factory('measureFactory', function ($http) {
    return {
        getModel: function (Id, qId) {
            return $http.get(urlMeasure + "?QuoteStoneId=" + Id + "&QuoteId=" + qId);
        },
        getModelByQuote: function (Id) {
            return $http.get(urlMeasure + "?QuoteId=" + Id);
        },
        getLookupModel: function () {
            return $http.get(urlMeasure + "?Lookup");
        },
        addModel: function (measure) {
            return $http.post(urlMeasure, measure);
        },
        deleteModel: function (measure) {
            return $http.delete(urlMeasure + measure.Id);
        },
        updateModel: function (measure) {
            return $http.put(urlMeasure + measure.Id, measure);
        },

    };
});

gabsApp.controller('measureController', function myController($scope, measureFactory) {
    $scope.measureModel = [];
    $scope.measureLookup = [];
    $scope.loading = true;
    $scope.adding = false;
    $scope.saving = false;
    $scope.deleting = false;
    $scope.addMode = true;
    $scope.isNumber = angular.isNumber;
    $scope.qsId = null;
    $scope.total = 0;

    $scope.edit = function () {
        $scope.originalModel = angular.copy(this.measure);
        this.saving = false;
        this.measure.editMode = true;
    };

    $scope.discard = function () {
        this.measure = angular.copy($scope.originalModel);
        this.measure.editMode = false;
    }

    $scope.toggleAdd = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.saveMeasure = function () {
        this.saving = true;
        var thisModel = this.measure;
        measureFactory.updateModel(thisModel).success(function (data) {
            thisModel.editMode = false;
            this.saving = false;
            $scope.total = $scope.totalArea();
        }).error(function (data) {
            thisModel.editMode = false;
            $.each($scope.measureModel, function (i) {
                if ($scope.measureModel[i].Id === thisModel.Id) {
                    $scope.measureModel[i] = angular.copy($scope.originalModel);
                }
            });
            this.saving = false;
        });
    };

    // add Data
    $scope.addMeasure = function (Id) {
        this.newMeasureModel.QuoteId = Id;
        $scope.adding = true;
        measureFactory.addModel(this.newMeasureModel).success(function (data) {
            $scope.measureModel.push(data);
            $scope.adding = false;
            $scope.total = $scope.totalArea();
            $scope.newMeasureModel = { MeasureName: $scope.measureModel.length + 1, Length: null, Width: null };
        }).error(function (data) {
            $scope.adding = false;
            $scope.newMeasureModel = { MeasureName: $scope.measureModel.length + 1, Length: null, Width: null };
        });
        this.newMeasureModel = null;
    };

    // delete Data
    $scope.deleteMeasure = function () {
        var result = confirm("Are you sure?");
        if (result == true) {
            this.deleting = true;
            var currentModel = this.measure;
            //alert(currentModel.Id);
            measureFactory.deleteModel(currentModel).success(function (data) {
                $.each($scope.measureModel, function (i) {
                    if ($scope.measureModel[i].Id === currentModel.Id) {
                        $scope.measureModel.splice(i, 1);
                        return false;
                    }
                });
                this.deleting = false;
                $scope.total = $scope.totalArea();
                $scope.newMeasureModel = { MeasureName: $scope.measureModel.length + 1, Length: null, Width: null };
            }).error(function (data) {
                this.deleting = false;
            });
        }
    };

    //get all Data
    $scope.loadMeasure = function (Id) {
        measureFactory.getModelByQuote(Id).success(function (data) {
            $scope.qsId = Id;
            $scope.measureModel = data;
            $scope.newMeasureModel = { MeasureName: $scope.measureModel.length + 1, Length: null, Width: null };
            $scope.loading = false;
            $scope.total = $scope.totalArea();
        })
        .error(function (data) {
            $scope.loading = false;
        });

    };

    $scope.setSF = function () {
        var stoneSF = $('#stone_' + $scope.qsId + ' .SF');
        if ($scope.total()) {
            stoneSF.val($scope.total());
        }
        else {
            stoneSF.val(null);
        }
        angular.element(stoneSF).triggerHandler('change');
    }

    $scope.totalArea = function () {
        var totalArea = 0;
        angular.forEach($scope.measureModel, function (measure) {
            totalArea += (measure.Length * measure.Width) / 144;
        })
        return Math.ceil(totalArea);
    }

    //$scope.total = function () {
    //    var total = 0;
    //    angular.forEach($scope.measureModel, function (measure) {
    //        total += (measure.Length * measure.Width) / 144;
    //    })
    //    $scope.Math = window.Math;
    //    return Math.ceil(total);
    //}


});

gabsApp.directive("measureLookup", function ($compile) {
    function linker(scope, element, attrs) {
        //p as p.MeasureName for p in measureLookup
        var template = '<input type="text" name="MeasureName" data-ng-model="' + attrs.model + '"  typeahead=" m.MeasureName for m in measureLookup | filter:$viewValue | limitTo:8" typeahead-min-length="1" typeahead-wait-ms="200"  class="form-control input-xs col-md-1"></input>';
        element.html(template);
        $compile(element.contents())(scope);
        //var el = $compile(element.contents())(scope);
        //element.replaceWith(el);
    }

    return {
        restrict: 'E',
        replace: true,
        link: linker
    };
})

