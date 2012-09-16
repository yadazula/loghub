ko.bindingHandlers.tooltip = {
    init: function (element, valueAccessor) {
        $(element).tooltip();
    }
};

ko.bindingHandlers.datepicker = {
    init: function (element, valueAccessor) {
        $(element).datepicker({
            format: 'dd-mm-yyyy'
        });
    }
};

ko.bindingHandlers.fadeIn = {
    init: function (element, valueAccessor) {
        element.style.display = "none";
        var value = valueAccessor();
        $(element).fadeIn(value);
    }
};

formatDate = function(date) {
    var month = date.getMonth() + 1;
    var day = date.getDate();
    var hour = date.getHours();
    var minute = date.getMinutes();
    var second = date.getSeconds();

    return date.getFullYear() + '-' +
        (('' + month).length < 2 ? '0' : '') + month + '-' +
        (('' + day).length < 2 ? '0' : '') + day + ' ' +
        (('' + hour).length < 2 ? '0' : '') + hour + ':' +
        (('' + minute).length < 2 ? '0' : '') + minute + ':' +
        (('' + second).length < 2 ? '0' : '') + second;
};