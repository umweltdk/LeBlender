angular.module("umbraco").controller("leblender.editormanager.delete",
	function ($scope, $timeout, LeBlenderRequestHelper, navigationService, treeService) {

		$scope.delete = function () {
			$scope.deleting = true;
			if ($scope.deleteAll) {
				LeBlenderRequestHelper.deleteAllEditors($scope.editors).then(function (response) {
					setMessage(response);
				});
			} else {
				$scope.editors.splice($scope.indexModel, 1);
				LeBlenderRequestHelper.deleteGridEditor($scope.model.value.id).then(function (response) {
					setMessage(response);
				});
			}
		};

		$scope.cancelDelete = function () {
			navigationService.hideNavigation();
		};

		function setMessage(response) {
			$scope.deletion.message = response.data;
			$scope.deleting = false;
			$scope.deletionDone = true;
			if (response.status !== 200) {
				$scope.deletion.textColor = "red";
			} else {
				$timeout(function () {
					treeService.removeNode($scope.currentNode);
					navigationService.hideMenu();
				}, 7000);
			}
		}

		function init() {
			$scope.deleting = false;
			$scope.deletionDone = false;
			$scope.deletion = {
				message: "",
				textColor: "green"
			};

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
