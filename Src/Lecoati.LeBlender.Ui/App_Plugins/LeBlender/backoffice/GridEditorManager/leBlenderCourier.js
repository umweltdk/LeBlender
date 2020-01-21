angular.module("umbraco").controller("leblender.editormanager.leBlenderCourier",
	function ($scope, assetsService, $http, $location, LeBlenderRequestHelper, dialogService, $routeParams, navigationService, treeService) {

		$scope.transferring = false;
		$scope.transferDone = false;

		$scope.transfer = function (remoteUrl) {
			$scope.transferring = true;

			LeBlenderRequestHelper.transferEditor($scope.model.value, remoteUrl).then(function (response) {
				console.log('TRANSFER RESPONSE', response);
				$scope.transferMessage = response.data.message;
				$scope.transferring = false;
				$scope.transferDone = true;
			});
		};

		$scope.cancelCourier = function () {
			navigationService.hideNavigation();
			console.log('WE CANCEL');
		};

		LeBlenderRequestHelper.getGridEditors().then(function (response) {

			// init model
			$scope.editors = response

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

		});

		$scope.courierUrls = [];

		LeBlenderRequestHelper.getCourierRepositories().then(function (response) {
			var currentUrl = $location.protocol() + '://' + $location.host();
			if (response) {
				if (response.length > 0) {
					for (var i = 0; i < response.length; i++) {
						if (response[i] !== currentUrl) {
							$scope.courierUrls.push(response[i]);
						}
					}
				}
			}
		});
		console.log('COURIERURLS', $scope.courierUrls);
	});
