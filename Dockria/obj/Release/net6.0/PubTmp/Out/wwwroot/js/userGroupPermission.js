// This fuction for All Checkbox Make Checked
$(document).ready(function () {
    debugger;
    // Event listener for checkAllPermissions checkbox function strat here ->
    $('#checkAllPermissions').on('change', function () {
        var isChecked = $(this).is(':checked');
        debugger;
        // Update all checkboxes
        $('.form-check-input').prop('checked', isChecked);

    });

    $('.form-check-input').on('change', function () {
        debugger;
        // Check if any of the individual checkboxes are unchecked
        var anyUnchecked = $('.form-check-input').filter(':not(:checked)').length > 0;
        // Update the main checkbox based on the status of individual checkboxes
        $('#checkAllPermissions').prop('checked', !anyUnchecked);
    });
    // End here

    // Checked All EndUserManagmentPermission Checkbox function strat here ->
    $('#endUserManagementCheckbox').on('change', function () {
        var isChecked = $(this).is(':checked');
        // Update all checkboxes        
        $('.endUserManagement').prop('checked', isChecked);
    });

    // Add a click event listener to the individual checkboxes
    $('.endUserManagement').on('change', function () {
        debugger;
        // Check if any of the individual checkboxes are unchecked
        var anyUnchecked = $('.endUserManagement').filter(':not(:checked)').length > 0;
        // Update the main checkbox based on the status of individual checkboxes
        $('#endUserManagementCheckbox').prop('checked', !anyUnchecked);
    });
    //  strat here ->

    // Checked All RADManagementPermission Checkbox function Start Here ->
    $('#radManagementCheckbox').on('change', function () {
        var isChecked = $(this).is(':checked');
        // Update all checkboxes        
        $('.radManagement').prop('checked', isChecked);
    });

    // Add a click event listener to the individual checkboxes
    $('.radManagement').on('change', function () {
        debugger;
        // Check if any of the individual checkboxes are unchecked
        var anyUnchecked = $('.radManagement').filter(':not(:checked)').length > 0;
        // Update the main checkbox based on the status of individual checkboxes
        $('#radManagementCheckbox').prop('checked', !anyUnchecked);
    });
    // End here ->

    // Checked All DocumentManagementPermission Checkbox function start here ->
    $('#checkAllDocMngPermissions').on('change', function () {
        var isChecked = $(this).is(':checked');
        // Update all checkboxes        
        $('.documentMangCheckbox').prop('checked', isChecked);
    });

    // Add a click event listener to the individual checkboxes
    $('.documentMangCheckbox').on('change', function () {
        debugger;
        // Check if any of the individual checkboxes are unchecked
        var anyUnchecked = $('.documentMangCheckbox').filter(':not(:checked)').length > 0;
        // Update the main checkbox based on the status of individual checkboxes
        $('#checkAllDocMngPermissions').prop('checked', !anyUnchecked);
    });
    // End here ->

    // This function for Clear all Form
    $('#clearButton').on('click', function () {
        // Reload the HTML page
        window.location.reload();
    });

    // This Funtion For Save User Group Permission in Database ->
    $("#submitButton").click(function (e) {
        e.preventDefault(); // Prevent the default form submission
        sendDataToController(); // Call the function to send the data
    });

    // This function for Show All Users in Dropdown box...
    fetchDropdownOptions();

    // Enforce integer-only input for the maxDocUpNum field
    document.getElementById("maxDocUpNum").addEventListener("input", function () {
        this.value = this.value.replace(/[^0-9]/g, ""); // Remove any non-numeric characters        
    });
});

// This function for save group Permissions "POST" Method ->
function sendDataToController() {
    debugger;

    var userGroupPermission = {
        UserGroupName: $('#groupList option:selected').text(),
    };

    var documentManagementPermission = {
        ViewDoc: $('#viewDoc').is(':checked'),
        DocSinAdd: $('#docSinAdd').is(':checked'),
        DocMulAdd: $('#docMulAdd').is(':checked'),
        DocCopy: $('#docCopy').is(':checked'),
        DocMove: $('#docMove').is(':checked'),
        DocDelete: $('#docDelete').is(':checked'),
        DocRename: $('#docRename').is(':checked'),
        DocPrivate: $('#docPrivate').is(':checked'),
        DocDown: $('#docDown').is(':checked'),
        DocPrint: $('#docPrint').is(':checked'),
        ViewMatadata: $('#viewMatadata').is(':checked'),
        EditMatadata: $('#editMatadata').is(':checked'),
        ShareDocInt: $('#shareDocInt').is(':checked'),
        ShareDocExt: $('#shareDocExt').is(':checked'),
        ShareSigExt: $('#shareSigExt').is(':checked'),
        AuditLogDoc: $('#auditLogDoc').is(':checked'),
        DocVerView: $('#docVerView').is(':checked'),
        DocRollBack: $('#docRollBack').is(':checked'),
        DownCsvRpt: $('#downCsvRpt').is(':checked'),
        AuditLogUser: $('#auditLogUser').is(':checked'),
        AsgnDocUser: $('#asgnDocUser').is(':checked'),
        MaxDocUpSize: $('#maxDocUpSize').is(':checked'),
        MaxDocUpNum: $('#maxDocUpNum').val(),
    };

    var endUserManagementPermission = {
        EditEmail: $('#editEmail').is(':checked'),
        EditPassword: $('#editPassword').is(':checked'),
        EditSign: $('#editSign').is(':checked'),
    };

    var radManagementPermission = {
        RadView: $('#radView').is(':checked'),
        RadEdit: $('#radEdit').is(':checked'),
        RadFormFill: $('#radFormFill').is(':checked'),
    };

    var maxDocUpNumValue = parseInt($('#maxDocUpNum').val());
    if (isNaN(maxDocUpNumValue) || maxDocUpNumValue < 1 || maxDocUpNumValue > 50) {
        // Display validation error message in the modal
        $("#errorModalBody").text("Please enter a valid integer value between 1 and 50 for Maximum Document Upload Size in MB.");
        $("#errorModal").modal("show");
        return; // Prevent AJAX call if validation fails
    }

    var userGroupPermissionVM = {
        UserGroup: userGroupPermission,
        documentManagement: documentManagementPermission,
        endUserManagement: endUserManagementPermission,
        radManagement: radManagementPermission,
    };

    $.ajax({
        url: "/CompanyAdmin/GroupPermission/SavePermission",
        type: "POST",
        data: JSON.stringify(userGroupPermissionVM),
        contentType: "application/json",
        success: function (response) {
            debugger;
            // Close the modal
            setTimeout(function () {
                window.location.reload();
            }, 2000);
            toastr.success(response.message);
            $("#editDmsModal").modal("hide");

            //dataTable.ajax.reload(null, false);

        },
        error: function (error) {
            // Handle the error response
            console.log(error);
            // Display an error message or handle the error condition
        }
    });
}



function fetchDropdownOptions() {
    debugger;
    $.ajax({
        url: '/CompanyAdmin/GroupPermission/ShowUserList',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            debugger;
            console.log(data.userlist);
            populateDropdown('#groupList', data.userlist);
        },
        error: function (error) {
            console.error(error.statusText);
        }
    });
}

function populateDropdown(selector, options) {
    var dropdown = $(selector);
    dropdown.empty();

    // Check if the options array is empty or has a length of 0
    if (options.length === 0) {
        // Add a custom option when there are no options available
        dropdown.append('<option disabled selected>No New Group Available for giving any Permissions</option>');
    } else {
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
}
