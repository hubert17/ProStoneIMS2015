//Payment
var urlPayment = ROOT + 'api/payment/';

app.factory('paymentFactory', function ($http) {
    return {
        getModel: function (qId) {
            return $http.get(urlPayment + "?QuoteId=" + qId);
        },
        addModel: function (payment) {
            return $http.post(urlPayment, payment);
        },
        deleteModel: function (payment) {
            return $http.delete(urlPayment + payment.Id);
        },
        updateModel: function (payment) {
            return $http.put(urlPayment + payment.Id, payment);
        },

    };
});

app.controller('paymentController', function myController($scope, paymentFactory) {
    $scope.paymentModel = [];
    $scope.paymentLookup = [];
    $scope.loading = true;
    $scope.adding = false;
    $scope.saving = false;
    $scope.deleting = false;
    $scope.addMode = true;
    $scope.isNumber = angular.isNumber;
    $scope.qsId = null;

    $scope.edit = function () {
        $scope.originalModel = angular.copy(this.payment);
        this.saving = false;
        this.payment.editMode = true;
    };

    $scope.discard = function () {
        this.payment = angular.copy($scope.originalModel);
        this.payment.editMode = false;
    }

    $scope.toggleAdd = function () {
        $scope.addMode = !$scope.addMode;
    };

    $scope.savePayment = function () {
        this.saving = true;
        var thisModel = this.payment;
        paymentFactory.updateModel(thisModel).success(function (data) {
            //$scope.addAlert('success', 'Saved Successfully!', 5000);
            thisModel.editMode = false;
            this.saving = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Saving data! ' + data.ExceptionMessage);
            thisModel.editMode = false;
            $.each($scope.paymentModel, function (i) {
                if ($scope.paymentModel[i].Id === thisModel.Id) {
                    $scope.paymentModel[i] = angular.copy($scope.originalModel);
                }
            });
            this.saving = false;
        });
    };

    // add Data
    $scope.addPayment = function (Id) {
        this.newPaymentModel.QuoteId = Id;
        $scope.adding = true;
        paymentFactory.addModel(this.newPaymentModel).success(function (data) {
            //$scope.addAlert('success', 'Added Successfully!' + data, 5000);
            $scope.paymentModel.push(data);
            $scope.adding = false;
        }).error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Adding data! ' + data.ExceptionMessage);
            $scope.adding = false;
        });
        this.newPaymentModel = null;
        $scope.newPaymentModel = { PaymentDate: null, Amount: null };
    };

    // delete Data
    $scope.deletePayment = function () {
        var result = confirm("Are you sure?");
        if (result == true) {
            this.deleting = true;
            var currentModel = this.payment;
            //alert(currentModel.Id);
            paymentFactory.deleteModel(currentModel).success(function (data) {
                //$scope.addAlert('warning', 'Deleted Successfully!', 5000);
                $.each($scope.paymentModel, function (i) {
                    if ($scope.paymentModel[i].Id === currentModel.Id) {
                        $scope.paymentModel.splice(i, 1);
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
    $scope.loadPayment = function (qId) {
        paymentFactory.getModel(qId).success(function (data) {
            $scope.paymentModel = data;
            $scope.newPaymentModel = { PaymentDate: null, Amount: null };
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Loading data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

    $scope.totalPayment = function () {
        var total = 0;
        angular.forEach($scope.paymentModel, function (payment) {
            total += payment.Amount;
        })
        return total;
    }


});


