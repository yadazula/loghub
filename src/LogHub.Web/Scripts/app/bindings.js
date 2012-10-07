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
	init: function (element, valueAccessor, allBindingsAccessor, viewModel, bindingContext) {
        var allBindings = allBindingsAccessor();
        var message = allBindings.confirmMessage;
        var onDelete = ko.utils.unwrapObservable(valueAccessor());
        var handler = function (e) {
            e.preventDefault();
            var el = $('#confirmDelete');
            ko.cleanNode(el[0]);
            ko.applyBindings({
                message: message,
                onDelete: function () {
                	onDelete.call(bindingContext.$root, viewModel);
                    el.modal('hide');
                },
            }, el[0]);
            el.modal('show');
        };

        $(element).bind('click', handler);
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).unbind('click', handler);
        });
    }
};

ko.bindingHandlers.autoComplete = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var el = $(element);
        var binding = ko.utils.unwrapObservable(valueAccessor());

        el.val(binding.target());

        el.typeahead({
            source: function (query, process) {
                return binding.source(query, process);
            }
        });

        var handler = function () {
            var val = el.val();
            binding.target(val);
        };

        el.bind('change', handler);
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            el.unbind('change', handler);
        });
    }
}