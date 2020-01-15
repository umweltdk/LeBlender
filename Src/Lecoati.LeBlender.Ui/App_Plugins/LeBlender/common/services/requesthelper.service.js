angular.module("umbraco").factory("LeBlenderRequestHelper",
	function ($rootScope, $q, $http, $parse, $routeParams, umbRequestHelper) {
		return {

			/*********************/
			/*********************/
			GetPartialViewResultAsHtmlForEditor: function (control) {
				console.log('GetPartialViewResultAsHtmlForEditor.control', control);
				var view = "grid/editors/base";
				var url = "/umbraco/backoffice/leblender/Helper/GetPartialViewResultAsHtmlForEditor";
				var resultParameters = { model: angular.toJson(control, false), view: view, id: $routeParams.id, doctype: $routeParams.doctype };

				//$http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
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

			/*********************/
			/*********************/
			getGridEditors: function () {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/leblender/Helper/GetEditors"), 'Failed to retrieve datatypes from tree service');
			},

			/*********************/
			/*********************/
			getAllPropertyGridEditors: function () {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/PropertyGridEditor/GetAll"), 'Failed to retrieve datatypes from tree service');
			},

			deleteGridEditor: function (id) {
				var url = "/umbraco/backoffice/leblender/Helper/DeleteEditor";
				var data = { id: id };
				return $http.post(url, data)
					.then(function successCallback(response) {
					}, function errorCallback(reponse) {
						console.error(response.data.Message);
					});
			},

			updateGridSortOrder: function (items) {
				var url = "/umbraco/backoffice/leblender/Helper/UpdateGridSortOrder";
				var data = { items: items };
				return $http.post(url, data)
					.then(function successCallback(response) {
					}, function errorCallback(reponse) {
						console.error(response.data.Message);
					});
			},

			/*********************/
			/*********************/
			updateGridEditor: function (editor) {

				var url = "/umbraco/backoffice/leblender/Helper/UpdateEditor";
				var resultParameters = { editor: JSON.stringify(editor, null, 4) };

				//$http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
				return $http.post(url, resultParameters, {
					headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
					transformRequest: function (result) {
						return $.param(result);
					}
				});

			},

			/*********************/
			/*********************/
			getAllDataTypes: function () {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/DataType/GetAll"), 'Failed to retrieve datatypes from tree service');
			},

			/*********************/
			/*********************/
			getDataType: function (guid) {
				return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/DataType/GetPropertyEditors?guid=" + guid, { cache: true }), 'Failed to retrieve datatype');
			},

		}

	});
