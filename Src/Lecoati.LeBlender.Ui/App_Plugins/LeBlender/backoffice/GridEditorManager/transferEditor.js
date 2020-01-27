﻿angular.module("umbraco").controller("leblender.editormanager.transferEditor",
	function ($scope, $timeout, $location, LeBlenderRequestHelper, navigationService) {
		$scope.transferEditor = function (remoteUrl) {
			$scope.transferring = true;

			if ($scope.transferAll) {
				LeBlenderRequestHelper.transferAllEditors($scope.editors, remoteUrl).then(function (response) {
					setMessage(response);
				});
			} else {
				LeBlenderRequestHelper.transferEditor($scope.model.value, remoteUrl).then(function (response) {
					setMessage(response);
				});
			}
		};

		function setMessage(response) {
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
		}

		function init() {
			$scope.transferAll = $scope.dialogOptions.currentAction.metaData.TransferAll || false;
			$scope.transferring = false;
			$scope.transferDone = false;
			$scope.transfer = {
				message: "",
				textColor: "green"
			};

			LeBlenderRequestHelper.getGridEditors().then(function (response) {
				$scope.transferUrls = [];
				$scope.editors = response;
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

			// Get Repositories
			LeBlenderRequestHelper.getTransferUrls().then(function (response) {
				var currentUrl = $location.protocol() + '://' + $location.host();
				if (response) {
					if (response.length > 0) {
						for (var i = 0; i < response.length; i++) {
							if (response[i] !== currentUrl) {
								$scope.transferUrls.push(response[i]);
							}
						}
					}
				}
			});
		}

		$scope.cancelTransfer = function () {
			navigationService.hideNavigation();
		};

		init();

	});
