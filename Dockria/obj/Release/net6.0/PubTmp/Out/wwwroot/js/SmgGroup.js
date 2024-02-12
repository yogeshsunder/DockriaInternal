$(document).ready(function () {
    fetchDropdownOptions();
});

//  Add change event handler to the dropdown
$('#payment-interval').on('change', function () {
    var selectedValue = $(this).val();
    $(this).data('selected-value', selectedValue);

});
$('#payment-type').on('change', function () {
    var selectedValue = $(this).val();
    $(this).data('selected-value', selectedValue);

});

// Add change event handler to the dropdown
$('#payment-currency').on('change', function () {
    var selectedValue = $(this).val();
    $(this).data('selected-value', selectedValue);
});


function fetchDropdownOptions() {
    // AJAX request
    $.ajax({
        url: '/Admin/Smg/Create', // Replace with the actual URL to your Create action method
        type: 'GET',
        dataType: 'json', // Set the data type to JSON
        success: function (data) {
            // Populate the dropdown options
            if (data.paymentCurrencyList.length > 0) {
                populateDropdown('payment-currency', "Select Currency", data.paymentCurrencyList);
            }
            if (data.paymentIntervalList.length > 0) {
                populateDropdown('payment-interval', "Select Interval", data.paymentIntervalList);
            }
            if (data.paymentTypeList.length > 0) {
                populateDropdown('payment-type', "Select Payment type", data.paymentTypeList);
            }
        },
        error: function (xhr, status, error) {
            // Handle error
            console.error(error);
        }
    });
    function populateDropdown(dropdownId, selectedValue, options) {
        var dropdown = $('#' + dropdownId);
        dropdown.empty();

        // Add the selected value as the first option
        var selectedOption = $('<option>').val(selectedValue).text(selectedValue);
        dropdown.append(selectedOption);

        // Add the remaining options from the options array
        $.each(options, function (index, option) {
            if (option.Value !== selectedValue) {
                var optionElement = $('<option>').val(option.value).text(option.text);
                dropdown.append(optionElement);
            }
        });
    }
}

function populateDropdown(selector, options) {
    var dropdown = $(selector);
    dropdown.empty();
    dropdown.append('<option disabled selected>Select an option</option>');

    // Populate options
    $.each(options, function (index, option) {
        dropdown.append($('<option>', {
            value: option.value,
            text: option.text
        }));
    });

    // Set the selected value as the default option
    dropdown.val(dropdown.data('selected-value'));
}


// Initialize an array to store dynamically added SmgNames
var dynamicSmgNames = [];

function onAddClick() {

    if (!$("#addsmgForm").valid()) {
        return;
    }

    // Check if the entered SmgName already exists
    var enteredName = $('#smg-name').val();
    if (isSmgNameExists(enteredName)) {
        // Show error message in the modal
        $('#errorModalBody').html('#errorModalBody');
        $('#errorModal').modal('show');
        return;
    }

    // Add the entered SmgName to the dynamicSmgNames array
    dynamicSmgNames.push(enteredName);

    var data = {
        SmgName: enteredName,
        PaymentIntervalName: $('#payment-interval option:selected').text(),
        PaymentCurrencyName: $('#payment-currency option:selected').text(),
        TaxName: $('#tax-name').val(),
        TaxPercentage: $('#tax-percentage').val(),
        PaymentTypeName: $('#payment-type option:selected').text(),
        CartItems: []
    };

    // Loop through each row in the cart
    $('.cart-row').each(function () {
        var row = $(this); // Reference the current row

        var description = row.find('.description').val();
        var qty = row.find('.qty').val();
        var cost = row.find('.cost').val();
        var total = row.find('.total').val();
      
        var grandtotal = $('#grand-total').val();

        // Create an object to store the form data
        var cartItem = {
            description: description,
            qty: qty,
            cost: cost,
            total: total,
         
            
            grandtotal: grandtotal
        };

        // Add the cart item to the cart data
        data.CartItems.push(cartItem);
    });

    console.log(JSON.stringify(data)); // Output the data object in JSON format

    $.ajax({
        url: '/Admin/Smg/Create',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (result) {
            // Show success message in the modal
            $('#successModalBody').html('SMG saved successfully!');
            $('#successModal').modal('show');
            $('#addsmgForm')[0].reset();
            $('#addsmgCostForm')[0].reset();
            $('#admin-container')[0].reset();
            location.reload();
        },
        error: function () {
            // Show error message in the modal
            $('#errorModalBody').html('SmgName already exists. Please enter a different name.');
            $('#errorModal').modal('show');
        }
    });
}


// Initially hide all buttons except "CREATE NEW SMG"
$(document).ready(function () {
    $("#deleteSmgButton, #clearFormButton, #updateSmgListButton, #updateInfoButton, #updateCostButton").hide();
});

// Function to check if the SmgName already exists in the dynamicSmgNames array
function isSmgNameExists(name) {
    return dynamicSmgNames.includes(name);
}

$('#addsmgForm').submit(function (event) {
    event.preventDefault();
    onAddClick();
});

function updateSmgList() {
    if (!$("#addsmgForm").valid()) {
        return;
    }

    var data = {
        Id: $('#id').val(),
        SmgName: $('#smg-name').val(),
        PaymentIntervalName: $('#payment-interval option:selected').text(),
        PaymentCurrencyName: $('#payment-currency option:selected').text(),
        TaxName: $('#tax-name').val(),
        TaxPercentage: $('#tax-percentage').val(),
        PaymentTypeName: $('#payment-type option:selected').text(),
        CartItems: []
    };

    // Loop through each row in the cart
    $('.cart-row').each(function () {
        var row = $(this); // Reference the current row
        var id = row.find('.cart-id').val();
        var description = row.find('.description').val();
        var qty = row.find('.qty').val();
        var cost = row.find('.cost').val();
        var total = row.find('.total').val();
        var subtotal = row.find('.subtotal').val();
        var vatpercentage = row.find('.vatpercentage').val();
        var vat = row.find('.vat').val();
        var grandtotal = row.find('.grandtotal').val();

        // Create an object to store the form data
        var cartItem = {
            id: id,
            description: description,
            qty: qty,
            cost: cost,
            total: total,
            subtotal: subtotal,
            vatpercentage: vatpercentage,
            vat: vat,
            grandtotal: grandtotal
        };

        // Add the cart item to the cart data
        data.CartItems.push(cartItem);
    });

    $.ajax({
        url: '/Admin/Smg/Edit',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (result) {
            if (result && result.success) {
                // Show success message in the success modal
                $('#successModalBody').html('Subscription Management updated successfully!');
                $('#successModal').modal('show');

                // Reset forms and reload the page
                $('#addsmgForm')[0].reset();
                $("#addsmgCostForm")[0].reset();
                location.reload();
            } else {
                $('#successModalBody').html(result.message || 'Subscription Management updated successfully!');
                $('#successModal').modal('show');
            }
        },
        error: function () {
            // Show error message in the error modal
            $('#errorModalBody').html('An error occurred.');
            $('#errorModal').modal('show');
        }
    });
}




function deleteSmgList(formId) {
    var smgId = $('#id').val();

    // Show confirmation message in the modal
    $('#confirmationModalBody').html('Do you want to delete this Smg Person?');
    $('#confirmationModal').modal('show');

    // Set up click event for the confirm button
    $('#confirmBtn').on('click', function () {
        $.ajax({
            url: '/Admin/Smg/Delete/' + smgId,
            type: 'POST',
            dataType: 'json',
            success: function (result) {
                // Show success message in the modal
                $('#successModalBody').html('Delete Successfully this Smg.');
                $('#successModal').modal('show');
                $('#addsmgForm')[0].reset();
                $("#addsmgCostForm")[0].reset();
                location.reload();
            },
            error: function (xhr, status, error) {
                // Show error message in the modal
                $('#errorModalBody').html('An error occurred while deleting the Smg Person.');
                $('#errorModal').modal('show');
            }
        });

        // Close the confirmation modal
        $('#confirmationModal').modal('hide');
    });
}
// Edit Get method
$(document).ready(function () {
    $('.edit-user').on('click', function () {
        var smgId = $(this).data('id');
        loadDropdownOptions(smgId);
    });



    function loadDropdownOptions(smgId) {
        $.ajax({
            url: '/Admin/Smg/Edit',
            type: 'GET',
            dataType: 'json',
            data: { smgId: smgId },
            success: function (response) {
                console.log(response);
                populateDropdownEditGetFun('payment-currency', response.smg.paymentCurrencyName, response.paymentCurrencyList);
                populateDropdownEditGetFun('payment-interval', response.smg.paymentIntervalName, response.paymentIntervalList);
                populateDropdownEditGetFun('payment-type', response.smg.paymentTypeName, response.paymentTypeList);

                $('#dropdownContainer').show();

                $('#smg-name').val(response.smg.smgName);
                $('#id').val(response.smg.id);
                $('#tax-name').val(response.smg.taxName);
                $('#tax-percentage').val(response.smg.taxPercentage);
                $('#payment-type').val(response.smg.paymentTypeName);
                $('#payment-currency').val(response.smg.paymentCurrencyName);
                $('#payment-interval').val(response.smg.paymentIntervalName);

                var cartItems = response.cartItem; // Retrieve cartItems data from response

                var productLines = $('.form-row.cart-row');

                for (var i = 0; i < cartItems.length; i++) {
                    var cartItem = cartItems[i];

                    if (i >= productLines.length) {
                        var newProductLine = $(productLines[0]).clone();
                        $('#productLines').append(newProductLine);
                        productLines = $('.form-row.cart-row'); // Update productLines array with the new element
                    }

                    var currentProductLine = $(productLines[i]);

                    // Update the existing input fields with the updated values
                    currentProductLine.find('.cart-id').val(cartItem.id);
                    currentProductLine.find('.description').val(cartItem.description);
                    currentProductLine.find('.qty').val(cartItem.qty);
                    currentProductLine.find('.cost').val(cartItem.cost);
                    currentProductLine.find('.total').val(cartItem.total);
                }

                // Remove any extra product lines if there are more in the DOM than in the cartItems
                if (productLines.length > cartItems.length) {
                    for (var j = cartItems.length; j < productLines.length; j++) {
                        $(productLines[j]).remove();
                    }
                }
                // Calculate and display subtotal, VAT, VAT percentage, and grand total
                var subTotal = 0;
                for (var k = 0; k < cartItems.length; k++) {
                    subTotal += parseFloat(cartItems[k].total);
                }


                var vatPercentage = parseFloat(response.cartItem[0].vatPercentage) || 0;
                var vat = subTotal * (vatPercentage / 100);
                var grandTotal = subTotal + vat;


                calculateTotal(subTotal, vatPercentage, vat, grandTotal);
                // Enable all buttons
                enableAllButtons();

                // Disable the "CREATE NEW SMG" button
                disableCreateSmgButton();
            },
            error: function (xhr, status, error) {
                console.log(xhr.responseText);
            }
        });
    }

    function enableAllButtons() {
        $("#deleteSmgButton, #clearFormButton, #updateSmgListButton, #updateInfoButton, #updateCostButton").show();
    }

    function disableCreateSmgButton() {


        $("#createSmgButton").hide();
    }


    function populateDropdownEditGetFun(dropdownId, selectedValue, options) {
        var dropdown = $('#' + dropdownId);
        dropdown.empty();

        // Add the selected value as the first option
        var selectedOption = $('<option>').val(selectedValue).text(selectedValue);
        dropdown.append(selectedOption);

        // Add the remaining options from the options array
        $.each(options, function (index, option) {
            if (option.value !== selectedValue) {
                var optionElement = $('<option>').val(option.value).text(option.text);
                dropdown.append(optionElement);
            }
        });
    }

    function calculateTotal(subTotal, vatPercentage, vat, grandTotal) {
        $('#sub-total').val(subTotal.toFixed(2));
        $('#vat-percentage').val(vatPercentage.toFixed(2));
        $('#vat').val(vat.toFixed(2));
        $('#grand-total').val(grandTotal.toFixed(2));
    }
});


// SmgCost
function onAddCostClick() {

    if (!$("#addsmgCostForm").valid()) {
        return;
    }
    // Get the form input values manually
    var description = $('#addsmgCostForm .description').val();
    var qty = $('#addsmgCostForm .qty').val();
    var cost = $('#addsmgCostForm .cost').val();
    var total = $('#addsmgCostForm .total').val();
    var subtotal = $('#sub-total').val();
    var vat = $(' #vat').val();
    var grandtotal = $('#grand-total').val();

    // Create an object or FormData to store the form data
    var formData = {
        description: description,
        qty: qty,
        cost: cost,
        total: total,
        subtotal: subtotal,
        vat: vat,
        grandtotal: grandtotal
    };

    // Perform the AJAX request
    $.ajax({
        type: 'POST',
        url: '/Admin/SmgCost/Create',
        data: formData,
        success: function (response) {
            // Handle the success response
            alert("Data saved successfully");
            $("#addsmgCostForm")[0].reset();
            window.location.href = '/Admin/SmgCost';
        },
        error: function (xhr, status, error) {
            // Handle the error response
            alert("An error has occured while saving the data")
            console.error('Error saving data:', error);
        }
    });
}

function updateCostform(formId) {
    if (!$("#addsmgCostForm").valid()) {
        return;
    }
    var formData = $(formId).serialize() || {};
};

// Define the clearForm function
function clearForm(formId) {
    $(formId)[0].reset();
}

// Attach a click event listener to the "CLEAR FORM" button
$(document).ready(function () {
    $('#clearFormButton').on('click', function () {
        clearFormsAndManageButtons();
    });
});

function clearFormsAndManageButtons() {
    clearForm('#addsmgForm');
    clearForm('#addsmgCostForm');
    enableCreateSmgButton();
    disableOtherButtons();
    hideClearFormButton();
    location.reload();
}

function enableCreateSmgButton() {
    $("#createSmgButton").show();

}

function disableOtherButtons() {
    $("#deleteSmgButton, #updateSmgListButton, #updateInfoButton, #updateCostButton").hide();
}

function hideClearFormButton() {
    $("#clearFormButton").hide();
}
// Add this code where you handle your modals
$(document).on('click', '#okButton', function () {
    location.reload(); // Reload the entire page
});