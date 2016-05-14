var urlJob = ROOT + 'api/job/';

//Job
app.factory('jobFactory', function ($http) {
    return {
        getModel: function (Id) {
            return $http.get(urlJob + Id);
        },
        getJobModel: function () {
            return $http.get(urlJob);
        },
        updateModel: function (job) {
            return $http.put(urlJob + job.Id, job);
        },
    };
});

app.controller('jobController', function jobController($scope, jobFactory) {
    $scope.jobddl = null;
    $scope.job = null;
    $scope.loading = true;

    $scope.edit = function () {
        $scope.originalModel = angular.copy(this.job);
        if (this.job.JobNo = null) {
            this.job.JobNo = angular.copy(this.job.Id);
        }
        this.job.editMode = true;
    };

    $scope.discard = function () {
        $scope.job = angular.copy($scope.originalModel);
        this.job.editMode = false;
    }

    $scope.save = function () {
        $scope.loading = true;
        var thisModel = this.job;
        jobFactory.updateModel(thisModel).success(function (data) {
            //$scope.addAlert('success', 'Saved Successfully!', 5000);
            thisModel.editMode = false;
            $scope.loading = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Saving data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

    //get all Data
    $scope.loadData = function (Id) {
        jobFactory.getJobModel().success(function (data) {
            $scope.jobddl = data;
            jobFactory.getModel(Id).success(function (data) {
                $scope.job = data;
                $scope.loading = false;
            })
            .error(function (data) {
                $scope.addAlert('danger', 'An Error has occured while Loading data! ' + data.ExceptionMessage);
                $scope.loading = false;
            });
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Loading dropdown data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

});


app.directive('shadow', function () {
    return {
        scope: true,
        link: function (scope, el, att) {
            scope[att.shadow] = angular.copy(scope[att.shadow]);

            scope.save = function () {
                scope.$parent[att.shadow] = angular.copy(scope[att.shadow]);
            };
        }
    };
});