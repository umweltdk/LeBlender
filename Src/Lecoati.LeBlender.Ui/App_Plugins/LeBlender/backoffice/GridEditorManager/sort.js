angular.module("umbraco").controller("leblender.editormanager.sort",
	function ($scope, assetsService, $http, LeBlenderRequestHelper, dialogService, $routeParams, navigationService, treeService) {

		$scope.save = function () {

			var sortItems = [];

			for (var i = 0; i < $scope.editors.length; i++) {
				sortItems.push({
					key: $scope.editors[i].id,
					value: i
				});
			}

			LeBlenderRequestHelper.updateGridSortOrder(sortItems).then(function (response) {
				treeService.loadNodeChildren({ node: $scope.currentNode });
				navigationService.hideMenu();
			});
		};

		$scope.close = function () {
			navigationService.hideNavigation();
		};

		LeBlenderRequestHelper.getGridEditors().then(function (response) {
			$scope.editors = response
		});

	});
