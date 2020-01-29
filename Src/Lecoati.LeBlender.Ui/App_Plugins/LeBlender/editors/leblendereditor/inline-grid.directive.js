angular.module("umbraco").
	directive('inlineGrid', function () {
		return {
			scope: {
				name: "=",
				value: "=",
				config: "=",
			},
			restrict: 'E',
			replace: false,
			templateUrl: '/App_Plugins/LeBlender/editors/leblendereditor/inline-grid.template.html',
			controller: function ($scope, $timeout, $element, assetsService, LeBlenderRequestHelper, umbPropEditorHelper) {
				// Listener which reloads tinymce editor when control is moved.
				var innerCell = $element.parents('.umb-cell-inner');
				innerCell.on('sortstop', sortstopHandler);

				$scope.$on('$destroy', function () {
					innerCell.off('sortstop', sortstopHandler);
				});

				function sortstopHandler() {
					$element.find('.umb-rte').each(function () {
						var scope = angular.element(this).scope();

						if (!scope) {
							return;
						}

						$timeout(function () {
							tinyMCE.execCommand('mceRemoveEditor', false, scope.textAreaHtmlId);
							tinyMCE.execCommand('mceAddEditor', false, scope.textAreaHtmlId);
						});
					});
				}

				$scope.searchEditor = function (alias) {
					var sEditor = undefined;
					if ($scope.config.editors) {
						_.each($scope.config.editors, function (editor) {
							if (editor.alias === alias) {
								sEditor = editor
							}
						})
					}
					return sEditor;
				}

				$scope.searchPropertyItem = function (item, alias) {
					var sProperty = undefined;
					_.each(item, function (property) {
						if (property.editorAlias === alias) {
							sProperty = property
						}
					})
					return sProperty;
				}

				var initEditor = function () {
					_.each($scope.value, function (item) {
						var order = 0;
						if ($scope.config.editors) {
							_.each($scope.config.editors, function (editor) {
								var property = $scope.searchPropertyItem(item, editor.alias);
								if (property) {
									property.$editor = editor;
									property.$order = order;
									if (!property.dataTypeGuid)
										property.dataTypeGuid = editor.dataType;
								}
								else {
									var newProperty = {
										value: null,
										dataTypeGuid: editor.dataType,
										editorAlias: editor.alias,
										editorName: editor.name,
										$editor: editor,
										$order: editor.sortOrder,
										$valid: false
									};
									item[editor.alias] = newProperty;
								}
								order++;
							})
						}

						_.each(item, function (property) {
							if (!$scope.searchEditor(property.editorAlias)) {
								delete item[property.editorAlias];
							}
						})
					})
				}

				$scope.updateEditor = function () {
					if ($scope.value) {
						var watchAppStart = $scope.$watch(function () {
							var isLoadedCounter = 0
							_.each($scope.config.editors, function (editor) {
								if (editor.$isLoaded) {
									isLoadedCounter++
								}
							});
							return isLoadedCounter;
						}, function (newValue) {
							if (newValue === $scope.config.editors.length) {
								initEditor();
								watchAppStart();
								$scope.configLoaded = true;
							}
						}, true);

						/***************************************/
						/* load dataType Info */
						/***************************************/
						if ($scope.config.editors) {
							_.each($scope.config.editors, function (editor) {

								if (!$scope.value.propretyType) {
									$scope.value.propretyType = {};
								}
								if (editor.propretyType == null) {
									editor.propretyType = {};
								}

								if (editor.dataType && !editor.$isLoaded) {
									LeBlenderRequestHelper.getDataType(editor.dataType).then(function (data) {
										// Get config prevalues
										var configObj = {};
										_.each(data.preValues, function (p) {
											configObj[p.key] = p.value;
										});

										// Get config default prevalues
										var defaultConfigObj = {};
										if (data.defaultPreValues) {
											_.extend(defaultConfigObj, data.defaultPreValues);
										}

										// Merge prevalue and default prevalues
										var mergedConfig = _.extend(defaultConfigObj, configObj);

										editor.$isLoaded = true;
										editor.propretyType.config = mergedConfig;
										editor.propretyType.view = umbPropEditorHelper.getViewPath(data.view);
									});
								} else {
									editor.$isLoaded = true;
								}
							})
						}
					}
				}

				assetsService.loadCss("/App_Plugins/LeBlender/editors/leblendereditor/inline-grid.css");

				$scope.updateEditor();
			}
		};
	});
