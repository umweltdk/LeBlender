angular.module('umbraco.services').config([
	'$httpProvider',
	function ($httpProvider, $http) {

		$httpProvider.interceptors.push(function ($q) {
			return {
				'request': function (request) {
					// Redirect Umbraco Grid to get GridEditors from Database/Cache instead of Web\config\grid.editors.config.js
					if (request.url === "/umbraco/GetGridConfig") {
						request.url = '/umbraco/backoffice/leblender/Helper/GetEditors';
					}
					return request || $q.when(request);
				}
			};
		});
	}]);
