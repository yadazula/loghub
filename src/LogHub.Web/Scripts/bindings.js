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