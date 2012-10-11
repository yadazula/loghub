var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.baseEditableList = Class.extend({
	url: '',

	currentItem: ko.observable(),

	getNewModel: function () { return {}; },

	getEditModel: function (model) { return model; },

	buildDeleteUrl: function (model) {
		return this.url + '?id=' + model.id();
	},

	validate: function (model, validationErrors) { },
	
	mapToViewModel: function(model) {
		return ko.mapping.fromJS(model);
	},

	getIgnoredFields: function () {
		return ['isNew', 'validationErrors', 'isLoading'];
	},

	create: function () {
		var newModel = this.getNewModel();
		newModel.isNew = ko.observable(true),
		newModel.validationErrors = ko.observable([]);
		newModel.isLoading = ko.observable(false);

		this.currentItem(newModel);
		$('#editModal').modal('show');
	},

	'delete': function (model) {
		var self = this;
		var url = this.buildDeleteUrl(model);
		loghub.restClient.delete(url, {
			success: function () {
				$('#editModal').modal('hide');
				self.load();
			}
		});
	},

	edit: function (model) {
		var editModel = this.getEditModel(ko.mapping.toJS(model));
		editModel.isNew = false;
		editModel.isLoading = false;
		editModel.validationErrors = [];
		
		var clonedEditModel = this.mapToViewModel(editModel);
		this.currentItem(clonedEditModel);
		$('#editModal').modal('show');
	},

	save: function (model) {
		var validationErrors = [];
		this.validate(model, validationErrors);
		model.validationErrors(validationErrors);
		if (validationErrors.length != 0)
			return;

		var data = ko.mapping.toJSON(model, { ignore: this.getIgnoredFields() });
		model.isLoading(true);

		var self = this;
		if (model.isNew()) {
			loghub.restClient.post(self.url, data, {
				success: function () {
					$('#editModal').modal('hide');
					self.load();
				},
				error: function () {
					model.isLoading(false);
				}
			});

			return;
		}

		loghub.restClient.put(self.url, data, {
			success: function () {
				$('#editModal').modal('hide');
				self.load();
			},
			error: function () {
				model.isLoading(false);
			}
		});
	},

	load: function (callback) {
		var self = this;
		this.currentItem(null);
		loghub.restClient.read(self.url, {
			success: function (data) {
				if (self.items)
					ko.mapping.fromJS(data, self.items);
				else
					self.items = ko.mapping.fromJS(data);
			},
			complete: function () {
				if (callback) callback();
			}
		});
	}
});