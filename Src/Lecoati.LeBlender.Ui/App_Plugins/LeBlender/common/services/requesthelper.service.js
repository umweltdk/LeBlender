angular.module("umbraco").factory("LeBlenderRequestHelper",
	function ($rootScope, $q, $http, $parse, $routeParams, umbRequestHelper) {
		return {

			GetPartialViewResultAsHtmlForEditor: function (control) {
				var view = "grid/editors/base";
				var url = "/umbraco/backoffice/leblender/Helper/GetPartialViewResultAsHtmlForEditor";
				var resultParameters = { model: angular.toJson(control, false), view: view, id: $routeParams.id, doctype: $routeParams.doctype };

				var promise = $http.post(url, resultParameters, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				})
					.success(function (htmlResult) {
						if (htmlResult.trim().length > 0) {
							return htmlResult;
						}
					});

				return promise;
			},

			getGridEditors: function () {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/leblender/Helper/GetEditors"), 'Failed to retrieve editors from tree service');
			},


			getTransferUrls: function () {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/leblender/helper/GetTransferUrls"), 'Failed to retrieve transfer-urls from tree service');
			},

			getAllPropertyGridEditors: function () {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/PropertyGridEditor/GetAll"), 'Failed to retrieve datatypes from tree service');
			},


			deleteGridEditor: function (id) {
				var url = "/umbraco/backoffice/leblender/Helper/DeleteEditor";
				var data = { id: id };
				return $http.post(url, data, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				});
			},

			updateGridSortOrder: function (items) {
				var url = "/umbraco/backoffice/leblender/Helper/UpdateGridSortOrder";
				var data = { items: items };
				return $http.post(url, data, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				});
			},

			deleteAllEditors: function (editors) {
				var url = "/umbraco/backoffice/leblender/Helper/DeleteAllEditors";
				var data = { editors: JSON.stringify(editors, null, 4) };
				return $http.post(url, data, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				});
			},

			updateGridEditor: function (editor) {
				var url = "/umbraco/backoffice/leblender/Helper/UpdateEditor";
				var resultParameters = { editor: JSON.stringify(editor, null, 4) };

				return $http.post(url, resultParameters, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				});
			},

			transferEditor: function (editor, remoteUrl) {
				var url = remoteUrl + "/umbraco/api/Transfer/TransferEditor";
				var data = {
					editor: JSON.stringify(editor, null, 4)
				};

				return $http.post(url, data, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				});
			},

			transferAllEditors: function (editors, remoteUrl) {
				var url = remoteUrl + "/umbraco/api/Transfer/TransferAllEditors";
				var data = {
					editors: JSON.stringify(editors, null, 4)
				};

				return $http.post(url, data, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				});
			},

			getAllDataTypes: function () {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/DataType/GetAll"), 'Failed to retrieve datatypes from tree service');
			},

			getDataType: function (guid) {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/DataType/GetPropertyEditors?guid=" + guid, { cache: true }), 'Failed to retrieve datatype');
			},
		}
	});
