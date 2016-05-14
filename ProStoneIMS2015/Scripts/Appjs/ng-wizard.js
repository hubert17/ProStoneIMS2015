var gabsApp = angular.module("gabsApp", ['ui.bootstrap']);


gabsApp.controller("MeasureCtrl", ['$scope', '$http', function ($scope, $http) {
    $scope.measureSets = {
        items: [{
            setId: null,
        }]
    };

    $scope.measures = {
        items: [{
            length: '',
            measurename: '',
            width: ''
        }]
    };

    $scope.addItem = function () {
        var count = $scope.measures.items.length + 1;
        $scope.measures.items.push({
            length: '',
            measurename: count,
            width: ''
        });
    },

    $scope.removeItem = function (index) {
        $scope.measures.items.splice(index, 1);
    },

    $scope.total = function () {
        var total = 0;
        angular.forEach($scope.measures.items, function (item) {
            total += (item.length * item.width) / 144;
        })
        $scope.Math = window.Math;
        return Math.ceil(total);
    }

    $scope.TotalInput = function () {
        if ($scope.total() > 0)
            return true;
        else
            return false;
    };

    $scope.showhide = true;
    $scope.toggleShowhide = function () {
        $scope.showhide = $scope.showhide === false ? true : false;
    };

}]);


//Inventory
var urlStone = ROOT + 'api/stone/';

gabsApp.factory('inventoryFactory', function ($http) {
    return {
        getCurrentInv: function (QuoteId) {
            return $http.get(urlStone + "?QuoteId=" + QuoteId);
        },
        getModel: function (Id) {
            return $http.get(urlStone + "?stoneId=" + Id);
        },
        updateModel: function (Id, QuoteId, EdgeId) {
            return $http.put(urlStone + "?Id=" + Id + "&QuoteId=" + QuoteId + "&EdgeId=" + EdgeId);
        }
    };
});
gabsApp.controller('StoneCtrl', function myController($scope, inventoryFactory, shareDataService) {
    $scope.loading = true;
    this.saving = false;
    $scope.inventoryModel = [];
    $scope.holdinv = [];

    $scope.holdInventory = function (EdgeId) {
        if (this.inventory.QuoteId != -1) {
            this.inventory.QuoteId = -1;
            var inventory = angular.copy(this.inventory);
            inventory.SerialNo = EdgeId;
            inventory.LotNo = $("#ddlEdge option[value='" + EdgeId + "']").text();
            $scope.holdinv.push(inventory);
        }
        else {
            for (var i = $scope.holdinv.length - 1; i >= 0; i--) {
                if ($scope.holdinv[i].Id == this.inventory.Id) {
                    $scope.holdinv.splice(i, 1);
                }
            }
            this.inventory.QuoteId = null;
        }
    };

    $scope.holdEditInventory = function (EdgeId, QuoteId) {
        if (this.inventory.QuoteId == null) {
            this.inventory.QuoteId = QuoteId;
            var inventory = angular.copy(this.inventory);
            inventory.EdgeId = EdgeId;
            inventory.EdgeName = $("#ddlEdge option[value='" + EdgeId + "']").text();

            inventoryFactory.updateModel(inventory.Id, QuoteId, EdgeId).success(function (data) {
                $scope.holdinv.push(inventory);
                this.saving = false;
            }).error(function (data) {
                this.saving = false;
            });

            this.saving = true;
        }
        else {
            inventoryFactory.updateModel(this.inventory.Id, null, EdgeId).success(function (data) {
                for (var i = $scope.holdinv.length - 1; i >= 0; i--) {
                    if ($scope.holdinv[i].Id == data.Id) {
                        $scope.holdinv.splice(i, 1);
                    }
                }
                this.inventory.QuoteId = null;
                this.saving = false;
            }).error(function (data) {
                this.saving = false;
            });

        }
    };


    //get all Data
    $scope.loadInventory = function (Id) {
        inventoryFactory.getModel(Id).success(function (data) {
            //alert('heo ' + data)
            $scope.inventoryModel = data;
            //$scope.holdinv = [];

            angular.forEach($scope.inventoryModel, function (inventoryModel, key) {
                angular.forEach($scope.holdinv, function (holdinv, key) {
                    if (holdinv.Id != null && holdinv.Id == inventoryModel.Id) {
                        inventoryModel.QuoteId = -1;
                    }
                });
            });


            $scope.loading = false;
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while loading Inventory data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

    $scope.currentInventory = function (QuoteId) {
        inventoryFactory.getCurrentInv(QuoteId).success(function (data) {
            $scope.holdinv = data;
            //angular.forEach($scope.inventoryModel, function (inventoryModel, key) {
            //    angular.forEach($scope.holdinv, function (holdinv, key) {
            //        if (holdinv.Id != null && holdinv.Id == inventoryModel.Id) {
            //            inventoryModel.QuoteId = -1;
            //        }
            //    });
            //});
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.loading = false;
        });
    };


    $scope.total = function () {
        var total = 0;
        angular.forEach($scope.holdinv, function (inv) {
            total += inv.Total;
        })
        shareDataService.stoneTotal(total);
        return total;
    }
});



var urlSink = ROOT + 'api/sink/';
gabsApp.factory('sinkInventoryFactory', function ($http) {
    return {
        getCurrentInv: function (QuoteId) {
            return $http.get(urlSink + "?QId=" + QuoteId);
        },
        getModel: function (Id) {
            return $http.get(urlSink + "?sinkId=" + Id);
        },
        updateModel: function (Id, QuoteId) {
            return $http.put(urlSink + "?Id=" + Id + "&QuoteId=" + QuoteId);
        }
    };
});
gabsApp.controller('SinkCtrl', function myController($scope, sinkInventoryFactory, shareDataService) {
    $scope.loading = true;
    this.saving = false;
    $scope.inventoryModel = [];
    $scope.holdinv = [];

    $scope.holdInventory = function () {
        if (this.sinkInventory.QuoteId != -1) {
            this.sinkInventory.QuoteId = -1;
            var sinkInventory = angular.copy(this.sinkInventory);
            $scope.holdinv.push(sinkInventory);
        }
        else {
            for (var i = $scope.holdinv.length - 1; i >= 0; i--) {
                if ($scope.holdinv[i].Id == this.sinkInventory.Id) {
                    $scope.holdinv.splice(i, 1);
                }
            }
            this.sinkInventory.QuoteId = null;
        }
    };

    $scope.holdEditInventory = function (QuoteId) {
        if (this.sinkInventory.QuoteId == null) {
            this.sinkInventory.QuoteId = QuoteId;
            var sinkInventory = angular.copy(this.sinkInventory);

            sinkInventoryFactory.updateModel(sinkInventory.Id, QuoteId).success(function (data) {
                $scope.holdinv.push(sinkInventory);
                this.saving = false;
            }).error(function (data) {
                this.saving = false;
            });

            this.saving = true;
        }
        else {
            sinkInventoryFactory.updateModel(this.sinkInventory.Id, null).success(function (data) {
                for (var i = $scope.holdinv.length - 1; i >= 0; i--) {
                    if ($scope.holdinv[i].Id == data.Id) {
                        $scope.holdinv.splice(i, 1);
                    }
                }
                this.sinkInventory.QuoteId = null;
                this.saving = false;
            }).error(function (data) {
                this.saving = false;
            });

        }
    };

    //get all Data
    $scope.loadInventory = function (Id) {
        sinkInventoryFactory.getModel(Id).success(function (data) {
            //alert('heo ' + data)
            $scope.inventoryModel = data;
            //$scope.holdinv = [];

            angular.forEach($scope.inventoryModel, function (inventoryModel, key) {
                angular.forEach($scope.holdinv, function (holdinv, key) {
                    if (holdinv.Id != null && holdinv.Id == inventoryModel.Id) {
                        inventoryModel.QuoteId = -1;
                    }
                });
            });


            $scope.loading = false;
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while loading Inventory data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

    $scope.currentInventory = function (QuoteId) {
        sinkInventoryFactory.getCurrentInv(QuoteId).success(function (data) {
            $scope.holdinv = data;
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.loading = false;
        });
    };


    $scope.total = function () {
        var total = 0;
        angular.forEach($scope.holdinv, function (inv) {
            total += inv.Total;
        })
        shareDataService.sinkTotal(total);
        return total;
    }

});

gabsApp.controller('SummaryCtrl', function myController($scope, inventoryFactory, shareDataService) {
    $scope.$watch(function () {
        $scope.stoneTotal = shareDataService.getStoneTotal();
        $scope.sinkTotal = shareDataService.getSinkTotal();
        var dblFabPrice = parseFloat($scope.FabPrice);
        if (!isNaN(dblFabPrice) && angular.isNumber(dblFabPrice)) {
            $scope.JobCost = shareDataService.getTotal() + dblFabPrice;
        }
        else {
            $scope.JobCost = shareDataService.getTotal();
        }
    });

});

gabsApp.service('shareDataService', function () {
    var tStone = 0;
    var tSink = 0;

    var stoneTotal = function (data) {
        tStone = data;
    }

    var sinkTotal = function (data) {
        tSink = data;
    }

    var getStoneTotal = function () {
        return tStone;
    }

    var getSinkTotal = function () {
        return tSink;
    }

    var getTotal = function () {
        return tStone + tSink;
    }

    return {
        stoneTotal: stoneTotal,
        sinkTotal: sinkTotal,
        getStoneTotal: getStoneTotal,
        getSinkTotal: getSinkTotal,
        getTotal: getTotal
    };
});


gabsApp.controller("_SinkCtrl", ['$scope', '$http', function ($scope, $http) {

    $scope.sinks = {
        items: [{
            sinkID: '',
            sinkqty: '1',
            sinkprice: null,
        }]
    };

    $scope.addItem = function () {
        $scope.sinks.items.push({
            sinkID: '',
            sinkqty: '1',
            sinkprice: null,
        });
    },

    $scope.setDdlSink = function (index, SinkID) {
        //$scope.sinks.items.item.sinkID
    },

    $scope.removeItem = function (index) {
        $scope.sinks.items.splice(index, 1);
    },

    $scope.showhide = true;
    $scope.toggleShowhide = function () {
        $scope.showhide = $scope.showhide === false ? true : false;
    };

}]);

gabsApp.controller("ServiceCtrl", ['$scope', '$http', function ($scope, $http) {

    $scope.services = {
        items: [{
            serviceID: '',
            serviceqty: '1',
            serviceprice: null,
        }]
    };

    $scope.addItem = function () {
        $scope.services.items.push({
            serviceID: '',
            serviceqty: '1',
            serviceprice: null,
        });
    },

    $scope.setDdlService = function (index, ServiceID) {
        //$scope.services.items.item.serviceID
    },

    $scope.removeItem = function (index) {
        $scope.services.items.splice(index, 1);
    },

    $scope.showhide = true;
    $scope.toggleShowhide = function () {
        $scope.showhide = $scope.showhide === false ? true : false;
    };

}]);


gabsApp.filter('ceil', function () {
    return function (input) {
        return Math.ceil(input);
    };
});

gabsApp.directive('ngModelOnblur', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        priority: 1,
        link: function (scope, elm, attr, ngModelCtrl) {
            if (attr.type === 'radio' || attr.type === 'checkbox') return;

            elm.unbind('input').unbind('keydown').unbind('change');
            elm.bind('blur', function () {
                scope.$apply(function () {
                    ngModelCtrl.$setViewValue(elm.val());
                });
            });
        }
    };
});

gabsApp.directive('elastic', [
    '$timeout',
    function ($timeout) {
        return {
            restrict: 'A',
            link: function ($scope, element) {
                var resize = function () {
                    return element[0].style.height = "" + element[0].scrollHeight + "px";
                };
                element.on("blur keyup change", resize);
                $timeout(resize, 0);
            }
        };
    }
]);


