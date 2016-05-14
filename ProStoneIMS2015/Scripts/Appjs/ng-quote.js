var app = angular.module('gabsNgApp', ['ui.bootstrap', 'ngAnimate', 'ui.bootstrap.showErrors', 'ui.bootstrap.datetimepicker']);
var urlQuote = ROOT + 'api/quote/';
//Quote
app.factory('quoteFactory', function ($http) {
    return {
        getModel: function (Id) {
            return $http.get(urlQuote + Id);
        },
        updateModel: function (quote) {
            return $http.put(urlQuote + quote.Id, quote);
        },
    };
});

app.controller('quoteController', function sinkController($scope, quoteFactory) {
    $scope.quote = null;
    $scope.loading = true;

    $scope.edit = function () {
        $scope.originalModel = angular.copy(this.quote);
        this.quote.editMode = true;
    };

    $scope.discard = function () {
        $scope.quote = angular.copy($scope.originalModel);
        this.quote.editMode = false;
    }

    $scope.save = function () {
        $scope.loading = true;
        var thisModel = this.quote;
        quoteFactory.updateModel(thisModel).success(function (data) {
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
        quoteFactory.getModel(Id).success(function (data) {
            $scope.quote = data;
            $scope.loading = false;
        })
        .error(function (data) {
            $scope.addAlert('danger', 'An Error has occured while Loading data! ' + data.ExceptionMessage);
            $scope.loading = false;
        });
    };

});


app.directive('myDate', function (dateFilter, $parse) {
    return {
        restrict: 'EAC',
        require: '?ngModel',
        link: function (scope, element, attrs, ngModel, ctrl) {
            ngModel.$parsers.push(function (viewValue) {
                return dateFilter(viewValue, 'yyyy-MM-dd');
            });
        }
    }
});

app.directive('customPopover', ['$compile', function ($compile) {

    var popoverBodyData = "<a> {{tooltiplabel}} </a>";
    var popoverTitleData = "<span>Description</span> <button type='button' class='close'>&times;</button>";

    return {
        restrict: 'A',
        //transclude: true,
        //template: "Description <button id='btnClose' type='button' class='close' onclick='$(&quot;.popover&quot;).hide();'>&times;</button>",          
        //template: "<span ng-transclude></span>",
        link: function (scope, element, attribute, controller) {


            var sib = $(this).parent().parent().siblings('.collapse');
            if (!sib.hasClass('flagged')) {
                scope.tooltiplabel = "Hello Everybody this is PopOver Demo !!!";

                //var compliedData = $compile(popoverBodyData)(scope);
                var compliedData = $compile($("#" + attribute.popoverBodydata).contents())(scope);
                var compliedTitle = $compile(popoverTitleData)(scope);

                //var getTitle = "<span>"+attribute.popoverTitle+"</span><button id='btnClose' type='button' class='close' onclick='$(&quot;.popover&quot;).prev().removeAttr(&quot;aria-describedby&quot;);$(&quot;.popover&quot;).remove();'>&times;</button>";
                var getTitle = "<strong>" + attribute.popoverTitle + "</strong><button type='button' class='close'>&times;</button>";

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
                            angular.element($('#MeasurePopover')).scope().setSF();
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
                        angular.element($('#MeasurePopover')).scope().setSF();
                        $('#overlay-back').fadeOut(500);
                    });
                });


            }


        }

    };
}]);


app.directive('modal', function () {
    return {
        template: '<div class="modal fade">' +
            '<div class="modal-dialog">' +
              '<div class="modal-content">' +
                '<div class="modal-header">' +
                  '<button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>' +
                  '<h4 class="modal-title">{{ title }}</h4>' +
                '</div>' +
                '<div class="modal-body" ng-transclude></div>' +
              '</div>' +
            '</div>' +
          '</div>',
        restrict: 'E',
        transclude: true,
        replace: true,
        scope: true,
        link: function postLink(scope, element, attrs) {
            scope.title = attrs.title;

            scope.$watch(attrs.visible, function (value) {
                if (value == true)
                    $(element).modal('show');
                else
                    $(element).modal('hide');
            });

            $(element).on('shown.bs.modal', function () {
                scope.$apply(function () {
                    scope.$parent[attrs.visible] = true;
                });
            });

            $(element).on('hidden.bs.modal', function () {
                scope.$apply(function () {
                    scope.$parent[attrs.visible] = false;
                });
            });
        }
    };
});

app.directive('aDisabled', function() {
    return {
        compile: function(tElement, tAttrs, transclude) {
            //Disable ngClick
            tAttrs["ngClick"] = "!("+tAttrs["aDisabled"]+") && ("+tAttrs["ngClick"]+")";

            //Toggle "disabled" to class when aDisabled becomes true
            return function (scope, iElement, iAttrs) {
                scope.$watch(iAttrs["aDisabled"], function(newValue) {
                    if (newValue !== undefined) {
                        iElement.toggleClass("disabled", newValue);
                    }
                });

                //Disable href on click
                iElement.on("click", function(e) {
                    if (scope.$eval(iAttrs["aDisabled"])) {
                        e.preventDefault();
                    }
                });
            };
        }
    };
});
