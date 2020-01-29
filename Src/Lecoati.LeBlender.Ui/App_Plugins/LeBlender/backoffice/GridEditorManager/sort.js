angular.module("umbraco").controller("leblender.editormanager.sort",
	function ($scope, LeBlenderRequestHelper, navigationService, treeService, assetsService) {

		$scope.save = function () {
			$scope.saving = true;
			var sortItems = [];

			for (var i = 0; i < $scope.editors.length; i++) {
				sortItems.push({
					key: $scope.editors[i].id,
					value: i
				});
			}

			LeBlenderRequestHelper.updateGridSortOrder(sortItems).then(function (response) {
				$scope.saving = false;
				treeService.loadNodeChildren({ node: $scope.currentNode });
				navigationService.hideMenu();
			});
		};

		$scope.close = function () {
			navigationService.hideNavigation();
		};

		LeBlenderRequestHelper.getGridEditors().then(function (response) {
			$scope.editors = response;
		});
		$scope.saving = false;
		assetsService.loadCss("/App_Plugins/LeBlender/Backoffice/GridEditorManager/gridEditorManager.css");
	});
