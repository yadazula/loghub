var loghub = loghub || {};
loghub.viewmodels = loghub.viewmodels || {};

loghub.viewmodels.Page = function (url, icon, name, template) {
    this.url = url;
    this.icon = icon;
    this.name = name;
    this.template = template;
    this.isActive = ko.observable(false);
};