//Sink
var urlHistory = ROOT + 'api/history/';

app.factory('historyFactory', function ($http) {
    return {
        getModel: function (Id) {
            return $http.get(urlHistory + Id);
        },
        deleteModel: function (history) {
            return $http.delete(urlHistory + history.Id);
        },
    };
});

app.controller('historyController', function myController($scope, historyFactory) {
    $scope.historyModel = null;
    $scope.loading = false;

    // delete Data
    $scope.delete = function () {
        //var result = confirm("Are you sure?");
        var result = true;
        if (result == true) {
            $scope.loading = true;
            var currentModel = this.history;
            historyFactory.deleteModel(currentModel).success(function (data) {
                $.each($scope.historyModel, function (i) {
                    if ($scope.historyModel[i].Id === currentModel.Id) {
                        $scope.historyModel.splice(i, 1);
                        return false;
                    }
                });
                $scope.loading = false;
            }).error(function (data) {
                $scope.loading = false;
            });
        }
    };

    //get all Data
    $scope.loadData = function (Id) {
        $scope.loading = true;
        historyFactory.getModel(Id).success(function (data) {
            $scope.historyModel = data;
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.loading = false;
        });
        $('.popover').css('top', parseInt($('.popover').css('top')) + 18 + 'px')
    };

});

