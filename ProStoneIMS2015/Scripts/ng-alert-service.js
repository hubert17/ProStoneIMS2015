angular.module('gabsNgApp', ['ui.bootstrap']);
function AlertDemoCtrl($rootScope, $timeout) {
    $rootScope.alerts = [];

    $rootScope.addAlert = function (type, msg, timeout) {
        $rootScope.alerts.push({ type: type, msg: msg, close: function () { $rootScope.closeAlert($rootScope.alerts.indexOf(alert)); } });
        if (timeout) {
            window.setTimeout(function () {
                $(".ng-alert-fadeOut").fadeTo(500, 0).slideUp(500, function () {
                    $(this).remove();
                });
            }, timeout);
            $timeout(function () {
                $rootScope.closeAlert($rootScope.alerts.indexOf(alert));
            }, timeout + 1000);
        }
    };

    $rootScope.closeAlert = function (index) {
        $rootScope.alerts.splice(index, 1);
    };

}