angular.module("umbraco").
	directive('propertygrideditorwrapper', function ($timeout) {
		return {
			scope: {
				key: "=",
				view: "=",
				value: "="
			},
			restrict: 'E',
			replace: true,
			template: '<ng-form name="propertyForm"><div ng-include="view"></div></ng-form>',

			controller: function ($scope) {
				if (!$scope.value.config) {
					$scope.value.config = {};
				}

				$scope.model = {
					value: angular.copy($scope.value.config[$scope.key]),
					parentValue: $scope.value
				}

				$scope.$watch("model.value", function (newValue, oldValue) {
					$scope.value.config[$scope.key] = $scope.model.value;

					// If there are more than 1 property we want to assign sortorders based on current index
					if (Array.isArray($scope.model.value)) {
						for (var i = 0; i < $scope.model.value.length; i++) {
							$scope.model.value[i].sortOrder = i;
						}
					}

				}, true);
			}
		};
	});
