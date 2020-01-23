angular.module("umbraco").controller("leblender.editormanager.leBlenderCourier",
	function ($scope, assetsService, $http, $timeout, $location, LeBlenderRequestHelper, dialogService, $routeParams, navigationService, treeService) {



		$scope.transferEditor = function (remoteUrl) {
			$scope.transferring = true;

			LeBlenderRequestHelper.transferEditor($scope.model.value, remoteUrl).then(function (response) {
				$scope.transfer.message = response.data;
				if (response.status !== 200) {
					$scope.transfer.textColor = "red";
				}
				$scope.transferring = false;
				$scope.transferDone = true;
				$timeout(function () {
					$scope.transferDone = false;
					navigationService.hideMenu();
				}, 7000);
			});
		};

		function init() {
			$scope.transferring = false;
			$scope.transferDone = false;
			$scope.transfer = {
				message: "",
				textColor: "green"
			};


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

		}

		$scope.cancelCourier = function () {
			navigationService.hideNavigation();
		};

		init();

	});
