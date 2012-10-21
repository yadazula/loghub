var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.page = function (url, icon, name, template, render) {
	this.url = url;
	this.icon = icon;
	this.name = name;
	this.template = template + '-template';
	this.isActive = ko.observable(false);
	this.render = render;
};