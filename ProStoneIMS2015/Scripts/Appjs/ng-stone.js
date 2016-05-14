//Stone
var urlStone = ROOT + 'api/stone/';

app.factory('stoneFactory', function ($http) {
    return {
        getModel: function (Id) {
            return $http.get(urlStone + "?QuoteId=" + Id);
        },
        getLookupModel: function () {
            return $http.get(urlStone + "?Lookup");
        },
        getInventoryModel: function (Id) {
            return $http.get(urlStone + "?stoneId=" + Id);
        },
        addModel: function (stone) {
            return $http.post(urlStone, stone);
        },
        deleteModel: function (stone) {
            return $http.delete(urlStone + stone.Id);
        },
        updateModel: function (stone) {
            return $http.put(urlStone + stone.Id, stone);
        },

    };
});

app.controller('stoneController', function myController($scope, stoneFactory) {
    $scope.stoneModel = [];
    $scope.stoneLookup = [];
    $scope.edgeLookup = [];
    $scope.stoneDefault = null;
    $scope.loading = true;
    $scope.addMode = false;
    $scope.isNumber = angular.isNumber;

    //Calc
    $scope.calc = function () {
        var stone;
        if (this.stone) {
            stone = this.stone;
        }
        else {
            stone = $scope.newStoneModel;
        }            
        //$scope.newStoneModel = { StateTax: $scope.stoneDefault.StateTax, SFplus: $scope.stoneDefault.SFplus, FabMin: $scope.stoneDefault.FabMin, FabMinPrice: $scope.stoneDefault.FabMinPrice };
        stone.SquareFeetQty = stone.SF;
        //stone.StateTax = 0.0825;
        stone.Surcharge = 0;        
        stone.SquareFeet = stone.Length * stone.Width / 144;
        stone.NSlabSF = stone.NSlab ? stone.NSlab : Math.ceil(stone.SquareFeetQty / 60); // IIf(Not IsNull([NSlab]);[NSlab];Int([SquareFeetQty]/60)+1)  
        stone.SFPlus = stone.SF * (1 + stone.PlusVal/100);
        stone.Price = stone.PublishedPrice + (stone.PublishedPrice * stone.Surcharge);
        stone.TotPricePSF = stone.Price + (stone.Price * stone.StateTax / 100.00 );
        stone.SlabPrice = stone.TotPricePSF * stone.SquareFeet;
        stone.Subtot = stone.SlabPrice * stone.NSlabSF;
        stone.FabPrice = Math.round(stone.Price * stone.FabMinPrice) < stone.FabMin ? stone.FabMin : Math.round(stone.Price * stone.FabMinPrice);
        stone.FabPricePrintTextbox = Math.round(stone.FabPrice + stone.TotPricePSF);
        stone.FabPricePrint = stone.FabPricePrintOveride == null ? stone.FabPricePrintTextbox : stone.FabPricePrintOveride;
        stone.TotSlabs = stone.NSlabSF * stone.SlabPrice;
        stone.Total = stone.NSlabSF * stone.TotSlabs;    
        stone.SubtotFab = stone.FabPrice * stone.SFPlus;
        stone.SubtotFabPrint = stone.FabPricePrint * stone.SFPlus;
        stone.ExtAmt = stone.PublishedPrice * stone.SquareFeet;

        //$scope.loadInventoryData(stone.Id);
    }


    $scope.edit = function () {
        $scope.originalModel = angular.copy(this.stone);
        this.stone.editMode = true;
    };

    $scope.discard = function () {
        this.stone = angular.copy($scope.originalModel);
        this.stone.editMode = false;
    }

    $scope.toggleAdd = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.result = null;
    $scope.findPrice = function (enteredValue) {
        var stone;
        if (this.stone) {
            stone = this.stone;
        }
        else {
            stone = $scope.newStoneModel;
        }
        angular.forEach($scope.stoneLookup, function (stoneLookup, key) {
            if (stoneLookup.Id == enteredValue) {
                stone.PublishedPrice = stoneLookup.PublishedPrice;
            }

        });
    };

    $scope.save = function () {
        $scope.loading = true;
        var thisModel = this.stone;
        stoneFactory.updateModel(thisModel).success(function (data) {
            $.each($scope.stoneModel, function (i) {
                if ($scope.stoneModel[i].Id === thisModel.Id) {
                    $scope.stoneModel[i] = data;
                }
            });
            //$scope.addAlert('success', 'Saved Successfully!', 5000);
            thisModel.editMode = false;
            $scope.loading = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Saving data! ' + data.ExceptionMessage);
            thisModel.editMode = false;
            $scope.loading = false;
        });
    };

    // add Data
    $scope.add = function (Id) {
        this.newStoneModel.QuoteId = Id;
        $scope.loading = true;
        stoneFactory.addModel(this.newStoneModel).success(function (data) {
            //$scope.addAlert('success', 'Added Successfully!' + data, 5000);
            $scope.stoneModel.push(data);

                $('#stone_null').parent('.collapse').toggleClass('in');
                $(this).parent('.collapse').toggleClass('in');
            $scope.loading = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Adding data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
        this.newStoneModel = null;
        $scope.newStoneModel = { StateTax: $scope.stoneDefault.StateTax, PlusVal: $scope.stoneDefault.PlusVal, FabMin: $scope.stoneDefault.FabMin, FabMinPrice: $scope.stoneDefault.FabMinPrice };
    };

    // delete Data
    $scope.delete = function () {
        var result = confirm("Are you sure?");
        if (result == true) {
            $scope.loading = true;
            var currentModel = this.stone;
            //alert(currentModel.Id);
            stoneFactory.deleteModel(currentModel).success(function (data) {
                $scope.addAlert('warning', 'Deleted Successfully!', 5000);
                $.each($scope.stoneModel, function (i) {
                    if ($scope.stoneModel[i].Id === currentModel.Id) {
                        $scope.stoneModel.splice(i, 1);
                        return false;
                    }
                });
                $scope.loading = false;
            }).error(function (data) {
                $scope.addAlert('danger', 'An Error has occured while Saving data! ' + data.ExceptionMessage);
                $scope.loading = false;
            });
        }
    };

    //get all Data
    $scope.loadData = function (Id) {
        stoneFactory.getLookupModel().success(function (data) {
            $scope.stoneLookup = data.stoneLookup;
            $scope.stoneDefault = data.stoneDefault;
            $scope.edgeLookup = data.edgeLookup;
            stoneFactory.getModel(Id).success(function (data) {
                $scope.stoneModel = data;
                $scope.loading = false;
            })
            .error(function (data) {
                $scope.addAlert('danger', 'An Error has occured while loading Stone data! ' + data.ExceptionMessage);
                $scope.loading = false;
            });
            $scope.newStoneModel = { StateTax: $scope.stoneDefault.StateTax, PlusVal: $scope.stoneDefault.PlusVal, FabMin: $scope.stoneDefault.FabMin, FabMinPrice: $scope.stoneDefault.FabMinPrice };
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while loading Stone Lookup data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

    $scope.loadMeasureData = function (Id, qId) {
        angular.element($('#MeasurePopover')).scope().loading = true;
        angular.element($('#MeasurePopover')).scope().loadMeasure(Id, qId);
    }

    $scope.loadInventoryData = function (Id) {
        angular.element($('#InventoryPopover')).scope().loading = true;
        angular.element($('#InventoryPopover')).scope().loadInventory(Id);
    }

    $scope.reset = function () {
        $scope.$broadcast('show-errors-reset');
        $scope.newStoneModel = null;
        $scope.addMode = false;
    }

    $scope.total = function () {
        var total = 0;
        angular.forEach($scope.stoneModel, function (stone) {
            total += stone.Subtot;
        })
        return total;
    }

    $scope.totalSF = function () {
        var totalSF = 0;
        angular.forEach($scope.stoneModel, function (stone) {
            totalSF += stone.SF * 1;
        })
        return totalSF;
    }

});


app.directive('numberOnly', function () {
    return {
        require: '?ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {
            if (!ngModelCtrl) {
                return;
            }

            ngModelCtrl.$parsers.push(function (val) {
                if (angular.isUndefined(val)) {
                    var val = '';
                }
                var clean = val.replace(/[^0-9\.]/g, '');
                var decimalCheck = clean.split('.');

                if (!angular.isUndefined(decimalCheck[1])) {
                    decimalCheck[1] = decimalCheck[1].slice(0, 3);
                    clean = decimalCheck[0] + '.' + decimalCheck[1];
                }

                if (val !== clean) {
                    ngModelCtrl.$setViewValue(clean);
                    ngModelCtrl.$render();
                }
                return clean;
            });

            element.bind('keypress', function (event) {
                if (event.keyCode === 32) {
                    event.preventDefault();
                }
            });
        }
    };
});



app.directive('collapseToggler', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.on('click', function (e) {
                var sib = $(this).parent().parent().siblings('.collapse');
                sib.toggleClass('in');
                sib.addClass('flagged');
                if(sib.hasClass('in')) {
                    $('#overlay-back').css({ 'position': 'absolute', 'height': $(document).height() });
                    $('#overlay-back').fadeIn(500);
                }

            });
        }
    };
});

app.directive('collapseToggler2', function () {
    return {
        restrict: 'A',
        link: function (scope, elem, attrs) {
            elem.on('click', function () {
                $(this).siblings('.collapse').toggleClass('in');
            });
        }
    };
});


app.directive('inventoryPopover', ['$compile', function ($compile) {

    var popoverBodyData = "<a> {{tooltiplabel}} </a>";
    var popoverTitleData = "<span>Description</span> <button type='button' class='close'>&times;</button>";

    return {
        restrict: 'A',
        link: function (scope, element, attribute, controller) {
            var sib = $(this).parent().parent().siblings('.collapse');
            if (!sib.hasClass('flagged')) {
                scope.tooltiplabel = "Hello Everybody this is PopOver Demo !!!";

                var compliedData = $compile($("#" + attribute.popoverBodydata).contents())(scope);
                var compliedTitle = $compile(popoverTitleData)(scope);

                var getTitle = "<strong>" + attribute.popoverTitle + " - " + $(element).val() + "</strong><button type='button' class='close'>&times;</button>";

                var proc = $compile(getTitle)(scope);

                $(element).popover({
                    'placement': attribute.popoverPlacement,
                    'html': true,
                    'title': proc,
                    'content': compliedData
                });


                $('html').on('mouseup', function (e) {
                    if (!$(e.target).closest('.popover').length) {
                        $('.popover').each(function () {
                            $(this.previousSibling).popover('hide');
                            //angular.element($('#MeasurePopover')).scope().setSF();
                            $('#overlay-back').fadeOut(500);
                        });
                    }
                });

                return $(element).bind('click', function () {
                    var popoverDiv = $(element).next();  // popover div
                    // getting closeBtn handle inside popover div
                    var closeBtn = $($(popoverDiv).children()[1]).children()[1];
                    $(closeBtn).bind('click', function () {
                        $(popoverDiv).popover('hide');
                        //angular.element($('#MeasurePopover')).scope().setSF();
                        $('#overlay-back').fadeOut(500);
                    });
                });
            }
        }
    };
}]);
