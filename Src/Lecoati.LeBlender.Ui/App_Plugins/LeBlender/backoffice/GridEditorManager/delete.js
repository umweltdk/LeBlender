angular.module("umbraco").controller("leblender.editormanager.delete",
	function ($scope, assetsService, $http, LeBlenderRequestHelper, dialogService, $routeParams, navigationService, treeService) {

		$scope.delete = function () {
			if ($scope.deleteAll) {
				LeBlenderRequestHelper.deleteAllEditors($scope.editors).then(function (response) {
					if (response.status !== 200) {
						console.error(response.data);
					}
					LeBlenderRequestHelper.getGridEditors().then(function (result) {
						$scope.editors = result;
					});
				});
			} else {
				$scope.editors.splice($scope.indexModel, 1);
				LeBlenderRequestHelper.deleteGridEditor($scope.model.value.id).then(function (response) {
					if (response.status !== 200) {
						console.error(response.data);
					}
					treeService.removeNode($scope.currentNode);
				});
			}
			navigationService.hideMenu();
		};

		$scope.cancelDelete = function () {
			navigationService.hideNavigation();
		};

		function init() {

			$scope.deleteAll = $scope.dialogOptions.currentAction.metaData.DeleteAll || false;

			LeBlenderRequestHelper.getGridEditors().then(function (response) {
				// init model
				$scope.editors = response;

				// Init model value
				$scope.model = {
					value: {
						name: "",
						alias: "",
						view: "",
						icon: ""
					}
				};

				// look for the current editor
				_.each($scope.editors, function (editor, editorIndex) {
					if (editor.alias === $scope.currentNode.id) {
						$scope.indexModel = editorIndex;
						angular.extend($scope, {
							model: {
								value: editor
							}
						});
					}
				});

				$scope.deleteText = $scope.deleteAll ? 'all editors?' : 'editor with name: ' + $scope.model.value.name + ' - alias: ' + $scope.currentNode.id + '?';

			});
		}

		init();

	});
