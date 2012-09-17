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

ko.bindingHandlers.confirmDelete = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var allBindings = allBindingsAccessor();
        var message = allBindings.confirmMessage;
        var onDelete = ko.utils.unwrapObservable(valueAccessor());
        var handler = function (e) {
            e.preventDefault();
            var el = $('#confirmDelete');
            ko.applyBindings({
                message: message,
                onDelete: function () { onDelete(viewModel); el.modal('hide'); },
            }, el[0]);
            el.modal('show');
        };

        $(element).bind('click', handler);
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).unbind('click', handler);
        });
    }
}